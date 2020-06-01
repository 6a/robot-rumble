using UnityEngine;
using RR.Utility;
using System.Collections.Generic;

namespace RR.Facilitators.Scenery.Impl
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Scenery : MonoBehaviour, IScenery
    {
        private Color _currentColor;
        private Range _yRange;
        private float _speed;
        private Queue<IScenery> _poolReference;
        private List<IScenery> _sceneryInMotion;

        public void Init(Color color, Range yRange, float unitsPerSecond, Queue<IScenery> pool, List<IScenery> sceneryInMotion)
        {
            _currentColor = color;
            GetComponent<MeshRenderer>().materials[0].SetColor("_MainColor", _currentColor);

            _yRange = yRange;
            _speed = unitsPerSecond;
            _poolReference = pool;
            _sceneryInMotion = sceneryInMotion;
        }

        public void Animate()
        {
            var translationDelta = _speed * Time.deltaTime;
            var newY = _yRange.ClampToRange(gameObject.transform.position.y + translationDelta);

            var pos = gameObject.transform.position;
            pos.y = newY;
            gameObject.transform.position = pos;

            if (newY >= _yRange.Max)
            {
                _sceneryInMotion.Remove(this);
                _poolReference.Enqueue(this);
            }
        }
    }
}
