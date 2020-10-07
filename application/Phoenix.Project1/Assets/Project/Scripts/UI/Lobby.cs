using Phoenix.Project1.Common.Users;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
namespace Phoenix.Project1.Client.UI
{
    public class Lobby : MonoBehaviour
    {
        public UnityEngine.GameObject ActorPrefab;
        public UnityEngine.Transform ActorParent;
        readonly UniRx.CompositeDisposable _Disposables;

        readonly System.Collections.Generic.List<Actor> _Actors;

        public Lobby()
        {

            _Actors = new System.Collections.Generic.List<Actor>();
            _Disposables = new UniRx.CompositeDisposable();
        }
        internal void Hide()
        {
            gameObject.SetActive(false);

            
        }
        private void OnDestroy()
        {
            _Disposables.Dispose();
        }
        internal void Show()
        {
            _Disposables.Clear();
            gameObject.SetActive(true);

            NotifierRx.ToObservable().Supply<IActor>().Subscribe(_AddActor).AddTo(_Disposables);
            NotifierRx.ToObservable().Unsupply<IActor>().Subscribe(_RemoveActor).AddTo(_Disposables);
        }

        private void _RemoveActor(IActor gpi)
        {
            var actor = _Actors.SingleOrDefault(a => a.Id == gpi.Id);
            if (actor == null)
                return;
            GameObject.Destroy(actor.gameObject);
            _Actors.Remove(actor);
        }

        private void _AddActor(IActor actor)
        {
            var actorUi = GameObject.Instantiate(ActorPrefab , ActorParent);
            actorUi.SetActive(true);
            var actorComponment = actorUi.GetComponent<Actor>();
            actorComponment.Setup(actor);
            _Actors.Add(actorComponment);
        }
    }

}
