using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Phoenix.Project1.Editors
{
    public class TimelineEditorSceneWindow : DataSearchWindowBase<TimelineEditorSceneWindow>
    {      
        private string _SourceFileType = ".prefab";

        private string _TargetTexPath;
        
        [MenuItem("Phoenix/OpenEditorScene")]
        public static void OpenEditorScene()
        {
            EditorSceneManager.OpenScene("Assets/Tests/Scene/TimelineEditorScene.unity", OpenSceneMode.Single);

            var windows = GetWindow<TimelineEditorSceneWindow>();
            windows.titleContent = new GUIContent("TimelineEditorWindow");
            windows.minSize = new Vector2(640, 320);            
        }

        protected override void OnEnable()
        {
            _SourcePath = "Assets/Project/Assetbundles/Prefabs/Characters/";
            base.OnEnable();
        }

        protected override void OnGUI()
        {
            _SourcePath = DataSearchView.SelectSourceView(_SourcePath, ClearData);
			
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {				
                DrawSourceTypeView();

                SearchSourceButton();
            }
            EditorGUILayout.EndVertical();

            SourceResultScrollView();

            GenerateButton();
        }
        
        private void DrawSourceTypeView()
        {
            EditorGUILayout.BeginHorizontal();
            {	
                EditorGUILayout.LabelField("SourceType", GUILayout.Width(100));
                _SourceFileType = EditorGUILayout.TextField(_SourceFileType);								
            }
            EditorGUILayout.EndHorizontal();
        }

        protected override bool SourcePathCheck()
        {
            if (string.IsNullOrEmpty(_SourceFileType))
            {
                EditorUtility.DisplayDialog("資料錯誤", "請填寫檔案類型", "OK");
                return false;
            }

            if (string.IsNullOrEmpty(_SourcePath))
            {
                EditorUtility.DisplayDialog("資料錯誤", "請選擇資源資料夾", "OK");
                return false;
            }

            return true;
        }

        private void GenerateButton()
        {
            if (GUILayout.Button("GenerateRole"))
            {
                CreateRole();
            }
        }

        private void CreateRole()
        {
//            if (_SourceDataList.Count() == 0)
//            {
//                EditorUtility.DisplayDialog("資料錯誤", "請填寫檔案類型", "OK");
//                return false;
//            }
        }

        private void RoleBinding()
        {
            
        }

        protected override string GetExtraType()
        {
            return _SourceFileType;
        }
    }
}