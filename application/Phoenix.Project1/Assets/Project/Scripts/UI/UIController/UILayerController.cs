using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public enum UILayer
    {
        Main = 0,
        Mask = 1,
        MessageBox = 2,
    }

    [Serializable]
    public class UILayerData
    {
        public UILayer Oder;
        public Canvas Canvas;
    }
    public class UILayerController : MonoBehaviour
    {
        public static UILayerController Instance => _GetInstance();

        private static UILayerController _GetInstance()
        {
            return FindObjectOfType<UILayerController>();
        }

        //default setting
        [SerializeField]
        private List<UILayerData> _CanvasList = new List<UILayerData>()
        {
            new UILayerData(){ Oder = UILayer.Main },
            new UILayerData(){ Oder = UILayer.Mask },
            new UILayerData(){ Oder = UILayer.MessageBox },
        };

        public Canvas GetCanvas(UILayer layer)
        {
            var data = _CanvasList.Find(x => x.Oder == layer);

            return data?.Canvas;
        }

        public T MoveToLayer<T>(UILayer order, T component) where T : Component
        {
            if(component == null)
                return default(T);

            var canvas = GetCanvas(order);

            if(canvas != null)
            {
                var rectTransform = component.GetComponent<RectTransform>();
                rectTransform.SetParent(canvas.transform);
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.SetAsLastSibling();
            }

            return component;
        }

        public Camera GetUICamera(UILayer layer)
        {
            var canvas = GetCanvas(layer);

            return canvas.worldCamera;
        }

        public static void SetLayer()
        {
        }
    }       
}
