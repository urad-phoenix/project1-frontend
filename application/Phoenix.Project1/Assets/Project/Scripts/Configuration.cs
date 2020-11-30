using Phoenix.Project1.Configs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client
{
    public class Configuration : MonoBehaviour
    {
        private Phoenix.Project1.Game.Configuration _Configuration;
        public Cultures Culture;
        public UnityEngine.TextAsset DatabaseJson;

        public static Configuration Instance => _GetInstance();

        private static Configuration _GetInstance()
        {
            return UnityEngine.GameObject.FindObjectOfType<Configuration>();
        }

        public Phoenix.Project1.Game.Configuration Resource => _GetResource();

        private Phoenix.Project1.Game.Configuration _GetResource()
        {
            return _GetInstance()._Configuration;
        }

        public static System.IObservable<Configuration> ToObservable()
        {
            return UniRx.Observable.FromCoroutine<Configuration>(_RunWaitAgent);
        }

        private static IEnumerator _RunWaitAgent(System.IObserver<Configuration> observer)
        {
            while (Instance == null || !Instance._IsReady())
            {
                yield return new WaitForEndOfFrame();
            }
            observer.OnNext(Instance);
            observer.OnCompleted();
        }

        public void Start()
        {
            _BuildResource();
        }

        private void _BuildResource()
        {
            var db = Phoenix.Project1.Configs.Database.DeserializeJson(DatabaseJson.text);
            var database = new Regulus.RelationalTables.Database(db.Tables);
            _Configuration = new Phoenix.Project1.Game.Configuration(database);
        }

        public string GetText(int id, params string[] args)
        {
            var txt = from localization in _Configuration.Localizations
                      where localization.Id == id
                      select localization;
            var str = txt.SingleOrDefault().GetText(Culture);
            return string.Format(str, args);
        }

        private bool _IsReady()
        {
            return _Configuration != null;
        }

        public IEnumerable<T> Query<T>()
        {
            return _Configuration.Query<T>();
        }
    }
}