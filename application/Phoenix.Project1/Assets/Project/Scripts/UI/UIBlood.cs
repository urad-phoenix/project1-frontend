using System;
using DG.Tweening;
using Phoenix.Project1.Client;
using Phoenix.Project1.Common.Battles;
using Regulus.Remote.Reactive;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIBlood : MonoBehaviour
{
    [SerializeField] 
    private Slider _RealBloodSlider;

    [SerializeField]
    private Slider _AnimBloodSlider;

    [SerializeField] 
    private Image _AnimBooldImage; 

    [SerializeField]
    private Color _IncreaseColor;
    
    [SerializeField] 
    private Color _ResuceColor;

    [SerializeField]
    private float _Duration;
    
    [SerializeField]
    private int _CurrentBloodValue;
    
    [SerializeField]
    private int _MaxBloodValue;

    private bool _Isinitialized;

    private int _UpdateValue;

    private CompositeDisposable _Disposable;

    public UIBlood()
    {
        _Disposable = new CompositeDisposable();
    }  

    public void Init(int value)
    {
        _Isinitialized = true;
        _MaxBloodValue = value;
        _CurrentBloodValue = value;
        
        _AnimBloodSlider.maxValue = value;
        _RealBloodSlider.maxValue = value;
        
        _AnimBloodSlider.minValue = 0;
        _RealBloodSlider.minValue = 0;

        _AnimBloodSlider.value = value;
        _RealBloodSlider.value = value;
    }   
    
    public void SetCurrentBlood(int value)
    {
        if (!_Isinitialized)
        {
            Init(value);
            //return;
        }

//        _CurrentBloodValue = value;
//        _AnimBloodSlider.value = _CurrentBloodValue;
//        _RealBloodSlider.value = _CurrentBloodValue;
    }

    public void ReduceValue(int value)
    {
        var targetValue = Mathf.Max(0, _CurrentBloodValue - value);

        _CurrentBloodValue = targetValue;
        
        _RealBloodSlider.value = targetValue;

        _AnimBooldImage.color = _ResuceColor;
        
        _AnimBloodSlider.DOValue(targetValue, _Duration);      
    }
    
    public void IncreaseValue(int value)
    {
        var targetValue = Mathf.Min(_CurrentBloodValue + value, _MaxBloodValue);

        _CurrentBloodValue = targetValue;
        
        _AnimBloodSlider.value = targetValue;

        _AnimBooldImage.color = _IncreaseColor;
        
        _RealBloodSlider.DOValue(targetValue, _Duration);        
    }

    private void OnDestroy()
    {
        _Disposable.Clear();
    }
}
