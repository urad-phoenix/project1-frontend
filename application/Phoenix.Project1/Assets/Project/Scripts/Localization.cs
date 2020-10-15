using UniRx;
using UnityEngine;

public class Localization : MonoBehaviour
{
    public int Identify;
    
    readonly UniRx.CompositeDisposable _UIDisposables;

    public Localization()
    {
        _UIDisposables = new UniRx.CompositeDisposable();
    }
    private void OnDestroy()
    {
        _UIDisposables.Clear();
    }
    // Start is called before the first frame update
    void Start()
    {
        Configuration.ToObservable().Subscribe(_Switch).AddTo(_UIDisposables);
    }

    private void _Switch(Configuration configuration)
    {
        var Source = gameObject.GetComponent<UnityEngine.UI.Text>(); 
        Source.text = configuration.GetText(Identify);
    }
    
}
