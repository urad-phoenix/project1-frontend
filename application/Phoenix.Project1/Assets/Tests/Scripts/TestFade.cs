using Phoenix.Project1.Client.UI;
using System;
using UniRx;
using UnityEngine;

namespace Tests
{
    public class TestFade : MonoBehaviour
    {
        [SerializeField] private GameObject _TestTarget;

        private Transform _Clone;

        private bool _IsLock;

        public void GetUI()
        {
            if (_IsInputLock())
                return;

            if (_Clone)
            {
                Destroy(_Clone.gameObject);
            }

            var go = Instantiate(_TestTarget);

            _Clone = go.transform;

            _Clone.transform.SetLayer(UILayer.Main);
        }

        private bool _IsInputLock()
        {
            if (_IsLock)
                return true;

            _IsLock = true;

            Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ => _IsLock = false);

            return false;
        }

        public void TestFadeIn()
        {
            if (_IsInputLock())
                return;

            var observable = _Clone.transform.FadeInAsObserver();

            observable.Subscribe(_ =>
                Debug.Log("FadeIn Finidhed"));
        }

        public void TestFadeOut()
        {
            if (_IsInputLock())
                return;

            var observable = _Clone.transform.FadeOutAsObserver();

            observable.Subscribe(_ =>
                Debug.Log("FadeOut Finidhed"));
        }
    }
}