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
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class FileManagerRepository : IFileManagerRepository
    {
        public TreeNode CreateTreeNode(Item item)
        {
            TreeNode treeNode = null;

            if (item.Type == ItemType.file)
            {
                treeNode = new TreeNode(item.Name, 1, 1)
                {
                    Tag = item,
                    Name = item.Id
                };
            }
            else if (item.Type == ItemType.folder)
            {
                treeNode = new TreeNode(item.Name, 0, 0)
                {
                    Tag = item,
                    Name = item.Id
                };
            }

            return treeNode;
        }

        /// <summary>
        /// Display a warning when files are loading
        /// </summary>
        /// <returns></returns>
        public TreeNode DisplayLoadingFiles()
        {
            return InfoNode(FileManagerService.LOADING);
        }

        public TreeNode InfoNode(string message)
        {
            return new TreeNode(message)
            {
                SelectedImageIndex = -1,
                ImageIndex = -1,
                ForeColor = Color.Gray,
            };
        }

        public bool IsAStructureId(string structureId)
        {
            structureId = structureId.ToUpper();
            string firstLetter = structureId.Substring(0, 1);
            string rest = structureId.Substring(1, structureId.Length - 1);

            if (structureId.Length != 7 && structureId.Length != 11)
            {
                return false;
            }

            if (!firstLetter.Equals("B") && !firstLetter.Equals("P") && !firstLetter.Equals("C"))
            {
                return false;
            }

            try
            {
                int number = Convert.ToInt32(rest);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<TreeNode[]> ListFilesAsync(string folderId, Project project, UserAccount userAccount, bool showFiles = true)
        {
            List<TreeNode> treeNodes = new List<TreeNode>();
            var items = await Folder.ContentsAsync(folderId, 10000);

            foreach (var item in items)
            {
                TreeNode node = CreateTreeNode(item);

                if (item.Type == ItemType.folder)
                {
                    node.Nodes.Add(DisplayLoadingFiles());

                    if (IsAStructureId(item.Name) && !project.WorkConcepts.Any(w => w.StructureId.Equals(item.Name)))
                    {
                        continue;
                    }

                    if (item.Name.ToUpper().Equals("SECURED"))
                    {
                        if (userAccount.IsPrecertificationLiaison || userAccount.IsPrecertificationSupervisor
                            || userAccount.IsCertificationLiaison || userAccount.IsCertificationSupervisor
                            || userAccount.IsAdministrator)
                        {
                            treeNodes.Add(node);
                        }
                    }
                    else
                    {
                        treeNodes.Add(node);
                    }
                }
                else if (item.Type == ItemType.file && showFiles)
                {
                    treeNodes.Add(node);
                }
            }

            if (treeNodes.Count() == 0)
            {
                treeNodes.Add(NoFiles());
            }

            return treeNodes.ToArray();
        }

        public TreeNode NoFiles()
        {
            return InfoNode("No files exist yet. Upload some first.");
        }

        public async Task UpdateFolder(TreeNode node, Project project, UserAccount userAccount)
        {
            try
            {
                var nodes = await ListFilesAsync((node.Tag as Item).Id, project, userAccount);
                node.Nodes.Clear();
                node.Nodes.AddRange(nodes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
