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
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Infrastructure;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;

namespace WisDot.Bos.Sct.Core.Domain.Services
{
    public class FileManagerService : IFileManagerService
    {
        public static string certificationRootFolder = "";
        public static readonly string LOADING = "Loading...";
        public static string ROOT_FOLDER { get; set; }
        public static IDatabaseService dataServ;
        private static IFileManagerQuery query = new FileManagerQuery();
        private static IFileManagerRepository repo = new FileManagerRepository();

        public FileManagerService(DatabaseService database)
        {
            dataServ = database;
            certificationRootFolder = database.GetCertificationRootFolder();
            ROOT_FOLDER = "0";
        }

        public TreeNode CreateTreeNode(Item item)
        {
            return repo.CreateTreeNode(item);
        }

        public Task<Project> UpdateBoxCertificationDirectory(Project project)
        {
            return query.UpdateBoxCertificationDirectory(project);
        }

        public Task<Item> UpdateProjectFileTree(Project project, TreeView view, PictureBox loadingWheel)
        {
            return query.UpdateProjectFileTree(project, view, loadingWheel);
        }

        public Task UpdateFolder(TreeNode node, Project project, UserAccount userAccount)
        {
            return repo.UpdateFolder(node, project, userAccount);
        }

        public bool IsAStructureId(string structureId)
        {
            return repo.IsAStructureId(structureId);
        }

        public Task<TreeNode[]> ListFilesAsync(string folderId, Project project, UserAccount userAccount, bool showFiles = true)
        {
            return repo.ListFilesAsync(folderId, project, userAccount, showFiles);
        }

        public TreeNode InfoNode(string message)
        {
            return repo.InfoNode(message);
        }

        public TreeNode DisplayLoadingFiles()
        {
            return repo.DisplayLoadingFiles();
        }

        public TreeNode NoFiles()
        {
            return repo.NoFiles();
        }
    }
}
