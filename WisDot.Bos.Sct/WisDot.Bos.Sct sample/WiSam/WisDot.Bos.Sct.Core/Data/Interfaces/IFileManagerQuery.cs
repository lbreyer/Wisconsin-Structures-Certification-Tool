using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BOS.Box;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WisDot.Bos.Sct.Core.Data.Interfaces
{
    public interface IFileManagerQuery
    {
        Task<Project> UpdateBoxCertificationDirectory(Project project);
        Task<Item> UpdateProjectFileTree(Project project, TreeView view, PictureBox loadingWheel);

    }
}
