using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WisDot.Bos.Sct.Core.Data;
using WisDot.Bos.Sct.Core.Domain.Models;
using WisDot.Bos.Sct.Core.Domain.Services;
using WisDot.Bos.Sct.Core.Domain.Services.Interfaces;
using WisDot.Bos.Sct.Core.Infrastructure.Interfaces;

namespace WisDot.Bos.Sct.Core.Infrastructure
{
    public class MainRepository : IMainRepository
    {
        private static IDatabaseService dataServ = new DatabaseService();

        public bool ValidateStructureId(string structureId, UserAccount userAccount)
        {
            //bool isValid = true;

            if (String.IsNullOrEmpty(structureId))
            {
                MessageBox.Show("Enter a Structure Id.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (userAccount.IsSuperUser || userAccount.IsAdministrator)
            {
                if (!IsStructureInHsi(structureId))
                {
                    MessageBox.Show("Structure Id's not in HSI.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (userAccount.IsRegionalProgrammer)
            {
                if (!IsStructureInHsi(structureId, userAccount))
                {
                    MessageBox.Show("Structure Id's not in HSI or it's not in your region.", "SCT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            return true;
        }

        public bool IsStructureInHsi(string structureId, UserAccount userAccount = null)
        {
            return dataServ.IsStructureInHsi(structureId, userAccount);
        }

        public bool IsFormOpen(string formName)
        {
            bool isFormOpen = false;

            foreach (Form form in Application.OpenForms)
            {
                if (form.Name.Equals(formName))
                {
                    isFormOpen = true;
                    break;
                }
            }

            return isFormOpen;
        }

        public List<int> FilterProjectsListGrid(StructuresProgramType.ProjectStatus status, bool show, DataGridView dataGridViewProjectsList)
        {
            List<int> affectedRows = new List<int>();

            foreach (DataGridViewRow row in dataGridViewProjectsList.Rows)
            {
                string projectStatus = row.Cells["dgvcProjectStatus"].Value.ToString();

                if (status == StructuresProgramType.ProjectStatus.QuasiCertified)
                {
                    if (projectStatus.Equals("Transitionally Certified"))
                    {
                        //row.Visible = show;
                        affectedRows.Add(row.Index);
                    }
                }
                else if (projectStatus.Equals(status.ToString()))
                {
                    //row.Visible = show;
                    affectedRows.Add(row.Index);
                }
            }

            return affectedRows;
        }

        public bool ValidateFiscalYear(int fiscalYear, int workConceptStartFiscalYear, int workConceptEndFiscalYear)
        {
            bool isValid = false;

            if (fiscalYear >= workConceptStartFiscalYear && fiscalYear <= workConceptEndFiscalYear)
            {
                isValid = true;
            }

            return isValid;
        }

        public int DoFindStructuresProject(int structuresProjectIdToFind, DataGridView dataGridViewProjectsList)
        {
            int hitCount = 0;
            DataGridView dgv = dataGridViewProjectsList;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    try
                    {
                        int structuresProjectId = Convert.ToInt32(row.Cells[1].Value);

                        if (structuresProjectId == structuresProjectIdToFind)
                        {
                            hitCount++;
                            row.Selected = true;
                            dgv.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                    }
                    catch { }
                }
            }

            return hitCount;
        }

        public int DoFindFiipsProject(string fosProjectIdToFind, DataGridView dataGridViewProjectsList)
        {
            int hitCount = 0;
            DataGridView dgv = dataGridViewProjectsList;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Cells["dgvcConstructionId"].Value != null)
                {
                    try
                    {
                        string fosProjectId = row.Cells["dgvcConstructionId"].Value.ToString().Replace("-", "");

                        if (fosProjectIdToFind.Equals(fosProjectId))
                        {
                            hitCount++;
                            row.Selected = true;

                            dgv.FirstDisplayedScrollingRowIndex = row.Index;
                        }
                    }
                    catch { }
                }
            }

            return hitCount;
        }

        public int DoFindStructureInProjects(string structureIdToFind, DataGridView dataGridViewProjectsList, List<Project> existingFiipsProjects, List<Project> existingStructureProjects)
        {
            int hitCount = 0;
            DataGridView dgv = dataGridViewProjectsList;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                string projectId = "";

                if (row.Cells[1].Value != null)
                {
                    projectId = row.Cells[1].Value.ToString();
                }

                string status = row.Cells[6].Value.ToString().ToUpper();
                Project project = null;

                if (status.Equals("FIIPS"))
                {
                    projectId = row.Cells[9].Value.ToString();
                    project = existingFiipsProjects.Where(f => f.FosProjectId.Equals(projectId)).First();
                }
                else
                {
                    project = existingStructureProjects.Where(s => s.ProjectDbId == Convert.ToInt32(projectId)).First();
                }

                if (project.WorkConcepts.Any(w => w.StructureId.Equals(structureIdToFind)))
                {
                    hitCount++;
                    row.Selected = true;
                    dgv.FirstDisplayedScrollingRowIndex = row.Index;
                }
            }

            return hitCount;
        }

        public int DoFindStructure(string structureIdToFind, DataGridView dgv)
        {
            int hitCount = 0;

            if (dgv != null)
            {
                foreach (DataGridViewRow dgvr in dgv.Rows)
                {
                    string structureId = dgvr.Cells[0].Value.ToString();

                    if (structureId.Equals(structureIdToFind))
                    {
                        hitCount++;
                        dgvr.Selected = true;
                    }
                }
            }

            return hitCount;
        }

        public string FormatConstructionId(string constructionId)
        {
            string formattedId = constructionId;

            try
            {
                formattedId = String.Format("{0}-{1}-{2}", formattedId.Substring(0, 4), formattedId.Substring(4, 2), formattedId.Substring(6, 2));
            }
            catch { }

            return formattedId;
        }

        public bool IsProposedWorkConceptInAProject(int workConceptDbId, string structureId, List<Project> existingStructureProjects)
        {
            bool inProject = false;

            foreach (var p in existingStructureProjects)
            {
                foreach (var w in p.WorkConcepts)
                {
                    try
                    {
                        if (w.FromProposedList && w.WorkConceptDbId == workConceptDbId && w.StructureId.Equals(structureId))
                        {
                            inProject = true;
                            break;
                        }
                    }
                    catch { }
                }

                if (inProject)
                {
                    break;
                }
            }
            /*
            try
            {
                if (existingStructureProjects.Any(p => p.WorkConcepts != null && p.WorkConcepts.Where(w => w.WorkConceptDbId == workConceptDbId && w.StructureId != null && w.StructureId.Equals(structureId) && w.FromProposedList).Count() > 0))
                {
                    inProject = true;
                }
            }
            catch { }*/

            return inProject;
        }

        public string[] ParseWorkConceptFullDescription(string workConcept)
        {
            string[] parsed = workConcept.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
            string workActionCode = parsed[0];
            string workActionDescription = dataServ.GetWorkConceptDescription(workActionCode);

            if (workActionCode.Equals("EV"))
            {
                workActionDescription = "EVALUATE FOR SECONDARY WORK CONCEPTS";
            }

            string[] parts = new string[2];
            parts[0] = workActionCode;
            parts[1] = workActionDescription;
            return parts;
        }

        public int GetCurrentFiscalYear()
        {
            int currentYear = DateTime.Now.Year;

            if (DateTime.Compare(DateTime.Now, new DateTime(currentYear, 7, 1)) >= 0)
            {
                currentYear = currentYear + 1;
            }

            return currentYear;
        }

        public void RenderAllWorkConceptsGrid(List<WorkConcept> wcs, DataGridView dgv, string color = "")
        {
            foreach (WorkConcept wc in wcs)
            {
                if (!wc.WorkConceptDescription.Trim().Equals(""))
                {
                    dgv.Rows.Add();
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;

                    if (wc.FromFiips)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = wc.WorkConceptDescription;
                    }
                    else
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = wc.CertifiedWorkConceptDescription;
                    }

                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[2].Value = wc.WorkConceptDbId;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;

                    if (wc.ProjectDbId != 0)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[4].Value = wc.ProjectDbId;
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = wc.StructureProjectFiscalYear;
                    }

                    //dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = str proj year;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[6].Value = wc.StructuresConcept;
                    //dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[7].Value = str proj approval;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[8].Value = wc.FosProjectId;

                    if (wc.FromFiips)
                    {
                        dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[9].Value = wc.FiscalYear;
                    }

                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[10].Value = wc.FiipsImprovementConcept;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[11].Value = wc.IsQuasicertified;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[12].Value = wc.FromFiips;
                    dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[13].Value = wc.Region;

                    if (!color.Equals(""))
                    {
                        switch (color)
                        {
                            case "yellow":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Yellow;
                                break;
                            case "orange":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Orange;
                                break;
                            case "darkgreen":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.DarkGreen;
                                break;
                            case "lightgreen":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightGreen;
                                break;
                            case "pink":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightPink;
                                break;
                            case "red":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.DarkRed;
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.ForeColor = Color.White;
                                break;
                            case "lightskyblue":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.LightSkyBlue;
                                break;
                            case "darkorange":
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.DarkOrange;
                                //dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.ForeColor = Color.White;
                                break;
                            default:
                                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.Empty;
                                break;
                        }
                    }
                }
            }
        }

        public void AddWorkConceptToEligibilityGrid(DataGridView dgv, WorkConcept wc)
        {
            dgv.Rows.Add();
            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[0].Value = wc.StructureId;

            if (wc.FromEligibilityList)
            {
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = wc.WorkConceptDescription;
            }
            else if (wc.Status == StructuresProgramType.WorkConceptStatus.Proposed || wc.FromProposedList)
            {
                dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[1].Value = "(PR) " + wc.CertifiedWorkConceptDescription;
                //dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.ForeColor = Color.White;
                dgv.Rows[dgv.Rows.GetLastRow(0)].DefaultCellStyle.BackColor = Color.DarkOrange;
            }

            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[2].Value = wc.PriorityScore;
            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[3].Value = wc.FiscalYear;
            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[4].Value = wc.WorkConceptDbId;
            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[5].Value = wc.WorkConceptCode;
            dgv.Rows[dgv.Rows.GetLastRow(0)].Cells[6].Value = wc.ProjectYear;
        }

        public string GetRandomExcelFileName(string baseDir)
        {
            string newPath = Path.GetRandomFileName();
            string fileExt = Path.GetExtension(newPath);
            newPath = newPath.Replace(fileExt, ".xlsx");
            newPath = Path.Combine(baseDir, newPath);
            return newPath;
        }


    }
}
