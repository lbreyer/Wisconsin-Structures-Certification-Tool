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
using WisDot.Bos.Sct.Core.Data.Interfaces;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure;

namespace WisDot.Bos.Sct.Core.Data
{
    public class FileManagerQuery : IFileManagerQuery
    {
        private static IFileManagerRepository repo = new FileManagerRepository();
        public async Task<Project> UpdateBoxCertificationDirectory(Project project)
        {
            Item projectFolder = await Folder.GetFolderAsync(String.Format("Project-{0}", project.ProjectDbId), FileManagerService.certificationRootFolder);

            if (projectFolder == null)
            {
                projectFolder = await Folder.CreateAsync(String.Format("Project-{0}", project.ProjectDbId), FileManagerService.certificationRootFolder);
                FileManagerService.dataServ.UpdateProjectBoxId(project.ProjectDbId, projectFolder.Id);
                project.BoxId = projectFolder.Id;
            }

            foreach (var wc in project.WorkConcepts)
            {
                Item structureFolder = null;

                try
                {
                    structureFolder = await Folder.GetFolderAsync(String.Format("{0}", wc.StructureId), project.BoxId);

                    if (structureFolder == null)
                    {
                        structureFolder = await Folder.CreateAsync(String.Format("{0}", wc.StructureId), project.BoxId);
                    }
                }
                catch (Exception ex)
                { }

                /*
                try
                {
                    Item precertificationFolder = await Folder.GetFolderAsync("Precertification", structureFolder.Id);

                    if (precertificationFolder == null)
                    {
                        precertificationFolder = await Folder.CreateAsync("Precertification", structureFolder.Id);
                    }
                }
                catch (Exception ex)
                { }
                */

                /*
                try
                {
                    Item certificationFolder = await Folder.GetFolderAsync("Certification", structureFolder.Id);

                    if (certificationFolder == null)
                    {
                        certificationFolder = await Folder.CreateAsync("Certification", structureFolder.Id);
                    }
                }
                catch (Exception ex)
                { }*/

                try
                {
                    Item securedFolder = await Folder.GetFolderAsync("Secured", structureFolder.Id);

                    if (securedFolder == null)
                    {
                        securedFolder = await Folder.CreateAsync("Secured", structureFolder.Id);
                    }
                }
                catch (Exception ex)
                { }
            }

            return project;
        }

        public async Task<Item> UpdateProjectFileTree(Project project, TreeView view, PictureBox loadingWheel)
        {
            Item item = null;

            if (loadingWheel != null)
            {
                loadingWheel.Visible = true;
            }

            view.Nodes.Clear();
            view.Nodes.Add(repo.DisplayLoadingFiles());

            try
            {
                item = await Folder.GetFolderAsync(project.BoxId);
                TreeNode node = null;

                if (item != null)
                {
                    node = repo.CreateTreeNode(item);
                    node.Nodes.Add(repo.DisplayLoadingFiles());
                }

                view.Nodes.Clear();

                if (node != null)
                {
                    view.Nodes.Add(node);
                }

                if (loadingWheel != null)
                {
                    loadingWheel.Visible = false;
                }
            }
            catch (Exception ex)
            {
                view.Nodes.Clear();

                if (loadingWheel != null)
                {
                    loadingWheel.Visible = false;
                }
            }

            return item;
        }
    }
}
