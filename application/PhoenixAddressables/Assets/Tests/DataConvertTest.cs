using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Phoenix.Pool;
using Phoenix.Project1.DataConvertTool;
using UnityEngine;

namespace Tests
{
    [Category("DataConvertTest")]
    public class DataConvertTest
    {
        private ExcelGenerator _ExcelGenerator;

        private TableData _TableData;

        [SetUp]
        public void SetupTestPoolManager()
        {
            _ExcelGenerator = new ExcelGenerator();

            _TableData = new TableData();

            _TableData.OutputPath = @"D:/urad/project1-configs/戰鬥表現流程.xlsx";
                
            _TableData.Name = "TestConvert";

            _TableData.Sheets = new List<SheetData>();

            _TableData.Sheets.Add(new SheetData()
            {
                Name = "Sheet_Test_1",

                Rows = new List<RowData>()
            });


            for (int i = 0; i < 10; ++i)
            {
                var sheet = _TableData.Sheets[0];

                sheet.Rows.Add(new RowData()
                {
                    Columns = new List<string>()
                    {
                        $"col_1_{i}",
                        $"col_2_{i}",
                    }
                });
            }                         
        }

        [TestCase("D:/urad/project1-configs")]
        public void TestPathConvert(string testPath)
        {
            var path = _ExcelGenerator.CheckFilePath(testPath);
                                   
            Assert.IsTrue(Directory.Exists(path));
        }

        [Test]
        public void TestWorkbookData()
        {
            _ExcelGenerator.Generate(_TableData);           
            
            Assert.IsTrue(File.Exists(_TableData.OutputPath));
        }
    }
}