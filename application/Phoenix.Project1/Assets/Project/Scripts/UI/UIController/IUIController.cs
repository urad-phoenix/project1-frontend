using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public abstract class IUIController : MonoBehaviour
    {
        [SerializeField] 
        protected UILoader _Loader;

        protected IUIQueryable _Queryable;

        private void Awake()
        {    
            _Loader.LoadCompletedEvent += LoadCompleted;
        }

        private void LoadCompleted()
        {
            _LoadCompleted(_Loader);
        }

        private void _LoadCompleted(IUIQueryable uiQueryable)
        {
            _Queryable = uiQueryable;
            
            _Loaded();
        }

        protected abstract void _Loaded();

        private void OnDestroy()
        {
            _Loader.LoadCompletedEvent -= LoadCompleted;
        }
    }
}