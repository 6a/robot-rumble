using UnityEngine;
using System.Collections.Generic;
using RR.Facilitators.Platform.Impl;
using RR.Facilitators.Scenery.Impl;
using RR.Utility;

namespace RR.Properties
{
    public class ArenaProperties : BaseProperties
    {
        [Header("Arena generation")]
        [Tooltip("The number of rings that the platform should have")][Range(1, 15)] public int Rings;
        public float TileGenerationTime;
        public Tile TilePrefab;
        public Tile PillarPrefab;
        public Color TileInnerColor;
        public Color TileOuterColor;

        [Header("Camera transition")]
        public GameObject CameraContainer;
        public Camera ArenaCamera;
        public Camera PostGameCamera;
        public float CameraTransitionEndHeight;
        public float CameraTransitionEndZ;
        public float CameraTransitionDuration;

        [Header("Arena Pedestals")]
        public List<Pedestal> Pedestals = new List<Pedestal>();
        public int PedestalHeight = 4;
        public Color[] PedestalColors;

        [Header("Arena Scenery")]
        public Scenery[] SceneryPrefabs;
        [Tooltip("Rate in units per second")][Range(5f, 100f)] public float ScenerySpawnrate;
        public int SceneryPoolMaxSize;
        public Range SceneryYRange;
        public Range SceneryDistanceFromCenterRange;
        public Range SceneryUnitsPerSecondRange;
    }
}
