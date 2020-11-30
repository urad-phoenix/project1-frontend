using System;
using DG.Tweening;
using Phoenix.Project1.Client.Utilities.RxExtensions;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class TextJumpComponent : MonoBehaviour
{    
    [Space(1)]
    [Header("End Setting")]
    [SerializeField]
    private Vector2 _EndXRandomRange;
    
    [SerializeField]
    private Vector2 _EndYRandomRange;
    
    [Space(1)]
    [Header("Jump Setting")]
    
    [SerializeField]
    private Vector2 _JumpYRandomRange;       
    
    [Header("Time")]
    [SerializeField]
    private float _Duration;

    public bool InvertEnd;

    [SerializeField]
    private UnityEngine.UI.Text _Text;
    
    public bool IsAutoDestroy;
    
    public event Action<GameObject> OnCompletedEvent;

    private RectTransform _RectTransform;

    private CompositeDisposable _Disposable = new CompositeDisposable();

    private Tween _Tween;
    
    public void SetTexture(string text)
    {
        _Text.text = text;                        

        var endPosition = new Vector3(transform.position.x + (Random.Range(_EndXRandomRange.x, _EndXRandomRange.y) * (InvertEnd ? -1 : 1)),
            transform.position.y + Random.Range(_EndYRandomRange.x, _EndYRandomRange.y), 0.0f);
        
        var jump = Random.Range(_JumpYRandomRange.x, _JumpYRandomRange.y);  
        
        transform.DOJump(endPosition, jump, 1, _Duration).OnComplete(_Complete);
    }

    public IObservable<GameObject> RegisterCompleteCallback()
    {
        return UniRx.Observable.FromEvent<Action<GameObject>, GameObject>(h => (gpi) => h(gpi), h => OnCompletedEvent += h, h => OnCompletedEvent -= h);
    }

    public IObservable<Unit> SetTextJumpAsObservable(string text)
    {
        _Text.text = text;             

        if(_RectTransform == null)
            _RectTransform = GetComponent<RectTransform>();

        return PlayTweenAsObservable();
    }

    private IObservable<Unit> PlayTweenAsObservable()
    {
        var endPosition = new Vector3(transform.position.x + (Random.Range(_EndXRandomRange.x, _EndXRandomRange.y) * (InvertEnd ? -1 : 1)),
            transform.position.y + Random.Range(_EndYRandomRange.x, _EndYRandomRange.y), 0.0f);
        
        var jump = Random.Range(_JumpYRandomRange.x, _JumpYRandomRange.y); 
        
        return transform.DOJump(endPosition, jump, 1, _Duration)
                    .OnComplete(_Complete).OnCompleteAsObservable();
    }

    void _Complete()
    {
       // _Disposable?.Dispose();        
        
        OnCompletedEvent?.Invoke(this.gameObject);

        if(IsAutoDestroy)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //_Disposable?.Dispose();
    }
}
