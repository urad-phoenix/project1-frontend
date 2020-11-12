using UnityEngine;

namespace Phoenix.Project1.Editors.Tools
{
    [CreateAssetMenu(menuName = "Urad/DataGeneratorSetting")]
    public class DataGeneratorSetting : ScriptableObject
    {
        public string OutputPath = "D:/urad/project1-configs/";
        public string SourcePath = "Assets/Project/Assetbundles/Timelines/";        
        public string[] FilterTypes = {".playable"};        
        public const string EXT_XSD_FILES = ".xsd";
        public const string EXT_XML_FILES = ".xml";
        public const string EXT_XLS_FILES = ".xls";
        public const string EXT_XLSX_FILES = ".xlsx";
        public const string EXT_XLSM_FILES = ".xlsm";                       
    }
}
