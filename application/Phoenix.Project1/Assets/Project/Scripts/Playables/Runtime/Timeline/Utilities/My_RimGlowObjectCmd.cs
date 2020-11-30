using UnityEngine;
using System.Collections.Generic;

namespace Phoenix.Playables.Utilities
{
    public class My_RimGlowObjectCmd : MonoBehaviour
    {
        bool UseCommandBuff;
        public Camera MainC;

        public Renderer[] Renderers
        {
            get;
            private set;
        }

        public Color CurrentColor
        {
            get
            {
                return _currentColor;
            }
        }

        private Color _currentColor;
        private Color _targetColor;

        void Start()
        {
            Renderers = GetComponentsInChildren<Renderer>();
            My_RimGlowController.RegisterObject(this);
            foreach(var i in Camera.allCameras)
            {
                if(i.gameObject.tag == "FightCamera")
                {
                    MainC = i;
                }
            }
        }
        /*
        void OnWillRenderObject()
        {
            if (!MainC.GetComponent<My_RimGlowController>().enabled) MainC.GetComponent<My_RimGlowController>().enabled = true;
            if (UseCommandBuff) MainC.GetComponent<My_GlowComposite>().enabled = true;

             UseCommandBuff = true;

        }*/

    }
}