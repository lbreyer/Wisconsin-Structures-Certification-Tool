﻿using System;
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

namespace WisDot.Bos.Sct.Core.Infrastructure.Interfaces
{
    public interface IFileManagerRepository
    {
        TreeNode CreateTreeNode(Item item);
        Task UpdateFolder(TreeNode node, Project project, UserAccount userAccount);
        bool IsAStructureId(string structureId);
        Task<TreeNode[]> ListFilesAsync(string folderId, Project project, UserAccount userAccount, bool showFiles = true);
        TreeNode InfoNode(string message);
        TreeNode DisplayLoadingFiles();
        TreeNode NoFiles();

    }
}
