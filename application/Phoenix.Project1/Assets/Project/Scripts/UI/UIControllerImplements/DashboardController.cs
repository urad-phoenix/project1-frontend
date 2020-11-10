using Phoenix.Project1.Common;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public class DashboardController : IUIController
    {
        private GameObject _Dashboard;

        private void Start()
        {            
            _Loader.Load();                        
        }
        
        protected override void _Loaded()
        {
            _Dashboard = _Queryable.Query("DashboardUI");
            _Dashboard.SetActive(true);
            _Dashboard.transform.SetAsFirstSibling();
        }
    }
}