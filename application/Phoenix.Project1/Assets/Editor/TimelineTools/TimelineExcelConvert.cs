using System.Collections.Generic;
using System.Security.Policy;
using Phoenix.Project1.Configs;
using Phoenix.Project1.DataConvertTool;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Phoenix.Project1.Editors.Tools
{
    public class TimelineExcelConvert
    {
        [MenuItem("Phoenix/TimelineExcelConvert")]
        public static void ConvertTimelineToExcel()
        {
            var convertSetting = AssetDatabase.FindAssets("TimelineDataGeneratorSetting t:DataGeneratorSetting");

            string assetPath = "";
            
            if(convertSetting.Length > 0)
               assetPath = AssetDatabase.GUIDToAssetPath(convertSetting[0]);

            if(string.IsNullOrEmpty(assetPath))
                return;
            
            var setting = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DataGeneratorSetting)) as DataGeneratorSetting;

            var timelineAssets = TimelineOutputExcelTool.GetTimelineFiles(setting.SourcePath, setting.FilterTypes);
                       
            var timelineSheet = NewSheetData("sheet1", new [] {"Timline資源名稱", "總幀數"});
            timelineSheet.Rows.Add(NewRowData(new[] {"Key", "TotalFrame"}));

            var hitSheet = NewSheetData("sheet1", new[] {"Timline資源名稱", "總幀數"});
            hitSheet.Rows.Add(NewRowData(new[] {"Key", "Frame"}));
            
            for (int i = 0; i < timelineAssets.Count; ++i)
            {
                var asset = timelineAssets[i];

                var timelineData = TimelineOutputExcelTool.ConvertData(asset.name, asset);

                timelineSheet.Rows.Add(NewRowData(new[] {timelineData.Key, timelineData.TotalFrame.ToString()}));                                                                
                
                for (int j = 0; j < timelineData.HitDatas.Count; ++j)
                {
                    var hit = timelineData.HitDatas[j];

                    hitSheet.Rows.Add(NewRowData(new[] {hit.Key, hit.Frame.ToString()}));                                        
                }
            }
            
            var timelineSheets = new List<SheetData>();
            timelineSheets.Add(timelineSheet);                   
            ExcelGenerator.Generate(NewTable(timelineSheets, setting.OutputPath + $"Motion{DataGeneratorSetting.EXT_XLSX_FILES}"));
            
            var hitSheets = new List<SheetData>();
            hitSheets.Add(hitSheet);
            ExcelGenerator.Generate(NewTable(hitSheets, setting.OutputPath + $"MotionHit{DataGeneratorSetting.EXT_XLSX_FILES}"));
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