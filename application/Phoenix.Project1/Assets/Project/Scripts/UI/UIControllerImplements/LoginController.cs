using System.Collections;
using System.Collections.Generic;
using Phoenix.Project1.Client.UI;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public class LoginController : IUIController
    {
        private GameObject _Verify;

        private void Start()
        {            
            _Loader.Load();                        
        }

        public void Loaded(GameObject queryable)
        {
        }

        protected override void _Loaded()
        {
            _Verify = _Queryable.Query("Verify");
            _Verify.SetActive(true);
        }
    }
}