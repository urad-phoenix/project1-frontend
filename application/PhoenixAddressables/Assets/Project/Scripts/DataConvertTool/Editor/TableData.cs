using System.Collections.Generic;

namespace Phoenix.Project1.DataConvertTool
{
    public class TableData
    {
        public string OutputPath;

        public string Name;
        
        public List<SheetData> Sheets;        
    }
    
    public class RowData
    {
        public List<string> Columns;
    }
    
    public class SheetData
    {
        public string Name;
        public List<RowData> Rows;
    }
}