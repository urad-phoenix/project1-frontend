using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public abstract class IUIController : MonoBehaviour
    {
        [SerializeField] 
        protected UILoader _Loader;

        protected IUIQueryable _Queryable;
        
        protected virtual void Start()
        {
            _Loader.LoadCompletedEvent += LoadCompleted;
        }

        private void LoadCompleted(IUIQueryable uiQueryable)
        {
            _Queryable = uiQueryable;
            
            _Loaded();
        }

        protected abstract void _Loaded();
    }
}