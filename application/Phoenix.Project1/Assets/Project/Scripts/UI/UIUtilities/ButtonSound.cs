using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Project.Scripts.UI.UIUtilities
{
    public enum SoundMode
    {
        Internal,
        Custom,
    }
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {
        [HideInInspector]
        public SoundMode Mode;       

        [HideInInspector]
        public string AudioKey;
      
        private void Start()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(_Click);
        }

        private void _Click()
        {
            //TODO Sound manager play sound
            if(Mode == SoundMode.Internal)
            {

            }
        }
    }
}
