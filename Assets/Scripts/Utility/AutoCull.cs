using UnityEngine;
using System.Collections;

namespace RR.Utility.Gameplay
{
    public interface IAutoCull
    {
        void Init(float lifespan = 10f);
    }

    public class AutoCull : MonoBehaviour, IAutoCull
    {
        private float _lifespan = 10f;

        private void Start()
        {
            StartCoroutine(Cull());
        }

        public void Init(float lifespan = 10f)
        {
            _lifespan = lifespan;
        }
        
        private IEnumerator Cull ()
        {
            yield return new WaitForSeconds(_lifespan);
            Destroy(gameObject);
        }
    }
}