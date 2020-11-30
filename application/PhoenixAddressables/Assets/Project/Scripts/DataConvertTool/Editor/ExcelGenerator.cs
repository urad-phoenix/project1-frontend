namespace Phoenix.Project1.DataConvertTool
{
    using DocumentFormat.OpenXml.Packaging;
    using System;
    using System.IO;
    using DocumentFormat.OpenXml;
    using System.Linq;
    using DocumentFormat.OpenXml.Spreadsheet;
    
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
            
            using (var doc = SpreadsheetDocument.Create(path, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = doc.AddWorkbookPart();

                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                
                var sheetData =new DocumentFormat.OpenXml.Spreadsheet.SheetData();
                
                workbookPart.Workbook = new DocumentFormat.OpenXml.Spreadsheet.Workbook(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
             
                string relationshipId = workbookPart.GetIdOfPart(worksheetPart);
                
                for (int i = 0; i < tableData.Sheets.Count; ++i)
                {
                    var sheet = new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = relationshipId,
                        SheetId = (uint) i + 1,
                        Name = "sheet"
                    };               
                    sheets.Append(sheet);
                    
                    worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(new DocumentFormat.OpenXml.Spreadsheet.SheetData());
                    
                    sheetData = worksheetPart.Worksheet.GetFirstChild<DocumentFormat.OpenXml.Spreadsheet.SheetData>();
     
                    foreach (var row in tableData.Sheets[i].Rows)
                    {
                        var excelRow = new Row();
                                                
                        var cells = row.Columns.Select((col) => new DocumentFormat.OpenXml.Spreadsheet.Cell()
                        {
                            CellValue = new CellValue(col),
                            DataType = new EnumValue<CellValues>(CellValues.String)
                        });

                        foreach (var cell in cells)
                        {
                            excelRow.Append(cell);
                        }

                        sheetData.AppendChild(excelRow);
                    }
                }  
                
                doc.WorkbookPart.Workbook.Save();
                doc.Close();
            }         
        }                    

        public static string CheckFilePath(string path)
        {
            path = path.Replace("/", @"\");                        

            return path;
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