using System;
using Phoenix.Playables;
using Phoenix.Project1.Client.Battles;
using Phoenix.Project1.Common.Battles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Editors
{
    [CustomEditor(typeof(TriggerDirectorClip))]
    public class TriggerDirectorClipInspector : Editor
    {
        private TriggerDirectorClip _Clip;

        private MotionType _Type;

        private PlayableDirector _Director;

        private void OnEnable()
        {
            _Clip = target as TriggerDirectorClip;

            try
            {
                _Type = (MotionType) (string.IsNullOrEmpty(_Clip.AssetKey)
                    ? ((MotionType) 0)
                    : Enum.Parse(typeof(MotionType), _Clip.AssetKey));
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                _Type = (MotionType) 0;
                _Clip.AssetKey = _Type.ToString();
            }
            

            _Director = _Clip.Director.defaultValue as PlayableDirector;
        }

        public override void OnInspectorGUI()
        {
            _Clip.TimelineAsset = EditorGUILayout.ObjectField("TimelineAsset", _Clip.TimelineAsset, typeof(TimelineAsset)) as TimelineAsset;

            _Director = EditorGUILayout.ObjectField("Director", _Director, typeof(PlayableDirector)) as PlayableDirector;
            
            _Type = (MotionType)EditorGUILayout.EnumPopup("ActionKey", _Type);

            if (GUI.changed)
            {
                _Clip.AssetKey = _Type.ToString();
                _Clip.Director.defaultValue = _Director;
            }
        }
    }
}