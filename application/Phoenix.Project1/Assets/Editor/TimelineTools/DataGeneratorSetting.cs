using UnityEngine;

namespace Phoenix.Project1.DataConvertTool
{
    [CreateAssetMenu(menuName = "Urad/DataGeneratorSetting")]
    public class DataGeneratorSetting : ScriptableObject
    {
        public string OutputPath = ".../.../.../.../.../.../project1-configs/";
        public string SourcePath = "Asset/Project/Assetbundles/Timelines/";
        public const string EXT_XSD_FILES = "*.xsd";
        public const string EXT_XML_FILES = "*.xml";
        public const string EXT_XLS_FILES = "*.xls";
        public const string EXT_XLSX_FILES = "*.xlsx";
        public const string EXT_XLSM_FILES = "*.xlsm";
    }
}
