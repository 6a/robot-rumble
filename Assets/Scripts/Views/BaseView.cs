using RR.Presenters;
using RR.Properties;
using UnityEngine;

namespace RR.Views
{
    public class BasePhotonView : Photon.MonoBehaviour
    {
        public string Name
        {
            get
            {
                return gameObject.name;
            }
        }

        private void Start()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void EnterState()
        {

        }

        public virtual void ExitState()
        {

        }

        public virtual void Dispose()
        {

        }
    }

    public class BaseView : MonoBehaviour
    {
        public string Name
        {
            get
            {
                return gameObject.name;
            }
        }

        private void Start()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            
        }

        public virtual void EnterState()
        {

        }

        public virtual void ExitState()
        {

        }

        public virtual void Dispose()
        {

        }
    }
    
    public class BaseView<T> : BaseView where T : BasePresenter
    {
        private T _presenter;
        
        public T Presenter
        {
            get
            {
                if (_presenter == null)
                {
                    _presenter = GetComponent<T>();
                }
                
                if (_presenter == null)
                {
                    Debug.LogWarning("Warning: No presenter was found on {0}", this);
                }

                return _presenter;
            }
        }
    }
    
    public abstract class BaseView<T,W> : BaseView<T> where T : BasePresenter where W : BaseProperties
    {
        private W _properties;

        public W Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = GetComponent<W>();
                }
                
                return _properties;
            }
        }
    }

    public class BasePhotonView<T> : BasePhotonView where T : BasePresenter
    {
        private T _presenter;
        
        public T Presenter
        {
            get
            {
                if (_presenter == null)
                {
                    _presenter = GetComponent<T>();
                }
                
                if (_presenter == null)
                {
                    Debug.LogWarning("Warning: No presenter was found on {0}", this);
                }

                return _presenter;
            }
        }
    }
    
    public abstract class BasePhotonView<T,W> : BasePhotonView<T> where T : BasePresenter where W : BaseProperties
    {
        private W _properties;

        public W Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = GetComponent<W>();
                }
                
                return _properties;
            }
        }
    }
}