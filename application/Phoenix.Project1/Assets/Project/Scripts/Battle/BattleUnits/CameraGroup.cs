using System;
using System.Linq;
using Cinemachine;
using UnityEngine;

namespace Phoenix.Project1.Client.Battles
{
    public class CameraGroup : MonoBehaviour
    {
        [Serializable]
        public class CameraData
        {
            public int Index;
            public CinemachineVirtualCamera VirtualCamera;
        }

        public CameraData[] CameraDatas;


        public CinemachineVirtualCamera GetVirtualCamera(int index)
        {
            try
            {
                var cameraData = CameraDatas.First(x => x.Index == index);

                return cameraData.VirtualCamera;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }
    }
}