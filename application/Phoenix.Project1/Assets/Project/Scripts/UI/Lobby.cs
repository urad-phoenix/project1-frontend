using System;
using UnityEngine;
namespace Phoenix.Project1.Client.UI
{
    public class Lobby : MonoBehaviour
    {
        internal void Hide()
        {
            gameObject.SetActive(false);
        }

        internal void Show()
        {
            gameObject.SetActive(true);
        }
    }

}
