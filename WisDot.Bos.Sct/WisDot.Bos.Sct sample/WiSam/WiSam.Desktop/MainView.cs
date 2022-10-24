using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using Excel = Microsoft.Office.Interop.Excel;
using WiSam.Business;
using Wisdot.Bos.WiSam.Core.Domain.Models;

namespace WiSam.Desktop
{
    public partial class FormMain : Form
    {
        private static string userName = Environment.UserName.ToString().ToUpper();
        private static Interface iface;
        private static Excel.Application xlsApp;
        private static Excel.Workbooks xlsBooks;
        private List<WorkActionRule> workActionRules;
        private List<StructureWorkAction> workActions;
        private List<string> ruleCategories;
        private bool isProcessRunning = false;
        private NeedsAnalysisInput currentNeedsAnalysisInput = new NeedsAnalysisInput(Environment.UserName);
        private static MainController controller = new MainController();

        #region NeedsAnalysisEventHandlers
        private void UpdateBudgetGrid(int annualBudget, int startYear, int endYear)
        {
            dgvBudget.Rows.Clear();
            int rowCounter = 0;

            for (int i = startYear; i <= endYear; i++)
            {
                dgvBudget.Rows.Add();
                dgvBudget.Rows[rowCounter].Cells[0].Value = i;
                dgvBudget.Rows[rowCounter].Cells[1].Value = annualBudget;
                rowCounter++;
            }
        }

        private void UpdateBudgetGrid(int startYear, int endYear)
        {
            DataGridViewRowCollection oldRows = new DataGridViewRowCollection(dgvBudget);
            dgvBudget.Rows.Clear();
            int rowCounter = 0;

            for (int i = startYear; i <= endYear; i++)
            {
                dgvBudget.Rows.Add();
                dgvBudget.Rows[rowCounter].Cells[0].Value = i;
                rowCounter++;
            }
        }

        private void RenderBudgetGrid(int startYear, int endYear)
        {
            dgvBudget.Rows.Clear();
            int rowCounter = 0;
            for (int i = startYear; i <= endYear; i++)
            {
                dgvBudget.Rows.Add();
                dgvBudget.Rows[rowCounter].Cells[0].Value = i;
                rowCounter++;
            }
        }

        private void RenderFundingSourcesCheckListBox()
        {
            foreach (var source in Enum.GetValues(typeof(WisamType.FundingSources)))
            {
                checkedListBoxFundings.Items.Add(source);
            }
        }

        private void chkApplyBudget_CheckedChanged(object sender, EventArgs e)
        {
            if (chkApplyBudget.Checked)
            {
                dgvBudget.Enabled = true;
            }
            else
            {
                dgvBudget.Enabled = false;
            }
        }

        private void tbxEndYear_Leave(object sender, EventArgs e)
        {
            int endYear = 0;
            if (int.TryParse(tbxEndYear.Text.Trim(), out endYear))
            {
                int startYear = 0;
                if (int.TryParse(tbxStartYear.Text.Trim(), out startYear))
                {
                    if (endYear < startYear)
                    {
                        MessageBox.Show("Please enter an End Year >= Start Year.");
                        tbxEndYear.Focus();
                    }
                    else
                    {
                        // Update dgvBudget
                        currentNeedsAnalysisInput.AnalysisStartYear = startYear;
                        currentNeedsAnalysisInput.AnalysisEndYear = endYear;
                        UpdateBudgetGrid(startYear, endYear);
                    }
                }
                else
                {
                    tbxStartYear.Focus();
                }
            }
            else
            {
                MessageBox.Show("Please enter an End Year >= Start Year.");
                tbxEndYear.Focus();
            }
        }

        private void tbxStartYear_Leave(object sender, EventArgs e)
        {
            int startYear = 0;
            if (int.TryParse(tbxStartYear.Text.Trim(), out startYear))
            {
                int endYear = 0;
                if (int.TryParse(tbxEndYear.Text.Trim(), out endYear))
                {
                    if (startYear > endYear)
                    {
                        MessageBox.Show("Please enter a Start Year <= End Year.");
                        tbxStartYear.Focus();
                    }
                    else
                    {
                        // Update budget
                        currentNeedsAnalysisInput.AnalysisStartYear = startYear;
                        currentNeedsAnalysisInput.AnalysisEndYear = endYear;
                        UpdateBudgetGrid(startYear, endYear);
                    }
                }
                else
                {
                    tbxStartYear.Focus();
                }
            }
            else
            {
                MessageBox.Show("Please enter a Start Year <= End Year.");
                tbxStartYear.Focus();
            }
        }

        private void tsbNeedsAnalysisSaveInput_Click(object sender, EventArgs e)
        {
            SetNeedsAnalysisInput();
            if (ValidateNeedsAnalysisTab())
            {

            }
        }

        private void btnRunNeedsAnalysis_Click(object sender, EventArgs e)
        {
            RunNeedsAnalysis();
        }

        private void RunNeedsAnalysis()
        {
            SetNeedsAnalysisInput();

            if (ValidateNeedsAnalysisTab())
            {
                ClearResults();

                if (chkApplyBudget.Checked)
                {
                    DialogResult result = MessageBox.Show("Also Run the 'Unconstrained Analysis'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result.Equals(DialogResult.Yes))
                    {
                        currentNeedsAnalysisInput.RunUnconstrainedAnalysis = true;
                    }
                    else
                    {
                        currentNeedsAnalysisInput.RunUnconstrainedAnalysis = false;
                    }
                }

                SetAnalysisFiles();
                iface.AnalyzeStructures(currentNeedsAnalysisInput);
            }

            WriteResults();
        }

        private void ClearResults()
        {
            gbxResults.Controls.Clear();
        }

        private void WriteResults()
        {
            gbxResults.Controls.Clear();
            int pointX = 20;
            int pointY = 24;
            int fileCounter = 0;

            foreach (var analysisFile in currentNeedsAnalysisInput.AnalysisFiles.OrderBy(e => e.NeedsAnalysisFileType.ToString()))
            {
                fileCounter++;
                LinkLabel lnkLbl = controller.WriteResult(fileCounter, analysisFile, pointX, pointY);
                lnkLbl.LinkClicked += new LinkLabelLinkClickedEventHandler(LinkedLabelClicked);
                gbxResults.Controls.Add(lnkLbl);
                pointY += 25;

                if (fileCounter % 5 == 0)
                {
                    pointX += 200;
                    pointY = 24;
                }
            }
        }

        private void LinkedLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lnkLbl = (LinkLabel)sender;
            lnkLbl.LinkVisited = true;

            if (File.Exists(lnkLbl.Tag.ToString()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(lnkLbl.Tag.ToString());
            }
            else
            {
                MessageBox.Show(lnkLbl.Text + " does not exist.", "Missing File");
            }
        }

        private string GetWisamsDatabaseSelection()
        {
            if (rbtWisamDbProd.Checked)
            {
                return "Prod";
            }
            else if (rbtWisamDbTest.Checked)
            {
                return "Test";
            }
            else
            {
                return "Dev";
            }
        }

        private void SetNeedsAnalysisInput()
        {
            int maxNumberToAnalyze = 0;
            if (int.TryParse(tbxMaxNumToAnalyze.Text.Trim(), out maxNumberToAnalyze))
            {
                if (maxNumberToAnalyze > 0)
                {
                    currentNeedsAnalysisInput.MaxNumberToAnalyze = maxNumberToAnalyze;
                }
            }
            else
            {
                currentNeedsAnalysisInput.MaxNumberToAnalyze = maxNumberToAnalyze;
            }
            currentNeedsAnalysisInput.RunDate = DateTime.Now;
            currentNeedsAnalysisInput.WisamsDatabase = GetWisamsDatabaseSelection();
            currentNeedsAnalysisInput.AnalysisType = (WisamType.AnalysisTypes)cboAnalysisTypes.SelectedItem;
            currentNeedsAnalysisInput.IncludeCStructures = chkIncludeCStructures.Checked;

            if (radioButtonBrmDeterioration.Checked)
            {
                currentNeedsAnalysisInput.ElementDeteriorationMethod = WisamType.ElementDeteriorationRates.ByBrm;
            }
            else
            {
                currentNeedsAnalysisInput.ElementDeteriorationMethod = WisamType.ElementDeteriorationRates.ByElement;
            }

            try
            {
                currentNeedsAnalysisInput.LeastCostProject = Convert.ToSingle(tbxLeastCost.Text.Trim());
            }
            catch (Exception ex)
            {
                currentNeedsAnalysisInput.LeastCostProject = 20000;
            }

            if (rbtStructuresById.Checked)
            {
                currentNeedsAnalysisInput.StructureSelection = "ByIds";
                currentNeedsAnalysisInput.StructureIds.Clear();

                try
                {
                    currentNeedsAnalysisInput.StructureIds = tbxStructureIds.Text.ToUpper().Trim().Split(new string[] { ",", ";", " ", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                catch { }
            }
            else if (rbtStructuresByFunding.Checked)
            {
                currentNeedsAnalysisInput.StructureSelection = "ByFundings";
                currentNeedsAnalysisInput.ApplyRegionSelectionsWhenByFunding = false;
                currentNeedsAnalysisInput.Regions.Clear();
                currentNeedsAnalysisInput.RegionNumbers.Clear();

                if (chkApplyRegionSelections.Checked)
                {
                    currentNeedsAnalysisInput.ApplyRegionSelectionsWhenByFunding = true;

                    if (chkSouthwest.Checked)
                    {
                        currentNeedsAnalysisInput.Regions.Add("SW");
                        currentNeedsAnalysisInput.RegionNumbers.Add("1");
                    }

                    if (chkSoutheast.Checked)
                    {
                        currentNeedsAnalysisInput.Regions.Add("SE");
                        currentNeedsAnalysisInput.RegionNumbers.Add("2");
                    }

                    if (chkNortheast.Checked)
                    {
                        currentNeedsAnalysisInput.Regions.Add("NE");
                        currentNeedsAnalysisInput.RegionNumbers.Add("3");
                    }

                    if (chkNorthcentral.Checked)
                    {
                        currentNeedsAnalysisInput.Regions.Add("NC");
                        currentNeedsAnalysisInput.RegionNumbers.Add("4");
                    }

                    if (chkNorthwest.Checked)
                    {
                        currentNeedsAnalysisInput.Regions.Add("NW");
                        currentNeedsAnalysisInput.RegionNumbers.Add("5");
                    }
                }
            }
            else
            {
                currentNeedsAnalysisInput.StructureSelection = "ByRegions";
                currentNeedsAnalysisInput.Regions.Clear();
                currentNeedsAnalysisInput.RegionNumbers.Clear();

                if (chkSouthwest.Checked)
                {
                    currentNeedsAnalysisInput.Regions.Add("SW");
                    currentNeedsAnalysisInput.RegionNumbers.Add("1");
                }

                if (chkSoutheast.Checked)
                {
                    currentNeedsAnalysisInput.Regions.Add("SE");
                    currentNeedsAnalysisInput.RegionNumbers.Add("2");
                }

                if (chkNortheast.Checked)
                {
                    currentNeedsAnalysisInput.Regions.Add("NE");
                    currentNeedsAnalysisInput.RegionNumbers.Add("3");
                }

                if (chkNorthcentral.Checked)
                {
                    currentNeedsAnalysisInput.Regions.Add("NC");
                    currentNeedsAnalysisInput.RegionNumbers.Add("4");
                }

                if (chkNorthwest.Checked)
                {
                    currentNeedsAnalysisInput.Regions.Add("NW");
                    currentNeedsAnalysisInput.RegionNumbers.Add("5");
                }

                currentNeedsAnalysisInput.IncludeStateOwned = chkStateBridges.Checked;
                currentNeedsAnalysisInput.IncludeLocalOwned = chkLocalBridges.Checked;
            }

            try
            {
                currentNeedsAnalysisInput.AnalysisStartYear = Convert.ToInt32(tbxStartYear.Text.Trim());
            }
            catch { }

            try
            {
                currentNeedsAnalysisInput.AnalysisEndYear = Convert.ToInt32(tbxEndYear.Text.Trim());
            }
            catch { }

            if (rbtCalendarYear.Checked)
            {
                currentNeedsAnalysisInput.CalendarType = WisamType.CalendarTypes.CalendarYear;
            }
            else if (rbtStateFiscalYear.Checked)
            {
                currentNeedsAnalysisInput.CalendarType = WisamType.CalendarTypes.StateFiscalYear;
            }
            else
            {
                currentNeedsAnalysisInput.CalendarType = WisamType.CalendarTypes.FederalFiscalYear;
            }

            currentNeedsAnalysisInput.DeteriorateOverlayDefects = chkDeteriorateOlayDefects.Checked;
            currentNeedsAnalysisInput.InterpolateNbiRatings = chkInterpolateNbi.Checked;
            currentNeedsAnalysisInput.CountThinPolymerOverlays = chkCountTpo.Checked;
            currentNeedsAnalysisInput.ShowPriorityScore = chkPiFactors.Checked;
            currentNeedsAnalysisInput.CreateDebugFile = chkDebug.Checked;

            if (currentNeedsAnalysisInput.CreateDebugFile)
            {
                currentNeedsAnalysisInput.ElementsToReport.Clear();

                try
                {
                    foreach (var elemNum in tbxElements.Text.Trim().Split(new string[] { ",", ";", " ", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList())
                    {
                        currentNeedsAnalysisInput.ElementsToReport.Add(Convert.ToInt32(elemNum));
                    }
                }
                catch { }
            }

            currentNeedsAnalysisInput.CreateBridgeInventoryFile = chkBridgeInventory.Checked;

            currentNeedsAnalysisInput.EligiblePrimaryWorkActionCodes.Clear();
            foreach (var item in checkedListBoxWorkActions.CheckedItems)
            {
                currentNeedsAnalysisInput.EligiblePrimaryWorkActionCodes.Add(item.ToString().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).First());
            }

            currentNeedsAnalysisInput.FundingSources.Clear();
            foreach (var item in checkedListBoxFundings.CheckedItems)
            {
                currentNeedsAnalysisInput.FundingSources.Add((WisamType.FundingSources)Enum.Parse(typeof(WisamType.FundingSources), item.ToString()));
            }

            currentNeedsAnalysisInput.RunUnconstrainedAnalysis = !chkApplyBudget.Checked;
            currentNeedsAnalysisInput.ApplyBudget = chkApplyBudget.Checked;
            currentNeedsAnalysisInput.PrimaryWorkActionBudget.Clear();

            if (currentNeedsAnalysisInput.ApplyBudget)
            {
                currentNeedsAnalysisInput.IsMultiYearBudget = chkBigBucket.Checked;
                if (currentNeedsAnalysisInput.IsMultiYearBudget)
                {
                    try
                    {
                        Budget budget = new Budget(currentNeedsAnalysisInput.AnalysisStartYear, currentNeedsAnalysisInput.AnalysisEndYear, Convert.ToSingle(dgvBudget.Rows[0].Cells["budgetAmount"].Value));
                        currentNeedsAnalysisInput.PrimaryWorkActionBudget.Add(budget);
                    }
                    catch (Exception ex)
                    { }
                }
                else
                {
                    currentNeedsAnalysisInput.IsMultiYearBudget = false;
                    int currentRow = 0;
                    for (int year = currentNeedsAnalysisInput.AnalysisStartYear; year <= currentNeedsAnalysisInput.AnalysisEndYear; year++)
                    {
                        try
                        {
                            Budget budget = new Budget(Convert.ToInt32(dgvBudget.Rows[currentRow].Cells["budgetYear"].Value),
                                                        Convert.ToInt32(dgvBudget.Rows[currentRow].Cells["budgetYear"].Value),
                                                        Convert.ToSingle(dgvBudget.Rows[currentRow].Cells["budgetAmount"].Value));
                            currentNeedsAnalysisInput.PrimaryWorkActionBudget.Add(budget);
                        }
                        catch (Exception ex)
                        { }

                        currentRow++;
                    }
                }
            }

            currentNeedsAnalysisInput.PriorityScorePolicyEffects.Clear();
            foreach (DataGridViewRow dgvRow in dgvPolicies.Rows)
            {
                if (Convert.ToBoolean(dgvRow.Cells["CheckPolicy"].Value))
                {
                    PriorityScorePolicyEffect priorityScorePolicyEffect = new PriorityScorePolicyEffect();
                    priorityScorePolicyEffect.Year = -1;
                    string effect = dgvRow.Cells["PriorityScoreEffect"].Value.ToString().Trim();
                    priorityScorePolicyEffect.MathOperation = effect.Substring(0, 1);
                    priorityScorePolicyEffect.ScoreEffect = Convert.ToSingle(effect.Substring(1));
                    priorityScorePolicyEffect.Policy = dgvRow.Cells["Policy"].Value.ToString();
                    priorityScorePolicyEffect.PolicyCriteria = dgvRow.Cells["Criteria"].Value.ToString();
                    currentNeedsAnalysisInput.PriorityScorePolicyEffects.Add(priorityScorePolicyEffect);
                }
            }

            currentNeedsAnalysisInput.AnalysisNotes = tbxComments.Text;
            SetAnalysisFiles();
        }

        private void SetAnalysisFiles()
        {
            controller.SetAnalysisFiles(currentNeedsAnalysisInput);
        }

        private NeedsAnalysisFile CreateNeedsAnalysisFile(WisamType.NeedsAnalysisFileTypes needsAnalysisFileType, string baseDirectory, string fileExtension)
        {
            return controller.CreateNeedsAnalysisFile(needsAnalysisFileType, baseDirectory, fileExtension);
        }

        private bool ValidateNeedsAnalysisTab()
        {
            bool isValid = true;
            //TabPage naTabPage = tctMain.TabPages["tpgNeedsAnalysis"];
            
            if (!isValid)
            {

            }

            return isValid;
        }
        #endregion NeedsAnalysisEventHandlers

        public FormMain()
        {
            InitializeComponent();
            iface = new Interface();
            PopulateControls();
            SetNeedsAnalysisInput();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (userName.Equals("DOTJBB"))
            {
                btnUpdatePmic.Enabled = true;
                //btnGenerateRegionNeedsReport.Visible = true;
                button3.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
            }

            if (userName.Equals("MSCAWW"))
            {
                tctMain.SelectedTab = tpgTimesheet;
            }
            
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            iface.CloseDb();
        }

        private void RenderPolicyGrid()
        {
            dgvPolicies.Rows.Clear();
            dgvPolicies.Rows.Add();
            dgvPolicies.Rows[0].Cells["CheckPolicy"].Value = false;
            dgvPolicies.Rows[0].Cells["Policy"].Value = "Prioritize Sealing Deck - Concrete";
            dgvPolicies.Rows[0].Cells["Criteria"].Value = "WorkActionCode=35";
            dgvPolicies.Rows[0].Cells["PriorityScoreEffect"].Value = "+50";
            dgvPolicies.Rows.Add();
            dgvPolicies.Rows[1].Cells["CheckPolicy"].Value = false;
            dgvPolicies.Rows[1].Cells["Policy"].Value = "Prioritize Str Replacements";
            dgvPolicies.Rows[1].Cells["Criteria"].Value = "WorkActionCode=91";
            dgvPolicies.Rows[1].Cells["PriorityScoreEffect"].Value = "+50";
            dgvPolicies.Rows.Add();
            dgvPolicies.Rows[2].Cells["CheckPolicy"].Value = false;
            dgvPolicies.Rows[2].Cells["Policy"].Value = "Prioritize Tied Arches";
            dgvPolicies.Rows[2].Cells["Criteria"].Value = "StructureTypeCode=50";
            dgvPolicies.Rows[2].Cells["PriorityScoreEffect"].Value = "+40";
        }

        private void RenderAnalysisTypesComboBox()
        {
            foreach (var item in Enum.GetValues(typeof(WisamType.AnalysisTypes)))
            {
                cboAnalysisTypes.Items.Add(item);
            }

            cboAnalysisTypes.SelectedIndex = 0;
        }

        private void PopulateControls(bool databaseChange = false)
        {
            RenderFundingSourcesCheckListBox();
            RenderAnalysisTypesComboBox();
            RenderPolicyGrid();
            tbxTimesheetDataFile.Text = iface.TimesheetDataFileConnectionString;
            tbxMonthWeekEndingDate.Text = DateTime.Now.Month.ToString();
            tbxYearWeekEndingDate.Text = DateTime.Now.Year.ToString();
            tbxAccessDatabase.Text = iface.GetTimesheetAccessDatabaseFilePath();

            if (tbxStartYear.Text.Trim().Length == 0)
            {
                tbxStartYear.Text = (DateTime.Now.Year + 1).ToString();
            }

            if (tbxEndYear.Text.Trim().Length == 0)
            {
                tbxEndYear.Text = (DateTime.Now.Year + 20).ToString();
            }

            RenderBudgetGrid(Convert.ToInt32(tbxStartYear.Text), Convert.ToInt32(tbxEndYear.Text));

            if (tbxFirstYear.Text.Trim().Length == 0)
            {
                tbxFirstYear.Text = DateTime.Now.Year.ToString();
            }
            
            if (tbxLastYear.Text.Trim().Length == 0)
            {
                tbxLastYear.Text = (DateTime.Now.Year + 1).ToString();
            }

            Dictionary<string, string> comboSource = new Dictionary<string, string>();
            foreach (var swa in iface.GetWorkActions())
            {
                comboSource.Add(swa.WorkActionCode, swa.WorkActionDesc);
            }
            
            cbxWorkAction.DataSource = new BindingSource(comboSource, null);
            cbxWorkAction.DisplayMember = "Value";
            cbxWorkAction.ValueMember = "Key";

            ruleCategories = iface.GetRuleCategories();
            lbxRuleCategory.Items.Clear();
            foreach (var rc in ruleCategories)
            {
                lbxRuleCategory.Items.Add(rc);
            }

            // lbxRuleId
            workActionRules = iface.GetWorkActionRules();
            lbxRuleId.Items.Clear();
            foreach (var war in workActionRules)
            {
                lbxRuleId.Items.Add(war.RuleId);
            }

            lbxRuleId.Items.Add("-new rule-");

            if (lbxRuleId.Items.Count > 0)
            {
                lbxRuleId.SelectedIndex = 0;
            }

            if (lbxNbiComponent.Items.Count == 0)
            {
                lbxNbiComponent.Items.Add("DECK");
                lbxNbiComponent.Items.Add("SUPER");
                lbxNbiComponent.Items.Add("SUB");
                lbxNbiComponent.Items.Add("CULV");
            }
            lbxNbiComponent.SelectedIndex = 0;

            comboBoxQualifiedDeterioration.Items.Add("");
            foreach (var item in iface.GetQualifiedDeteriorationCurves())
            {
                comboBoxQualifiedDeterioration.Items.Add(item);
            }
            comboBoxQualifiedDeterioration.SelectedIndex = 0;

            workActions = iface.GetProgrammableWorkActions().OrderBy(e =>e.WorkActionDesc).ToList();

            if (checkedListBoxWorkActions.Items.Count == 0)
            {
                //checkedListBoxWorkActions.Items.Clear();
                foreach (var workAction in workActions)
                {
                    checkedListBoxWorkActions.Items.Add(String.Format("{0}-{1}", workAction.WorkActionCode, workAction.WorkActionDesc));
                    checkedListBoxWorkActions.SetItemChecked(checkedListBoxWorkActions.Items.Count - 1, true);
                }
            }

            string cats = "";
            int counter = 0;
            float maxScore = iface.GetPriorityIndexCategories().Sum(e => e.PriorityIndexMaxValue);
            tbxMaxPriorityScore.Text = maxScore.ToString();

            foreach (var cat in iface.GetPriorityIndexCategories())
            {
                if (counter > 0)
                {
                    cats += ", ";
                }

                cats += cat.PriorityIndexCategoryName + "(" + cat.PriorityIndexMaxValue + ")";
                counter++;
            }
            lblPiCategories.Text += cats;
        }

        private void btnGenerateStatePmdssReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.StatePmdss, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }
            else
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.StatePmdss, tbxRegions.Text.Trim(), true, true, Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnGenerateStateFiipsReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.StateFiips, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }
            else
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.StateFiips, tbxRegions.Text.Trim(), true, true, Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnGenerateStateNeedsReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.StateNeeds, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }
            else
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.StateNeeds, tbxRegions.Text.Trim(), true, true, Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnGenerateDebugReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.AnalysisDebug, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }
            else
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.AnalysisDebug, tbxRegions.Text.Trim(), true, true, Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnDeleteWorkActionCriteria_Click(object sender, EventArgs e)
        {
            int selectedRuleId = -1;

            try
            {
                selectedRuleId = (Int32)lbxRuleId.SelectedItem;
            }
            catch { }

            if (selectedRuleId == -1)
            {
                MessageBox.Show("Select a rule to delete.");
            }
            else
            {
                iface.DeleteWorkActionCriteria(selectedRuleId, Int32.Parse(tbxRuleSequence.Text.Trim()));
                lbxRuleId.Items.Clear();
                PopulateControls();
                MessageBox.Show("Done");
            }
        }

        private void btnUpdateWorkActionCriteria_Click(object sender, EventArgs e)
        {
            int selectedRuleId = -1;
            int oldRuleSequence = -1;

            try
            {
                selectedRuleId = (Int32)lbxRuleId.SelectedItem;
                oldRuleSequence = workActionRules.Where(w => w.RuleId == selectedRuleId).First().RuleSequence;
            }
            catch { }

            WorkActionRule workActionRule = new WorkActionRule();
            workActionRule.RuleId = selectedRuleId;
            workActionRule.RuleFormula = tbxRuleCriteria.Text.Trim();
            workActionRule.RuleCategory = lbxRuleCategory.SelectedItem.ToString();
            workActionRule.RuleSequence = Int32.Parse(tbxRuleSequence.Text.Trim());
            workActionRule.RuleNotes = tbxRuleNotes.Text.Trim();
            workActionRule.RuleWorkActionNotes = tbxRuleWorkActionNotes.Text.Trim();
            workActionRule.ResultingWorkActionCode = cbxWorkAction.SelectedValue.ToString();

            if (chkRuleActive.Checked)
            {
                workActionRule.Active = true;
            }
            else
            {
                workActionRule.Active = false;
            }

            iface.UpdateWorkActionCriteria(workActionRule, oldRuleSequence);

            lbxRuleId.Items.Clear();
            PopulateControls();
            MessageBox.Show("Done");
        }

        private void btnBrowsePonModDeterExcelInputFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel | *.xlsx";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbxPonModDeterExcelInputFilePath.Text = dialog.FileName;
            }
        }

        private void btnUpdatePonModDeter_Click(object sender, EventArgs e)
        {
            iface.UpdatePonModDeter(tbxPonModDeterExcelInputFilePath.Text.Trim());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.AnalysisDebug, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }
            else
            {

            }
        }

        private void lbkOpenOutputFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*
            if (File.Exists(lbkOpenOutputFile.Tag.ToString()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(lbkOpenOutputFile.Tag.ToString());
            }
            */

            if (File.Exists(tbxExcelOuputFilePath.Text.Trim()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(tbxExcelOuputFilePath.Text.Trim());
            }
         
        }

        private void btnUpdateOverlaysCombinedWorkActions_Click(object sender, EventArgs e)
        {
            iface.UpdateOverlaysCombinedWorkActions();
        }

        private void btnGenerateRulesTable_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;
            iface.CreateRulesTable(tbxMiscReportsOutputFilePath.Text.Trim());
            lbkOpenMiscReport.Enabled = true;
        }

        private void btnGenerateAllCurrentNeeds_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.AllCurrentNeeds, tbxStructureIds.Text.Trim(), DateTime.Now.Year,
                                                DateTime.Now.Year, Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                false,
                                                chkDebug.Checked ? true : false);
            }
            else
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.AllCurrentNeeds, tbxRegions.Text.Trim(), true, true, DateTime.Now.Year,
                                                DateTime.Now.Year, Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                false,
                                                chkDebug.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnCreateRulesTable_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            iface.CreateRulesTable(tbxExcelOuputFilePath.Text.Trim());
            lbkOpenOutputFile.Enabled = true;
        }

        private void btnCalculateNbiDeteriorationRates_Click(object sender, EventArgs e)
        {
            iface.UpdateNbiDeteriorationRates();
        }

        private void btnUpdatePmic_Click(object sender, EventArgs e)
        {
            iface.UpdatePmic(tbxExcelInputFilePath.Text.Trim());
        }

        private void btnGenerateFiipsReport_Click(object sender, EventArgs e)
        {
            string expressionToEvaluate = "ROUTESYSTEMON <> 'STH'";
            expressionToEvaluate = expressionToEvaluate.Replace("ROUTESYSTEMON", "'TH'");
            bool b = Convert.ToBoolean(new DataTable().Compute(expressionToEvaluate, null));
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.FiipsAnalysisDebug, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }
            else
            {
                iface.GenerateAnalysisReport(WisamType.AnalysisReports.FiipsAnalysisDebug, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnGenerateAssetManagementReport_Click(object sender, EventArgs e)
        {
            lbkAssetManagementOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxAssetManagementExcelOutputFilePath.Text = outputFilePath;
            iface.CreateAssetManagementReport(tbxAssetManagementExcelOutputFilePath.Text.Trim(), Convert.ToInt32(tbxFirstYear.Text.Trim()),
                                                Convert.ToInt32(tbxLastYear.Text.Trim()));
            lbkAssetManagementOpenOutputFile.Enabled = true;
        }

        private void lbkAssetManagementOpenOutputFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(tbxAssetManagementExcelOutputFilePath.Text.Trim()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(tbxAssetManagementExcelOutputFilePath.Text.Trim());
            }
        }

        private void btnCreateFiipsBridgeList_Click(object sender, EventArgs e)
        {
            lbkOpenFiipsBridgeListFile.Enabled = false;
            iface.CreateFiipsBridgeListFile(@"c:\temp\fiips-bridge-list.txt");
            lbkOpenFiipsBridgeListFile.Enabled = true;
        }

        private void btnGenerateRegionNeedsReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                if (tbxStructureIds.Text.Trim().Equals(""))
                {
                    MessageBox.Show("Enter structure ID(s).");
                    return;
                }

                iface.GenerateAnalysisReport(WisamType.AnalysisReports.RegionNeeds, tbxStructureIds.Text.Trim(), Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false,
                                                chkPiFactors.Checked ? true : false);
            }
            else
            {
                string regionNumber = tbxRegions.Text.Trim();

                if (!regionNumber.Equals("1") && !regionNumber.Equals("2") && !regionNumber.Equals("3") && !regionNumber.Equals("4") && !regionNumber.Equals("5"))
                {
                    MessageBox.Show("Enter a region number: 1, 2, 3, 4 or 5.");
                    return;
                }

                var stateOwned = chkStateBridges.Checked;
                var localOwned = chkLocalBridges.Checked;

                if (!stateOwned && !localOwned)
                {
                    MessageBox.Show("Check state and/or local structures.");
                    return;
                }

                iface.GenerateAnalysisReport(WisamType.AnalysisReports.RegionNeeds, regionNumber, stateOwned, localOwned, Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                chkDeteriorateOlayDefects.Checked ? true : false,
                                                DateTime.Now,
                                                true,
                                                chkDebug.Checked ? true : false, 
                                                chkPiFactors.Checked ? true : false);
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnGetNbiDeterioration_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            
            for (int i = 1; i <= 5; i++)
            {
                    string filePath = String.Format(@"c:\temp\region{0}-nbi.csv", i);
                    //iface.GetRegionNbiRatingChangeHistory(i.ToString(), filePath);
                    iface.GetRegionNbiRatingHistory(i.ToString(), filePath);
            }

            lbkOpenOutputFile.Enabled = true;

            /*
            if (File.Exists(filePath))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(tbxExcelOuputFilePath.Text.Trim());
            }*/
            
        }

        private void WisamDb_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null)
            {
                if (rb.Checked)
                {
                    if (rb.Name.ToUpper().Equals("RBTWISAMDBPROD"))
                    {
                        iface.UpdateDbConnections(WisamType.Databases.WiSamProduction);
                    }
                    else if (rb.Name.ToUpper().Equals("RBTWISAMDBTEST"))
                    {
                        iface.UpdateDbConnections(WisamType.Databases.WiSamTest);
                    }
                    else if (rb.Name.ToUpper().Equals("RBTWISAMDBDEV"))
                    {
                        iface.UpdateDbConnections(WisamType.Databases.WiSamDev);
                    }

                    lblCurrentDb.Text = rb.Text;
                    lbxRuleId.Items.Clear();
                    lbxNbiComponent.Items.Clear();
                    PopulateControls();
                }
            }
            /*
            foreach (Control control in this.gbxPickDatabase.Controls)
            {
                if (control is RadioButton)
                {
                    RadioButton radio = control as RadioButton;

                    if (radio.Checked)
                    {
                        if (radio.Name.ToUpper().Equals("RBTWISAMDBPROD"))
                        {
                            iface.UpdateDbConnections(WisamType.Databases.WiSamProduction);
                        }
                        else if (radio.Name.ToUpper().Equals("RBTWISAMDBTEST"))
                        {
                            iface.UpdateDbConnections(WisamType.Databases.WiSamTest);
                        }
                        else if (radio.Name.ToUpper().Equals("RBTWISAMDBDEV"))
                        {
                            iface.UpdateDbConnections(WisamType.Databases.WiSamDev);
                        }

                        break;
                    }
                }
            }
            */
        }

        private void lbxRuleId_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedRuleId = -1;

            try
            {
                selectedRuleId = (Int32)lbxRuleId.SelectedItem;
            }
            catch { }

            if (selectedRuleId == -1)
            {
                lbxRuleCategory.SelectedIndex = 0;
                cbxWorkAction.SelectedIndex = 0;
                tbxRuleCriteria.Text = "";
                tbxRuleNotes.Text = "";
                tbxRuleWorkActionNotes.Text = "";
                tbxRuleSequence.Text = "";
                chkRuleActive.Checked = true;
            }
            else
            {
                var workActionRule = workActionRules.Where(r => r.RuleId == selectedRuleId).First();

                // find rule category
                int rcIndex = lbxRuleCategory.FindString(workActionRule.RuleCategory.ToUpper());

                if (rcIndex != -1)
                {
                    lbxRuleCategory.SetSelected(rcIndex, true);
                }

                cbxWorkAction.SelectedValue = workActionRule.ResultingWorkAction.WorkActionCode;

                tbxRuleCriteria.Text = workActionRule.RuleFormula;
                tbxRuleNotes.Text = workActionRule.RuleNotes;
                tbxRuleWorkActionNotes.Text = workActionRule.RuleWorkActionNotes;
                tbxRuleSequence.Text = workActionRule.RuleSequence.ToString();

                if (workActionRule.Active)
                {
                    chkRuleActive.Checked = true;
                }
                else
                {
                    chkRuleActive.Checked = false;
                }
            }
        }

        private void btnAddNewWorkRule_Click(object sender, EventArgs e)
        {
            lbxRuleId.SelectedIndex = lbxRuleId.Items.Count - 1;
        }

        private void lbkOpenFiipsBridgeListFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void lbxNbiComponent_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxQualifiedDeterioration.Checked = false;
            string nbiComponent = lbxNbiComponent.SelectedItem.ToString().ToUpper();
            WisamType.NbiRatingTypes componentType = WisamType.NbiRatingTypes.Deck;
            
            if (nbiComponent.Equals("SUPER"))
            {
                componentType = WisamType.NbiRatingTypes.Superstructure;
            }
            else if (nbiComponent.Equals("SUB"))
            {
                componentType = WisamType.NbiRatingTypes.Substructure;
            }
            else if (nbiComponent.Equals("CULV"))
            {
                componentType = WisamType.NbiRatingTypes.Culvert;
            }

            // Grab selected NBI component's deterioration values
            tbxNbiComponentDeteriorationFormula.Clear();
            tbxNbiComponentDeteriorationFormula.Text = iface.GetNbiDeteriorationFormula(componentType);

            tbxNbiComponentDeterioratedRatings.Clear();
            List<NbiDeterioratedRating> deterioratedRatings= iface.GetNbiDeterioratedRatings(componentType);

            foreach (var deterioratedRating in deterioratedRatings)
            {
                tbxNbiComponentDeterioratedRatings.Text += String.Format("\t\t{0}\t\t{1}\r\n", deterioratedRating.Year, Math.Round((decimal)deterioratedRating.RatingValue, 2));
            }
        }

        private void btnRecalcNbiComponentDeterioration_Click(object sender, EventArgs e)
        {
            tbxNbiComponentDeterioratedRatings.Clear();
            List<NbiDeterioratedRating> deterioratedRatings = iface.RecalcNbiComponentDeterioration(tbxNbiComponentDeteriorationFormula.Text.Trim());
            
            foreach (var deterioratedRating in deterioratedRatings)
            {
                tbxNbiComponentDeterioratedRatings.Text += String.Format("\t\t{0}\t\t{1}\r\n", deterioratedRating.Year, Math.Round((decimal)deterioratedRating.RatingValue, 2));
            }
            MessageBox.Show("Done");
        }

        private void btnSaveNbiComponentDeterioration_Click(object sender, EventArgs e)
        {
            if (!checkBoxQualifiedDeterioration.Checked)
            {
                string nbiComponent = lbxNbiComponent.SelectedItem.ToString().ToUpper();
                WisamType.NbiRatingTypes componentType = WisamType.NbiRatingTypes.Deck;

                if (nbiComponent.Equals("SUPER"))
                {
                    componentType = WisamType.NbiRatingTypes.Superstructure;
                }
                else if (nbiComponent.Equals("SUB"))
                {
                    componentType = WisamType.NbiRatingTypes.Substructure;
                }
                else if (nbiComponent.Equals("CULV"))
                {
                    componentType = WisamType.NbiRatingTypes.Culvert;
                }

                iface.UpdateNbiDeterioration(componentType, tbxNbiComponentDeteriorationFormula.Text.Trim());
            }
            else
            {
                string qualifiedCode = comboBoxQualifiedDeterioration.SelectedItem.ToString();
                iface.UpdateNbiQualifiedDeterioration(qualifiedCode, tbxNbiComponentDeteriorationFormula.Text.Trim(), textBoxQualifiedExpression.Text.Trim());
            }

            MessageBox.Show("Done");
        }

        private void btnBrowseExcelInputFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel | *.xlsx";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbxExcelInputFilePath.Text = dialog.FileName;
            }
        }

        private void btnUpdateCorridorCodes_Click(object sender, EventArgs e)
        {
            iface.UpdateStructureCorridorCodes(tbxExcelInputFilePath.Text.Trim());
        }

        private void btnUpdateHighClearanceRoutes_Click(object sender, EventArgs e)
        {
            iface.UpdateHighClearanceRoutes(tbxExcelInputFilePath.Text.Trim());
        }

        private void lbkOpenMiscReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(tbxMiscReportsOutputFilePath.Text.Trim()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(tbxMiscReportsOutputFilePath.Text.Trim());
            }
        }

        private void btnCreateElementDeteriorationReport_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;
            iface.CreateElementDeteriorationRatesTable(tbxMiscReportsOutputFilePath.Text.Trim());
            lbkOpenMiscReport.Enabled = true;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            button1.Enabled = false;
            iface.UpdateWiSamsWithPmicStructure();
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            iface.UpdateWisamsWithPmicRoadway();
            button2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            iface.UpdateImprovement(tbxExcelInputFilePath.Text.Trim());
        }

        private bool ValidateStartEndYears()
        {
            return controller.ValidateStartEndYears(tbxStartYear.Text.Trim(), tbxEndYear.Text.Trim());
        }

        private bool ValidateStructureId()
        {
            return controller.ValidateStructureId(tbxStructureIds.Text.Trim());
        }

        private bool ValidateRegions()
        {
            bool valid = true;
            /*
            string regionNumber = tbxRegions.Text.Trim();

            if (!regionNumber.Equals("1") && !regionNumber.Equals("2") && !regionNumber.Equals("3") && !regionNumber.Equals("4") && !regionNumber.Equals("5"))
            {
                MessageBox.Show("Enter a region number: 1, 2, 3, 4 or 5.");
                valid = false;
            }
            */
            return valid;
        }

        private bool ValidateStateOrLocal()
        {
            return controller.ValidateStateOrLocal(chkStateBridges.Checked, chkLocalBridges.Checked);
        }

        private List<string> ConvertStringToList(string stringToConvert)
        {
            return controller.ConvertStringToList(stringToConvert);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Validate inputs 
            List<string> strIds = ConvertStringToList(tbxStructureIds.Text.Trim().ToUpper());
            
            if (strIds.Count == 0)
            {
                MessageBox.Show("Enter at least 1 structure ID.");
                return;
            }

            if (isProcessRunning)
            {
                MessageBox.Show("A process is already running.");
                return;
            }

            ProgressDialog progressDialog = new ProgressDialog();

            Thread backgroundThread = new Thread(
                new ThreadStart(() =>
                {
                    isProcessRunning = true;

                    // Process
                    for (int i = 0; i < 100; i++)
                    {
                        Thread.Sleep(50);
                        progressDialog.UpdateProgress(i);
                    }
                    
                    MessageBox.Show("Analysis completed.");
                    progressDialog.BeginInvoke(new Action(() => progressDialog.Close()));
                    isProcessRunning = false;
                }
            ));

            backgroundThread.Start();
            progressDialog.ShowDialog();
        }

        private void btnAnalyzeRegionNeeds_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            lbkOpenDebugFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxExcelOuputFilePath.Text = outputFilePath;
            string debugFilePath = debugFilePath = iface.GetRandomFileName(@"c:\temp", ".csv");
            

            if (ValidateStartEndYears())
            {
                if (rbtStructuresById.Checked)
                {
                    if (ValidateStructureId())
                    {
                        iface.GenerateAnalysisReport(WisamType.AnalysisReports.RegionNeedsNew,
                                                    tbxStructureIds.Text.Trim().ToUpper(),
                                                    Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                    Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                    Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                    outputFilePath,
                                                    chkDeteriorateOlayDefects.Checked ? true : false,
                                                    DateTime.Now,
                                                    true,
                                                    chkDebug.Checked ? true : false,
                                                    chkPiFactors.Checked ? true : false,
                                                    "",
                                                    chkInterpolateNbi.Checked ? true : false,
                                                    chkCountTpo.Checked ? true : false,
                                                    chkDebug.Checked ? debugFilePath : ""
                                                    );
                        //MessageBox.Show("Analysis completed.");
                        lbkOpenOutputFile.Tag = outputFilePath;
                        lbkOpenOutputFile.Enabled = true;

                        if (chkDebug.Checked)
                        {
                            lbkOpenDebugFile.Tag = debugFilePath;
                            lbkOpenDebugFile.Enabled = true;
                        }
                    }
                }
                else
                {
                    if (ValidateRegions() && ValidateStateOrLocal())
                    {
                        iface.GenerateAnalysisReport(WisamType.AnalysisReports.RegionNeedsNew,
                                                    tbxRegions.Text.Trim(),
                                                    chkStateBridges.Checked,
                                                    chkLocalBridges.Checked,
                                                    Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                    Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                    Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                    outputFilePath,
                                                    chkDeteriorateOlayDefects.Checked ? true : false,
                                                    DateTime.Now,
                                                    true,
                                                    chkDebug.Checked ? true : false,
                                                    chkPiFactors.Checked ? true : false,
                                                    "",
                                                    chkInterpolateNbi.Checked ? true : false,
                                                    chkIncludeCStructures.Checked ? true : false,
                                                    chkCountTpo.Checked ? true : false,
                                                    chkDebug.Checked ? debugFilePath : ""
                                                    );
                        //MessageBox.Show("Analysis completed.");
                        lbkOpenOutputFile.Tag = outputFilePath;
                        lbkOpenOutputFile.Enabled = true;

                        if (chkDebug.Checked)
                        {
                            lbkOpenDebugFile.Tag = debugFilePath;
                            lbkOpenDebugFile.Enabled = true;
                        }
                    }
                }
            }
        }

        private void btnAnalyzeStrDeckReplacements_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxExcelOuputFilePath.Text = outputFilePath;

            if (ValidateStartEndYears())
            {
                if (rbtStructuresById.Checked)
                {
                    if (ValidateStructureId())
                    {

                        iface.GenerateAnalysisReport(WisamType.AnalysisReports.StrDeckReplacements,
                                                        tbxStructureIds.Text.Trim().ToUpper(),
                                                        Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                        Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                        Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                        tbxExcelOuputFilePath.Text.Trim(),
                                                        chkDeteriorateOlayDefects.Checked ? true : false,
                                                        DateTime.Now,
                                                        true,
                                                        chkDebug.Checked ? true : false,
                                                        chkPiFactors.Checked ? true : false,
                                                        "06,91",
                                                        chkInterpolateNbi.Checked ? true : false,
                                                        chkCountTpo.Checked ? true : false);

                        lbkOpenOutputFile.Enabled = true;
                    }
                }
                else
                {
                    if (ValidateRegions() && ValidateStateOrLocal())
                    {

                        iface.GenerateAnalysisReport(WisamType.AnalysisReports.StrDeckReplacements,
                                                        tbxRegions.Text.Trim(),
                                                        chkStateBridges.Checked,
                                                        chkLocalBridges.Checked,
                                                        Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                        Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                        Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                        tbxExcelOuputFilePath.Text.Trim(),
                                                        chkDeteriorateOlayDefects.Checked ? true : false,
                                                        DateTime.Now,
                                                        true,
                                                        chkDebug.Checked ? true : false,
                                                        chkPiFactors.Checked ? true : false,
                                                        "06,91",
                                                        chkInterpolateNbi.Checked ? true : false,
                                                        chkIncludeCStructures.Checked ? true : false,
                                                        chkCountTpo.Checked ? true : false);

                        lbkOpenOutputFile.Enabled = true;
                    }
                }
            }
        }

        private void btnAnalyzeFlexibleScenario_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxExcelOuputFilePath.Text = outputFilePath;

            if (ValidateStartEndYears())
            {
                if (checkedListBoxWorkActions.SelectedItems.Count > 0)
                {
                    var workActionCodes = "";
                    var counter = 0;

                    foreach (var item in checkedListBoxWorkActions.CheckedItems)
                    {
                        if (counter > 0)
                        {
                            workActionCodes += ",";
                        }
                        var curCode = item.ToString().Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries).First();
                        workActionCodes += curCode;
                        counter++;
                    }

                    if (rbtStructuresById.Checked)
                    {
                        if (ValidateStructureId())
                        {

                            iface.GenerateAnalysisReport(WisamType.AnalysisReports.Flexible,
                                                            tbxStructureIds.Text.Trim().ToUpper(),
                                                            Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                            Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                            Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                            tbxExcelOuputFilePath.Text.Trim(),
                                                            chkDeteriorateOlayDefects.Checked ? true : false,
                                                            DateTime.Now,
                                                            true,
                                                            chkDebug.Checked ? true : false,
                                                            chkPiFactors.Checked ? true : false,
                                                            workActionCodes,
                                                            chkInterpolateNbi.Checked ? true : false,
                                                            chkCountTpo.Checked ? true : false);

                            lbkOpenOutputFile.Enabled = true;
                        }
                    }
                    else
                    {
                        if (ValidateRegions() && ValidateStateOrLocal())
                        {

                            iface.GenerateAnalysisReport(WisamType.AnalysisReports.Flexible,
                                                            tbxRegions.Text.Trim(),
                                                            chkStateBridges.Checked,
                                                            chkLocalBridges.Checked,
                                                            Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                            Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                            Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                            tbxExcelOuputFilePath.Text.Trim(),
                                                            chkDeteriorateOlayDefects.Checked ? true : false,
                                                            DateTime.Now,
                                                            true,
                                                            chkDebug.Checked ? true : false,
                                                            chkPiFactors.Checked ? true : false,
                                                            workActionCodes,
                                                            chkInterpolateNbi.Checked ? true : false,
                                                            chkIncludeCStructures.Checked ? true : false,
                                                            chkCountTpo.Checked ? true : false);

                            lbkOpenOutputFile.Enabled = true;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select at least one work action in Flexible Scenario.");
                }
            }
        }

        private void btnGenerateLocalBridgeProgramReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxExcelOuputFilePath.Text = outputFilePath;

            iface.GenerateLocalProgramNeedsReport(iface.GetFiscalYear() + 1, iface.GetFiscalYear() + 1, Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                chkDeteriorateOlayDefects.Checked ? true : false, true, chkDebug.Checked ? true : false, chkPiFactors.Checked ? true : false, false, chkCountTpo.Checked ? true : false);

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnGenerateStatePmdssAndNeedsReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;

            if (rbtStructuresById.Checked)
            {
                if (ValidateStructureId())
                {
                    iface.GenerateAnalysisReport(WisamType.AnalysisReports.StateNeedsPmdss, 
                                                    tbxStructureIds.Text.Trim().ToUpper(), 
                                                    Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                    Convert.ToInt32(tbxEndYear.Text.Trim()), 
                                                    Convert.ToInt32(tbxCaiId.Text.Trim()), 
                                                    tbxExcelOuputFilePath.Text.Trim(),
                                                    chkDeteriorateOlayDefects.Checked ? true : false,
                                                    DateTime.Now,
                                                    true,
                                                    chkDebug.Checked ? true : false);
                }
            }
            else
            {
                if (ValidateRegions() && ValidateStateOrLocal())
                {

                    iface.GenerateAnalysisReport(WisamType.AnalysisReports.StateNeedsPmdss,
                                                    tbxRegions.Text.Trim(),
                                                    chkStateBridges.Checked,
                                                    chkLocalBridges.Checked,
                                                    Convert.ToInt32(tbxStartYear.Text.Trim()) - 1,
                                                    Convert.ToInt32(tbxEndYear.Text.Trim()), Convert.ToInt32(tbxCaiId.Text.Trim()), tbxExcelOuputFilePath.Text.Trim(),
                                                    chkDeteriorateOlayDefects.Checked ? true : false,
                                                    DateTime.Now,
                                                    true,
                                                    chkDebug.Checked ? true : false);
                }
            }

            lbkOpenOutputFile.Enabled = true;
        }

        private void btnCreateBidItemsReport_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;
            iface.CreateBidItemsReport(tbxMiscReportsOutputFilePath.Text.Trim(), Convert.ToInt32(tbxStartLetYear.Text.Trim()), Convert.ToInt32(tbxEndLetYear.Text.Trim()));
            lbkOpenMiscReport.Enabled = true;
        }

        private void btnCreateDesignBillableReport_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;

            try
            {
                iface.CreateDesignBillableReport(tbxMiscReportsOutputFilePath.Text.Trim(), 
                                                    Convert.ToInt32(tbxStartMonthBillableReport.Text.Trim()), 
                                                    Convert.ToInt32(tbxEndMonthBillableReport.Text.Trim()), 
                                                    Convert.ToInt32(tbxStartYearBillableReport.Text.Trim()), 
                                                    Convert.ToInt32(tbxEndYearBillableReport.Text.Trim())
                                                 );
                lbkOpenMiscReport.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnSelectAllWorkActions_Click(object sender, EventArgs e)
        {
            CheckStateListBoxWorkActions(true);
        }

        private void btnUnselectAllWorkActions_Click(object sender, EventArgs e)
        {
            CheckStateListBoxWorkActions(false);
        }

        private void CheckStateListBoxWorkActions(bool state)
        {
            for (int i = 0; i < checkedListBoxWorkActions.Items.Count; i++)
            {
                checkedListBoxWorkActions.SetItemChecked(i, state);
            }
        }

        private void btnImportTimesheet_Click(object sender, EventArgs e)
        {
            tbxImportResults.Text = "";
            btnImportTimesheet.Enabled = false;
            lbkOpenBillableReport.Enabled = false;
            string results = iface.ImportTimesheetData(tbxTimesheetDataFile.Text.Trim(), Convert.ToInt32(tbxMonthWeekEndingDate.Text.Trim()), Convert.ToInt32(tbxYearWeekEndingDate.Text.Trim()));
            string billableReportPath = Path.GetRandomFileName();
            string fileExt = Path.GetExtension(billableReportPath);
            billableReportPath = billableReportPath.Replace(fileExt, ".xlsx");
            billableReportPath = Path.Combine(@"c:\temp", billableReportPath);

            try
            {
                iface.CreateDesignBillableReport(billableReportPath,
                                                    Convert.ToInt32(tbxMonthWeekEndingDate.Text.Trim()),
                                                    Convert.ToInt32(tbxMonthWeekEndingDate.Text.Trim()),
                                                    Convert.ToInt32(tbxYearWeekEndingDate.Text.Trim()),
                                                    Convert.ToInt32(tbxYearWeekEndingDate.Text.Trim())
                                                 );
                lbkOpenBillableReport.Tag = billableReportPath;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            tbxImportResults.Text = results;
            btnImportTimesheet.Enabled = true;
            lbkOpenBillableReport.Enabled = true;
        }

        private void btnBrowseTimesheetDataFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Timesheet Data Files|*.xlsm;*.xlsx;*.xls";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbxTimesheetDataFile.Text = dlg.FileName;
            }
        }

        private void btnBrowseAccessDatabase_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Timesheet Access Database Files|*.accdb";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbxAccessDatabase.Text = dlg.FileName;
                if (!dlg.FileName.Equals(iface.GetTimesheetAccessDatabaseFilePath()))
                {
                    if (!iface.UpdateTimesheetDbConnection(dlg.FileName))
                    {
                        MessageBox.Show("Unable to connect to the Access database.");
                    }
                }
            }
        }

        private void lbkOpenBillableReport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(lbkOpenBillableReport.Tag.ToString()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(lbkOpenBillableReport.Tag.ToString());
            }
        }

        private void btnGetCoreData_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            string fileExt = Path.GetExtension(outputFilePath);
            outputFilePath = outputFilePath.Replace(fileExt, ".csv");
            tbxExcelOuputFilePath.Text = outputFilePath;

            if (rbtStructuresById.Checked)
            {
                if (ValidateStructureId())
                {
                    iface.GenerateCoreDataReport(tbxStructureIds.Text.Trim(), outputFilePath, chkCountTpo.Checked ? true : false);
                    lbkOpenOutputFile.Enabled = true;
                }
            }
            else
            {
                if (ValidateRegions() && ValidateStateOrLocal())
                {
                    iface.GenerateCoreDataReport(tbxRegions.Text.Trim().Split(new char[] { ',' }).ToList(), outputFilePath, chkStateBridges.Checked, chkLocalBridges.Checked, chkIncludeCStructures.Checked, chkCountTpo.Checked);
                    lbkOpenOutputFile.Enabled = true;
                }
            }
        }

        private void btnCreateStructureProgramReport_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;
            iface.CreateStructureProgramReport(tbxMiscReportsOutputFilePath.Text.Trim(), Convert.ToInt32(tbxStartLetYear.Text.Trim()), Convert.ToInt32(tbxEndLetYear.Text.Trim()));
            lbkOpenMiscReport.Enabled = true;
        }

        private void btnCreateMetaReport_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxExcelOuputFilePath.Text = outputFilePath;

            if (ValidateStartEndYears())
            {
                if (rbtStructuresById.Checked)
                {
                    if (ValidateStructureId())
                    {
                        iface.GenerateAnalysisReport(WisamType.AnalysisReports.MetaManager,
                                                    tbxStructureIds.Text.Trim().ToUpper(),
                                                    Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                    Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                    Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                    outputFilePath,
                                                    chkDeteriorateOlayDefects.Checked ? true : false,
                                                    DateTime.Now,
                                                    true,
                                                    chkDebug.Checked ? true : false,
                                                    chkPiFactors.Checked ? true : false,
                                                    "",
                                                    chkInterpolateNbi.Checked ? true : false,
                                                    chkCountTpo.Checked ? true : false
                                                    );
                        //MessageBox.Show("Analysis completed.");
                        lbkOpenOutputFile.Tag = outputFilePath;
                        lbkOpenOutputFile.Enabled = true;
                    }
                }
                else
                {
                    if (ValidateRegions() && ValidateStateOrLocal())
                    {
                        iface.GenerateAnalysisReport(WisamType.AnalysisReports.MetaManager,
                                                    tbxRegions.Text.Trim(),
                                                    chkStateBridges.Checked,
                                                    chkLocalBridges.Checked,
                                                    Convert.ToInt32(tbxStartYear.Text.Trim()),
                                                    Convert.ToInt32(tbxEndYear.Text.Trim()),
                                                    Convert.ToInt32(tbxCaiId.Text.Trim()),
                                                    outputFilePath,
                                                    chkDeteriorateOlayDefects.Checked ? true : false,
                                                    DateTime.Now,
                                                    true,
                                                    chkDebug.Checked ? true : false,
                                                    chkPiFactors.Checked ? true : false,
                                                    "",
                                                    chkInterpolateNbi.Checked ? true : false,
                                                    chkIncludeCStructures.Checked ? true : false,
                                                    chkCountTpo.Checked ? true : false
                                                    );
                        //MessageBox.Show("Analysis completed.");
                        lbkOpenOutputFile.Tag = outputFilePath;
                        lbkOpenOutputFile.Enabled = true;
                    }
                }
            }
        }

        private void lbkOpenDebugFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (File.Exists(lbkOpenDebugFile.Tag.ToString().Trim()))
            {
                xlsApp = new Excel.Application();
                xlsApp.Visible = true;
                xlsBooks = xlsApp.Workbooks;
                Excel.Workbook xlsBook = xlsBooks.Open(lbkOpenDebugFile.Tag.ToString().Trim());
            }
        }

        private void tsbNeedsAnalysisRun_Click(object sender, EventArgs e)
        {
            RunNeedsAnalysis();
        }

        private void cboAnalysisTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckStateListBoxWorkActions(false);

        }

        private void tctMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tctMain.SelectedTab == tpgAdmin) && (!userName.Equals("DOTP3M") && !userName.Equals("DOTRYB") && !userName.Equals("DOTJBB")))
            {
                MessageBox.Show("Unable to load 'Admin' tab. You have insufficient access privileges.");
                tctMain.SelectedTab = tpgNeedsAnalysis;
            }
            else
            {
                
                //tabControl1.SelectedTab = tabPage3;
            }
        }

        private void btnCreateLetDatesReport_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomFileName(@"c:\temp", ".csv");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;
            iface.CreateLetDatesReport(tbxMiscReportsOutputFilePath.Text.Trim(), Convert.ToInt32(tbxStartLetYear.Text.Trim()), Convert.ToInt32(tbxEndLetYear.Text.Trim()));
            lbkOpenMiscReport.Enabled = true;
        }

        private void btnSetAnnualBudget_Click(object sender, EventArgs e)
        {
            int annualBudget = 0;

            if (int.TryParse(tbxAnnualBudget.Text.Trim(), out annualBudget))
            {
                UpdateBudgetGrid(annualBudget, currentNeedsAnalysisInput.AnalysisStartYear, currentNeedsAnalysisInput.AnalysisEndYear);
            }
            else
            {
                MessageBox.Show("Enter a valid annual budget.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            iface.UpdateWorkDuplicates();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            iface.UpdateIsBridge();
        }

        private void buttonGetStructuresDataForGis_Click(object sender, EventArgs e)
        {
            lbkOpenMiscReport.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxMiscReportsOutputFilePath.Text = outputFilePath;
            iface.GetStructuresDataForGis(tbxMiscReportsOutputFilePath.Text.Trim());
            lbkOpenMiscReport.Enabled = true;
        }

        private void comboBoxQualifiedDeterioration_SelectedIndexChanged(object sender, EventArgs e)
        {
            string qualifiedCode = comboBoxQualifiedDeterioration.SelectedItem.ToString();
            textBoxQualifiedExpression.Clear();
            tbxNbiComponentDeteriorationFormula.Clear();
            tbxNbiComponentDeterioratedRatings.Clear();
            checkBoxQualifiedDeterioration.Checked = false;

            if (!String.IsNullOrEmpty(qualifiedCode))
            {
                checkBoxQualifiedDeterioration.Checked = true;
                textBoxQualifiedExpression.Text = iface.GetNbiQualificationExpression(qualifiedCode);
                tbxNbiComponentDeteriorationFormula.Text = iface.GetNbiQualifiedDeteriorationFormula(qualifiedCode);
                List<NbiDeterioratedRating> deterioratedRatings = iface.GetNbiQualifiedDeterioratedRatings(qualifiedCode);

                foreach (var deterioratedRating in deterioratedRatings)
                {
                    tbxNbiComponentDeterioratedRatings.Text += String.Format("\t\t{0}\t\t{1}\r\n", deterioratedRating.Year, Math.Round((decimal)deterioratedRating.RatingValue, 2));
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            lbkOpenOutputFile.Enabled = false;
            string outputFilePath = iface.GetRandomExcelFileName(@"c:\temp");
            tbxExcelOuputFilePath.Text = outputFilePath;
            //List<string> structureIds = dbObj.GetStructuresByRegion("1", true, false, false);



            iface.GenerateStateUnconstrainedMaintenanceNeedsReport(outputFilePath, Convert.ToInt32(tbxCaiId.Text));

            lbkOpenOutputFile.Enabled = true;
        }






        // End: btnGetCoreData_Click()
    }
}  
