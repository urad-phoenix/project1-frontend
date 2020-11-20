using System;
using DG.Tweening;
using Phoenix.Project1.Client.Utilities.RxExtensions;
using UniRx;
using UnityEngine;
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

    [SerializeField]
    private UnityEngine.UI.Text _Text;

    [SerializeField]
    private bool _IsAutoDestroy;
    
    public event Action<GameObject> OnCompletedEvent;

    private RectTransform _RectTransform;   
    
    public void SetTexture(string text)
    {
        if(_RectTransform == null)
            _RectTransform = GetComponent<RectTransform>();

        _RectTransform.DOLocalJump(
            new Vector3(Random.Range(_EndXRandomRange.x, _EndXRandomRange.y),
                Random.Range(_EndYRandomRange.x, _EndYRandomRange.y), 0.0f),
            Random.Range(_JumpYRandomRange.x, _JumpYRandomRange.y), 1, _Duration).OnComplete(_Complete);
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
        return _RectTransform.DOLocalJump(
            new Vector3(Random.Range(_EndXRandomRange.x, _EndXRandomRange.y),
                Random.Range(_EndYRandomRange.x, _EndYRandomRange.y), 0.0f),
            Random.Range(_JumpYRandomRange.x, _JumpYRandomRange.y), 1, _Duration).OnComplete(_Complete).OnCompleteAsObservable();        
    }

    void _Complete()
    {
        OnCompletedEvent?.Invoke(this.gameObject);
        
        if(_IsAutoDestroy)
            Destroy(gameObject);
    }     
}
