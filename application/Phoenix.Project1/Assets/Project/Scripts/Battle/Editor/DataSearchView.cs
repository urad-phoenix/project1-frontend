using System;
using System.IO;

namespace Phoenix.Project1.Editors
{
    using UnityEditor;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class SourceData
    {
        public GUIContent Content;
        public string AssetPath;
        public Object DataAssets;
        public bool IsSelected;
        public bool IsToggleSelected;
    }
    
    public static class DataSearchView
    {        
        public static string SelectSourceView(string path, Action callback)
        {
            EditorGUILayout.BeginHorizontal();
            			
                EditorGUILayout.LabelField("SourcePath", GUILayout.Width(100));
                var selectPath = EditorGUILayout.TextField(path);
				
                if (GUILayout.Button("SelectFolder"))
                {
                    selectPath = SelectSourcesFolder(selectPath);
                    
                    if(callback != null)
                        callback.Invoke();
                }
            
            EditorGUILayout.EndHorizontal();

            return selectPath;
        }
        
        private static string SelectSourcesFolder(string path)
        {           
            return EditorUtility.OpenFolderPanel("選擇目標資料夾", path, "");						
        }
        
        public static void NameRuleView(string splitChar, int ruleIndex, string[] ruleList, Action<string, int> changeCallback)
        {
            EditorGUILayout.BeginHorizontal();
            
                EditorGUILayout.LabelField("SplitChart", GUILayout.Width(100));
                var  slplit = EditorGUILayout.TextField(splitChar);
				
                EditorGUILayout.LabelField("NumberRuleCount", GUILayout.Width(100));
                //m_NumberRuleCount = EditorGUILayout.IntField(m_NumberRuleCount);
                var index = EditorGUILayout.Popup(ruleIndex, ruleList);
            
            EditorGUILayout.EndHorizontal();

            if (slplit != splitChar || index != ruleIndex)
            {
                changeCallback.Invoke(slplit, index);
            }
        }      
        
        public static void DrawSourceContent(List<SourceData> sourceDatas, int selectIndex, Action<List<SourceData>, int> changeCallback)
        {			
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var index = selectIndex;

            bool isChange = false;
            
            for(int i = 0; i < sourceDatas.Count; ++i)
            {
                var data = sourceDatas[i];

                GUIStyle style = "Label";
            
                Rect rt = GUILayoutUtility.GetRect (data.Content, style);
            
                rt.position = new Vector2(rt.position.x + 30, rt.position.y);
            
                Rect toggleRt = GUILayoutUtility.GetLastRect();
                toggleRt.width = 20;
                toggleRt.height = 20;
            
                if (data.IsSelected)
                {
                    EditorGUI.DrawRect(rt, Color.gray);
                }

                GUILayout.BeginHorizontal("box");
                {                    
                    var toggle = GUI.Toggle(toggleRt, data.IsToggleSelected, "");

                    //修改原始資料
                    if (data.IsToggleSelected != toggle)
                    {
//                        var sourceData = sourceDatas.Find(x => x.DataAssets == data.DataAssets);
//                        sourceData.IsToggleSelected = toggle;
                        data.IsToggleSelected = toggle;
                        isChange = true;
                    }
                
                    if (GUI.Button(rt, data.Content, style))
                    {
                        isChange = true;
                        Selection.activeObject = data.DataAssets;
                        index = i;
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (index > -1)
            {
                for (int i = 0; i < sourceDatas.Count; ++i)
                {
                    var data = sourceDatas[i];

                    if (i != selectIndex)
                    {
                        data.IsSelected = false;
                        continue;						
                    }

                    data.IsSelected = true;
                }				
            }
            EditorGUILayout.EndVertical();

            if (isChange)
            {
                if (changeCallback != null)
                {
                    changeCallback(sourceDatas, index);
                }
            }
        }
        
        public static void SourceResultScrollView(List<SourceData> sourceDatas, string searchName, bool isAllSelect, Action<List<SourceData>, string, bool, bool> changeCallback)
        {           
            var isChange = false;
            var isRefresh = false;
            
            EditorGUILayout.BeginHorizontal();
            
            var naem = EditorGUILayout.TextField("Search Result", searchName);

            if (searchName != naem)
            {
                isChange = true;
            }
            
            if (GUILayout.Button("SearchResult"))
            {
                isChange = true;
                isRefresh = true;
            }
            
            EditorGUILayout.EndHorizontal();
          
            var allSelected = EditorGUILayout.Toggle("AllSelect", isAllSelect);
                            
            if (allSelected != isAllSelect)
            {
                isAllSelect = allSelected;
                isChange = true;
                for (int i = 0; i < sourceDatas.Count; ++i)
                {
                    var data = sourceDatas[i];
                    data.IsToggleSelected = allSelected;
                }
            }                                                
            
            if (isChange)
            {
                searchName = naem;
                
                changeCallback(sourceDatas, searchName, isAllSelect, isRefresh);
            }
        }
        
        public static void SearchSource(string sourcePath, string extraFileType, Action<List<SourceData>> callback)
        {			          
            List<SourceData> resultList = new List<SourceData>();
            string[] files = Directory.GetFiles(sourcePath, "*",SearchOption.AllDirectories);
			
            foreach (string file in files)
            {
                if (file.Contains(".meta"))				
                    continue;

                bool isContains = true;
                
                if (!string.IsNullOrEmpty(extraFileType) && !file.Contains(extraFileType))
                {
                   continue;
                }                  
                
                var assetPath = PathStringUtilities.AssetPath(file);
				
                if (!string.IsNullOrEmpty(assetPath))
                {
                    var data = GetSourceData(assetPath);
					
                    if(data != null)						
                        resultList.Add(data);
                }
            }                       
            
            callback.Invoke(resultList);
        }
        
        private static SourceData GetSourceData (string path)
        {			
            Object asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                        
            if (asset)
            {                                                          
                SourceData data = new SourceData();
                data.Content = new GUIContent(asset.name, AssetDatabase.GetCachedIcon(path));
                data.AssetPath = path;
                data.DataAssets = asset;
                return data;
            }
            return null;
        }
    }    
}