using UniRx;
using UnityEngine;

public class LocalizationText : MonoBehaviour
{
    public int Identify;
    
    readonly UniRx.CompositeDisposable _UIDisposables;

    public LocalizationText()
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
        Localization.ToObservable().Subscribe(_Switch).AddTo(_UIDisposables);
    }

    private void _Switch(Localization localization)
    {
        var Source = gameObject.GetComponentInParent<UnityEngine.UI.Text>(); 
        Source.text = localization.GetText(Identify);
    }
    
}
