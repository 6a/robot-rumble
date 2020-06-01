using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RR.Presenters;
using RR.Properties;
using RR.Facilitators.Platform;
using RR.Facilitators.Scenery;
using RR.Utility.Hex;
using RR.Utility;
using RR.Facilitators.UI;
using RR.Models.NetworkModel;
using RR.Models.ArenaModel;

namespace RR.Views
{
    public enum Transition
    {
        TileGeneration,
        CameraTransition
    }

    public class ArenaView : BaseView<ArenaPresenter, ArenaProperties>
    {
        private float _initialCameraContainerY;
        private float _initialCameraZ;
        private Queue<IScenery> _sceneryPool = new Queue<IScenery>();
        private List<IScenery> _sceneryInMotion = new List<IScenery>();
        private Coroutine _sceneryGenerationCoroutine;
        private Coroutine _sceneryAnimationCoroutine;
        private bool _animateScenery;
        private int _sessionSeed;
        private bool _cameraReady;
        private bool _tilesReady;
        private List<GameObject> _generatedGameObjectRoots = new List<GameObject>();
        private PlayerDetails _localPlayerDetails;

        public override void Initialize()
        {
            Presenter.Initialize();
            AddListeners();

            _initialCameraContainerY = Properties.CameraContainer.transform.position.y;
            _initialCameraZ = Properties.ArenaCamera.transform.position.z;
        }
    
        public override void Dispose()
        {
            RemoveListeners();
        }

        private void AddListeners()
        { 
            Presenter.GameStart += OnGameStart;
            Presenter.Reset += OnReset;
            Presenter.NewSessionSeed += OnNewSessionSeed;
            Presenter.PlayerCameraTransformUpdated += OnPlayerCameraTransformUpdated;
            Presenter.EnablePostGameCamera += OnEnablePostGameCamera;
        }

        private void RemoveListeners()
        {
            Presenter.GameStart -= OnGameStart;
            Presenter.Reset -= OnReset;
            Presenter.NewSessionSeed -= OnNewSessionSeed;
            Presenter.PlayerCameraTransformUpdated -= OnPlayerCameraTransformUpdated;
            Presenter.EnablePostGameCamera -= OnEnablePostGameCamera;
        }
        
        public void Construct()
        {
            StartCoroutine(ConstructAsync());
        }

        private IEnumerator ConstructAsync()
        {
            var bounds = Properties.TilePrefab.GetBounds();

            var tileZOffet = bounds.z / 2f;
            var tileXOffset = bounds.x * 0.75f;
            var tileHeight = bounds.y * 0.8f;
            var layerYOffset = 0f;
            var ringToMakeHolesRangeSource = 0.1f +  (Properties.Rings / 2f);
            var ringsToMakeHoles = (Mathf.FloorToInt(ringToMakeHolesRangeSource), Mathf.CeilToInt(ringToMakeHolesRangeSource));
            var holeRandom = new System.Random(_sessionSeed);

            PlacePedestals(bounds);

            for (int i = 0; i < Properties.Rings; i++)
            {
                var layer = AddRingContainer(i);
                var tiles = Hex.MakeRing(i);
                var colorLerp = ((float)i / (Properties.Rings - 1));
                var color = Color.Lerp(Properties.TileInnerColor, Properties.TileOuterColor, colorLerp);

                var timeAtStart = Time.time;
   
                for (int j = 0; j < tiles.Count; j++)
                {
                    var tile = tiles[j];

                    var modifiedCoords = new Vector3(tile.x * tileXOffset, layerYOffset, tile.z * tileZOffet - tile.y * tileZOffet);
                    
                    ITile prefab = null;
                    if (i >= ringsToMakeHoles.Item1 && i <= ringsToMakeHoles.Item2)
                    {
                        var makeHole = holeRandom.Next(0, i * 6) < i * 6 * 0.7f;
                        if (!makeHole)
                        {
                            prefab = Properties.TilePrefab;
                        }
                    }
                    else
                    {
                        prefab = Properties.TilePrefab;
                    }

                    if (prefab != null)
                    {
                        var extraHeight = Vector3.zero;
                        if (i < 3) 
                        {
                            extraHeight.y = (3 - i) * tileHeight;
                        }

                        var t = AddTile(prefab, layer, modifiedCoords + extraHeight, j);
                        t.HexCoords = tile.ToVector3();
                        t.SetColor(color);

                        yield return null;
                    }
                }   
            }

            ReportComplete(Transition.TileGeneration);
        }

        private void PlacePedestals(Vector3 tileBounds)
        {
            var pedestalBounds = Properties.Pedestals[0].GetBounds();
            var pedestalPositionX = tileBounds.x * 0.75f * Properties.Rings + (pedestalBounds.x / 2) - (tileBounds.x / 2);
            var pedestalPositionZ = tileBounds.z * Properties.Rings + (pedestalBounds.z / 2) - (tileBounds.z / 2);

            Properties.Pedestals[0].Transform.position = new Vector3(0, Properties.PedestalHeight, -pedestalPositionZ);
            Properties.Pedestals[1].Transform.position = new Vector3(pedestalPositionX, Properties.PedestalHeight, 0);
            Properties.Pedestals[2].Transform.position = new Vector3(0, Properties.PedestalHeight, pedestalPositionZ);
            Properties.Pedestals[3].Transform.position = new Vector3(-pedestalPositionX, Properties.PedestalHeight, 0);

            var pedestals = new List<IPedestal>();         
            for (var i = 0; i < Properties.Pedestals.Count; i++)
            {
                var pedestal = Properties.Pedestals[i] as IPedestal;
                pedestal.SetColor(Properties.PedestalColors[i]);
                pedestals.Add(pedestal);
            }

            Presenter.AlertPedestalsConfigured(new Pedestals(pedestals.ToArray()));
        }

        private Transform AddRingContainer(int index)
        {
            const string RING_NAME_PREFIX = "Ring_";

            var container = new GameObject($"{RING_NAME_PREFIX}{index}");
            _generatedGameObjectRoots.Add(container);
            container.transform.SetParent(transform);
            container.transform.localPosition = Vector3.zero;

            return container.transform;
        }

        private ITile AddTile(ITile prefab, Transform container, Vector3 localPosition, int id)
        {
            const string TILE_NAME_PREFIX = "Tile_";

            var tile = Instantiate(prefab.gameObject, container, false);
            tile.gameObject.name = $"{TILE_NAME_PREFIX}{id}";
            tile.gameObject.transform.localPosition = localPosition;
            return tile.GetComponent<ITile>();
        }

        private void TransitionArenaCameraToPedestal()
        {
            StartCoroutine(TransitionArenaCameraAsync());
        }

        private IEnumerator TransitionArenaCameraAsync()
        {
            var designatedPedestal = Properties.Pedestals[_localPlayerDetails.ID];
            var startingYRotation = 0 - (_localPlayerDetails.ID * 90f);

            Properties.CameraContainer.transform.rotation = Quaternion.Euler(0, startingYRotation, 0);

            var lerp = 0f;
            var animationDuration = Properties.CameraTransitionDuration * 0.8f;
            var cameraBlendDuration = Properties.CameraTransitionDuration * 0.2f;
            while (lerp < 1)
            {
                var delta =  (1 / (animationDuration / Time.deltaTime));
                lerp = Mathf.Clamp01(lerp + delta);
                var ease = Interpolation.EaseInOutQuad(lerp, 0, 1);

                var containerRot = 360f * ease;
                Properties.CameraContainer.transform.rotation = Quaternion.Euler(0, startingYRotation + containerRot, 0);

                var containerPos = Properties.CameraContainer.transform.position;
                var containerPosOffset = Mathf.Lerp(Properties.CameraTransitionEndHeight, _initialCameraContainerY, 1 - ease);
                Properties.CameraContainer.transform.position = new Vector3(containerPos.x, containerPosOffset, containerPos.z);

                var cameraPos = Properties.ArenaCamera.transform.localPosition;
                var cameraZOffset = Mathf.Lerp(_initialCameraZ, Properties.CameraTransitionEndZ, ease);
                Properties.ArenaCamera.transform.localPosition = new Vector3(cameraPos.x, cameraPos.y, cameraZOffset);

                yield return null;
            }

            // Now lerp to the player camera

            lerp = 0;

            var startPosition = Properties.ArenaCamera.transform.position;
            var startRotation = Properties.ArenaCamera.transform.rotation;

            while (lerp < 1)
            {
                var delta =  (1 / (cameraBlendDuration / Time.deltaTime));
                lerp = Mathf.Clamp01(lerp + delta);
                var ease = Interpolation.EaseInQuad(lerp, 0, 1);

                var pos = Vector3.Lerp(startPosition, _localPlayerDetails.PlayerCameraTransform.position, ease);
                var rot = Quaternion.Slerp(startRotation, _localPlayerDetails.PlayerCameraTransform.rotation, ease);

                Properties.ArenaCamera.transform.position = pos;
                Properties.ArenaCamera.transform.rotation = rot;

                yield return null;
            }

            Properties.ArenaCamera.gameObject.SetActive(false);

            Presenter.AlertCameraSwitchReady();

            ReportComplete(Transition.CameraTransition);
        }

        private IEnumerator SceneryGeneration()
        {
            float interval = 1f / Properties.ScenerySpawnrate;
            var sceneryParent = transform.GetChild(2);
            if (sceneryParent.name != "Scenery")
            {
                var newParent = new GameObject("Scenery");
                _generatedGameObjectRoots.Add(newParent);
                
                newParent.transform.SetParent(transform);
                newParent.transform.SetSiblingIndex(2);
                sceneryParent = newParent.transform;
            }
            
            while (_animateScenery)
            {
                IScenery scenery = null;
                if (_sceneryPool.Count + _sceneryInMotion.Count < Properties.SceneryPoolMaxSize)
                {
                    var rand = Random.Range(0, Properties.SceneryPrefabs.Length);
                    scenery = Instantiate(Properties.SceneryPrefabs[rand], Vector3.zero, Quaternion.identity) as IScenery;
                    scenery.gameObject.transform.SetParent(sceneryParent);
                }
                else if (_sceneryPool.Count > 0)
                {
                    scenery = _sceneryPool.Dequeue();
                }

                if (scenery != null)
                {
                    var speed = Properties.SceneryUnitsPerSecondRange.GetRandom();
                    var color = Random.ColorHSV(0, 1, 0.3f, 0.3f, 0.7f, 0.7f);
                    
                    _sceneryInMotion.Add(scenery);
                    scenery.Init(color, Properties.SceneryYRange, speed, _sceneryPool, _sceneryInMotion);

                    var radius = Properties.SceneryDistanceFromCenterRange.GetRandom();   
                    var xzPosition = Random.insideUnitCircle.normalized * radius;
                    scenery.gameObject.transform.position = new Vector3(xzPosition.x, Properties.SceneryYRange.Min, xzPosition.y);
                }

                yield return new WaitForSeconds(interval);
            }
        }

        private IEnumerator SceneryAnimation()
        {
            while (_animateScenery)
            {
                for (int i = _sceneryInMotion.Count - 1; i >= 0; i--)
                {
                    _sceneryInMotion[i].Animate();
                }

                yield return null;
            }
        }

        private void OnGameStart(PlayerDetails playerDetails)
        {
            _localPlayerDetails = playerDetails;
            Properties.ArenaCamera.gameObject.SetActive(true);
            Properties.PostGameCamera.gameObject.SetActive(false);

            Construct();
            TransitionArenaCameraToPedestal();
            

            _animateScenery = true;

            if (_sceneryGenerationCoroutine != null) StopCoroutine(_sceneryGenerationCoroutine);
            _sceneryGenerationCoroutine = StartCoroutine(SceneryGeneration());

            if (_sceneryAnimationCoroutine != null) StopCoroutine(_sceneryAnimationCoroutine);
            _sceneryAnimationCoroutine = StartCoroutine(SceneryAnimation());
        }

        private void OnNewSessionSeed(int seed)
        {
            _sessionSeed = seed;
        }

        private void OnPlayerCameraTransformUpdated(PlayerDetails playerDetails)
        {
            _localPlayerDetails = playerDetails;
        }

        private void OnEnablePostGameCamera()
        {
            Properties.PostGameCamera.gameObject.SetActive(true);
        }

        private void OnReset()
        {
            if (_sceneryGenerationCoroutine != null) StopCoroutine(_sceneryGenerationCoroutine);
            if (_sceneryAnimationCoroutine != null) StopCoroutine(_sceneryAnimationCoroutine);

            _animateScenery = false;
            _sceneryPool.Clear();
            _sceneryInMotion.Clear();

            foreach (var go in _generatedGameObjectRoots)
            {
                Destroy(go);
            }

            _generatedGameObjectRoots.Clear();

            Properties.ArenaCamera.gameObject.SetActive(true);
            Properties.PostGameCamera.gameObject.SetActive(false);
        }

        private void ReportComplete(Transition transition)
        {
            if (transition == Transition.TileGeneration) _tilesReady = true;
            if (transition == Transition.CameraTransition) _cameraReady = true;

            if (_cameraReady && _tilesReady)
            {

                Presenter.AlertGenerationComplete();
                _cameraReady = _tilesReady = false;
            }
        }
    }
}
