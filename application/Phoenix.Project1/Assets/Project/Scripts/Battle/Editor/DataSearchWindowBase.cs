using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Phoenix.Project1.Editors
{    
    public abstract class DataSearchWindowBase<T> : EditorWindow where T : DataSearchWindowBase<T>
    {              
        protected string _SourcePath;        

        protected List<SourceData> _SourceDataList;

        private Vector2 _SourceScroll = Vector2.zero;
        
        private List<SourceData> _DrawDataList;
        
        private bool _IsAllSelect;

        private string _SearchName;
        
        private int _SelectedIndex = -1;

        protected virtual void OnEnable()
        {
            _IsAllSelect = false;           
            _SourceScroll = Vector2.zero;
            _SourceDataList = new List<SourceData>();
            _SelectedIndex = -1;
            _DrawDataList = new List<SourceData>();
        }

        protected abstract void OnGUI();

        protected void SearchSourceButton()
        {
            EditorGUILayout.BeginHorizontal();
            {				
                if (GUILayout.Button("Search"))
                {
                    if (!SourcePathCheck())
                    {
                        return;				
                    }
					
                    SearchSource();           
                }	
            }
            EditorGUILayout.EndHorizontal();
        }

        protected abstract bool SourcePathCheck();                   
        
        protected void SourceResultScrollView()
        {
            if (_SourceDataList.Count == 0)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            DataSearchView.SourceResultScrollView(_DrawDataList, _SearchName, _IsAllSelect,
                                                  SourceResultSelectCallback);
            
            _SourceScroll = GUILayout.BeginScrollView(_SourceScroll, GUILayout.Height(400));
            {
                DataSearchView.DrawSourceContent(_DrawDataList, _SelectedIndex, DrawSourceContentCallback);
                //DrawSourceContent();
            }
            GUILayout.EndScrollView();
        
            EditorGUILayout.EndVertical();            
        }
        
        private void SourceResultSelectCallback(List<SourceData> list, string searchName, bool isAllSelect,
                                                bool isRefresh)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var data = list[i];
                var sourceData = _SourceDataList.Find(x => x.DataAssets == data.DataAssets);
                sourceData.IsToggleSelected = data.IsToggleSelected;
            }

            _SearchName = searchName;

            _IsAllSelect = isAllSelect;

            if (isRefresh || string.IsNullOrEmpty(_SearchName))
            {
                RefreshDrawList();
            }
        }

        private void DrawSourceContentCallback(List<SourceData> list, int selectIndex)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                var data = list[i];
                var sourceData = _SourceDataList.Find(x => x.DataAssets == data.DataAssets);
                sourceData.IsToggleSelected = data.IsToggleSelected;
            }

            _SelectedIndex = selectIndex;
        }

        protected virtual void SearchResultCallback(List<SourceData> sourceDatas)
        {
            _IsAllSelect = false;
            if (sourceDatas.Count == 0)
            {
                EditorUtility.DisplayDialog("搜尋結果", "沒有資源", "OK");
                return;
            }
            
            _SourceDataList.AddRange(sourceDatas);
            RefreshDrawList();
        }
    
        protected void SearchSource()
        {
            ClearData();
            _SearchName = "";
            DataSearchView.SearchSource(_SourcePath, GetExtraType(), SearchResultCallback);
        }

        protected abstract string GetExtraType();
        
        protected void RefreshDrawList()
        {            
            _DrawDataList.Clear();
            if (string.IsNullOrEmpty(_SearchName))
            {
                _DrawDataList.AddRange(_SourceDataList);    
            }
            else
            {
                var list = _SourceDataList.FindAll(x => x.DataAssets.name.Equals(_SearchName) || x.DataAssets.name.Contains(_SearchName));
                
                _DrawDataList.AddRange(list);
            }                        
        }
        
        protected void ClearData()
        {
            _SourceDataList.Clear();
            _DrawDataList.Clear();
        }
    }  
}