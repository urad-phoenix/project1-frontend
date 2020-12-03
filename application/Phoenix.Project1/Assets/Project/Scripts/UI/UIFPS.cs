using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFPS : MonoBehaviour
{
    [SerializeField] private Text _TitleText;
    [SerializeField] private Text _FPSText;

    private double _Fps;
    
    void Start()
    {
        _TitleText.text = "FPS";
    }

    // Update is called once per frame
    void Update()
    {
        _Fps = 1.0 / Time.deltaTime;

        _FPSText.text = _Fps.ToString();
    }
}
