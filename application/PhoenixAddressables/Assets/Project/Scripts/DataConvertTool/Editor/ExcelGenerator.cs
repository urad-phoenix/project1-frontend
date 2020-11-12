namespace Phoenix.Project1.DataConvertTool
{
    using System;
    using System.IO;
    using OfficeOpenXml;
    
    public class ExcelGenerator
    {
        public static void Generate(TableData tableData)
        {
            var path = CheckFilePath(tableData.OutputPath);
            
            if (CheckDirectory(path))
            {
                throw new Exception("_GetOrCreateWorkbook need file path");                
            }

            if (CheckFile(path))
            {
                File.Delete(path); 
            }                        
            
            using (var file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                var wb = new ExcelPackage(file);
                    
                _ParseWorkbook(tableData, wb);                        
                     
                wb.Save();                
            }
            

            

            //wb.Write(memoryStream);
            
            //memoryStream.Position = 0;
            
            //var byteArray = memoryStream.ToArray();
            
            //System.IO.File.WriteAllBytes(path, byteArray);           
        }                    

        public static string CheckFilePath(string path)
        {
            path = path.Replace("/", @"\");                        

            return path;
        }

        private static void _ParseWorkbook(TableData tableData, ExcelPackage wb)
        {                                  
            for (int i = 0; i < tableData.Sheets.Count; ++i)
            {
                var sheet = tableData.Sheets[i];
                
                var wbSheet = wb.Workbook.Worksheets.Add(sheet.Name);

                _ParseSheetData(sheet, wbSheet);
            }        
        }

        private static void _ParseSheetData(SheetData sheetData, ExcelWorksheet sheet)
        {
            for (int i = 0; i < sheetData.Rows.Count; ++i)
            {
                var rowData = sheetData.Rows[i];
                
                for (int j = 0; j < rowData.Columns.Count; ++j)
                {
                    var col = rowData.Columns[j];
                       
                    sheet.Cells[i + 1, j + 1].Value = col;                    
                }
            }
        }

        private static bool CheckFile(string path)
        {
            return File.Exists(path);
        }

        private static bool CheckDirectory(string path)
        {            
            return Directory.Exists(path);
        }
    }
}