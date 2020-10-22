using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phoenix.Project1.Client.UI
{
    public enum UILayer
    {
        Main = 0,
        Mask = 1,
        MessageBox = 2,
        Console
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

        public T SetToLayer<T>(UILayer order, T component) where T : Component
        {
            return SetLayerAndRect(order, component);
        }

        private T SetLayerAndRect<T>(UILayer order, T component) where T : Component
        {
            if(component == null)
                return default(T);

            var canvas = GetCanvas(order);

            if(canvas != null)
            {
                SetTransformParent(component.transform, canvas.transform);
            }

            return component;
        }

        public void SetToParent(Transform transform, Transform parent)
        {
            SetTransformParent(transform, parent);
        }

        private void SetTransformParent(Transform transform, Transform parent)
        {
            var rectTransform = transform.GetComponent<RectTransform>();

            var localPosition = transform.localPosition;
            var localRotation = transform.localRotation;
            var localScale = transform.localScale;
            var sizeDelta = rectTransform.sizeDelta;

            rectTransform.SetParent(parent.transform);
            rectTransform.localPosition = localPosition;
            rectTransform.localRotation = localRotation;
            rectTransform.localScale = localScale;
            rectTransform.sizeDelta = sizeDelta;
                
            rectTransform.SetAsLastSibling();
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
