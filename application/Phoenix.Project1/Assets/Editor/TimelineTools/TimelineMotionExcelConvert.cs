using System.Collections.Generic;
using System.Security.Policy;
using Phoenix.Playables;
using Phoenix.Project1.Configs;
using Phoenix.Project1.DataConvertTool;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Editors.Tools
{
    public class TimelineMotionExcelConvert
    {        
        public class TimelineMotionData
        {
            public string Key;

            public TimelineAsset Asset;

        }
        [MenuItem("Phoenix/輸出MotionExcel")]
        public static void ConvertTimelineToExcel()
        {
            var convertSetting = AssetDatabase.FindAssets("SpellGeneratorSetting t:DataGeneratorSetting");

            string assetPath = "";
            
            if(convertSetting.Length > 0)
               assetPath = AssetDatabase.GUIDToAssetPath(convertSetting[0]);

            if(string.IsNullOrEmpty(assetPath))
                return;
            
            var setting = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DataGeneratorSetting)) as DataGeneratorSetting;

            var timelineAssets = TimelineOutputExcelTool.GetTimelineFiles(setting.SourcePath,  setting.FilterFolder, setting.FilterTypes);
             
            var timelineSheet = NewSheetData("sheet1", new [] {"技能表SpellKey", "是否開啟", "總幀數"});
            timelineSheet.Rows.Add(NewRowData(new[] {"Key", "__extra" ,  "TotalFrame"}));
            timelineSheet.Rows.Add(NewRowData(new[] { "Both", "Both", "Both" }));

            var hitSheet = NewSheetData("sheet1", new[] {"技能表SpellKey", "是否開啟",  "總幀數"});
            hitSheet.Rows.Add(NewRowData(new[] {"Key", "__extra",  "Frame"}));
            hitSheet.Rows.Add(NewRowData(new[] { "Both", "Both", "Both" }));

            for (int i = 0; i < timelineAssets.Count; ++i)
            {
                var spellAsset = timelineAssets[i];

                var outputTracks = spellAsset.GetOutputTracks();

                foreach (var track in outputTracks)
                {
                    var director = track as TriggerDirectorTrack;

                    if(director == null)
                        continue;
                    
                    var motions = GetMotionTimeline(director.GetClips());

                    foreach (var motion in motions)
                    {
                        var timelineData = TimelineOutputExcelTool.ConvertData(motion.Key, motion.Asset);

                        timelineSheet.Rows.Add(NewRowData(new [] {$"{spellAsset.name}_{motion.Key }","1", timelineData.TotalFrame.ToString() }));                                                                
                
                        for (int j = 0; j < timelineData.HitDatas.Count; ++j)
                        {
                            var hit = timelineData.HitDatas[j];

                            hitSheet.Rows.Add(NewRowData(new[] {$"{spellAsset.name}_Hit", "1",  hit.Frame.ToString()}));                                        
                        }    
                    }                   
                }              
            }
            
            var timelineSheets = new List<SheetData>();
            timelineSheets.Add(timelineSheet);
            
            var motionPath = System.IO.Path.Combine(UnityEngine.Application.dataPath,setting.OutputPath, $"Motion{DataGeneratorSetting.EXT_XLSX_FILES}");
            motionPath = System.IO.Path.GetFullPath(motionPath);
            ExcelGenerator.Generate(NewTable(timelineSheets, motionPath));
            
            var hitSheets = new List<SheetData>();
            hitSheets.Add(hitSheet);

            var motionHitPath = System.IO.Path.Combine(UnityEngine.Application.dataPath,setting.OutputPath, $"MotionHit{DataGeneratorSetting.EXT_XLSX_FILES}");
            motionHitPath = System.IO.Path.GetFullPath(motionHitPath);
            ExcelGenerator.Generate(NewTable(hitSheets, motionHitPath));
        }

        private static List<TimelineMotionData> GetMotionTimeline(IEnumerable<TimelineClip> clips)
        {
            List<TimelineMotionData> timelineAssets = new List<TimelineMotionData>();
            foreach (var clip in clips)
            {
                var c = clip.asset as TriggerDirectorClip;

                if (c.TimelineAsset != null)
                {
                    timelineAssets.Add(new TimelineMotionData()
                    {
                        Key = c.AssetKey,
                        Asset = c.TimelineAsset
                    });
                }
            }

            return timelineAssets;
        }

        public static TableData NewTable(List<SheetData> sheetDatas, string outputPath)
        {
            var table = new TableData();
            table.Sheets = sheetDatas;
            table.OutputPath = outputPath;
            return table;
        }

        public static RowData NewRowData(string[] colDatas)
        {            
            var row = new RowData();
                                
            row.Columns = new List<string>(); 
                
            row.Columns.AddRange(colDatas);                            

            return row;
        }
        
        public static SheetData NewSheetData(string name, string[] firstRowColDatas)
        {
            var sheet = new SheetData();

            sheet.Name = name;
            
            sheet.Rows = new List<RowData>();

            var rowData = new RowData();
            
            rowData.Columns = new List<string>();
            
            rowData.Columns.AddRange(firstRowColDatas);
            
            sheet.Rows.Add(rowData);
            
            return sheet;
        }       
    }
}