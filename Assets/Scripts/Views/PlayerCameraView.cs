using UnityEngine;
using RR.Presenters;
using RR.Properties;
using RR.Utility.Gameplay;

namespace RR.Views
{
    public class PlayerCameraView : BaseView<PlayerCameraPresenter, PlayerCameraProperties>
    {
        public Camera Camera { get { return Properties.Camera; } }

        private Quaternion _originalRootRotation;
        private bool _active;
        private Vector2 _lookInputBuffer;

        private Vector3 _cameraPositionAtDetach;
        private Quaternion _originalCameraRotation;
        private Quaternion _originalPitch;
        private Quaternion _originalYaw;

        private Quaternion _targetPitch;
        private Quaternion _targetYaw;
        private Transform _detachedCameraParent;

        private bool _invertYAxis;
        private bool _detached;

        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();

            Presenter.SetPhotonView(Properties.PlayerPhotonView);
            _originalRootRotation = transform.rotation;

            _cameraPositionAtDetach = Properties.Camera.transform.localPosition;
            _originalCameraRotation = Properties.Camera.transform.localRotation;

            _targetPitch = _originalPitch = Properties.Boom.transform.rotation;
            _targetYaw = _originalYaw = Properties.BoomRoot.transform.localRotation;

            _detachedCameraParent = FindObjectOfType<DetachedCameraContainer>().transform;

            _invertYAxis = UnityEngine.PlayerPrefs.GetInt("invert-y", 1) == 1 ? true : false;
        }

        private void FixedUpdate()
        {
            if (_active)
            {
                if (transform.position.y < (Properties.KillFloorY))
                {
                    Detach();
                }

                if (!_detached)
                {
                    var delta = _lookInputBuffer;

                    var rotation = delta;
                    rotation.y = delta.x * Properties.LookSensitivity.x * Time.deltaTime;
                    rotation.x = delta.y * Properties.LookSensitivity.y * Time.deltaTime;

                    var invertMod = _invertYAxis ? 1 : -1;
                    rotation.x *= invertMod;

                    var projectedX = WrapAngle(Properties.BoomRoot.transform.localRotation.eulerAngles.x + rotation.x);

                    if (projectedX > 35)
                    {
                        rotation.x -= (projectedX - 35);
                    }
                    else if (projectedX < -30)
                    {
                        rotation.x -= (projectedX + 30);
                    }

                    _targetPitch *= Quaternion.Euler(0, rotation.y, 0);
                    _targetYaw *= Quaternion.Euler(rotation.x, 0, 0);

                    var boomRot = Properties.Boom.transform.rotation;
                    Properties.Boom.transform.rotation = Quaternion.Slerp(boomRot, _targetPitch, Properties.PitchSmooth * Time.deltaTime);

                    var boomRootRot = Properties.BoomRoot.transform.localRotation;
                    Properties.BoomRoot.transform.localRotation = Quaternion.Slerp(boomRootRot, _targetYaw, Properties.YawSmooth * Time.deltaTime);

                    var boomRotCurrentEA = Properties.BoomRoot.transform.eulerAngles;
                    var boomRotX = WrapAngle(boomRotCurrentEA.x);

                    if (boomRotX > 35)
                    {
                        boomRotCurrentEA.x -= (boomRotX - 35);
                    }
                    else if (boomRotX < -30)
                    {
                        boomRotCurrentEA.x -= (boomRotX + 30);
                    }

                    Properties.BoomRoot.transform.eulerAngles = boomRotCurrentEA;

                    _lookInputBuffer = Vector3.zero;
                    ClampRootRotation();
                }
                else
                {
                    Properties.Camera.transform.LookAt(Properties.LookAtTarget);
                }
            }
        }

        public void ClampRootRotation()
        {
            transform.rotation = _originalRootRotation;
        }

        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        {
            Presenter.LookUpdate += OnLookUpdate;
            Presenter.ZoomUpdate += OnZoomUpdate;
            Presenter.SetAsLocalPlayerCamera += OnSetAsLocalPlayerCamera;
            Presenter.ActivateLocalPlayerCamera += OnActivateLocalPlayerCamera;
            Presenter.Reset += OnReset;
            Presenter.InvertYChanged += OnInvertYChanged;
        }

        private void RemoveListeners()
        {
            Presenter.LookUpdate += OnLookUpdate;
            Presenter.ZoomUpdate += OnZoomUpdate;
            Presenter.SetAsLocalPlayerCamera -= OnSetAsLocalPlayerCamera;
            Presenter.ActivateLocalPlayerCamera -= OnActivateLocalPlayerCamera;
            Presenter.Reset -= OnReset;
            Presenter.InvertYChanged -= OnInvertYChanged;
        }

        public Vector3 GetForward()
        {
            var forward = Properties.Boom.forward;
            forward.y = 0;
            return forward.normalized;
        }

        private void OnLookUpdate(Vector2 delta)
        {
            _lookInputBuffer = delta;
        }

        private void OnZoomUpdate(int delta)
        {
            var change = delta * 0.25f;

            var newPos = Properties.Camera.transform.localPosition;
            var newZ = Mathf.Clamp(newPos.z + change, -Properties.ZoomRange.y, -Properties.ZoomRange.x);
            newPos.z = newZ;
            Properties.Camera.transform.localPosition = newPos;
        }

        private void OnReset()
        {
            ReAttach();

            Properties.Boom.transform.rotation = _originalPitch;
            Properties.BoomRoot.transform.localRotation = _originalYaw;
            Properties.Camera.transform.localRotation = _originalCameraRotation;
            _lookInputBuffer = Vector3.zero;

            _targetPitch = _originalPitch;
            _targetYaw = _originalYaw;
        }

        private void OnSetAsLocalPlayerCamera()
        {
            _active = true;
            Presenter.SetCameraTransformReference(Properties.Camera.transform);
        }

        private void OnActivateLocalPlayerCamera()
        {
            Properties.Camera.enabled = true;
            Properties.AudioListener.enabled = true;
        }

        private void OnInvertYChanged(bool invert)
        {
            _invertYAxis = invert;
        }

        private void Detach()
        {
            if (!_detached)
            {
                _detached = true;
                _cameraPositionAtDetach = Properties.Camera.transform.localPosition;
                Properties.Camera.transform.SetParent(_detachedCameraParent);
            }
        }

        private void ReAttach()
        {
            if (_detached)
            {
                ClampRootRotation();
                _detached = false;
                Properties.Camera.transform.SetParent(Properties.BoomRoot);
                Properties.Camera.transform.localPosition = _cameraPositionAtDetach;
            }
        }

        private float WrapAngle(float angle)
        {
            if (angle > -90 && angle <= 90)
            {
                return angle;
            }
            else
            {
                return (360 - angle) * -1;
            }
        }
    }
}
