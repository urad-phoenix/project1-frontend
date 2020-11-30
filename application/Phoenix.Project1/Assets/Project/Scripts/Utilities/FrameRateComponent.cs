using UnityEngine;

public class FrameRateComponent : MonoBehaviour
{
    public int FrameRate = 30;
    // Start is called before the first frame update
    private void Awake()
    {
        Application.targetFrameRate = FrameRate;
    }  
}
