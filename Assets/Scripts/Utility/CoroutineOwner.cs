using UnityEngine;

namespace RR.Utility.Gameplay
{
    class CoroutineOwner : MonoBehaviour
    {
        private void Start() 
        {
            transform.SetParent(FindObjectOfType<CoroutineOwnerContainer>().transform);
        }
    }
}
