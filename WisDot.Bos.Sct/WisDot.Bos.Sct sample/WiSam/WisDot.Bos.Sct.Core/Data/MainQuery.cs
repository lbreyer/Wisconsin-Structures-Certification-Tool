using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using BOS.Box;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Data.Interfaces;

namespace WisDot.Bos.Sct.Core.Data
{
    public class MainQuery : IMainQuery
    {
        public void OpenExcelFile(string filePath)
        {
            try
            {
                Excel.Application xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                Excel.Workbooks xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(filePath);
            }
            catch { }
        }
    }
}
