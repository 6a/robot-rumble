using RR.Utility;
using UnityEngine;

namespace RR.Presenters
{
    public abstract class BasePresenter : MonoBehaviour
    {
        protected ModelGetter Models = new ModelGetter();
        protected EventDispatcher EventDispatcher = new EventDispatcher();

        #region Lifecycle
        
        public virtual void Initialize()
        {
            
        }

        public virtual void OnEnterState()
        {

        }

        public virtual void OnExitState()
        {

        }

        public virtual void Dispose()
        {

        }

        #endregion
    }
}
