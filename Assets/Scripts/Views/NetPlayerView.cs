using RR.Presenters;
using RR.Properties;
using UnityEngine;
using RR.Utility.Input;
using RR.Utility;
using RR.Facilitators.UI;
using RR.Facilitators.Gameplay;
using RR.Models.NetworkModel;
using RR.Models.ArenaModel;
using System.Collections.Generic;
using RR.Utility.Gameplay;

namespace RR.Views
{
    public enum AttackType
    {
        None,
        Medium,
        Large,
        Mega,
        Ulti
    }

    public enum ParticleType
    {
        Attack,
        Death,
        Spawn
    }

    public class NetPlayerView : BasePhotonView<NetPlayerPresenter, NetPlayerProperties>
    {
        private const string ANIM_B_CROUCHING = "IsCrouching";
        private const string ANIM_F_MOVEZ = "MoveZ";
        private const string ANIM_F_MOVEX = "MoveX";
        private const string ANIM_B_ISINMOTION = "IsInMotion";
        private const string ANIM_B_ISATTACK_QUEUED = "IsAttackQueued";
        private const string ANIM_B_ISJUMPING = "IsJumping";
        private const string ANIM_TAG_JUMPCANINTERRUPT = "JumpCanInterrupt";
        private const string ANIM_B_ISDEAD = "IsDead";
        private const string ANIM_TR_DIE = "Die";

        private const string RPC_SETCURRENTVELOCITY = "SetCurrentVelocity";
        private const string RPC_SETJUMPING = "SetJumping";
        private const string RPC_ATTACKQUEUED = "SetAttackQueued";
        private const string RPC_SETCOLOR = "SetColor";
        private const string RPC_KILLFLOORBREACHED = "KillFloorBreached";
        private const string RPC_SETDEAD = "SetDead";
        private const string RPC_RANDOMPARTICLE = "RandomParticle";

        private const string CRP_GAME_FINISHED = "game-finished";

        private bool _processInputs;
        private Vector2 _movement;
        private Vector3 _currentVelocity;
        private bool _jumpAwaitingRelease;
        private bool _attackAwaitingRelease;
        private bool _jumping;
        private bool _attackQueued;
        private bool _lastSentAttackState;
        private float _attackQueueTime;
        private bool _isAttacking;
        private PlayerDetails[] _playerDetails;
        private int _playerID;
        private Pedestals _pedestalDetails;
        private int _lastDamageSourceID;
        private bool _outOfBounds;
        private bool _dead;
        private bool _jumpQueued;
        private bool _cleanedUp;
        private IFloatingHealthBar _healthbarInterface;
        private HashSet<int> _attackedPlayers = new HashSet<int>();
        private AttackType _currentAttack;
        private Transform _particlesContainer;

        public override void Initialize()
        {
            name = $"NetPlayer_{photonView.owner.NickName}";
            transform.SetParent(GameObject.FindObjectOfType<PlayerContainer>().transform);

            AddListeners();
            Presenter.Initialize();
            
            _processInputs = false;
            _jumpAwaitingRelease = false;
            _cleanedUp = false;

            if (!photonView.isMine)
            {
                Properties.Rigidbody.isKinematic = true;
                Properties.MinimapEnemyIcon.SetActive(true);
                
                var container = FindObjectOfType<HealthbarContainer>();
                _healthbarInterface = Instantiate(Properties.HealthBarUI, container.transform, false) as IFloatingHealthBar;
                _healthbarInterface.Init(photonView, Properties.HealthbarAnchor);
            }
            else
            {
                Properties.MinimapCamera.SetActive(true);
            }

            _particlesContainer = FindObjectOfType<ParticleContainer>().transform;
        }
    
        public override void Dispose()
        {
            RemoveListeners();
            Presenter.Dispose();
        }

        private void AddListeners()
        { 
            if (photonView.isMine)
            {
                Presenter.Action += OnAction;
                Presenter.PlayerMovement += OnMovement;
                Presenter.ConnectCamera += OnConnectCamera;
                Presenter.ReturnToPedestal += OnReturnToPedestal;
            }
            
            Presenter.GameDetailsUpdated += OnGameDetailsUpdated;
            Presenter.ProcessInputChanged += OnProcessInputChanged;
            Presenter.Cleanup += OnCleanup;
            Presenter.Death += OnDeath;
            Presenter.KilledByNetPlayer += OnKilledByNetPlayer;
        }

        private void RemoveListeners()
        {
            if (photonView.isMine)
            {
                Presenter.Action -= OnAction;
                Presenter.PlayerMovement -= OnMovement;
                Presenter.ConnectCamera -= OnConnectCamera;
                Presenter.ReturnToPedestal -= OnReturnToPedestal;
            }

            Presenter.GameDetailsUpdated -= OnGameDetailsUpdated;
            Presenter.ProcessInputChanged -= OnProcessInputChanged;
            Presenter.Cleanup -= OnCleanup;
            Presenter.Death -= OnDeath;
            Presenter.KilledByNetPlayer -= OnKilledByNetPlayer;
        }

        private void FixedUpdate()
        {
            if (_dead) return;

            if (photonView.isMine)
            {
                if (_processInputs && !_outOfBounds)
                {
                    var inputs = GatherPlayerInputs();
                    Presenter.SendInputFrame(inputs);

                    int layerMask = (1 << 9);
                    Vector3 start = transform.position + (Vector3.up * 0.1f);
                    Vector3 end = transform.position + (Vector3.down * 0.2f);

                    Debug.DrawLine(start, end, Color.red, 0);

                    var grounded = Physics.Linecast(start, end, layerMask);
                    var yVel = Properties.Rigidbody.velocity.y;

                    if (_jumpQueued && !_jumping && yVel >= 0)
                    {
                        _jumpQueued = false;
                        Properties.Rigidbody.AddForce(Vector3.up * 6f, ForceMode.Impulse);
                        SetJumping(true);
                    }
                    else if (_jumping)
                    {
                        SetJumping(!(grounded && yVel <= 0));
                    }

                    var delta = _movement;
                    Vector3 currentVelocity = Properties.Rigidbody.velocity;

                    if (delta.magnitude > 0.001) 
                    {
                        var forward = Properties.PlayerCamera.GetForward();
                        var right = -Vector3.Cross(forward, Vector3.up);

                        forward *= delta.y;
                        right *= delta.x;

                        currentVelocity = forward + right;
                        currentVelocity = currentVelocity.normalized * Properties.MaxSpeed;
                        currentVelocity.y = Properties.Rigidbody.velocity.y;
                    }
                    else
                    {
                        currentVelocity.x = 0;
                        currentVelocity.z = 0;
                    }

                    Properties.Rigidbody.velocity = currentVelocity;

                    SetCurrentVelocity(currentVelocity);

                    transform.rotation = Quaternion.LookRotation(Properties.PlayerCamera.GetForward());
                }

                if (!_outOfBounds && transform.position.y < Properties.KillFloorY)
                {
                    _outOfBounds = true;
                    Presenter.AlertOutOfBounds();
                }
            }

            Animate();
        }

        [PunRPC] private void SetCurrentVelocity(Vector3 currentVelocity)
        {
            if (_dead) return;

            if (_currentVelocity != currentVelocity)
            {
                _currentVelocity = currentVelocity;
                if (photonView.isMine)
                {
                    photonView.RPC(RPC_SETCURRENTVELOCITY, PhotonTargets.OthersBuffered, currentVelocity);
                }
            }
        }

        [PunRPC] private void SetJumping(bool jumping)
        {
            if (_dead) return;

            _jumping = jumping;
            
            if (photonView.isMine)
            {
                photonView.RPC(RPC_SETJUMPING, PhotonTargets.OthersBuffered, jumping);
            }
        }

        [PunRPC] private void SetAttackQueued(bool attackQueued)
        {
            if (_dead) return;

            _attackQueued = attackQueued;
            _attackQueueTime = Time.time;

            if (attackQueued == _lastSentAttackState) return;
            _lastSentAttackState = attackQueued;

            if (photonView.isMine)
            {
                photonView.RPC(RPC_ATTACKQUEUED, PhotonTargets.OthersBuffered, attackQueued);
            }
        }

        [PunRPC] private void SetDead()
        {
            if (!_dead && photonView.owner.GetScore() == 0)
            {
                _dead = true;
                Properties.Animator.SetBool(ANIM_B_ISDEAD, true);
                Properties.Animator.SetTrigger(ANIM_TR_DIE);
              
                RandomParticle((int)ParticleType.Death, transform.position, photonView.owner.ID);
            }

            if (photonView.isMine)
            {
                photonView.RPC(RPC_SETDEAD, PhotonTargets.OthersBuffered);
            }
        }

        [PunRPC] public void RandomParticle(int type, Vector3 position, int photonID)
        {
            if (type == 0) 
            {
                RandomParticleHelper(Properties.AttackParticles, position);
                HitEffect(photonID);
            }
            else if (type == 1) 
            {
                RandomParticleHelper(Properties.DeathParticles, position);
                HitEffect(photonID);
            }
            else if (type == 2) 
            {
                RandomParticleHelper(Properties.SpawnParticles, position);
            }

            if (photonView.isMine)
            {
                photonView.RPC(RPC_RANDOMPARTICLE, PhotonTargets.OthersBuffered, type, position, photonID);
            }
        }

        private void RandomParticleHelper(GameObject[] collection, Vector3 position)
        {
            var particle = collection[Random.Range(0, collection.Length)];

            var spawnedParticle = Instantiate(particle, _particlesContainer);
            spawnedParticle.transform.position = position;
            spawnedParticle.transform.rotation = Quaternion.identity;
            var ac = spawnedParticle.AddComponent<AutoCull>() as IAutoCull;
            ac.Init(5);
        }

        private void Animate()
        {
            if (_dead) return;

            if (_processInputs)
            {
                Properties.Animator.SetBool(ANIM_B_CROUCHING, false);

                var velocityForward = _currentVelocity;
                velocityForward.y = 0;
                velocityForward = transform.InverseTransformDirection(velocityForward);
                var moveX = velocityForward.x / Properties.MaxSpeed;
                var moveY = velocityForward.z / Properties.MaxSpeed;

                velocityForward.y = 0;
                var moving = velocityForward.magnitude > 0.001f;

                Properties.Animator.SetBool(ANIM_B_ISINMOTION, moving);
                Properties.Animator.SetFloat(ANIM_F_MOVEX, moveX, 0.1f, Time.deltaTime);
                Properties.Animator.SetFloat(ANIM_F_MOVEZ, moveY, 0.1f, Time.deltaTime);
                Properties.Animator.SetBool(ANIM_B_ISJUMPING, _jumping);    
 

                if (_attackQueued && Time.time - _attackQueueTime < Properties.AttackInputTimeBuffer)
                {
                    Properties.Animator.SetBool(ANIM_B_ISATTACK_QUEUED, true);
                }
                else
                {
                    SetAttackQueued(false);
                    Properties.Animator.SetBool(ANIM_B_ISATTACK_QUEUED, false);  
                }
            }
            else
            {
                Properties.Animator.SetBool(ANIM_B_CROUCHING, !_outOfBounds);
                Properties.Animator.SetBool(ANIM_B_ISINMOTION, false);
                Properties.Animator.SetFloat(ANIM_F_MOVEX, 0);
                Properties.Animator.SetFloat(ANIM_F_MOVEZ, 0);
                Properties.Animator.SetBool(ANIM_B_ISJUMPING, false);
                Properties.Animator.SetBool(ANIM_B_ISATTACK_QUEUED, false);
            }
        }

        private void HitEffect(int target)
        {
            if (PhotonNetwork.player.ID == target) 
            {
                PPVolume.Instance.ChromaPulse();
                Presenter.RequestUIShake();
            }
        }

        private void OnMovement(Vector2 delta)
        {
            if (_dead) return;
            if (!photonView.isMine) return;
            _movement = delta;
        }

        private void OnAction(RRAction action)
        {
            if (_dead) return;
            if (!photonView.isMine) return;

            switch (action)
            {
                case RRAction.Jump:
                {
                    _jumpQueued = true;
                    break;
                }
                case RRAction.Attack:
                {
                    SetAttackQueued(true);
                    break;
                }
            }
        }

        private void OnProcessInputChanged(bool processInput)
        {
            _processInputs = processInput;
        }

        private void OnConnectCamera()
        {
            // Properties.PlayerCamera.Connect();
        }

        private void OnJumpEnd()
        {
            if (_dead) return;
            if (!photonView.isMine) return;
            SetJumping(false);
        }

        private void OnGameDetailsUpdated(PlayerDetails[] playerDetails, Pedestals pedestals)
        {
            if (_dead) return;

            _playerDetails = playerDetails;
            var photonID = photonView.ownerId;

            foreach (var details in playerDetails)
            {
                if (details.PhotonID == photonID)
                {
                    _playerID = details.ID;
                    break;
                }
            }

            var c = Properties.PlayerColors[_playerID];
            SetColor(c);

            _pedestalDetails = pedestals;
        }

        private void OnReturnToPedestal()
        {
            if (_dead) return;
            
            if (photonView.isMine)
            {
                Properties.Rigidbody.velocity = Vector3.zero;
                transform.position = _pedestalDetails[_playerID].GetSpawnPosition();
                transform.rotation =  _pedestalDetails[_playerID].Facing;
                Presenter.RequestCameraReset();
                _outOfBounds = false;
                RandomParticle((int)ParticleType.Spawn, transform.position, photonView.owner.ID);
            }
        }

        private void OnCleanup()
        {
            _cleanedUp = true;
            Properties.PlayerCamera.Dispose();
            
            if (PhotonNetwork.player.GetScore() <= 0)
            {
                Properties.PlayerCamera.Camera.gameObject.SetActive(false);
            }

            CleanUpFloatingHealthBar();
            Dispose();
        }

        private void OnDeath()
        {
            SetDead();
            CleanUpFloatingHealthBar();
        }

        private void OnKilledByNetPlayer(int photonPlayerID)
        {
            if (photonPlayerID == photonView.owner.ID)
            {
                SetDead();
                CleanUpFloatingHealthBar();
            }
        }

        private void CleanUpFloatingHealthBar()
        {
            if (_healthbarInterface != null) _healthbarInterface.Destroy();
            _healthbarInterface = null;
        }

        private InputFrame GatherPlayerInputs()
        {
            var frame = new InputFrame();

            if (_cleanedUp) return frame;

            frame.Look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            frame.Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            frame.Scroll = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10f);

            if (Input.GetAxis("Attack") != 0 && !_attackAwaitingRelease) 
            {
                frame.Keys.Enqueue(RRAction.Attack);
                _attackAwaitingRelease = true;
            }
            else if (Input.GetAxis("Attack") == 0) _attackAwaitingRelease = false;

            if (Input.GetAxis("Block") != 0) frame.Keys.Enqueue(RRAction.Block);
            if (Input.GetAxis("AnimationCancel") != 0) frame.Keys.Enqueue(RRAction.AnimationCancel);

            if (Input.GetAxis("Jump") != 0 && !_jumpAwaitingRelease) 
            {
                frame.Keys.Enqueue(RRAction.Jump);
                _jumpAwaitingRelease = true;
            }
            else if (Input.GetAxis("Jump") == 0) _jumpAwaitingRelease = false;

            return frame;
        }

        private void SetColor(Color color)
        {
            Properties.SkinnedMeshRenderer.material.SetColor("_MainColor", color);
        }
        
        // Collisions
        private void OnTriggerEnter(Collider other)
        {
            if (_dead) return;

            if (photonView.isMine && _isAttacking)
            {
                PhotonView pview = null;
                other.gameObject.TryGetComponent<PhotonView>(out pview);

                if (pview && pview.owner != null && pview.owner.GetScore() > 0)
                {
                    if (!_attackedPlayers.Contains(pview.owner.ID))
                    {
                        _attackedPlayers.Add(pview.owner.ID);

                        var dmg = 0;
                        switch (_currentAttack)
                        {
                            case AttackType.None:
                            {
                                return;
                            }
                            case AttackType.Medium:
                            {
                                dmg = 10;
                                break;
                            }
                            case AttackType.Large:
                            {
                                dmg = 20;
                                break;
                            }
                            case AttackType.Mega:
                            {
                                dmg = 30;
                                break;
                            }
                            case AttackType.Ulti:
                            {
                                dmg = 40;
                                break;
                            }
                        }
                        
                        var particleSpawnPoint = other.ClosestPointOnBounds(Properties.WeaponCollider.transform.position);
                        RandomParticle((int)ParticleType.Attack, particleSpawnPoint, pview.owner.ID);
                        Presenter.DamagePlayer(pview.owner.ID, dmg);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_dead) return;
            
            if (photonView.isMine)
            {
                PhotonView pview = null;
                other.gameObject.TryGetComponent<PhotonView>(out pview);

                if (pview && pview.owner != null)
                {
                    if (!_isAttacking && _attackedPlayers.Contains(pview.owner.ID))
                    {
                        _attackedPlayers.Remove(pview.owner.ID);
                    }
                }
            }
        }

        // Events for attack animations
        public void ToggleAttackAnimationLock(string locked)
        {
            _isAttacking = (locked == "locked");
            _attackedPlayers.Clear();
        }

        public void SetCurrentAttackAnimation(int index)
        {
            _currentAttack = (AttackType)index;
        }
    }
}
