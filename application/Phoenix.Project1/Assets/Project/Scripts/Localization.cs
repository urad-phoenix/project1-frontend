using Phoenix.Project1.Configs;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;


public class Localization : MonoBehaviour
{
    private Configuration _Resource;
    public Cultures Culture;
    public UnityEngine.TextAsset DatabaseJson;
    
    public static Localization Instance => _GetInstance();

    private static Localization _GetInstance()
    {
        return UnityEngine.GameObject.FindObjectOfType<Localization>();
    }

    public static System.IObservable<Localization> ToObservable()
    {
        return UniRx.Observable.FromCoroutine<Localization>(_RunWaitAgent);
    }

    private static IEnumerator _RunWaitAgent(System.IObserver<Localization> observer)
    {
        while (Instance == null)
        {
            yield return new WaitForEndOfFrame();
        }
        observer.OnNext(Instance);
        observer.OnCompleted();
    }

    void Start()
    {
        _BuildResource();
    }

    public void _BuildResource()
    {
        var db = Phoenix.Project1.Configs.Database.DeserializeJson(DatabaseJson.text);
        var database = new Regulus.RelationalTables.Database(db.Tables);
        _Resource = new Configuration(database);
    }

    public string GetText(int id, params string [] args)
    {
        var txt = from localization in _Resource.Localizations 
                  where localization.Id == id
                  select localization;
        var str = txt.SingleOrDefault().GetText(Culture);
        return string.Format(str, args);
    }
}
