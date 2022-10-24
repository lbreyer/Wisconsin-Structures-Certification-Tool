namespace WiSam.Desktop
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.tctMain = new System.Windows.Forms.TabControl();
            this.tpgNeedsAnalysis = new System.Windows.Forms.TabPage();
            this.gbxResults = new System.Windows.Forms.GroupBox();
            this.tbxRegions = new System.Windows.Forms.TextBox();
            this.tbxExcelOuputFilePath = new System.Windows.Forms.TextBox();
            this.btnAnalyzeRegionNeeds = new System.Windows.Forms.Button();
            this.tsrNeedsAnalysis = new System.Windows.Forms.ToolStrip();
            this.tsbNeedsAnalysisOpenFile = new System.Windows.Forms.ToolStripButton();
            this.tsbNeedsAnalysisSaveInput = new System.Windows.Forms.ToolStripButton();
            this.tsbNeedsAnalysisSaveAs = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbNeedsAnalysisRun = new System.Windows.Forms.ToolStripButton();
            this.lbkOpenDebugFile = new System.Windows.Forms.LinkLabel();
            this.btnRunNeedsAnalysis = new System.Windows.Forms.Button();
            this.gbxComments = new System.Windows.Forms.GroupBox();
            this.tbxComments = new System.Windows.Forms.TextBox();
            this.lbkOpenOutputFile = new System.Windows.Forms.LinkLabel();
            this.cboAnalysisTypes = new System.Windows.Forms.ComboBox();
            this.label27 = new System.Windows.Forms.Label();
            this.gbxPolicy = new System.Windows.Forms.GroupBox();
            this.lblPiCategories = new System.Windows.Forms.Label();
            this.tbxMaxPriorityScore = new System.Windows.Forms.TextBox();
            this.lblPriorityScoreInfo = new System.Windows.Forms.Label();
            this.dgvPolicies = new System.Windows.Forms.DataGridView();
            this.CheckPolicy = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Policy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Criteria = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PriorityScoreEffect = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbxMiscellaneous = new System.Windows.Forms.GroupBox();
            this.tbxElements = new System.Windows.Forms.TextBox();
            this.chkBridgeInventory = new System.Windows.Forms.CheckBox();
            this.chkDeteriorateOlayDefects = new System.Windows.Forms.CheckBox();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.chkInterpolateNbi = new System.Windows.Forms.CheckBox();
            this.chkPiFactors = new System.Windows.Forms.CheckBox();
            this.chkCountTpo = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.gbxBudget = new System.Windows.Forms.GroupBox();
            this.btnSetAnnualBudget = new System.Windows.Forms.Button();
            this.tbxAnnualBudget = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.tbxLeastCost = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.chkApplyBudget = new System.Windows.Forms.CheckBox();
            this.chkBigBucket = new System.Windows.Forms.CheckBox();
            this.dgvBudget = new System.Windows.Forms.DataGridView();
            this.budgetYear = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.budgetAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbxEnterOtherCriteria = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButtonBrmDeterioration = new System.Windows.Forms.RadioButton();
            this.radioButtonOldDeterioration = new System.Windows.Forms.RadioButton();
            this.gbxYearType = new System.Windows.Forms.GroupBox();
            this.rbtCalendarYear = new System.Windows.Forms.RadioButton();
            this.rbtFederalFiscalYear = new System.Windows.Forms.RadioButton();
            this.rbtStateFiscalYear = new System.Windows.Forms.RadioButton();
            this.tbxStartYear = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxEndYear = new System.Windows.Forms.TextBox();
            this.gbxPickWorkTypes = new System.Windows.Forms.GroupBox();
            this.btnUnselectAllWorkActions = new System.Windows.Forms.Button();
            this.btnSelectAllWorkActions = new System.Windows.Forms.Button();
            this.checkedListBoxWorkActions = new System.Windows.Forms.CheckedListBox();
            this.gbxPickStructures = new System.Windows.Forms.GroupBox();
            this.chkApplyRegionSelections = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.chkIncludeCStructures = new System.Windows.Forms.CheckBox();
            this.tbxMaxNumToAnalyze = new System.Windows.Forms.TextBox();
            this.checkedListBoxFundings = new System.Windows.Forms.CheckedListBox();
            this.rbtStructuresByFunding = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkNorthwest = new System.Windows.Forms.CheckBox();
            this.chkNorthcentral = new System.Windows.Forms.CheckBox();
            this.chkLocalBridges = new System.Windows.Forms.CheckBox();
            this.chkNortheast = new System.Windows.Forms.CheckBox();
            this.chkStateBridges = new System.Windows.Forms.CheckBox();
            this.chkSoutheast = new System.Windows.Forms.CheckBox();
            this.chkSouthwest = new System.Windows.Forms.CheckBox();
            this.rbtStructuresById = new System.Windows.Forms.RadioButton();
            this.rbtStructuresByRegion = new System.Windows.Forms.RadioButton();
            this.tbxStructureIds = new System.Windows.Forms.TextBox();
            this.tpgSettings = new System.Windows.Forms.TabPage();
            this.tpgAdmin = new System.Windows.Forms.TabPage();
            this.tctAdmin = new System.Windows.Forms.TabControl();
            this.tpgWorkAction = new System.Windows.Forms.TabPage();
            this.btnUpdateOverlaysCombinedWorkActions = new System.Windows.Forms.Button();
            this.gbxUpdateWorkCriteria = new System.Windows.Forms.GroupBox();
            this.tbxRuleWorkActionNotes = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.btnDeleteWorkActionCriteria = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAddNewWorkRule = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxRuleSequence = new System.Windows.Forms.TextBox();
            this.lblRuleSequence = new System.Windows.Forms.Label();
            this.lblActive = new System.Windows.Forms.Label();
            this.cbxWorkAction = new System.Windows.Forms.ComboBox();
            this.lblWorkAction = new System.Windows.Forms.Label();
            this.chkRuleActive = new System.Windows.Forms.CheckBox();
            this.tbxRuleNotes = new System.Windows.Forms.TextBox();
            this.lbxRuleCategory = new System.Windows.Forms.ListBox();
            this.lblRuleNotes = new System.Windows.Forms.Label();
            this.lblRuleCategory = new System.Windows.Forms.Label();
            this.lblRuleCriteria = new System.Windows.Forms.Label();
            this.lblRuleId = new System.Windows.Forms.Label();
            this.tbxRuleCriteria = new System.Windows.Forms.TextBox();
            this.lbxRuleId = new System.Windows.Forms.ListBox();
            this.btnUpdateWorkActionCriteria = new System.Windows.Forms.Button();
            this.tpgDeterioration = new System.Windows.Forms.TabPage();
            this.gbxUpdateDeteriorationCurves = new System.Windows.Forms.GroupBox();
            this.label32 = new System.Windows.Forms.Label();
            this.textBoxQualifiedExpression = new System.Windows.Forms.TextBox();
            this.checkBoxQualifiedDeterioration = new System.Windows.Forms.CheckBox();
            this.comboBoxQualifiedDeterioration = new System.Windows.Forms.ComboBox();
            this.label31 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tbxNbiComponentDeterioratedRatings = new System.Windows.Forms.TextBox();
            this.btnSaveNbiComponentDeterioration = new System.Windows.Forms.Button();
            this.btnRecalcNbiComponentDeterioration = new System.Windows.Forms.Button();
            this.lblNbiComponentDeteriorationFormula = new System.Windows.Forms.Label();
            this.lblNbiComponent = new System.Windows.Forms.Label();
            this.tbxNbiComponentDeteriorationFormula = new System.Windows.Forms.TextBox();
            this.lbxNbiComponent = new System.Windows.Forms.ListBox();
            this.btnBrowsePonModDeterExcelInputFile = new System.Windows.Forms.Button();
            this.lblPonModDeterExcelInputFilePath = new System.Windows.Forms.Label();
            this.tbxPonModDeterExcelInputFilePath = new System.Windows.Forms.TextBox();
            this.btnUpdatePonModDeter = new System.Windows.Forms.Button();
            this.btnCalculateNbiDeteriorationRates = new System.Windows.Forms.Button();
            this.tpgFiips = new System.Windows.Forms.TabPage();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnUpdateHighClearanceRoutes = new System.Windows.Forms.Button();
            this.btnUpdateCorridorCodes = new System.Windows.Forms.Button();
            this.btnBrowseExcelInputFile = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbxExcelInputFilePath = new System.Windows.Forms.TextBox();
            this.btnUpdatePmic = new System.Windows.Forms.Button();
            this.lbkOpenFiipsBridgeListFile = new System.Windows.Forms.LinkLabel();
            this.btnCreateFiipsBridgeList = new System.Windows.Forms.Button();
            this.tpgMiscReports = new System.Windows.Forms.TabPage();
            this.buttonGetStructuresDataForGis = new System.Windows.Forms.Button();
            this.btnCreateLetDatesReport = new System.Windows.Forms.Button();
            this.btnCreateStructureProgramReport = new System.Windows.Forms.Button();
            this.tbxStartMonthBillableReport = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.tbxStartYearBillableReport = new System.Windows.Forms.TextBox();
            this.tbxEndMonthBillableReport = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.tbxEndYearBillableReport = new System.Windows.Forms.TextBox();
            this.btnCreateDesignBillableReport = new System.Windows.Forms.Button();
            this.btnCreateBidItemsReport = new System.Windows.Forms.Button();
            this.tbxStartLetYear = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.tbxEndLetYear = new System.Windows.Forms.TextBox();
            this.lbkOpenMiscReport = new System.Windows.Forms.LinkLabel();
            this.label11 = new System.Windows.Forms.Label();
            this.tbxMiscReportsOutputFilePath = new System.Windows.Forms.TextBox();
            this.btnCreateElementDeteriorationReport = new System.Windows.Forms.Button();
            this.btnCreateRulesTable = new System.Windows.Forms.Button();
            this.tpgAssetManagement = new System.Windows.Forms.TabPage();
            this.tbxFirstYear = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.tbxLastYear = new System.Windows.Forms.TextBox();
            this.lbkAssetManagementOpenOutputFile = new System.Windows.Forms.LinkLabel();
            this.lblAssetManagementExcelOutputFile = new System.Windows.Forms.Label();
            this.tbxAssetManagementExcelOutputFilePath = new System.Windows.Forms.TextBox();
            this.btnGenerateAssetManagementReport = new System.Windows.Forms.Button();
            this.tpgTimesheet = new System.Windows.Forms.TabPage();
            this.lbkOpenBillableReport = new System.Windows.Forms.LinkLabel();
            this.btnBrowseAccessDatabase = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.tbxAccessDatabase = new System.Windows.Forms.TextBox();
            this.btnBrowseTimesheetDataFile = new System.Windows.Forms.Button();
            this.label25 = new System.Windows.Forms.Label();
            this.tbxImportResults = new System.Windows.Forms.TextBox();
            this.tbxMonthWeekEndingDate = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.tbxYearWeekEndingDate = new System.Windows.Forms.TextBox();
            this.btnImportTimesheet = new System.Windows.Forms.Button();
            this.label22 = new System.Windows.Forms.Label();
            this.tbxTimesheetDataFile = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.gbxCreateReports = new System.Windows.Forms.GroupBox();
            this.btnCreateMetaReport = new System.Windows.Forms.Button();
            this.btnGetCoreData = new System.Windows.Forms.Button();
            this.btnAnalyzeFlexibleScenario = new System.Windows.Forms.Button();
            this.btnAnalyzeStrDeckReplacements = new System.Windows.Forms.Button();
            this.tbxCaiId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGenerateRegionNeedsReport = new System.Windows.Forms.Button();
            this.btnGetNbiDeterioration = new System.Windows.Forms.Button();
            this.btnGenerateLocalBridgeProgramReport = new System.Windows.Forms.Button();
            this.btnGenerateStatePmdssAndNeedsReport = new System.Windows.Forms.Button();
            this.btnGenerateFiipsReport = new System.Windows.Forms.Button();
            this.btnGenerateAllCurrentNeeds = new System.Windows.Forms.Button();
            this.btnGenerateDebugReport = new System.Windows.Forms.Button();
            this.btnGenerateStatePmdssReport = new System.Windows.Forms.Button();
            this.btnGenerateStateNeedsReport = new System.Windows.Forms.Button();
            this.btnGenerateStateFiipsReport = new System.Windows.Forms.Button();
            this.gbxPickDatabase = new System.Windows.Forms.GroupBox();
            this.rbtWisamDbDev = new System.Windows.Forms.RadioButton();
            this.rbtWisamDbTest = new System.Windows.Forms.RadioButton();
            this.rbtWisamDbProd = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.lblCurrentDb = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tctMain.SuspendLayout();
            this.tpgNeedsAnalysis.SuspendLayout();
            this.tsrNeedsAnalysis.SuspendLayout();
            this.gbxComments.SuspendLayout();
            this.gbxPolicy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPolicies)).BeginInit();
            this.gbxMiscellaneous.SuspendLayout();
            this.gbxBudget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).BeginInit();
            this.gbxEnterOtherCriteria.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.gbxYearType.SuspendLayout();
            this.gbxPickWorkTypes.SuspendLayout();
            this.gbxPickStructures.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tpgAdmin.SuspendLayout();
            this.tctAdmin.SuspendLayout();
            this.tpgWorkAction.SuspendLayout();
            this.gbxUpdateWorkCriteria.SuspendLayout();
            this.tpgDeterioration.SuspendLayout();
            this.gbxUpdateDeteriorationCurves.SuspendLayout();
            this.tpgFiips.SuspendLayout();
            this.tpgMiscReports.SuspendLayout();
            this.tpgAssetManagement.SuspendLayout();
            this.tpgTimesheet.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbxCreateReports.SuspendLayout();
            this.gbxPickDatabase.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // tctMain
            // 
            this.tctMain.Controls.Add(this.tpgNeedsAnalysis);
            this.tctMain.Controls.Add(this.tpgSettings);
            this.tctMain.Controls.Add(this.tpgAdmin);
            this.tctMain.Controls.Add(this.tpgMiscReports);
            this.tctMain.Controls.Add(this.tpgAssetManagement);
            this.tctMain.Controls.Add(this.tpgTimesheet);
            this.tctMain.Controls.Add(this.tabPage1);
            this.tctMain.Location = new System.Drawing.Point(13, 23);
            this.tctMain.Margin = new System.Windows.Forms.Padding(4);
            this.tctMain.Name = "tctMain";
            this.tctMain.SelectedIndex = 0;
            this.tctMain.Size = new System.Drawing.Size(1457, 661);
            this.tctMain.TabIndex = 0;
            this.tctMain.SelectedIndexChanged += new System.EventHandler(this.tctMain_SelectedIndexChanged);
            // 
            // tpgNeedsAnalysis
            // 
            this.tpgNeedsAnalysis.Controls.Add(this.gbxResults);
            this.tpgNeedsAnalysis.Controls.Add(this.tbxRegions);
            this.tpgNeedsAnalysis.Controls.Add(this.tbxExcelOuputFilePath);
            this.tpgNeedsAnalysis.Controls.Add(this.btnAnalyzeRegionNeeds);
            this.tpgNeedsAnalysis.Controls.Add(this.tsrNeedsAnalysis);
            this.tpgNeedsAnalysis.Controls.Add(this.lbkOpenDebugFile);
            this.tpgNeedsAnalysis.Controls.Add(this.btnRunNeedsAnalysis);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxComments);
            this.tpgNeedsAnalysis.Controls.Add(this.lbkOpenOutputFile);
            this.tpgNeedsAnalysis.Controls.Add(this.cboAnalysisTypes);
            this.tpgNeedsAnalysis.Controls.Add(this.label27);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxPolicy);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxMiscellaneous);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxBudget);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxEnterOtherCriteria);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxPickWorkTypes);
            this.tpgNeedsAnalysis.Controls.Add(this.gbxPickStructures);
            this.tpgNeedsAnalysis.Location = new System.Drawing.Point(4, 25);
            this.tpgNeedsAnalysis.Margin = new System.Windows.Forms.Padding(4);
            this.tpgNeedsAnalysis.Name = "tpgNeedsAnalysis";
            this.tpgNeedsAnalysis.Padding = new System.Windows.Forms.Padding(4);
            this.tpgNeedsAnalysis.Size = new System.Drawing.Size(1449, 632);
            this.tpgNeedsAnalysis.TabIndex = 0;
            this.tpgNeedsAnalysis.Text = "Needs Analysis";
            this.tpgNeedsAnalysis.UseVisualStyleBackColor = true;
            // 
            // gbxResults
            // 
            this.gbxResults.Location = new System.Drawing.Point(934, 435);
            this.gbxResults.Name = "gbxResults";
            this.gbxResults.Size = new System.Drawing.Size(471, 190);
            this.gbxResults.TabIndex = 0;
            this.gbxResults.TabStop = false;
            this.gbxResults.Text = "Results";
            // 
            // tbxRegions
            // 
            this.tbxRegions.Location = new System.Drawing.Point(1372, 42);
            this.tbxRegions.Margin = new System.Windows.Forms.Padding(4);
            this.tbxRegions.MaxLength = 9;
            this.tbxRegions.Name = "tbxRegions";
            this.tbxRegions.Size = new System.Drawing.Size(33, 22);
            this.tbxRegions.TabIndex = 40;
            this.tbxRegions.Text = "4";
            // 
            // tbxExcelOuputFilePath
            // 
            this.tbxExcelOuputFilePath.Enabled = false;
            this.tbxExcelOuputFilePath.Location = new System.Drawing.Point(1319, 43);
            this.tbxExcelOuputFilePath.Name = "tbxExcelOuputFilePath";
            this.tbxExcelOuputFilePath.Size = new System.Drawing.Size(33, 22);
            this.tbxExcelOuputFilePath.TabIndex = 87;
            // 
            // btnAnalyzeRegionNeeds
            // 
            this.btnAnalyzeRegionNeeds.Location = new System.Drawing.Point(1067, 38);
            this.btnAnalyzeRegionNeeds.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnalyzeRegionNeeds.Name = "btnAnalyzeRegionNeeds";
            this.btnAnalyzeRegionNeeds.Size = new System.Drawing.Size(71, 32);
            this.btnAnalyzeRegionNeeds.TabIndex = 85;
            this.btnAnalyzeRegionNeeds.Text = "Old Unconstrained Optimal";
            this.btnAnalyzeRegionNeeds.UseVisualStyleBackColor = true;
            this.btnAnalyzeRegionNeeds.Click += new System.EventHandler(this.btnAnalyzeRegionNeeds_Click);
            // 
            // tsrNeedsAnalysis
            // 
            this.tsrNeedsAnalysis.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsrNeedsAnalysis.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNeedsAnalysisOpenFile,
            this.tsbNeedsAnalysisSaveInput,
            this.tsbNeedsAnalysisSaveAs,
            this.toolStripSeparator1,
            this.tsbNeedsAnalysisRun});
            this.tsrNeedsAnalysis.Location = new System.Drawing.Point(4, 4);
            this.tsrNeedsAnalysis.Name = "tsrNeedsAnalysis";
            this.tsrNeedsAnalysis.Size = new System.Drawing.Size(1441, 27);
            this.tsrNeedsAnalysis.TabIndex = 84;
            this.tsrNeedsAnalysis.Text = "toolStrip1";
            // 
            // tsbNeedsAnalysisOpenFile
            // 
            this.tsbNeedsAnalysisOpenFile.Image = global::WiSam.Desktop.Properties.Resources.folder_edit;
            this.tsbNeedsAnalysisOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNeedsAnalysisOpenFile.Name = "tsbNeedsAnalysisOpenFile";
            this.tsbNeedsAnalysisOpenFile.Size = new System.Drawing.Size(69, 24);
            this.tsbNeedsAnalysisOpenFile.Text = "Open";
            // 
            // tsbNeedsAnalysisSaveInput
            // 
            this.tsbNeedsAnalysisSaveInput.Image = global::WiSam.Desktop.Properties.Resources.disk;
            this.tsbNeedsAnalysisSaveInput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNeedsAnalysisSaveInput.Name = "tsbNeedsAnalysisSaveInput";
            this.tsbNeedsAnalysisSaveInput.Size = new System.Drawing.Size(64, 24);
            this.tsbNeedsAnalysisSaveInput.Text = "Save";
            this.tsbNeedsAnalysisSaveInput.Click += new System.EventHandler(this.tsbNeedsAnalysisSaveInput_Click);
            // 
            // tsbNeedsAnalysisSaveAs
            // 
            this.tsbNeedsAnalysisSaveAs.Image = global::WiSam.Desktop.Properties.Resources.disk_multiple;
            this.tsbNeedsAnalysisSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNeedsAnalysisSaveAs.Name = "tsbNeedsAnalysisSaveAs";
            this.tsbNeedsAnalysisSaveAs.Size = new System.Drawing.Size(84, 24);
            this.tsbNeedsAnalysisSaveAs.Text = "Save As";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbNeedsAnalysisRun
            // 
            this.tsbNeedsAnalysisRun.Image = global::WiSam.Desktop.Properties.Resources.control_play;
            this.tsbNeedsAnalysisRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNeedsAnalysisRun.Name = "tsbNeedsAnalysisRun";
            this.tsbNeedsAnalysisRun.Size = new System.Drawing.Size(58, 24);
            this.tsbNeedsAnalysisRun.Text = "Run";
            this.tsbNeedsAnalysisRun.Click += new System.EventHandler(this.tsbNeedsAnalysisRun_Click);
            // 
            // lbkOpenDebugFile
            // 
            this.lbkOpenDebugFile.AutoSize = true;
            this.lbkOpenDebugFile.Enabled = false;
            this.lbkOpenDebugFile.Location = new System.Drawing.Point(1260, 45);
            this.lbkOpenDebugFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbkOpenDebugFile.Name = "lbkOpenDebugFile";
            this.lbkOpenDebugFile.Size = new System.Drawing.Size(50, 17);
            this.lbkOpenDebugFile.TabIndex = 76;
            this.lbkOpenDebugFile.TabStop = true;
            this.lbkOpenDebugFile.Text = "Debug";
            this.lbkOpenDebugFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbkOpenDebugFile_LinkClicked);
            // 
            // btnRunNeedsAnalysis
            // 
            this.btnRunNeedsAnalysis.Image = global::WiSam.Desktop.Properties.Resources.control_play;
            this.btnRunNeedsAnalysis.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRunNeedsAnalysis.Location = new System.Drawing.Point(492, 37);
            this.btnRunNeedsAnalysis.Name = "btnRunNeedsAnalysis";
            this.btnRunNeedsAnalysis.Size = new System.Drawing.Size(88, 32);
            this.btnRunNeedsAnalysis.TabIndex = 81;
            this.btnRunNeedsAnalysis.Text = "Run";
            this.btnRunNeedsAnalysis.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRunNeedsAnalysis.UseVisualStyleBackColor = true;
            this.btnRunNeedsAnalysis.Click += new System.EventHandler(this.btnRunNeedsAnalysis_Click);
            // 
            // gbxComments
            // 
            this.gbxComments.Controls.Add(this.tbxComments);
            this.gbxComments.Location = new System.Drawing.Point(934, 334);
            this.gbxComments.Name = "gbxComments";
            this.gbxComments.Size = new System.Drawing.Size(471, 95);
            this.gbxComments.TabIndex = 83;
            this.gbxComments.TabStop = false;
            this.gbxComments.Text = "Notes";
            // 
            // tbxComments
            // 
            this.tbxComments.Location = new System.Drawing.Point(16, 28);
            this.tbxComments.Margin = new System.Windows.Forms.Padding(4);
            this.tbxComments.MaxLength = 0;
            this.tbxComments.Multiline = true;
            this.tbxComments.Name = "tbxComments";
            this.tbxComments.Size = new System.Drawing.Size(438, 52);
            this.tbxComments.TabIndex = 27;
            // 
            // lbkOpenOutputFile
            // 
            this.lbkOpenOutputFile.AutoSize = true;
            this.lbkOpenOutputFile.Enabled = false;
            this.lbkOpenOutputFile.Location = new System.Drawing.Point(1146, 45);
            this.lbkOpenOutputFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbkOpenOutputFile.Name = "lbkOpenOutputFile";
            this.lbkOpenOutputFile.Size = new System.Drawing.Size(100, 17);
            this.lbkOpenOutputFile.TabIndex = 59;
            this.lbkOpenOutputFile.TabStop = true;
            this.lbkOpenOutputFile.Text = "Unconstrained";
            this.lbkOpenOutputFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbkOpenOutputFile_LinkClicked);
            // 
            // cboAnalysisTypes
            // 
            this.cboAnalysisTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAnalysisTypes.FormattingEnabled = true;
            this.cboAnalysisTypes.Location = new System.Drawing.Point(141, 42);
            this.cboAnalysisTypes.Name = "cboAnalysisTypes";
            this.cboAnalysisTypes.Size = new System.Drawing.Size(330, 24);
            this.cboAnalysisTypes.TabIndex = 80;
            this.cboAnalysisTypes.SelectedIndexChanged += new System.EventHandler(this.cboAnalysisTypes_SelectedIndexChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(32, 49);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(103, 17);
            this.label27.TabIndex = 79;
            this.label27.Text = "Analysis Types";
            // 
            // gbxPolicy
            // 
            this.gbxPolicy.Controls.Add(this.lblPiCategories);
            this.gbxPolicy.Controls.Add(this.tbxMaxPriorityScore);
            this.gbxPolicy.Controls.Add(this.lblPriorityScoreInfo);
            this.gbxPolicy.Controls.Add(this.dgvPolicies);
            this.gbxPolicy.Location = new System.Drawing.Point(298, 334);
            this.gbxPolicy.Name = "gbxPolicy";
            this.gbxPolicy.Size = new System.Drawing.Size(616, 291);
            this.gbxPolicy.TabIndex = 77;
            this.gbxPolicy.TabStop = false;
            this.gbxPolicy.Text = "Policies";
            // 
            // lblPiCategories
            // 
            this.lblPiCategories.AutoSize = true;
            this.lblPiCategories.Location = new System.Drawing.Point(176, 21);
            this.lblPiCategories.Name = "lblPiCategories";
            this.lblPiCategories.Size = new System.Drawing.Size(80, 17);
            this.lblPiCategories.TabIndex = 4;
            this.lblPiCategories.Text = "Categories:";
            // 
            // tbxMaxPriorityScore
            // 
            this.tbxMaxPriorityScore.Enabled = false;
            this.tbxMaxPriorityScore.Location = new System.Drawing.Point(142, 16);
            this.tbxMaxPriorityScore.Name = "tbxMaxPriorityScore";
            this.tbxMaxPriorityScore.Size = new System.Drawing.Size(27, 22);
            this.tbxMaxPriorityScore.TabIndex = 3;
            // 
            // lblPriorityScoreInfo
            // 
            this.lblPriorityScoreInfo.AutoSize = true;
            this.lblPriorityScoreInfo.Location = new System.Drawing.Point(14, 21);
            this.lblPriorityScoreInfo.Name = "lblPriorityScoreInfo";
            this.lblPriorityScoreInfo.Size = new System.Drawing.Size(126, 17);
            this.lblPriorityScoreInfo.TabIndex = 1;
            this.lblPriorityScoreInfo.Text = "Max Priority Score:";
            // 
            // dgvPolicies
            // 
            this.dgvPolicies.AllowUserToDeleteRows = false;
            this.dgvPolicies.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPolicies.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckPolicy,
            this.Policy,
            this.Criteria,
            this.PriorityScoreEffect});
            this.dgvPolicies.Location = new System.Drawing.Point(16, 53);
            this.dgvPolicies.Name = "dgvPolicies";
            this.dgvPolicies.RowHeadersWidth = 51;
            this.dgvPolicies.RowTemplate.Height = 24;
            this.dgvPolicies.Size = new System.Drawing.Size(580, 200);
            this.dgvPolicies.TabIndex = 0;
            // 
            // CheckPolicy
            // 
            this.CheckPolicy.HeaderText = "Select";
            this.CheckPolicy.MinimumWidth = 6;
            this.CheckPolicy.Name = "CheckPolicy";
            this.CheckPolicy.Width = 50;
            // 
            // Policy
            // 
            this.Policy.HeaderText = "Policy";
            this.Policy.MinimumWidth = 6;
            this.Policy.Name = "Policy";
            this.Policy.Width = 200;
            // 
            // Criteria
            // 
            this.Criteria.HeaderText = "Criteria";
            this.Criteria.MinimumWidth = 6;
            this.Criteria.Name = "Criteria";
            this.Criteria.Width = 200;
            // 
            // PriorityScoreEffect
            // 
            this.PriorityScoreEffect.HeaderText = "Priority Score Effect";
            this.PriorityScoreEffect.MaxInputLength = 4;
            this.PriorityScoreEffect.MinimumWidth = 6;
            this.PriorityScoreEffect.Name = "PriorityScoreEffect";
            this.PriorityScoreEffect.Width = 125;
            // 
            // gbxMiscellaneous
            // 
            this.gbxMiscellaneous.Controls.Add(this.tbxElements);
            this.gbxMiscellaneous.Controls.Add(this.chkBridgeInventory);
            this.gbxMiscellaneous.Controls.Add(this.chkDeteriorateOlayDefects);
            this.gbxMiscellaneous.Controls.Add(this.chkDebug);
            this.gbxMiscellaneous.Controls.Add(this.chkInterpolateNbi);
            this.gbxMiscellaneous.Controls.Add(this.chkPiFactors);
            this.gbxMiscellaneous.Controls.Add(this.chkCountTpo);
            this.gbxMiscellaneous.Controls.Add(this.label14);
            this.gbxMiscellaneous.Location = new System.Drawing.Point(701, 81);
            this.gbxMiscellaneous.Name = "gbxMiscellaneous";
            this.gbxMiscellaneous.Size = new System.Drawing.Size(213, 246);
            this.gbxMiscellaneous.TabIndex = 76;
            this.gbxMiscellaneous.TabStop = false;
            this.gbxMiscellaneous.Text = "Other Criteria";
            // 
            // tbxElements
            // 
            this.tbxElements.Location = new System.Drawing.Point(7, 190);
            this.tbxElements.Margin = new System.Windows.Forms.Padding(4);
            this.tbxElements.MaxLength = 0;
            this.tbxElements.Multiline = true;
            this.tbxElements.Name = "tbxElements";
            this.tbxElements.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxElements.Size = new System.Drawing.Size(199, 46);
            this.tbxElements.TabIndex = 60;
            this.tbxElements.Text = "510,8000,8511,8512,8513,8514,8515,3210,3220,8911,12,13,15,16,28,29,30,31,38,8039," +
    "54,60,65,1080,1130,8516,3440";
            // 
            // chkBridgeInventory
            // 
            this.chkBridgeInventory.AutoSize = true;
            this.chkBridgeInventory.Location = new System.Drawing.Point(89, 139);
            this.chkBridgeInventory.Margin = new System.Windows.Forms.Padding(4);
            this.chkBridgeInventory.Name = "chkBridgeInventory";
            this.chkBridgeInventory.Size = new System.Drawing.Size(66, 21);
            this.chkBridgeInventory.TabIndex = 59;
            this.chkBridgeInventory.Text = "Br Inv";
            this.chkBridgeInventory.UseVisualStyleBackColor = true;
            // 
            // chkDeteriorateOlayDefects
            // 
            this.chkDeteriorateOlayDefects.AutoSize = true;
            this.chkDeteriorateOlayDefects.Checked = true;
            this.chkDeteriorateOlayDefects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeteriorateOlayDefects.Location = new System.Drawing.Point(7, 27);
            this.chkDeteriorateOlayDefects.Margin = new System.Windows.Forms.Padding(4);
            this.chkDeteriorateOlayDefects.Name = "chkDeteriorateOlayDefects";
            this.chkDeteriorateOlayDefects.Size = new System.Drawing.Size(206, 21);
            this.chkDeteriorateOlayDefects.TabIndex = 55;
            this.chkDeteriorateOlayDefects.Text = "Deteriorate Overlay Defects";
            this.chkDeteriorateOlayDefects.UseVisualStyleBackColor = true;
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.Checked = true;
            this.chkDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDebug.Location = new System.Drawing.Point(7, 165);
            this.chkDebug.Margin = new System.Windows.Forms.Padding(4);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(178, 21);
            this.chkDebug.TabIndex = 54;
            this.chkDebug.Text = "Br Cond && Qty of Elems";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // chkInterpolateNbi
            // 
            this.chkInterpolateNbi.AutoSize = true;
            this.chkInterpolateNbi.Checked = true;
            this.chkInterpolateNbi.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkInterpolateNbi.Location = new System.Drawing.Point(7, 58);
            this.chkInterpolateNbi.Margin = new System.Windows.Forms.Padding(4);
            this.chkInterpolateNbi.Name = "chkInterpolateNbi";
            this.chkInterpolateNbi.Size = new System.Drawing.Size(175, 21);
            this.chkInterpolateNbi.TabIndex = 57;
            this.chkInterpolateNbi.Text = "Interpolate NBI Ratings";
            this.chkInterpolateNbi.UseVisualStyleBackColor = true;
            // 
            // chkPiFactors
            // 
            this.chkPiFactors.AutoSize = true;
            this.chkPiFactors.Location = new System.Drawing.Point(7, 139);
            this.chkPiFactors.Margin = new System.Windows.Forms.Padding(4);
            this.chkPiFactors.Name = "chkPiFactors";
            this.chkPiFactors.Size = new System.Drawing.Size(74, 21);
            this.chkPiFactors.TabIndex = 55;
            this.chkPiFactors.Text = "Priority";
            this.chkPiFactors.UseVisualStyleBackColor = true;
            // 
            // chkCountTpo
            // 
            this.chkCountTpo.AutoSize = true;
            this.chkCountTpo.Location = new System.Drawing.Point(7, 87);
            this.chkCountTpo.Margin = new System.Windows.Forms.Padding(4);
            this.chkCountTpo.Name = "chkCountTpo";
            this.chkCountTpo.Size = new System.Drawing.Size(100, 21);
            this.chkCountTpo.TabIndex = 58;
            this.chkCountTpo.Text = "Count TPO";
            this.chkCountTpo.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 115);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(117, 17);
            this.label14.TabIndex = 56;
            this.label14.Text = "Additional Output";
            // 
            // gbxBudget
            // 
            this.gbxBudget.Controls.Add(this.btnSetAnnualBudget);
            this.gbxBudget.Controls.Add(this.tbxAnnualBudget);
            this.gbxBudget.Controls.Add(this.label29);
            this.gbxBudget.Controls.Add(this.tbxLeastCost);
            this.gbxBudget.Controls.Add(this.label28);
            this.gbxBudget.Controls.Add(this.chkApplyBudget);
            this.gbxBudget.Controls.Add(this.chkBigBucket);
            this.gbxBudget.Controls.Add(this.dgvBudget);
            this.gbxBudget.Location = new System.Drawing.Point(32, 334);
            this.gbxBudget.Name = "gbxBudget";
            this.gbxBudget.Size = new System.Drawing.Size(242, 291);
            this.gbxBudget.TabIndex = 75;
            this.gbxBudget.TabStop = false;
            this.gbxBudget.Text = "Budget";
            // 
            // btnSetAnnualBudget
            // 
            this.btnSetAnnualBudget.Location = new System.Drawing.Point(175, 40);
            this.btnSetAnnualBudget.Margin = new System.Windows.Forms.Padding(4);
            this.btnSetAnnualBudget.Name = "btnSetAnnualBudget";
            this.btnSetAnnualBudget.Size = new System.Drawing.Size(46, 32);
            this.btnSetAnnualBudget.TabIndex = 86;
            this.btnSetAnnualBudget.Text = "Set";
            this.btnSetAnnualBudget.UseVisualStyleBackColor = true;
            this.btnSetAnnualBudget.Click += new System.EventHandler(this.btnSetAnnualBudget_Click);
            // 
            // tbxAnnualBudget
            // 
            this.tbxAnnualBudget.Location = new System.Drawing.Point(90, 45);
            this.tbxAnnualBudget.Margin = new System.Windows.Forms.Padding(4);
            this.tbxAnnualBudget.MaxLength = 12;
            this.tbxAnnualBudget.Name = "tbxAnnualBudget";
            this.tbxAnnualBudget.Size = new System.Drawing.Size(77, 22);
            this.tbxAnnualBudget.TabIndex = 81;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(8, 48);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(80, 17);
            this.label29.TabIndex = 80;
            this.label29.Text = "Annual Amt";
            // 
            // tbxLeastCost
            // 
            this.tbxLeastCost.Location = new System.Drawing.Point(102, 260);
            this.tbxLeastCost.Margin = new System.Windows.Forms.Padding(4);
            this.tbxLeastCost.MaxLength = 10;
            this.tbxLeastCost.Name = "tbxLeastCost";
            this.tbxLeastCost.Size = new System.Drawing.Size(59, 22);
            this.tbxLeastCost.TabIndex = 78;
            this.tbxLeastCost.Text = "20000";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(22, 262);
            this.label28.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(75, 17);
            this.label28.TabIndex = 77;
            this.label28.Text = "Least Cost";
            // 
            // chkApplyBudget
            // 
            this.chkApplyBudget.AutoSize = true;
            this.chkApplyBudget.Location = new System.Drawing.Point(57, 15);
            this.chkApplyBudget.Margin = new System.Windows.Forms.Padding(4);
            this.chkApplyBudget.Name = "chkApplyBudget";
            this.chkApplyBudget.Size = new System.Drawing.Size(65, 21);
            this.chkApplyBudget.TabIndex = 76;
            this.chkApplyBudget.Text = "Apply";
            this.chkApplyBudget.UseVisualStyleBackColor = true;
            this.chkApplyBudget.CheckedChanged += new System.EventHandler(this.chkApplyBudget_CheckedChanged);
            // 
            // chkBigBucket
            // 
            this.chkBigBucket.AutoSize = true;
            this.chkBigBucket.Location = new System.Drawing.Point(131, 15);
            this.chkBigBucket.Margin = new System.Windows.Forms.Padding(4);
            this.chkBigBucket.Name = "chkBigBucket";
            this.chkBigBucket.Size = new System.Drawing.Size(94, 21);
            this.chkBigBucket.TabIndex = 75;
            this.chkBigBucket.Text = "Multi-Year";
            this.chkBigBucket.UseVisualStyleBackColor = true;
            // 
            // dgvBudget
            // 
            this.dgvBudget.AllowUserToAddRows = false;
            this.dgvBudget.AllowUserToDeleteRows = false;
            this.dgvBudget.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBudget.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.budgetYear,
            this.budgetAmount});
            this.dgvBudget.Enabled = false;
            this.dgvBudget.Location = new System.Drawing.Point(25, 83);
            this.dgvBudget.Name = "dgvBudget";
            this.dgvBudget.RowHeadersWidth = 51;
            this.dgvBudget.RowTemplate.Height = 24;
            this.dgvBudget.Size = new System.Drawing.Size(196, 170);
            this.dgvBudget.TabIndex = 74;
            // 
            // budgetYear
            // 
            this.budgetYear.HeaderText = "Year";
            this.budgetYear.MinimumWidth = 6;
            this.budgetYear.Name = "budgetYear";
            this.budgetYear.ReadOnly = true;
            this.budgetYear.Width = 50;
            // 
            // budgetAmount
            // 
            this.budgetAmount.HeaderText = "Budget";
            this.budgetAmount.MinimumWidth = 6;
            this.budgetAmount.Name = "budgetAmount";
            this.budgetAmount.Width = 125;
            // 
            // gbxEnterOtherCriteria
            // 
            this.gbxEnterOtherCriteria.Controls.Add(this.groupBox3);
            this.gbxEnterOtherCriteria.Controls.Add(this.gbxYearType);
            this.gbxEnterOtherCriteria.Controls.Add(this.tbxStartYear);
            this.gbxEnterOtherCriteria.Controls.Add(this.label3);
            this.gbxEnterOtherCriteria.Controls.Add(this.label4);
            this.gbxEnterOtherCriteria.Controls.Add(this.tbxEndYear);
            this.gbxEnterOtherCriteria.Location = new System.Drawing.Point(465, 81);
            this.gbxEnterOtherCriteria.Margin = new System.Windows.Forms.Padding(4);
            this.gbxEnterOtherCriteria.Name = "gbxEnterOtherCriteria";
            this.gbxEnterOtherCriteria.Padding = new System.Windows.Forms.Padding(4);
            this.gbxEnterOtherCriteria.Size = new System.Drawing.Size(235, 246);
            this.gbxEnterOtherCriteria.TabIndex = 59;
            this.gbxEnterOtherCriteria.TabStop = false;
            this.gbxEnterOtherCriteria.Text = "Analysis Window";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButtonBrmDeterioration);
            this.groupBox3.Controls.Add(this.radioButtonOldDeterioration);
            this.groupBox3.Location = new System.Drawing.Point(16, 148);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(212, 88);
            this.groupBox3.TabIndex = 35;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Deterioration Method";
            // 
            // radioButtonBrmDeterioration
            // 
            this.radioButtonBrmDeterioration.AutoSize = true;
            this.radioButtonBrmDeterioration.Checked = true;
            this.radioButtonBrmDeterioration.Location = new System.Drawing.Point(7, 18);
            this.radioButtonBrmDeterioration.Name = "radioButtonBrmDeterioration";
            this.radioButtonBrmDeterioration.Size = new System.Drawing.Size(54, 21);
            this.radioButtonBrmDeterioration.TabIndex = 2;
            this.radioButtonBrmDeterioration.TabStop = true;
            this.radioButtonBrmDeterioration.Text = "BrM";
            this.radioButtonBrmDeterioration.UseVisualStyleBackColor = true;
            // 
            // radioButtonOldDeterioration
            // 
            this.radioButtonOldDeterioration.AutoSize = true;
            this.radioButtonOldDeterioration.Location = new System.Drawing.Point(7, 41);
            this.radioButtonOldDeterioration.Name = "radioButtonOldDeterioration";
            this.radioButtonOldDeterioration.Size = new System.Drawing.Size(51, 21);
            this.radioButtonOldDeterioration.TabIndex = 0;
            this.radioButtonOldDeterioration.Text = "Old";
            this.radioButtonOldDeterioration.UseVisualStyleBackColor = true;
            // 
            // gbxYearType
            // 
            this.gbxYearType.Controls.Add(this.rbtCalendarYear);
            this.gbxYearType.Controls.Add(this.rbtFederalFiscalYear);
            this.gbxYearType.Controls.Add(this.rbtStateFiscalYear);
            this.gbxYearType.Location = new System.Drawing.Point(14, 53);
            this.gbxYearType.Name = "gbxYearType";
            this.gbxYearType.Size = new System.Drawing.Size(212, 88);
            this.gbxYearType.TabIndex = 34;
            this.gbxYearType.TabStop = false;
            this.gbxYearType.Text = "Calendar Type";
            // 
            // rbtCalendarYear
            // 
            this.rbtCalendarYear.AutoSize = true;
            this.rbtCalendarYear.Location = new System.Drawing.Point(7, 18);
            this.rbtCalendarYear.Name = "rbtCalendarYear";
            this.rbtCalendarYear.Size = new System.Drawing.Size(120, 21);
            this.rbtCalendarYear.TabIndex = 2;
            this.rbtCalendarYear.Text = "Calendar Year";
            this.rbtCalendarYear.UseVisualStyleBackColor = true;
            // 
            // rbtFederalFiscalYear
            // 
            this.rbtFederalFiscalYear.AutoSize = true;
            this.rbtFederalFiscalYear.Location = new System.Drawing.Point(7, 65);
            this.rbtFederalFiscalYear.Name = "rbtFederalFiscalYear";
            this.rbtFederalFiscalYear.Size = new System.Drawing.Size(151, 21);
            this.rbtFederalFiscalYear.TabIndex = 1;
            this.rbtFederalFiscalYear.Text = "Federal Fiscal Year";
            this.rbtFederalFiscalYear.UseVisualStyleBackColor = true;
            // 
            // rbtStateFiscalYear
            // 
            this.rbtStateFiscalYear.AutoSize = true;
            this.rbtStateFiscalYear.Checked = true;
            this.rbtStateFiscalYear.Location = new System.Drawing.Point(7, 41);
            this.rbtStateFiscalYear.Name = "rbtStateFiscalYear";
            this.rbtStateFiscalYear.Size = new System.Drawing.Size(136, 21);
            this.rbtStateFiscalYear.TabIndex = 0;
            this.rbtStateFiscalYear.TabStop = true;
            this.rbtStateFiscalYear.Text = "State Fiscal Year";
            this.rbtStateFiscalYear.UseVisualStyleBackColor = true;
            // 
            // tbxStartYear
            // 
            this.tbxStartYear.Location = new System.Drawing.Point(64, 23);
            this.tbxStartYear.Margin = new System.Windows.Forms.Padding(4);
            this.tbxStartYear.MaxLength = 4;
            this.tbxStartYear.Name = "tbxStartYear";
            this.tbxStartYear.Size = new System.Drawing.Size(46, 22);
            this.tbxStartYear.TabIndex = 32;
            this.tbxStartYear.Leave += new System.EventHandler(this.tbxStartYear_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 30;
            this.label3.Text = "Start Yr";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(126, 28);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 17);
            this.label4.TabIndex = 31;
            this.label4.Text = "End Yr";
            // 
            // tbxEndYear
            // 
            this.tbxEndYear.Location = new System.Drawing.Point(180, 23);
            this.tbxEndYear.Margin = new System.Windows.Forms.Padding(4);
            this.tbxEndYear.MaxLength = 4;
            this.tbxEndYear.Name = "tbxEndYear";
            this.tbxEndYear.Size = new System.Drawing.Size(46, 22);
            this.tbxEndYear.TabIndex = 33;
            this.tbxEndYear.Leave += new System.EventHandler(this.tbxEndYear_Leave);
            // 
            // gbxPickWorkTypes
            // 
            this.gbxPickWorkTypes.Controls.Add(this.btnUnselectAllWorkActions);
            this.gbxPickWorkTypes.Controls.Add(this.btnSelectAllWorkActions);
            this.gbxPickWorkTypes.Controls.Add(this.checkedListBoxWorkActions);
            this.gbxPickWorkTypes.Location = new System.Drawing.Point(934, 81);
            this.gbxPickWorkTypes.Margin = new System.Windows.Forms.Padding(4);
            this.gbxPickWorkTypes.Name = "gbxPickWorkTypes";
            this.gbxPickWorkTypes.Padding = new System.Windows.Forms.Padding(4);
            this.gbxPickWorkTypes.Size = new System.Drawing.Size(471, 246);
            this.gbxPickWorkTypes.TabIndex = 58;
            this.gbxPickWorkTypes.TabStop = false;
            this.gbxPickWorkTypes.Text = "Eligible Primary Work Actions";
            // 
            // btnUnselectAllWorkActions
            // 
            this.btnUnselectAllWorkActions.Image = global::WiSam.Desktop.Properties.Resources.cross;
            this.btnUnselectAllWorkActions.Location = new System.Drawing.Point(254, 198);
            this.btnUnselectAllWorkActions.Margin = new System.Windows.Forms.Padding(4);
            this.btnUnselectAllWorkActions.Name = "btnUnselectAllWorkActions";
            this.btnUnselectAllWorkActions.Size = new System.Drawing.Size(114, 30);
            this.btnUnselectAllWorkActions.TabIndex = 73;
            this.btnUnselectAllWorkActions.Text = "Unselect All";
            this.btnUnselectAllWorkActions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUnselectAllWorkActions.UseVisualStyleBackColor = true;
            this.btnUnselectAllWorkActions.Click += new System.EventHandler(this.btnUnselectAllWorkActions_Click);
            // 
            // btnSelectAllWorkActions
            // 
            this.btnSelectAllWorkActions.Image = global::WiSam.Desktop.Properties.Resources.accept;
            this.btnSelectAllWorkActions.Location = new System.Drawing.Point(143, 198);
            this.btnSelectAllWorkActions.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectAllWorkActions.Name = "btnSelectAllWorkActions";
            this.btnSelectAllWorkActions.Size = new System.Drawing.Size(92, 30);
            this.btnSelectAllWorkActions.TabIndex = 72;
            this.btnSelectAllWorkActions.Text = "Select All";
            this.btnSelectAllWorkActions.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectAllWorkActions.UseVisualStyleBackColor = true;
            this.btnSelectAllWorkActions.Click += new System.EventHandler(this.btnSelectAllWorkActions_Click);
            // 
            // checkedListBoxWorkActions
            // 
            this.checkedListBoxWorkActions.FormattingEnabled = true;
            this.checkedListBoxWorkActions.Location = new System.Drawing.Point(18, 26);
            this.checkedListBoxWorkActions.Margin = new System.Windows.Forms.Padding(4);
            this.checkedListBoxWorkActions.Name = "checkedListBoxWorkActions";
            this.checkedListBoxWorkActions.Size = new System.Drawing.Size(436, 157);
            this.checkedListBoxWorkActions.TabIndex = 0;
            // 
            // gbxPickStructures
            // 
            this.gbxPickStructures.Controls.Add(this.chkApplyRegionSelections);
            this.gbxPickStructures.Controls.Add(this.groupBox2);
            this.gbxPickStructures.Controls.Add(this.checkedListBoxFundings);
            this.gbxPickStructures.Controls.Add(this.rbtStructuresByFunding);
            this.gbxPickStructures.Controls.Add(this.groupBox1);
            this.gbxPickStructures.Controls.Add(this.rbtStructuresById);
            this.gbxPickStructures.Controls.Add(this.rbtStructuresByRegion);
            this.gbxPickStructures.Controls.Add(this.tbxStructureIds);
            this.gbxPickStructures.Location = new System.Drawing.Point(32, 81);
            this.gbxPickStructures.Margin = new System.Windows.Forms.Padding(4);
            this.gbxPickStructures.Name = "gbxPickStructures";
            this.gbxPickStructures.Padding = new System.Windows.Forms.Padding(4);
            this.gbxPickStructures.Size = new System.Drawing.Size(439, 246);
            this.gbxPickStructures.TabIndex = 47;
            this.gbxPickStructures.TabStop = false;
            this.gbxPickStructures.Text = "Structures Selection";
            // 
            // chkApplyRegionSelections
            // 
            this.chkApplyRegionSelections.AutoSize = true;
            this.chkApplyRegionSelections.Location = new System.Drawing.Point(315, 20);
            this.chkApplyRegionSelections.Name = "chkApplyRegionSelections";
            this.chkApplyRegionSelections.Size = new System.Drawing.Size(121, 21);
            this.chkApplyRegionSelections.TabIndex = 60;
            this.chkApplyRegionSelections.Text = "Apply Regions";
            this.chkApplyRegionSelections.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label30);
            this.groupBox2.Controls.Add(this.chkIncludeCStructures);
            this.groupBox2.Controls.Add(this.tbxMaxNumToAnalyze);
            this.groupBox2.Location = new System.Drawing.Point(109, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(323, 44);
            this.groupBox2.TabIndex = 59;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Applies to Regions && Funding";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Location = new System.Drawing.Point(158, 22);
            this.label30.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(45, 17);
            this.label30.TabIndex = 59;
            this.label30.Text = "Cap #";
            // 
            // chkIncludeCStructures
            // 
            this.chkIncludeCStructures.AutoSize = true;
            this.chkIncludeCStructures.Location = new System.Drawing.Point(31, 21);
            this.chkIncludeCStructures.Margin = new System.Windows.Forms.Padding(4);
            this.chkIncludeCStructures.Name = "chkIncludeCStructures";
            this.chkIncludeCStructures.Size = new System.Drawing.Size(108, 21);
            this.chkIncludeCStructures.TabIndex = 49;
            this.chkIncludeCStructures.Text = "C Structures";
            this.chkIncludeCStructures.UseVisualStyleBackColor = true;
            // 
            // tbxMaxNumToAnalyze
            // 
            this.tbxMaxNumToAnalyze.Location = new System.Drawing.Point(206, 18);
            this.tbxMaxNumToAnalyze.Name = "tbxMaxNumToAnalyze";
            this.tbxMaxNumToAnalyze.Size = new System.Drawing.Size(45, 22);
            this.tbxMaxNumToAnalyze.TabIndex = 51;
            // 
            // checkedListBoxFundings
            // 
            this.checkedListBoxFundings.FormattingEnabled = true;
            this.checkedListBoxFundings.Location = new System.Drawing.Point(226, 47);
            this.checkedListBoxFundings.Name = "checkedListBoxFundings";
            this.checkedListBoxFundings.Size = new System.Drawing.Size(206, 72);
            this.checkedListBoxFundings.TabIndex = 58;
            // 
            // rbtStructuresByFunding
            // 
            this.rbtStructuresByFunding.AutoSize = true;
            this.rbtStructuresByFunding.Location = new System.Drawing.Point(236, 20);
            this.rbtStructuresByFunding.Margin = new System.Windows.Forms.Padding(4);
            this.rbtStructuresByFunding.Name = "rbtStructuresByFunding";
            this.rbtStructuresByFunding.Size = new System.Drawing.Size(80, 21);
            this.rbtStructuresByFunding.TabIndex = 51;
            this.rbtStructuresByFunding.Text = "Funding";
            this.rbtStructuresByFunding.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkNorthwest);
            this.groupBox1.Controls.Add(this.chkNorthcentral);
            this.groupBox1.Controls.Add(this.chkLocalBridges);
            this.groupBox1.Controls.Add(this.chkNortheast);
            this.groupBox1.Controls.Add(this.chkStateBridges);
            this.groupBox1.Controls.Add(this.chkSoutheast);
            this.groupBox1.Controls.Add(this.chkSouthwest);
            this.groupBox1.Location = new System.Drawing.Point(7, 167);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(425, 69);
            this.groupBox1.TabIndex = 50;
            this.groupBox1.TabStop = false;
            // 
            // chkNorthwest
            // 
            this.chkNorthwest.AutoSize = true;
            this.chkNorthwest.Location = new System.Drawing.Point(353, 12);
            this.chkNorthwest.Name = "chkNorthwest";
            this.chkNorthwest.Size = new System.Drawing.Size(66, 21);
            this.chkNorthwest.TabIndex = 4;
            this.chkNorthwest.Text = "NW-5";
            this.chkNorthwest.UseVisualStyleBackColor = true;
            // 
            // chkNorthcentral
            // 
            this.chkNorthcentral.AutoSize = true;
            this.chkNorthcentral.Location = new System.Drawing.Point(267, 12);
            this.chkNorthcentral.Name = "chkNorthcentral";
            this.chkNorthcentral.Size = new System.Drawing.Size(62, 21);
            this.chkNorthcentral.TabIndex = 3;
            this.chkNorthcentral.Text = "NC-4";
            this.chkNorthcentral.UseVisualStyleBackColor = true;
            // 
            // chkLocalBridges
            // 
            this.chkLocalBridges.AutoSize = true;
            this.chkLocalBridges.Location = new System.Drawing.Point(163, 41);
            this.chkLocalBridges.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalBridges.Name = "chkLocalBridges";
            this.chkLocalBridges.Size = new System.Drawing.Size(116, 21);
            this.chkLocalBridges.TabIndex = 48;
            this.chkLocalBridges.Text = "Local Bridges";
            this.chkLocalBridges.UseVisualStyleBackColor = true;
            // 
            // chkNortheast
            // 
            this.chkNortheast.AutoSize = true;
            this.chkNortheast.Location = new System.Drawing.Point(181, 12);
            this.chkNortheast.Name = "chkNortheast";
            this.chkNortheast.Size = new System.Drawing.Size(62, 21);
            this.chkNortheast.TabIndex = 2;
            this.chkNortheast.Text = "NE-3";
            this.chkNortheast.UseVisualStyleBackColor = true;
            // 
            // chkStateBridges
            // 
            this.chkStateBridges.AutoSize = true;
            this.chkStateBridges.Checked = true;
            this.chkStateBridges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStateBridges.Location = new System.Drawing.Point(29, 41);
            this.chkStateBridges.Margin = new System.Windows.Forms.Padding(4);
            this.chkStateBridges.Name = "chkStateBridges";
            this.chkStateBridges.Size = new System.Drawing.Size(115, 21);
            this.chkStateBridges.TabIndex = 47;
            this.chkStateBridges.Text = "State Bridges";
            this.chkStateBridges.UseVisualStyleBackColor = true;
            // 
            // chkSoutheast
            // 
            this.chkSoutheast.AutoSize = true;
            this.chkSoutheast.Location = new System.Drawing.Point(96, 12);
            this.chkSoutheast.Name = "chkSoutheast";
            this.chkSoutheast.Size = new System.Drawing.Size(61, 21);
            this.chkSoutheast.TabIndex = 1;
            this.chkSoutheast.Text = "SE-2";
            this.chkSoutheast.UseVisualStyleBackColor = true;
            // 
            // chkSouthwest
            // 
            this.chkSouthwest.AutoSize = true;
            this.chkSouthwest.Location = new System.Drawing.Point(7, 12);
            this.chkSouthwest.Name = "chkSouthwest";
            this.chkSouthwest.Size = new System.Drawing.Size(65, 21);
            this.chkSouthwest.TabIndex = 0;
            this.chkSouthwest.Text = "SW-1";
            this.chkSouthwest.UseVisualStyleBackColor = true;
            // 
            // rbtStructuresById
            // 
            this.rbtStructuresById.AutoSize = true;
            this.rbtStructuresById.Checked = true;
            this.rbtStructuresById.Location = new System.Drawing.Point(25, 20);
            this.rbtStructuresById.Margin = new System.Windows.Forms.Padding(4);
            this.rbtStructuresById.Name = "rbtStructuresById";
            this.rbtStructuresById.Size = new System.Drawing.Size(49, 21);
            this.rbtStructuresById.TabIndex = 45;
            this.rbtStructuresById.TabStop = true;
            this.rbtStructuresById.Text = "IDs";
            this.rbtStructuresById.UseVisualStyleBackColor = true;
            // 
            // rbtStructuresByRegion
            // 
            this.rbtStructuresByRegion.AutoSize = true;
            this.rbtStructuresByRegion.Location = new System.Drawing.Point(25, 148);
            this.rbtStructuresByRegion.Margin = new System.Windows.Forms.Padding(4);
            this.rbtStructuresByRegion.Name = "rbtStructuresByRegion";
            this.rbtStructuresByRegion.Size = new System.Drawing.Size(81, 21);
            this.rbtStructuresByRegion.TabIndex = 46;
            this.rbtStructuresByRegion.Text = "Regions";
            this.rbtStructuresByRegion.UseVisualStyleBackColor = true;
            // 
            // tbxStructureIds
            // 
            this.tbxStructureIds.Location = new System.Drawing.Point(8, 47);
            this.tbxStructureIds.Margin = new System.Windows.Forms.Padding(4);
            this.tbxStructureIds.MaxLength = 0;
            this.tbxStructureIds.Multiline = true;
            this.tbxStructureIds.Name = "tbxStructureIds";
            this.tbxStructureIds.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxStructureIds.Size = new System.Drawing.Size(201, 72);
            this.tbxStructureIds.TabIndex = 26;
            this.tbxStructureIds.Text = "B110008,B110010";
            // 
            // tpgSettings
            // 
            this.tpgSettings.Location = new System.Drawing.Point(4, 25);
            this.tpgSettings.Name = "tpgSettings";
            this.tpgSettings.Size = new System.Drawing.Size(1449, 632);
            this.tpgSettings.TabIndex = 6;
            this.tpgSettings.Text = "Settings";
            this.tpgSettings.UseVisualStyleBackColor = true;
            // 
            // tpgAdmin
            // 
            this.tpgAdmin.Controls.Add(this.tctAdmin);
            this.tpgAdmin.Location = new System.Drawing.Point(4, 25);
            this.tpgAdmin.Margin = new System.Windows.Forms.Padding(4);
            this.tpgAdmin.Name = "tpgAdmin";
            this.tpgAdmin.Size = new System.Drawing.Size(1449, 632);
            this.tpgAdmin.TabIndex = 3;
            this.tpgAdmin.Text = "Admin";
            this.tpgAdmin.UseVisualStyleBackColor = true;
            // 
            // tctAdmin
            // 
            this.tctAdmin.Controls.Add(this.tpgWorkAction);
            this.tctAdmin.Controls.Add(this.tpgDeterioration);
            this.tctAdmin.Controls.Add(this.tpgFiips);
            this.tctAdmin.Location = new System.Drawing.Point(31, 18);
            this.tctAdmin.Margin = new System.Windows.Forms.Padding(4);
            this.tctAdmin.Name = "tctAdmin";
            this.tctAdmin.SelectedIndex = 0;
            this.tctAdmin.Size = new System.Drawing.Size(1367, 526);
            this.tctAdmin.TabIndex = 0;
            // 
            // tpgWorkAction
            // 
            this.tpgWorkAction.Controls.Add(this.btnUpdateOverlaysCombinedWorkActions);
            this.tpgWorkAction.Controls.Add(this.gbxUpdateWorkCriteria);
            this.tpgWorkAction.Location = new System.Drawing.Point(4, 25);
            this.tpgWorkAction.Margin = new System.Windows.Forms.Padding(4);
            this.tpgWorkAction.Name = "tpgWorkAction";
            this.tpgWorkAction.Padding = new System.Windows.Forms.Padding(4);
            this.tpgWorkAction.Size = new System.Drawing.Size(1359, 497);
            this.tpgWorkAction.TabIndex = 0;
            this.tpgWorkAction.Text = "Work Action Rules";
            this.tpgWorkAction.UseVisualStyleBackColor = true;
            // 
            // btnUpdateOverlaysCombinedWorkActions
            // 
            this.btnUpdateOverlaysCombinedWorkActions.Location = new System.Drawing.Point(1037, 7);
            this.btnUpdateOverlaysCombinedWorkActions.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateOverlaysCombinedWorkActions.Name = "btnUpdateOverlaysCombinedWorkActions";
            this.btnUpdateOverlaysCombinedWorkActions.Size = new System.Drawing.Size(203, 26);
            this.btnUpdateOverlaysCombinedWorkActions.TabIndex = 54;
            this.btnUpdateOverlaysCombinedWorkActions.Text = "Update Overlays Combined Work Actions";
            this.btnUpdateOverlaysCombinedWorkActions.UseVisualStyleBackColor = true;
            this.btnUpdateOverlaysCombinedWorkActions.Visible = false;
            this.btnUpdateOverlaysCombinedWorkActions.Click += new System.EventHandler(this.btnUpdateOverlaysCombinedWorkActions_Click);
            // 
            // gbxUpdateWorkCriteria
            // 
            this.gbxUpdateWorkCriteria.Controls.Add(this.tbxRuleWorkActionNotes);
            this.gbxUpdateWorkCriteria.Controls.Add(this.label21);
            this.gbxUpdateWorkCriteria.Controls.Add(this.btnDeleteWorkActionCriteria);
            this.gbxUpdateWorkCriteria.Controls.Add(this.label5);
            this.gbxUpdateWorkCriteria.Controls.Add(this.btnAddNewWorkRule);
            this.gbxUpdateWorkCriteria.Controls.Add(this.label1);
            this.gbxUpdateWorkCriteria.Controls.Add(this.tbxRuleSequence);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblRuleSequence);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblActive);
            this.gbxUpdateWorkCriteria.Controls.Add(this.cbxWorkAction);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblWorkAction);
            this.gbxUpdateWorkCriteria.Controls.Add(this.chkRuleActive);
            this.gbxUpdateWorkCriteria.Controls.Add(this.tbxRuleNotes);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lbxRuleCategory);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblRuleNotes);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblRuleCategory);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblRuleCriteria);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lblRuleId);
            this.gbxUpdateWorkCriteria.Controls.Add(this.tbxRuleCriteria);
            this.gbxUpdateWorkCriteria.Controls.Add(this.lbxRuleId);
            this.gbxUpdateWorkCriteria.Controls.Add(this.btnUpdateWorkActionCriteria);
            this.gbxUpdateWorkCriteria.Location = new System.Drawing.Point(27, 20);
            this.gbxUpdateWorkCriteria.Margin = new System.Windows.Forms.Padding(4);
            this.gbxUpdateWorkCriteria.Name = "gbxUpdateWorkCriteria";
            this.gbxUpdateWorkCriteria.Padding = new System.Windows.Forms.Padding(4);
            this.gbxUpdateWorkCriteria.Size = new System.Drawing.Size(551, 459);
            this.gbxUpdateWorkCriteria.TabIndex = 1;
            this.gbxUpdateWorkCriteria.TabStop = false;
            this.gbxUpdateWorkCriteria.Text = "Work Action Rule Info";
            // 
            // tbxRuleWorkActionNotes
            // 
            this.tbxRuleWorkActionNotes.Location = new System.Drawing.Point(268, 214);
            this.tbxRuleWorkActionNotes.Margin = new System.Windows.Forms.Padding(4);
            this.tbxRuleWorkActionNotes.Multiline = true;
            this.tbxRuleWorkActionNotes.Name = "tbxRuleWorkActionNotes";
            this.tbxRuleWorkActionNotes.Size = new System.Drawing.Size(240, 87);
            this.tbxRuleWorkActionNotes.TabIndex = 21;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(265, 193);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(125, 17);
            this.label21.TabIndex = 20;
            this.label21.Text = "Work Action Notes";
            // 
            // btnDeleteWorkActionCriteria
            // 
            this.btnDeleteWorkActionCriteria.Location = new System.Drawing.Point(229, 420);
            this.btnDeleteWorkActionCriteria.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeleteWorkActionCriteria.Name = "btnDeleteWorkActionCriteria";
            this.btnDeleteWorkActionCriteria.Size = new System.Drawing.Size(100, 28);
            this.btnDeleteWorkActionCriteria.TabIndex = 19;
            this.btnDeleteWorkActionCriteria.Text = "Delete";
            this.btnDeleteWorkActionCriteria.UseVisualStyleBackColor = true;
            this.btnDeleteWorkActionCriteria.Click += new System.EventHandler(this.btnDeleteWorkActionCriteria_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 46);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "(Rule ID)";
            // 
            // btnAddNewWorkRule
            // 
            this.btnAddNewWorkRule.Location = new System.Drawing.Point(337, 27);
            this.btnAddNewWorkRule.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddNewWorkRule.Name = "btnAddNewWorkRule";
            this.btnAddNewWorkRule.Size = new System.Drawing.Size(133, 28);
            this.btnAddNewWorkRule.TabIndex = 17;
            this.btnAddNewWorkRule.Text = "Add New Rule";
            this.btnAddNewWorkRule.UseVisualStyleBackColor = true;
            this.btnAddNewWorkRule.Click += new System.EventHandler(this.btnAddNewWorkRule_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(279, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 17);
            this.label1.TabIndex = 16;
            this.label1.Text = "OR";
            // 
            // tbxRuleSequence
            // 
            this.tbxRuleSequence.Location = new System.Drawing.Point(107, 246);
            this.tbxRuleSequence.Margin = new System.Windows.Forms.Padding(4);
            this.tbxRuleSequence.MaxLength = 3;
            this.tbxRuleSequence.Name = "tbxRuleSequence";
            this.tbxRuleSequence.Size = new System.Drawing.Size(49, 22);
            this.tbxRuleSequence.TabIndex = 15;
            // 
            // lblRuleSequence
            // 
            this.lblRuleSequence.AutoSize = true;
            this.lblRuleSequence.Location = new System.Drawing.Point(11, 246);
            this.lblRuleSequence.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuleSequence.Name = "lblRuleSequence";
            this.lblRuleSequence.Size = new System.Drawing.Size(72, 17);
            this.lblRuleSequence.TabIndex = 14;
            this.lblRuleSequence.Text = "Sequence";
            // 
            // lblActive
            // 
            this.lblActive.AutoSize = true;
            this.lblActive.Location = new System.Drawing.Point(11, 284);
            this.lblActive.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblActive.Name = "lblActive";
            this.lblActive.Size = new System.Drawing.Size(46, 17);
            this.lblActive.TabIndex = 13;
            this.lblActive.Text = "Active";
            // 
            // cbxWorkAction
            // 
            this.cbxWorkAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxWorkAction.FormattingEnabled = true;
            this.cbxWorkAction.Location = new System.Drawing.Point(107, 159);
            this.cbxWorkAction.Margin = new System.Windows.Forms.Padding(4);
            this.cbxWorkAction.Name = "cbxWorkAction";
            this.cbxWorkAction.Size = new System.Drawing.Size(401, 24);
            this.cbxWorkAction.TabIndex = 12;
            // 
            // lblWorkAction
            // 
            this.lblWorkAction.AutoSize = true;
            this.lblWorkAction.Location = new System.Drawing.Point(11, 159);
            this.lblWorkAction.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWorkAction.Name = "lblWorkAction";
            this.lblWorkAction.Size = new System.Drawing.Size(84, 17);
            this.lblWorkAction.TabIndex = 11;
            this.lblWorkAction.Text = "Work Action";
            // 
            // chkRuleActive
            // 
            this.chkRuleActive.AutoSize = true;
            this.chkRuleActive.Location = new System.Drawing.Point(107, 284);
            this.chkRuleActive.Margin = new System.Windows.Forms.Padding(4);
            this.chkRuleActive.Name = "chkRuleActive";
            this.chkRuleActive.Size = new System.Drawing.Size(18, 17);
            this.chkRuleActive.TabIndex = 8;
            this.chkRuleActive.UseVisualStyleBackColor = true;
            // 
            // tbxRuleNotes
            // 
            this.tbxRuleNotes.Location = new System.Drawing.Point(107, 315);
            this.tbxRuleNotes.Margin = new System.Windows.Forms.Padding(4);
            this.tbxRuleNotes.Multiline = true;
            this.tbxRuleNotes.Name = "tbxRuleNotes";
            this.tbxRuleNotes.Size = new System.Drawing.Size(401, 84);
            this.tbxRuleNotes.TabIndex = 7;
            // 
            // lbxRuleCategory
            // 
            this.lbxRuleCategory.FormattingEnabled = true;
            this.lbxRuleCategory.ItemHeight = 16;
            this.lbxRuleCategory.Location = new System.Drawing.Point(107, 193);
            this.lbxRuleCategory.Margin = new System.Windows.Forms.Padding(4);
            this.lbxRuleCategory.Name = "lbxRuleCategory";
            this.lbxRuleCategory.Size = new System.Drawing.Size(120, 36);
            this.lbxRuleCategory.TabIndex = 9;
            // 
            // lblRuleNotes
            // 
            this.lblRuleNotes.AutoSize = true;
            this.lblRuleNotes.Location = new System.Drawing.Point(11, 315);
            this.lblRuleNotes.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuleNotes.Name = "lblRuleNotes";
            this.lblRuleNotes.Size = new System.Drawing.Size(45, 17);
            this.lblRuleNotes.TabIndex = 6;
            this.lblRuleNotes.Text = "Notes";
            // 
            // lblRuleCategory
            // 
            this.lblRuleCategory.AutoSize = true;
            this.lblRuleCategory.Location = new System.Drawing.Point(11, 193);
            this.lblRuleCategory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuleCategory.Name = "lblRuleCategory";
            this.lblRuleCategory.Size = new System.Drawing.Size(65, 17);
            this.lblRuleCategory.TabIndex = 10;
            this.lblRuleCategory.Text = "Category";
            // 
            // lblRuleCriteria
            // 
            this.lblRuleCriteria.AutoSize = true;
            this.lblRuleCriteria.Location = new System.Drawing.Point(11, 74);
            this.lblRuleCriteria.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuleCriteria.Name = "lblRuleCriteria";
            this.lblRuleCriteria.Size = new System.Drawing.Size(37, 17);
            this.lblRuleCriteria.TabIndex = 5;
            this.lblRuleCriteria.Text = "Rule";
            // 
            // lblRuleId
            // 
            this.lblRuleId.AutoSize = true;
            this.lblRuleId.Location = new System.Drawing.Point(11, 30);
            this.lblRuleId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRuleId.Name = "lblRuleId";
            this.lblRuleId.Size = new System.Drawing.Size(126, 17);
            this.lblRuleId.TabIndex = 4;
            this.lblRuleId.Text = "Select existing rule";
            // 
            // tbxRuleCriteria
            // 
            this.tbxRuleCriteria.Location = new System.Drawing.Point(109, 74);
            this.tbxRuleCriteria.Margin = new System.Windows.Forms.Padding(4);
            this.tbxRuleCriteria.Multiline = true;
            this.tbxRuleCriteria.Name = "tbxRuleCriteria";
            this.tbxRuleCriteria.Size = new System.Drawing.Size(399, 75);
            this.tbxRuleCriteria.TabIndex = 3;
            // 
            // lbxRuleId
            // 
            this.lbxRuleId.FormattingEnabled = true;
            this.lbxRuleId.ItemHeight = 16;
            this.lbxRuleId.Location = new System.Drawing.Point(145, 27);
            this.lbxRuleId.Margin = new System.Windows.Forms.Padding(4);
            this.lbxRuleId.Name = "lbxRuleId";
            this.lbxRuleId.Size = new System.Drawing.Size(111, 36);
            this.lbxRuleId.TabIndex = 2;
            this.lbxRuleId.SelectedIndexChanged += new System.EventHandler(this.lbxRuleId_SelectedIndexChanged);
            // 
            // btnUpdateWorkActionCriteria
            // 
            this.btnUpdateWorkActionCriteria.Location = new System.Drawing.Point(109, 420);
            this.btnUpdateWorkActionCriteria.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateWorkActionCriteria.Name = "btnUpdateWorkActionCriteria";
            this.btnUpdateWorkActionCriteria.Size = new System.Drawing.Size(96, 28);
            this.btnUpdateWorkActionCriteria.TabIndex = 1;
            this.btnUpdateWorkActionCriteria.Text = "Save";
            this.btnUpdateWorkActionCriteria.UseVisualStyleBackColor = true;
            this.btnUpdateWorkActionCriteria.Click += new System.EventHandler(this.btnUpdateWorkActionCriteria_Click);
            // 
            // tpgDeterioration
            // 
            this.tpgDeterioration.Controls.Add(this.gbxUpdateDeteriorationCurves);
            this.tpgDeterioration.Controls.Add(this.btnBrowsePonModDeterExcelInputFile);
            this.tpgDeterioration.Controls.Add(this.lblPonModDeterExcelInputFilePath);
            this.tpgDeterioration.Controls.Add(this.tbxPonModDeterExcelInputFilePath);
            this.tpgDeterioration.Controls.Add(this.btnUpdatePonModDeter);
            this.tpgDeterioration.Controls.Add(this.btnCalculateNbiDeteriorationRates);
            this.tpgDeterioration.Location = new System.Drawing.Point(4, 25);
            this.tpgDeterioration.Margin = new System.Windows.Forms.Padding(4);
            this.tpgDeterioration.Name = "tpgDeterioration";
            this.tpgDeterioration.Padding = new System.Windows.Forms.Padding(4);
            this.tpgDeterioration.Size = new System.Drawing.Size(1359, 497);
            this.tpgDeterioration.TabIndex = 1;
            this.tpgDeterioration.Text = "Deterioration";
            this.tpgDeterioration.UseVisualStyleBackColor = true;
            // 
            // gbxUpdateDeteriorationCurves
            // 
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.label32);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.textBoxQualifiedExpression);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.checkBoxQualifiedDeterioration);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.comboBoxQualifiedDeterioration);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.label31);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.label10);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.label9);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.label8);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.tbxNbiComponentDeterioratedRatings);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.btnSaveNbiComponentDeterioration);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.btnRecalcNbiComponentDeterioration);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.lblNbiComponentDeteriorationFormula);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.lblNbiComponent);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.tbxNbiComponentDeteriorationFormula);
            this.gbxUpdateDeteriorationCurves.Controls.Add(this.lbxNbiComponent);
            this.gbxUpdateDeteriorationCurves.Location = new System.Drawing.Point(39, 26);
            this.gbxUpdateDeteriorationCurves.Margin = new System.Windows.Forms.Padding(4);
            this.gbxUpdateDeteriorationCurves.Name = "gbxUpdateDeteriorationCurves";
            this.gbxUpdateDeteriorationCurves.Padding = new System.Windows.Forms.Padding(4);
            this.gbxUpdateDeteriorationCurves.Size = new System.Drawing.Size(902, 459);
            this.gbxUpdateDeteriorationCurves.TabIndex = 61;
            this.gbxUpdateDeteriorationCurves.TabStop = false;
            this.gbxUpdateDeteriorationCurves.Text = "NBI Deterioration Rates Info";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Location = new System.Drawing.Point(372, 86);
            this.label32.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(137, 17);
            this.label32.TabIndex = 20;
            this.label32.Text = "Qualified Expression";
            this.label32.UseMnemonic = false;
            // 
            // textBoxQualifiedExpression
            // 
            this.textBoxQualifiedExpression.Location = new System.Drawing.Point(398, 108);
            this.textBoxQualifiedExpression.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxQualifiedExpression.Multiline = true;
            this.textBoxQualifiedExpression.Name = "textBoxQualifiedExpression";
            this.textBoxQualifiedExpression.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxQualifiedExpression.Size = new System.Drawing.Size(460, 119);
            this.textBoxQualifiedExpression.TabIndex = 19;
            // 
            // checkBoxQualifiedDeterioration
            // 
            this.checkBoxQualifiedDeterioration.AutoSize = true;
            this.checkBoxQualifiedDeterioration.Enabled = false;
            this.checkBoxQualifiedDeterioration.Location = new System.Drawing.Point(411, 61);
            this.checkBoxQualifiedDeterioration.Name = "checkBoxQualifiedDeterioration";
            this.checkBoxQualifiedDeterioration.Size = new System.Drawing.Size(18, 17);
            this.checkBoxQualifiedDeterioration.TabIndex = 18;
            this.checkBoxQualifiedDeterioration.UseVisualStyleBackColor = true;
            // 
            // comboBoxQualifiedDeterioration
            // 
            this.comboBoxQualifiedDeterioration.FormattingEnabled = true;
            this.comboBoxQualifiedDeterioration.Location = new System.Drawing.Point(442, 31);
            this.comboBoxQualifiedDeterioration.Name = "comboBoxQualifiedDeterioration";
            this.comboBoxQualifiedDeterioration.Size = new System.Drawing.Size(362, 24);
            this.comboBoxQualifiedDeterioration.TabIndex = 17;
            this.comboBoxQualifiedDeterioration.SelectedIndexChanged += new System.EventHandler(this.comboBoxQualifiedDeterioration_SelectedIndexChanged);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Location = new System.Drawing.Point(371, 31);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(64, 17);
            this.label31.TabIndex = 16;
            this.label31.Text = "Qualified";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(289, 229);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 17);
            this.label10.TabIndex = 15;
            this.label10.Text = "Rating";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(149, 229);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 17);
            this.label9.TabIndex = 14;
            this.label9.Text = "Year";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(19, 229);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 17);
            this.label8.TabIndex = 13;
            this.label8.Text = "Deterioration";
            // 
            // tbxNbiComponentDeterioratedRatings
            // 
            this.tbxNbiComponentDeterioratedRatings.Location = new System.Drawing.Point(41, 252);
            this.tbxNbiComponentDeterioratedRatings.Margin = new System.Windows.Forms.Padding(4);
            this.tbxNbiComponentDeterioratedRatings.Multiline = true;
            this.tbxNbiComponentDeterioratedRatings.Name = "tbxNbiComponentDeterioratedRatings";
            this.tbxNbiComponentDeterioratedRatings.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxNbiComponentDeterioratedRatings.Size = new System.Drawing.Size(453, 179);
            this.tbxNbiComponentDeterioratedRatings.TabIndex = 12;
            // 
            // btnSaveNbiComponentDeterioration
            // 
            this.btnSaveNbiComponentDeterioration.Location = new System.Drawing.Point(265, 174);
            this.btnSaveNbiComponentDeterioration.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveNbiComponentDeterioration.Name = "btnSaveNbiComponentDeterioration";
            this.btnSaveNbiComponentDeterioration.Size = new System.Drawing.Size(100, 28);
            this.btnSaveNbiComponentDeterioration.TabIndex = 11;
            this.btnSaveNbiComponentDeterioration.Text = "Save";
            this.btnSaveNbiComponentDeterioration.UseVisualStyleBackColor = true;
            this.btnSaveNbiComponentDeterioration.Click += new System.EventHandler(this.btnSaveNbiComponentDeterioration_Click);
            // 
            // btnRecalcNbiComponentDeterioration
            // 
            this.btnRecalcNbiComponentDeterioration.Location = new System.Drawing.Point(80, 174);
            this.btnRecalcNbiComponentDeterioration.Margin = new System.Windows.Forms.Padding(4);
            this.btnRecalcNbiComponentDeterioration.Name = "btnRecalcNbiComponentDeterioration";
            this.btnRecalcNbiComponentDeterioration.Size = new System.Drawing.Size(155, 28);
            this.btnRecalcNbiComponentDeterioration.TabIndex = 10;
            this.btnRecalcNbiComponentDeterioration.Text = "Recalc Deterioration";
            this.btnRecalcNbiComponentDeterioration.UseVisualStyleBackColor = true;
            this.btnRecalcNbiComponentDeterioration.Click += new System.EventHandler(this.btnRecalcNbiComponentDeterioration_Click);
            // 
            // lblNbiComponentDeteriorationFormula
            // 
            this.lblNbiComponentDeteriorationFormula.AutoSize = true;
            this.lblNbiComponentDeteriorationFormula.Location = new System.Drawing.Point(15, 86);
            this.lblNbiComponentDeteriorationFormula.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNbiComponentDeteriorationFormula.Name = "lblNbiComponentDeteriorationFormula";
            this.lblNbiComponentDeteriorationFormula.Size = new System.Drawing.Size(145, 17);
            this.lblNbiComponentDeteriorationFormula.TabIndex = 9;
            this.lblNbiComponentDeteriorationFormula.Text = "Deterioration Formula";
            this.lblNbiComponentDeteriorationFormula.UseMnemonic = false;
            // 
            // lblNbiComponent
            // 
            this.lblNbiComponent.AutoSize = true;
            this.lblNbiComponent.Location = new System.Drawing.Point(15, 31);
            this.lblNbiComponent.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNbiComponent.Name = "lblNbiComponent";
            this.lblNbiComponent.Size = new System.Drawing.Size(121, 17);
            this.lblNbiComponent.TabIndex = 8;
            this.lblNbiComponent.Text = "Select component";
            // 
            // tbxNbiComponentDeteriorationFormula
            // 
            this.tbxNbiComponentDeteriorationFormula.Location = new System.Drawing.Point(41, 108);
            this.tbxNbiComponentDeteriorationFormula.Margin = new System.Windows.Forms.Padding(4);
            this.tbxNbiComponentDeteriorationFormula.Multiline = true;
            this.tbxNbiComponentDeteriorationFormula.Name = "tbxNbiComponentDeteriorationFormula";
            this.tbxNbiComponentDeteriorationFormula.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxNbiComponentDeteriorationFormula.Size = new System.Drawing.Size(287, 51);
            this.tbxNbiComponentDeteriorationFormula.TabIndex = 7;
            // 
            // lbxNbiComponent
            // 
            this.lbxNbiComponent.FormattingEnabled = true;
            this.lbxNbiComponent.ItemHeight = 16;
            this.lbxNbiComponent.Location = new System.Drawing.Point(149, 17);
            this.lbxNbiComponent.Margin = new System.Windows.Forms.Padding(4);
            this.lbxNbiComponent.Name = "lbxNbiComponent";
            this.lbxNbiComponent.Size = new System.Drawing.Size(111, 68);
            this.lbxNbiComponent.TabIndex = 6;
            this.lbxNbiComponent.SelectedIndexChanged += new System.EventHandler(this.lbxNbiComponent_SelectedIndexChanged);
            // 
            // btnBrowsePonModDeterExcelInputFile
            // 
            this.btnBrowsePonModDeterExcelInputFile.Location = new System.Drawing.Point(976, 26);
            this.btnBrowsePonModDeterExcelInputFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowsePonModDeterExcelInputFile.Name = "btnBrowsePonModDeterExcelInputFile";
            this.btnBrowsePonModDeterExcelInputFile.Size = new System.Drawing.Size(85, 28);
            this.btnBrowsePonModDeterExcelInputFile.TabIndex = 60;
            this.btnBrowsePonModDeterExcelInputFile.Text = "Browse ...";
            this.btnBrowsePonModDeterExcelInputFile.UseVisualStyleBackColor = true;
            this.btnBrowsePonModDeterExcelInputFile.Visible = false;
            this.btnBrowsePonModDeterExcelInputFile.Click += new System.EventHandler(this.btnBrowsePonModDeterExcelInputFile_Click);
            // 
            // lblPonModDeterExcelInputFilePath
            // 
            this.lblPonModDeterExcelInputFilePath.AutoSize = true;
            this.lblPonModDeterExcelInputFilePath.Location = new System.Drawing.Point(825, 26);
            this.lblPonModDeterExcelInputFilePath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPonModDeterExcelInputFilePath.Name = "lblPonModDeterExcelInputFilePath";
            this.lblPonModDeterExcelInputFilePath.Size = new System.Drawing.Size(102, 17);
            this.lblPonModDeterExcelInputFilePath.TabIndex = 59;
            this.lblPonModDeterExcelInputFilePath.Text = "Excel Input File";
            this.lblPonModDeterExcelInputFilePath.Visible = false;
            // 
            // tbxPonModDeterExcelInputFilePath
            // 
            this.tbxPonModDeterExcelInputFilePath.Location = new System.Drawing.Point(829, 62);
            this.tbxPonModDeterExcelInputFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.tbxPonModDeterExcelInputFilePath.Name = "tbxPonModDeterExcelInputFilePath";
            this.tbxPonModDeterExcelInputFilePath.Size = new System.Drawing.Size(339, 22);
            this.tbxPonModDeterExcelInputFilePath.TabIndex = 58;
            this.tbxPonModDeterExcelInputFilePath.Text = "C:\\bos\\projects\\development\\Workspaces\\WiSam\\WiSam\\Documentation\\wisdot-elements-" +
    "deterioration.xlsx";
            this.tbxPonModDeterExcelInputFilePath.Visible = false;
            // 
            // btnUpdatePonModDeter
            // 
            this.btnUpdatePonModDeter.Location = new System.Drawing.Point(829, 94);
            this.btnUpdatePonModDeter.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdatePonModDeter.Name = "btnUpdatePonModDeter";
            this.btnUpdatePonModDeter.Size = new System.Drawing.Size(189, 28);
            this.btnUpdatePonModDeter.TabIndex = 57;
            this.btnUpdatePonModDeter.Text = "Update Pon_Mod_Deter";
            this.btnUpdatePonModDeter.UseVisualStyleBackColor = true;
            this.btnUpdatePonModDeter.Visible = false;
            this.btnUpdatePonModDeter.Click += new System.EventHandler(this.btnUpdatePonModDeter_Click);
            // 
            // btnCalculateNbiDeteriorationRates
            // 
            this.btnCalculateNbiDeteriorationRates.Location = new System.Drawing.Point(829, 146);
            this.btnCalculateNbiDeteriorationRates.Margin = new System.Windows.Forms.Padding(4);
            this.btnCalculateNbiDeteriorationRates.Name = "btnCalculateNbiDeteriorationRates";
            this.btnCalculateNbiDeteriorationRates.Size = new System.Drawing.Size(309, 28);
            this.btnCalculateNbiDeteriorationRates.TabIndex = 56;
            this.btnCalculateNbiDeteriorationRates.Text = "Update NBI Deterioration";
            this.btnCalculateNbiDeteriorationRates.UseVisualStyleBackColor = true;
            this.btnCalculateNbiDeteriorationRates.Visible = false;
            this.btnCalculateNbiDeteriorationRates.Click += new System.EventHandler(this.btnCalculateNbiDeteriorationRates_Click);
            // 
            // tpgFiips
            // 
            this.tpgFiips.Controls.Add(this.button3);
            this.tpgFiips.Controls.Add(this.button2);
            this.tpgFiips.Controls.Add(this.button1);
            this.tpgFiips.Controls.Add(this.btnUpdateHighClearanceRoutes);
            this.tpgFiips.Controls.Add(this.btnUpdateCorridorCodes);
            this.tpgFiips.Controls.Add(this.btnBrowseExcelInputFile);
            this.tpgFiips.Controls.Add(this.label7);
            this.tpgFiips.Controls.Add(this.tbxExcelInputFilePath);
            this.tpgFiips.Controls.Add(this.btnUpdatePmic);
            this.tpgFiips.Controls.Add(this.lbkOpenFiipsBridgeListFile);
            this.tpgFiips.Controls.Add(this.btnCreateFiipsBridgeList);
            this.tpgFiips.Location = new System.Drawing.Point(4, 25);
            this.tpgFiips.Margin = new System.Windows.Forms.Padding(4);
            this.tpgFiips.Name = "tpgFiips";
            this.tpgFiips.Size = new System.Drawing.Size(1359, 497);
            this.tpgFiips.TabIndex = 2;
            this.tpgFiips.Text = "FIIPS";
            this.tpgFiips.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(324, 159);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(247, 28);
            this.button3.TabIndex = 75;
            this.button3.Text = "Update FIIPS Roadway Projects";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(452, 343);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(276, 68);
            this.button2.TabIndex = 74;
            this.button2.Text = "Update Roadway Projects from FIIPS-PMIC";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(97, 343);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(276, 68);
            this.button1.TabIndex = 73;
            this.button1.Text = "Update Structure Projects from FIIPS-PMIC";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnUpdateHighClearanceRoutes
            // 
            this.btnUpdateHighClearanceRoutes.Location = new System.Drawing.Point(560, 106);
            this.btnUpdateHighClearanceRoutes.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateHighClearanceRoutes.Name = "btnUpdateHighClearanceRoutes";
            this.btnUpdateHighClearanceRoutes.Size = new System.Drawing.Size(189, 28);
            this.btnUpdateHighClearanceRoutes.TabIndex = 68;
            this.btnUpdateHighClearanceRoutes.Text = "Update High Clearance Routes";
            this.btnUpdateHighClearanceRoutes.UseVisualStyleBackColor = true;
            this.btnUpdateHighClearanceRoutes.Click += new System.EventHandler(this.btnUpdateHighClearanceRoutes_Click);
            // 
            // btnUpdateCorridorCodes
            // 
            this.btnUpdateCorridorCodes.Location = new System.Drawing.Point(324, 106);
            this.btnUpdateCorridorCodes.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdateCorridorCodes.Name = "btnUpdateCorridorCodes";
            this.btnUpdateCorridorCodes.Size = new System.Drawing.Size(189, 28);
            this.btnUpdateCorridorCodes.TabIndex = 67;
            this.btnUpdateCorridorCodes.Text = "Update Corridor Codes";
            this.btnUpdateCorridorCodes.UseVisualStyleBackColor = true;
            this.btnUpdateCorridorCodes.Click += new System.EventHandler(this.btnUpdateCorridorCodes_Click);
            // 
            // btnBrowseExcelInputFile
            // 
            this.btnBrowseExcelInputFile.Location = new System.Drawing.Point(827, 57);
            this.btnBrowseExcelInputFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseExcelInputFile.Name = "btnBrowseExcelInputFile";
            this.btnBrowseExcelInputFile.Size = new System.Drawing.Size(85, 28);
            this.btnBrowseExcelInputFile.TabIndex = 66;
            this.btnBrowseExcelInputFile.Text = "Browse ...";
            this.btnBrowseExcelInputFile.UseVisualStyleBackColor = true;
            this.btnBrowseExcelInputFile.Click += new System.EventHandler(this.btnBrowseExcelInputFile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(79, 39);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 17);
            this.label7.TabIndex = 65;
            this.label7.Text = "Excel Input File";
            // 
            // tbxExcelInputFilePath
            // 
            this.tbxExcelInputFilePath.Location = new System.Drawing.Point(56, 59);
            this.tbxExcelInputFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.tbxExcelInputFilePath.Name = "tbxExcelInputFilePath";
            this.tbxExcelInputFilePath.Size = new System.Drawing.Size(737, 22);
            this.tbxExcelInputFilePath.TabIndex = 64;
            // 
            // btnUpdatePmic
            // 
            this.btnUpdatePmic.Enabled = false;
            this.btnUpdatePmic.Location = new System.Drawing.Point(37, 106);
            this.btnUpdatePmic.Margin = new System.Windows.Forms.Padding(4);
            this.btnUpdatePmic.Name = "btnUpdatePmic";
            this.btnUpdatePmic.Size = new System.Drawing.Size(235, 28);
            this.btnUpdatePmic.TabIndex = 63;
            this.btnUpdatePmic.Text = "Update FIIPS Structure Projects";
            this.btnUpdatePmic.UseVisualStyleBackColor = true;
            this.btnUpdatePmic.Click += new System.EventHandler(this.btnUpdatePmic_Click);
            // 
            // lbkOpenFiipsBridgeListFile
            // 
            this.lbkOpenFiipsBridgeListFile.AutoSize = true;
            this.lbkOpenFiipsBridgeListFile.Enabled = false;
            this.lbkOpenFiipsBridgeListFile.Location = new System.Drawing.Point(419, 250);
            this.lbkOpenFiipsBridgeListFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbkOpenFiipsBridgeListFile.Name = "lbkOpenFiipsBridgeListFile";
            this.lbkOpenFiipsBridgeListFile.Size = new System.Drawing.Size(140, 17);
            this.lbkOpenFiipsBridgeListFile.TabIndex = 62;
            this.lbkOpenFiipsBridgeListFile.TabStop = true;
            this.lbkOpenFiipsBridgeListFile.Text = "Open Bridge List File";
            // 
            // btnCreateFiipsBridgeList
            // 
            this.btnCreateFiipsBridgeList.Location = new System.Drawing.Point(83, 244);
            this.btnCreateFiipsBridgeList.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateFiipsBridgeList.Name = "btnCreateFiipsBridgeList";
            this.btnCreateFiipsBridgeList.Size = new System.Drawing.Size(309, 28);
            this.btnCreateFiipsBridgeList.TabIndex = 61;
            this.btnCreateFiipsBridgeList.Text = "Create FIIPS Bridge List";
            this.btnCreateFiipsBridgeList.UseVisualStyleBackColor = true;
            this.btnCreateFiipsBridgeList.Click += new System.EventHandler(this.btnCreateFiipsBridgeList_Click);
            // 
            // tpgMiscReports
            // 
            this.tpgMiscReports.Controls.Add(this.buttonGetStructuresDataForGis);
            this.tpgMiscReports.Controls.Add(this.btnCreateLetDatesReport);
            this.tpgMiscReports.Controls.Add(this.btnCreateStructureProgramReport);
            this.tpgMiscReports.Controls.Add(this.tbxStartMonthBillableReport);
            this.tpgMiscReports.Controls.Add(this.label19);
            this.tpgMiscReports.Controls.Add(this.label20);
            this.tpgMiscReports.Controls.Add(this.tbxStartYearBillableReport);
            this.tpgMiscReports.Controls.Add(this.tbxEndMonthBillableReport);
            this.tpgMiscReports.Controls.Add(this.label17);
            this.tpgMiscReports.Controls.Add(this.label18);
            this.tpgMiscReports.Controls.Add(this.tbxEndYearBillableReport);
            this.tpgMiscReports.Controls.Add(this.btnCreateDesignBillableReport);
            this.tpgMiscReports.Controls.Add(this.btnCreateBidItemsReport);
            this.tpgMiscReports.Controls.Add(this.tbxStartLetYear);
            this.tpgMiscReports.Controls.Add(this.label15);
            this.tpgMiscReports.Controls.Add(this.label16);
            this.tpgMiscReports.Controls.Add(this.tbxEndLetYear);
            this.tpgMiscReports.Controls.Add(this.lbkOpenMiscReport);
            this.tpgMiscReports.Controls.Add(this.label11);
            this.tpgMiscReports.Controls.Add(this.tbxMiscReportsOutputFilePath);
            this.tpgMiscReports.Controls.Add(this.btnCreateElementDeteriorationReport);
            this.tpgMiscReports.Controls.Add(this.btnCreateRulesTable);
            this.tpgMiscReports.Location = new System.Drawing.Point(4, 25);
            this.tpgMiscReports.Margin = new System.Windows.Forms.Padding(4);
            this.tpgMiscReports.Name = "tpgMiscReports";
            this.tpgMiscReports.Size = new System.Drawing.Size(1449, 632);
            this.tpgMiscReports.TabIndex = 4;
            this.tpgMiscReports.Text = "Misc Reports";
            this.tpgMiscReports.UseVisualStyleBackColor = true;
            // 
            // buttonGetStructuresDataForGis
            // 
            this.buttonGetStructuresDataForGis.Location = new System.Drawing.Point(669, 75);
            this.buttonGetStructuresDataForGis.Margin = new System.Windows.Forms.Padding(4);
            this.buttonGetStructuresDataForGis.Name = "buttonGetStructuresDataForGis";
            this.buttonGetStructuresDataForGis.Size = new System.Drawing.Size(276, 41);
            this.buttonGetStructuresDataForGis.TabIndex = 82;
            this.buttonGetStructuresDataForGis.Text = "Structures Data for GIS";
            this.buttonGetStructuresDataForGis.UseVisualStyleBackColor = true;
            this.buttonGetStructuresDataForGis.Click += new System.EventHandler(this.buttonGetStructuresDataForGis_Click);
            // 
            // btnCreateLetDatesReport
            // 
            this.btnCreateLetDatesReport.Location = new System.Drawing.Point(656, 229);
            this.btnCreateLetDatesReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateLetDatesReport.Name = "btnCreateLetDatesReport";
            this.btnCreateLetDatesReport.Size = new System.Drawing.Size(276, 41);
            this.btnCreateLetDatesReport.TabIndex = 81;
            this.btnCreateLetDatesReport.Text = "Let Dates Report";
            this.btnCreateLetDatesReport.UseVisualStyleBackColor = true;
            this.btnCreateLetDatesReport.Click += new System.EventHandler(this.btnCreateLetDatesReport_Click);
            // 
            // btnCreateStructureProgramReport
            // 
            this.btnCreateStructureProgramReport.Location = new System.Drawing.Point(357, 229);
            this.btnCreateStructureProgramReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateStructureProgramReport.Name = "btnCreateStructureProgramReport";
            this.btnCreateStructureProgramReport.Size = new System.Drawing.Size(276, 41);
            this.btnCreateStructureProgramReport.TabIndex = 80;
            this.btnCreateStructureProgramReport.Text = "Structure Program Report";
            this.btnCreateStructureProgramReport.UseVisualStyleBackColor = true;
            this.btnCreateStructureProgramReport.Click += new System.EventHandler(this.btnCreateStructureProgramReport_Click);
            // 
            // tbxStartMonthBillableReport
            // 
            this.tbxStartMonthBillableReport.Location = new System.Drawing.Point(149, 307);
            this.tbxStartMonthBillableReport.Margin = new System.Windows.Forms.Padding(4);
            this.tbxStartMonthBillableReport.MaxLength = 2;
            this.tbxStartMonthBillableReport.Name = "tbxStartMonthBillableReport";
            this.tbxStartMonthBillableReport.Size = new System.Drawing.Size(35, 22);
            this.tbxStartMonthBillableReport.TabIndex = 78;
            this.tbxStartMonthBillableReport.Text = "1";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(45, 310);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(81, 17);
            this.label19.TabIndex = 76;
            this.label19.Text = "Start Month";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(235, 310);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(72, 17);
            this.label20.TabIndex = 77;
            this.label20.Text = "Start Year";
            // 
            // tbxStartYearBillableReport
            // 
            this.tbxStartYearBillableReport.Location = new System.Drawing.Point(333, 306);
            this.tbxStartYearBillableReport.Margin = new System.Windows.Forms.Padding(4);
            this.tbxStartYearBillableReport.MaxLength = 4;
            this.tbxStartYearBillableReport.Name = "tbxStartYearBillableReport";
            this.tbxStartYearBillableReport.Size = new System.Drawing.Size(53, 22);
            this.tbxStartYearBillableReport.TabIndex = 79;
            this.tbxStartYearBillableReport.Text = "2017";
            // 
            // tbxEndMonthBillableReport
            // 
            this.tbxEndMonthBillableReport.Location = new System.Drawing.Point(149, 339);
            this.tbxEndMonthBillableReport.Margin = new System.Windows.Forms.Padding(4);
            this.tbxEndMonthBillableReport.MaxLength = 2;
            this.tbxEndMonthBillableReport.Name = "tbxEndMonthBillableReport";
            this.tbxEndMonthBillableReport.Size = new System.Drawing.Size(35, 22);
            this.tbxEndMonthBillableReport.TabIndex = 74;
            this.tbxEndMonthBillableReport.Text = "1";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(45, 342);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(76, 17);
            this.label17.TabIndex = 72;
            this.label17.Text = "End Month";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(235, 342);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(67, 17);
            this.label18.TabIndex = 73;
            this.label18.Text = "End Year";
            // 
            // tbxEndYearBillableReport
            // 
            this.tbxEndYearBillableReport.Location = new System.Drawing.Point(333, 338);
            this.tbxEndYearBillableReport.Margin = new System.Windows.Forms.Padding(4);
            this.tbxEndYearBillableReport.MaxLength = 4;
            this.tbxEndYearBillableReport.Name = "tbxEndYearBillableReport";
            this.tbxEndYearBillableReport.Size = new System.Drawing.Size(53, 22);
            this.tbxEndYearBillableReport.TabIndex = 75;
            this.tbxEndYearBillableReport.Text = "2017";
            // 
            // btnCreateDesignBillableReport
            // 
            this.btnCreateDesignBillableReport.Location = new System.Drawing.Point(48, 375);
            this.btnCreateDesignBillableReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateDesignBillableReport.Name = "btnCreateDesignBillableReport";
            this.btnCreateDesignBillableReport.Size = new System.Drawing.Size(276, 41);
            this.btnCreateDesignBillableReport.TabIndex = 71;
            this.btnCreateDesignBillableReport.Text = "Design Billable Report";
            this.btnCreateDesignBillableReport.UseVisualStyleBackColor = true;
            this.btnCreateDesignBillableReport.Click += new System.EventHandler(this.btnCreateDesignBillableReport_Click);
            // 
            // btnCreateBidItemsReport
            // 
            this.btnCreateBidItemsReport.Location = new System.Drawing.Point(47, 229);
            this.btnCreateBidItemsReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateBidItemsReport.Name = "btnCreateBidItemsReport";
            this.btnCreateBidItemsReport.Size = new System.Drawing.Size(276, 41);
            this.btnCreateBidItemsReport.TabIndex = 70;
            this.btnCreateBidItemsReport.Text = "Fabricated Bid Items Report";
            this.btnCreateBidItemsReport.UseVisualStyleBackColor = true;
            this.btnCreateBidItemsReport.Click += new System.EventHandler(this.btnCreateBidItemsReport_Click);
            // 
            // tbxStartLetYear
            // 
            this.tbxStartLetYear.Location = new System.Drawing.Point(148, 184);
            this.tbxStartLetYear.Margin = new System.Windows.Forms.Padding(4);
            this.tbxStartLetYear.MaxLength = 4;
            this.tbxStartLetYear.Name = "tbxStartLetYear";
            this.tbxStartLetYear.Size = new System.Drawing.Size(59, 22);
            this.tbxStartLetYear.TabIndex = 68;
            this.tbxStartLetYear.Text = "2018";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(44, 187);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(96, 17);
            this.label15.TabIndex = 66;
            this.label15.Text = "Start Let Year";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(235, 187);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(91, 17);
            this.label16.TabIndex = 67;
            this.label16.Text = "End Let Year";
            // 
            // tbxEndLetYear
            // 
            this.tbxEndLetYear.Location = new System.Drawing.Point(332, 183);
            this.tbxEndLetYear.Margin = new System.Windows.Forms.Padding(4);
            this.tbxEndLetYear.MaxLength = 4;
            this.tbxEndLetYear.Name = "tbxEndLetYear";
            this.tbxEndLetYear.Size = new System.Drawing.Size(53, 22);
            this.tbxEndLetYear.TabIndex = 69;
            this.tbxEndLetYear.Text = "2023";
            // 
            // lbkOpenMiscReport
            // 
            this.lbkOpenMiscReport.AutoSize = true;
            this.lbkOpenMiscReport.Enabled = false;
            this.lbkOpenMiscReport.Location = new System.Drawing.Point(680, 25);
            this.lbkOpenMiscReport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbkOpenMiscReport.Name = "lbkOpenMiscReport";
            this.lbkOpenMiscReport.Size = new System.Drawing.Size(116, 17);
            this.lbkOpenMiscReport.TabIndex = 65;
            this.lbkOpenMiscReport.TabStop = true;
            this.lbkOpenMiscReport.Text = "Open Output File";
            this.lbkOpenMiscReport.UseWaitCursor = true;
            this.lbkOpenMiscReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbkOpenMiscReport_LinkClicked);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(44, 25);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(114, 17);
            this.label11.TabIndex = 64;
            this.label11.Text = "Excel Output File";
            // 
            // tbxMiscReportsOutputFilePath
            // 
            this.tbxMiscReportsOutputFilePath.Enabled = false;
            this.tbxMiscReportsOutputFilePath.Location = new System.Drawing.Point(168, 21);
            this.tbxMiscReportsOutputFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.tbxMiscReportsOutputFilePath.Name = "tbxMiscReportsOutputFilePath";
            this.tbxMiscReportsOutputFilePath.Size = new System.Drawing.Size(464, 22);
            this.tbxMiscReportsOutputFilePath.TabIndex = 63;
            // 
            // btnCreateElementDeteriorationReport
            // 
            this.btnCreateElementDeteriorationReport.Location = new System.Drawing.Point(357, 75);
            this.btnCreateElementDeteriorationReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateElementDeteriorationReport.Name = "btnCreateElementDeteriorationReport";
            this.btnCreateElementDeteriorationReport.Size = new System.Drawing.Size(276, 41);
            this.btnCreateElementDeteriorationReport.TabIndex = 62;
            this.btnCreateElementDeteriorationReport.Text = "Element Deterioration";
            this.btnCreateElementDeteriorationReport.UseVisualStyleBackColor = true;
            this.btnCreateElementDeteriorationReport.Click += new System.EventHandler(this.btnCreateElementDeteriorationReport_Click);
            // 
            // btnCreateRulesTable
            // 
            this.btnCreateRulesTable.Location = new System.Drawing.Point(48, 75);
            this.btnCreateRulesTable.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateRulesTable.Name = "btnCreateRulesTable";
            this.btnCreateRulesTable.Size = new System.Drawing.Size(276, 41);
            this.btnCreateRulesTable.TabIndex = 61;
            this.btnCreateRulesTable.Text = "Work Action Rules";
            this.btnCreateRulesTable.UseVisualStyleBackColor = true;
            this.btnCreateRulesTable.Click += new System.EventHandler(this.btnGenerateRulesTable_Click);
            // 
            // tpgAssetManagement
            // 
            this.tpgAssetManagement.Controls.Add(this.tbxFirstYear);
            this.tpgAssetManagement.Controls.Add(this.label12);
            this.tpgAssetManagement.Controls.Add(this.label13);
            this.tpgAssetManagement.Controls.Add(this.tbxLastYear);
            this.tpgAssetManagement.Controls.Add(this.lbkAssetManagementOpenOutputFile);
            this.tpgAssetManagement.Controls.Add(this.lblAssetManagementExcelOutputFile);
            this.tpgAssetManagement.Controls.Add(this.tbxAssetManagementExcelOutputFilePath);
            this.tpgAssetManagement.Controls.Add(this.btnGenerateAssetManagementReport);
            this.tpgAssetManagement.Location = new System.Drawing.Point(4, 25);
            this.tpgAssetManagement.Margin = new System.Windows.Forms.Padding(4);
            this.tpgAssetManagement.Name = "tpgAssetManagement";
            this.tpgAssetManagement.Size = new System.Drawing.Size(1449, 632);
            this.tpgAssetManagement.TabIndex = 2;
            this.tpgAssetManagement.Text = "CAFR-FIIPS";
            this.tpgAssetManagement.UseVisualStyleBackColor = true;
            // 
            // tbxFirstYear
            // 
            this.tbxFirstYear.Location = new System.Drawing.Point(135, 84);
            this.tbxFirstYear.Margin = new System.Windows.Forms.Padding(4);
            this.tbxFirstYear.MaxLength = 4;
            this.tbxFirstYear.Name = "tbxFirstYear";
            this.tbxFirstYear.Size = new System.Drawing.Size(59, 22);
            this.tbxFirstYear.TabIndex = 70;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(55, 86);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 17);
            this.label12.TabIndex = 68;
            this.label12.Text = "Start Year";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(328, 86);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 17);
            this.label13.TabIndex = 69;
            this.label13.Text = "End Year";
            // 
            // tbxLastYear
            // 
            this.tbxLastYear.Location = new System.Drawing.Point(403, 82);
            this.tbxLastYear.Margin = new System.Windows.Forms.Padding(4);
            this.tbxLastYear.MaxLength = 4;
            this.tbxLastYear.Name = "tbxLastYear";
            this.tbxLastYear.Size = new System.Drawing.Size(53, 22);
            this.tbxLastYear.TabIndex = 71;
            // 
            // lbkAssetManagementOpenOutputFile
            // 
            this.lbkAssetManagementOpenOutputFile.AutoSize = true;
            this.lbkAssetManagementOpenOutputFile.Enabled = false;
            this.lbkAssetManagementOpenOutputFile.Location = new System.Drawing.Point(691, 46);
            this.lbkAssetManagementOpenOutputFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbkAssetManagementOpenOutputFile.Name = "lbkAssetManagementOpenOutputFile";
            this.lbkAssetManagementOpenOutputFile.Size = new System.Drawing.Size(116, 17);
            this.lbkAssetManagementOpenOutputFile.TabIndex = 67;
            this.lbkAssetManagementOpenOutputFile.TabStop = true;
            this.lbkAssetManagementOpenOutputFile.Text = "Open Output File";
            this.lbkAssetManagementOpenOutputFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbkAssetManagementOpenOutputFile_LinkClicked);
            // 
            // lblAssetManagementExcelOutputFile
            // 
            this.lblAssetManagementExcelOutputFile.AutoSize = true;
            this.lblAssetManagementExcelOutputFile.Location = new System.Drawing.Point(55, 46);
            this.lblAssetManagementExcelOutputFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAssetManagementExcelOutputFile.Name = "lblAssetManagementExcelOutputFile";
            this.lblAssetManagementExcelOutputFile.Size = new System.Drawing.Size(114, 17);
            this.lblAssetManagementExcelOutputFile.TabIndex = 66;
            this.lblAssetManagementExcelOutputFile.Text = "Excel Output File";
            // 
            // tbxAssetManagementExcelOutputFilePath
            // 
            this.tbxAssetManagementExcelOutputFilePath.Enabled = false;
            this.tbxAssetManagementExcelOutputFilePath.Location = new System.Drawing.Point(179, 42);
            this.tbxAssetManagementExcelOutputFilePath.Margin = new System.Windows.Forms.Padding(4);
            this.tbxAssetManagementExcelOutputFilePath.Name = "tbxAssetManagementExcelOutputFilePath";
            this.tbxAssetManagementExcelOutputFilePath.Size = new System.Drawing.Size(464, 22);
            this.tbxAssetManagementExcelOutputFilePath.TabIndex = 65;
            // 
            // btnGenerateAssetManagementReport
            // 
            this.btnGenerateAssetManagementReport.Location = new System.Drawing.Point(59, 138);
            this.btnGenerateAssetManagementReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateAssetManagementReport.Name = "btnGenerateAssetManagementReport";
            this.btnGenerateAssetManagementReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateAssetManagementReport.TabIndex = 64;
            this.btnGenerateAssetManagementReport.Text = "Bridge Projects";
            this.btnGenerateAssetManagementReport.UseVisualStyleBackColor = true;
            this.btnGenerateAssetManagementReport.Click += new System.EventHandler(this.btnGenerateAssetManagementReport_Click);
            // 
            // tpgTimesheet
            // 
            this.tpgTimesheet.Controls.Add(this.lbkOpenBillableReport);
            this.tpgTimesheet.Controls.Add(this.btnBrowseAccessDatabase);
            this.tpgTimesheet.Controls.Add(this.label26);
            this.tpgTimesheet.Controls.Add(this.tbxAccessDatabase);
            this.tpgTimesheet.Controls.Add(this.btnBrowseTimesheetDataFile);
            this.tpgTimesheet.Controls.Add(this.label25);
            this.tpgTimesheet.Controls.Add(this.tbxImportResults);
            this.tpgTimesheet.Controls.Add(this.tbxMonthWeekEndingDate);
            this.tpgTimesheet.Controls.Add(this.label23);
            this.tpgTimesheet.Controls.Add(this.label24);
            this.tpgTimesheet.Controls.Add(this.tbxYearWeekEndingDate);
            this.tpgTimesheet.Controls.Add(this.btnImportTimesheet);
            this.tpgTimesheet.Controls.Add(this.label22);
            this.tpgTimesheet.Controls.Add(this.tbxTimesheetDataFile);
            this.tpgTimesheet.Location = new System.Drawing.Point(4, 25);
            this.tpgTimesheet.Name = "tpgTimesheet";
            this.tpgTimesheet.Size = new System.Drawing.Size(1449, 632);
            this.tpgTimesheet.TabIndex = 5;
            this.tpgTimesheet.Text = "Timesheet";
            this.tpgTimesheet.UseVisualStyleBackColor = true;
            // 
            // lbkOpenBillableReport
            // 
            this.lbkOpenBillableReport.AutoSize = true;
            this.lbkOpenBillableReport.Enabled = false;
            this.lbkOpenBillableReport.Location = new System.Drawing.Point(128, 227);
            this.lbkOpenBillableReport.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbkOpenBillableReport.Name = "lbkOpenBillableReport";
            this.lbkOpenBillableReport.Size = new System.Drawing.Size(139, 17);
            this.lbkOpenBillableReport.TabIndex = 90;
            this.lbkOpenBillableReport.TabStop = true;
            this.lbkOpenBillableReport.Text = "Open Billable Report";
            this.lbkOpenBillableReport.UseWaitCursor = true;
            this.lbkOpenBillableReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbkOpenBillableReport_LinkClicked);
            // 
            // btnBrowseAccessDatabase
            // 
            this.btnBrowseAccessDatabase.Location = new System.Drawing.Point(786, 74);
            this.btnBrowseAccessDatabase.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseAccessDatabase.Name = "btnBrowseAccessDatabase";
            this.btnBrowseAccessDatabase.Size = new System.Drawing.Size(85, 30);
            this.btnBrowseAccessDatabase.TabIndex = 89;
            this.btnBrowseAccessDatabase.Text = "Browse ...";
            this.btnBrowseAccessDatabase.UseVisualStyleBackColor = true;
            this.btnBrowseAccessDatabase.Click += new System.EventHandler(this.btnBrowseAccessDatabase_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(49, 85);
            this.label26.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(118, 17);
            this.label26.TabIndex = 88;
            this.label26.Text = "Access Database";
            // 
            // tbxAccessDatabase
            // 
            this.tbxAccessDatabase.Location = new System.Drawing.Point(191, 82);
            this.tbxAccessDatabase.Margin = new System.Windows.Forms.Padding(4);
            this.tbxAccessDatabase.Name = "tbxAccessDatabase";
            this.tbxAccessDatabase.Size = new System.Drawing.Size(566, 22);
            this.tbxAccessDatabase.TabIndex = 87;
            this.tbxAccessDatabase.Text = " ";
            // 
            // btnBrowseTimesheetDataFile
            // 
            this.btnBrowseTimesheetDataFile.Location = new System.Drawing.Point(786, 34);
            this.btnBrowseTimesheetDataFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowseTimesheetDataFile.Name = "btnBrowseTimesheetDataFile";
            this.btnBrowseTimesheetDataFile.Size = new System.Drawing.Size(85, 30);
            this.btnBrowseTimesheetDataFile.TabIndex = 86;
            this.btnBrowseTimesheetDataFile.Text = "Browse ...";
            this.btnBrowseTimesheetDataFile.UseVisualStyleBackColor = true;
            this.btnBrowseTimesheetDataFile.Click += new System.EventHandler(this.btnBrowseTimesheetDataFile_Click);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(49, 227);
            this.label25.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(55, 17);
            this.label25.TabIndex = 85;
            this.label25.Text = "Results";
            // 
            // tbxImportResults
            // 
            this.tbxImportResults.Location = new System.Drawing.Point(52, 260);
            this.tbxImportResults.Margin = new System.Windows.Forms.Padding(4);
            this.tbxImportResults.MaxLength = 0;
            this.tbxImportResults.Multiline = true;
            this.tbxImportResults.Name = "tbxImportResults";
            this.tbxImportResults.Size = new System.Drawing.Size(705, 293);
            this.tbxImportResults.TabIndex = 84;
            // 
            // tbxMonthWeekEndingDate
            // 
            this.tbxMonthWeekEndingDate.Location = new System.Drawing.Point(104, 119);
            this.tbxMonthWeekEndingDate.Margin = new System.Windows.Forms.Padding(4);
            this.tbxMonthWeekEndingDate.MaxLength = 2;
            this.tbxMonthWeekEndingDate.Name = "tbxMonthWeekEndingDate";
            this.tbxMonthWeekEndingDate.Size = new System.Drawing.Size(35, 22);
            this.tbxMonthWeekEndingDate.TabIndex = 82;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(49, 122);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(47, 17);
            this.label23.TabIndex = 80;
            this.label23.Text = "Month";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(165, 122);
            this.label24.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(38, 17);
            this.label24.TabIndex = 81;
            this.label24.Text = "Year";
            // 
            // tbxYearWeekEndingDate
            // 
            this.tbxYearWeekEndingDate.Location = new System.Drawing.Point(216, 118);
            this.tbxYearWeekEndingDate.Margin = new System.Windows.Forms.Padding(4);
            this.tbxYearWeekEndingDate.MaxLength = 4;
            this.tbxYearWeekEndingDate.Name = "tbxYearWeekEndingDate";
            this.tbxYearWeekEndingDate.Size = new System.Drawing.Size(53, 22);
            this.tbxYearWeekEndingDate.TabIndex = 83;
            // 
            // btnImportTimesheet
            // 
            this.btnImportTimesheet.Location = new System.Drawing.Point(52, 166);
            this.btnImportTimesheet.Margin = new System.Windows.Forms.Padding(4);
            this.btnImportTimesheet.Name = "btnImportTimesheet";
            this.btnImportTimesheet.Size = new System.Drawing.Size(153, 41);
            this.btnImportTimesheet.TabIndex = 71;
            this.btnImportTimesheet.Text = "Import Timesheet";
            this.btnImportTimesheet.UseVisualStyleBackColor = true;
            this.btnImportTimesheet.Click += new System.EventHandler(this.btnImportTimesheet_Click);
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(49, 45);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(134, 17);
            this.label22.TabIndex = 51;
            this.label22.Text = "Timesheet Data File";
            // 
            // tbxTimesheetDataFile
            // 
            this.tbxTimesheetDataFile.Location = new System.Drawing.Point(191, 42);
            this.tbxTimesheetDataFile.Margin = new System.Windows.Forms.Padding(4);
            this.tbxTimesheetDataFile.Name = "tbxTimesheetDataFile";
            this.tbxTimesheetDataFile.Size = new System.Drawing.Size(566, 22);
            this.tbxTimesheetDataFile.TabIndex = 50;
            this.tbxTimesheetDataFile.Text = " ";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button6);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.gbxCreateReports);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1449, 632);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Testing";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(562, 251);
            this.button6.Margin = new System.Windows.Forms.Padding(4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(351, 55);
            this.button6.TabIndex = 76;
            this.button6.Text = "Unconstrained Maintenance List - Current Year";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(562, 185);
            this.button5.Margin = new System.Windows.Forms.Padding(4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(153, 41);
            this.button5.TabIndex = 77;
            this.button5.Text = "Update IsBridge";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(562, 118);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(153, 41);
            this.button4.TabIndex = 76;
            this.button4.Text = "Update Work Dups";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // gbxCreateReports
            // 
            this.gbxCreateReports.Controls.Add(this.btnCreateMetaReport);
            this.gbxCreateReports.Controls.Add(this.btnGetCoreData);
            this.gbxCreateReports.Controls.Add(this.btnAnalyzeFlexibleScenario);
            this.gbxCreateReports.Controls.Add(this.btnAnalyzeStrDeckReplacements);
            this.gbxCreateReports.Controls.Add(this.tbxCaiId);
            this.gbxCreateReports.Controls.Add(this.label2);
            this.gbxCreateReports.Controls.Add(this.btnGenerateRegionNeedsReport);
            this.gbxCreateReports.Controls.Add(this.btnGetNbiDeterioration);
            this.gbxCreateReports.Controls.Add(this.btnGenerateLocalBridgeProgramReport);
            this.gbxCreateReports.Controls.Add(this.btnGenerateStatePmdssAndNeedsReport);
            this.gbxCreateReports.Controls.Add(this.btnGenerateFiipsReport);
            this.gbxCreateReports.Controls.Add(this.btnGenerateAllCurrentNeeds);
            this.gbxCreateReports.Controls.Add(this.btnGenerateDebugReport);
            this.gbxCreateReports.Controls.Add(this.btnGenerateStatePmdssReport);
            this.gbxCreateReports.Controls.Add(this.btnGenerateStateNeedsReport);
            this.gbxCreateReports.Controls.Add(this.btnGenerateStateFiipsReport);
            this.gbxCreateReports.Location = new System.Drawing.Point(155, 103);
            this.gbxCreateReports.Margin = new System.Windows.Forms.Padding(4);
            this.gbxCreateReports.Name = "gbxCreateReports";
            this.gbxCreateReports.Padding = new System.Windows.Forms.Padding(4);
            this.gbxCreateReports.Size = new System.Drawing.Size(350, 189);
            this.gbxCreateReports.TabIndex = 57;
            this.gbxCreateReports.TabStop = false;
            this.gbxCreateReports.Text = "Analysis Types";
            // 
            // btnCreateMetaReport
            // 
            this.btnCreateMetaReport.Location = new System.Drawing.Point(177, 135);
            this.btnCreateMetaReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnCreateMetaReport.Name = "btnCreateMetaReport";
            this.btnCreateMetaReport.Size = new System.Drawing.Size(153, 41);
            this.btnCreateMetaReport.TabIndex = 75;
            this.btnCreateMetaReport.Text = "Meta";
            this.btnCreateMetaReport.UseVisualStyleBackColor = true;
            this.btnCreateMetaReport.Click += new System.EventHandler(this.btnCreateMetaReport_Click);
            // 
            // btnGetCoreData
            // 
            this.btnGetCoreData.Location = new System.Drawing.Point(16, 135);
            this.btnGetCoreData.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetCoreData.Name = "btnGetCoreData";
            this.btnGetCoreData.Size = new System.Drawing.Size(153, 41);
            this.btnGetCoreData.TabIndex = 74;
            this.btnGetCoreData.Text = "CoRe Data";
            this.btnGetCoreData.UseVisualStyleBackColor = true;
            this.btnGetCoreData.Click += new System.EventHandler(this.btnGetCoreData_Click);
            // 
            // btnAnalyzeFlexibleScenario
            // 
            this.btnAnalyzeFlexibleScenario.Location = new System.Drawing.Point(16, 82);
            this.btnAnalyzeFlexibleScenario.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnalyzeFlexibleScenario.Name = "btnAnalyzeFlexibleScenario";
            this.btnAnalyzeFlexibleScenario.Size = new System.Drawing.Size(153, 41);
            this.btnAnalyzeFlexibleScenario.TabIndex = 72;
            this.btnAnalyzeFlexibleScenario.Text = "Flexible Optimal";
            this.btnAnalyzeFlexibleScenario.UseVisualStyleBackColor = true;
            // 
            // btnAnalyzeStrDeckReplacements
            // 
            this.btnAnalyzeStrDeckReplacements.Location = new System.Drawing.Point(177, 29);
            this.btnAnalyzeStrDeckReplacements.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnalyzeStrDeckReplacements.Name = "btnAnalyzeStrDeckReplacements";
            this.btnAnalyzeStrDeckReplacements.Size = new System.Drawing.Size(153, 41);
            this.btnAnalyzeStrDeckReplacements.TabIndex = 71;
            this.btnAnalyzeStrDeckReplacements.Text = "Str && Deck Repl";
            this.btnAnalyzeStrDeckReplacements.UseVisualStyleBackColor = true;
            this.btnAnalyzeStrDeckReplacements.Click += new System.EventHandler(this.btnAnalyzeStrDeckReplacements_Click);
            // 
            // tbxCaiId
            // 
            this.tbxCaiId.Location = new System.Drawing.Point(1076, 241);
            this.tbxCaiId.Margin = new System.Windows.Forms.Padding(4);
            this.tbxCaiId.MaxLength = 2;
            this.tbxCaiId.Name = "tbxCaiId";
            this.tbxCaiId.Size = new System.Drawing.Size(127, 22);
            this.tbxCaiId.TabIndex = 29;
            this.tbxCaiId.Text = "10";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1156, 219);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.TabIndex = 28;
            this.label2.Text = "CAI Formula ID";
            this.label2.Visible = false;
            // 
            // btnGenerateRegionNeedsReport
            // 
            this.btnGenerateRegionNeedsReport.Location = new System.Drawing.Point(815, 23);
            this.btnGenerateRegionNeedsReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateRegionNeedsReport.Name = "btnGenerateRegionNeedsReport";
            this.btnGenerateRegionNeedsReport.Size = new System.Drawing.Size(167, 41);
            this.btnGenerateRegionNeedsReport.TabIndex = 65;
            this.btnGenerateRegionNeedsReport.Text = "Old Needs Analysis";
            this.btnGenerateRegionNeedsReport.UseVisualStyleBackColor = true;
            this.btnGenerateRegionNeedsReport.Visible = false;
            // 
            // btnGetNbiDeterioration
            // 
            this.btnGetNbiDeterioration.Location = new System.Drawing.Point(815, 233);
            this.btnGetNbiDeterioration.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetNbiDeterioration.Name = "btnGetNbiDeterioration";
            this.btnGetNbiDeterioration.Size = new System.Drawing.Size(276, 41);
            this.btnGetNbiDeterioration.TabIndex = 69;
            this.btnGetNbiDeterioration.Text = "NBI Deterioration";
            this.btnGetNbiDeterioration.UseVisualStyleBackColor = true;
            this.btnGetNbiDeterioration.Visible = false;
            // 
            // btnGenerateLocalBridgeProgramReport
            // 
            this.btnGenerateLocalBridgeProgramReport.Location = new System.Drawing.Point(177, 82);
            this.btnGenerateLocalBridgeProgramReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateLocalBridgeProgramReport.Name = "btnGenerateLocalBridgeProgramReport";
            this.btnGenerateLocalBridgeProgramReport.Size = new System.Drawing.Size(153, 41);
            this.btnGenerateLocalBridgeProgramReport.TabIndex = 66;
            this.btnGenerateLocalBridgeProgramReport.Text = "Local Prog- Current Year";
            this.btnGenerateLocalBridgeProgramReport.UseVisualStyleBackColor = true;
            this.btnGenerateLocalBridgeProgramReport.Click += new System.EventHandler(this.btnGenerateLocalBridgeProgramReport_Click);
            // 
            // btnGenerateStatePmdssAndNeedsReport
            // 
            this.btnGenerateStatePmdssAndNeedsReport.Location = new System.Drawing.Point(1076, 123);
            this.btnGenerateStatePmdssAndNeedsReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateStatePmdssAndNeedsReport.Name = "btnGenerateStatePmdssAndNeedsReport";
            this.btnGenerateStatePmdssAndNeedsReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateStatePmdssAndNeedsReport.TabIndex = 63;
            this.btnGenerateStatePmdssAndNeedsReport.Text = "DTIM- Needs Analysis";
            this.btnGenerateStatePmdssAndNeedsReport.UseVisualStyleBackColor = true;
            this.btnGenerateStatePmdssAndNeedsReport.Visible = false;
            // 
            // btnGenerateFiipsReport
            // 
            this.btnGenerateFiipsReport.Location = new System.Drawing.Point(815, 75);
            this.btnGenerateFiipsReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateFiipsReport.Name = "btnGenerateFiipsReport";
            this.btnGenerateFiipsReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateFiipsReport.TabIndex = 62;
            this.btnGenerateFiipsReport.Text = "FIIPS Analysis Debug Report";
            this.btnGenerateFiipsReport.UseVisualStyleBackColor = true;
            this.btnGenerateFiipsReport.Visible = false;
            // 
            // btnGenerateAllCurrentNeeds
            // 
            this.btnGenerateAllCurrentNeeds.Enabled = false;
            this.btnGenerateAllCurrentNeeds.Location = new System.Drawing.Point(815, 162);
            this.btnGenerateAllCurrentNeeds.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateAllCurrentNeeds.Name = "btnGenerateAllCurrentNeeds";
            this.btnGenerateAllCurrentNeeds.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateAllCurrentNeeds.TabIndex = 61;
            this.btnGenerateAllCurrentNeeds.Text = "All Current Needs";
            this.btnGenerateAllCurrentNeeds.UseVisualStyleBackColor = true;
            this.btnGenerateAllCurrentNeeds.Visible = false;
            // 
            // btnGenerateDebugReport
            // 
            this.btnGenerateDebugReport.Location = new System.Drawing.Point(1097, 15);
            this.btnGenerateDebugReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateDebugReport.Name = "btnGenerateDebugReport";
            this.btnGenerateDebugReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateDebugReport.TabIndex = 58;
            this.btnGenerateDebugReport.Text = "BA Debug Data Dump";
            this.btnGenerateDebugReport.UseVisualStyleBackColor = true;
            this.btnGenerateDebugReport.Visible = false;
            // 
            // btnGenerateStatePmdssReport
            // 
            this.btnGenerateStatePmdssReport.Enabled = false;
            this.btnGenerateStatePmdssReport.Location = new System.Drawing.Point(1099, 75);
            this.btnGenerateStatePmdssReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateStatePmdssReport.Name = "btnGenerateStatePmdssReport";
            this.btnGenerateStatePmdssReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateStatePmdssReport.TabIndex = 51;
            this.btnGenerateStatePmdssReport.Text = "BMDSS";
            this.btnGenerateStatePmdssReport.UseVisualStyleBackColor = true;
            this.btnGenerateStatePmdssReport.Visible = false;
            // 
            // btnGenerateStateNeedsReport
            // 
            this.btnGenerateStateNeedsReport.Enabled = false;
            this.btnGenerateStateNeedsReport.Location = new System.Drawing.Point(1099, 162);
            this.btnGenerateStateNeedsReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateStateNeedsReport.Name = "btnGenerateStateNeedsReport";
            this.btnGenerateStateNeedsReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateStateNeedsReport.TabIndex = 53;
            this.btnGenerateStateNeedsReport.Text = "Needs Analysis";
            this.btnGenerateStateNeedsReport.UseVisualStyleBackColor = true;
            this.btnGenerateStateNeedsReport.Visible = false;
            // 
            // btnGenerateStateFiipsReport
            // 
            this.btnGenerateStateFiipsReport.Location = new System.Drawing.Point(815, 114);
            this.btnGenerateStateFiipsReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnGenerateStateFiipsReport.Name = "btnGenerateStateFiipsReport";
            this.btnGenerateStateFiipsReport.Size = new System.Drawing.Size(276, 41);
            this.btnGenerateStateFiipsReport.TabIndex = 52;
            this.btnGenerateStateFiipsReport.Text = "DTIM- FIIPS Analysis";
            this.btnGenerateStateFiipsReport.UseVisualStyleBackColor = true;
            this.btnGenerateStateFiipsReport.Visible = false;
            // 
            // gbxPickDatabase
            // 
            this.gbxPickDatabase.Controls.Add(this.rbtWisamDbDev);
            this.gbxPickDatabase.Controls.Add(this.rbtWisamDbTest);
            this.gbxPickDatabase.Controls.Add(this.rbtWisamDbProd);
            this.gbxPickDatabase.Location = new System.Drawing.Point(952, 2);
            this.gbxPickDatabase.Margin = new System.Windows.Forms.Padding(4);
            this.gbxPickDatabase.Name = "gbxPickDatabase";
            this.gbxPickDatabase.Padding = new System.Windows.Forms.Padding(4);
            this.gbxPickDatabase.Size = new System.Drawing.Size(308, 42);
            this.gbxPickDatabase.TabIndex = 60;
            this.gbxPickDatabase.TabStop = false;
            this.gbxPickDatabase.Text = "Database Selection";
            // 
            // rbtWisamDbDev
            // 
            this.rbtWisamDbDev.AutoSize = true;
            this.rbtWisamDbDev.Location = new System.Drawing.Point(248, 18);
            this.rbtWisamDbDev.Margin = new System.Windows.Forms.Padding(4);
            this.rbtWisamDbDev.Name = "rbtWisamDbDev";
            this.rbtWisamDbDev.Size = new System.Drawing.Size(54, 21);
            this.rbtWisamDbDev.TabIndex = 2;
            this.rbtWisamDbDev.Text = "Dev";
            this.rbtWisamDbDev.UseVisualStyleBackColor = true;
            this.rbtWisamDbDev.CheckedChanged += new System.EventHandler(this.WisamDb_CheckedChanged);
            // 
            // rbtWisamDbTest
            // 
            this.rbtWisamDbTest.AutoSize = true;
            this.rbtWisamDbTest.Location = new System.Drawing.Point(141, 17);
            this.rbtWisamDbTest.Margin = new System.Windows.Forms.Padding(4);
            this.rbtWisamDbTest.Name = "rbtWisamDbTest";
            this.rbtWisamDbTest.Size = new System.Drawing.Size(57, 21);
            this.rbtWisamDbTest.TabIndex = 1;
            this.rbtWisamDbTest.Text = "Test";
            this.rbtWisamDbTest.UseVisualStyleBackColor = true;
            this.rbtWisamDbTest.CheckedChanged += new System.EventHandler(this.WisamDb_CheckedChanged);
            // 
            // rbtWisamDbProd
            // 
            this.rbtWisamDbProd.AutoSize = true;
            this.rbtWisamDbProd.Checked = true;
            this.rbtWisamDbProd.Location = new System.Drawing.Point(32, 17);
            this.rbtWisamDbProd.Margin = new System.Windows.Forms.Padding(4);
            this.rbtWisamDbProd.Name = "rbtWisamDbProd";
            this.rbtWisamDbProd.Size = new System.Drawing.Size(59, 21);
            this.rbtWisamDbProd.TabIndex = 0;
            this.rbtWisamDbProd.TabStop = true;
            this.rbtWisamDbProd.Text = "Prod";
            this.rbtWisamDbProd.UseVisualStyleBackColor = true;
            this.rbtWisamDbProd.CheckedChanged += new System.EventHandler(this.WisamDb_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(813, 2);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 17);
            this.label6.TabIndex = 1;
            this.label6.Text = "Current Database";
            // 
            // lblCurrentDb
            // 
            this.lblCurrentDb.AutoSize = true;
            this.lblCurrentDb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentDb.Location = new System.Drawing.Point(813, 17);
            this.lblCurrentDb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentDb.Name = "lblCurrentDb";
            this.lblCurrentDb.Size = new System.Drawing.Size(42, 17);
            this.lblCurrentDb.TabIndex = 2;
            this.lblCurrentDb.Text = "Prod";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::WiSam.Desktop.Properties.Resources.bos_small;
            this.pictureBox1.Location = new System.Drawing.Point(1264, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(208, 42);
            this.pictureBox1.TabIndex = 61;
            this.pictureBox1.TabStop = false;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 697);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblCurrentDb);
            this.Controls.Add(this.gbxPickDatabase);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tctMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormMain";
            this.Text = "WiSAMS (Wisconsin Structures Asset Management System)";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tctMain.ResumeLayout(false);
            this.tpgNeedsAnalysis.ResumeLayout(false);
            this.tpgNeedsAnalysis.PerformLayout();
            this.tsrNeedsAnalysis.ResumeLayout(false);
            this.tsrNeedsAnalysis.PerformLayout();
            this.gbxComments.ResumeLayout(false);
            this.gbxComments.PerformLayout();
            this.gbxPolicy.ResumeLayout(false);
            this.gbxPolicy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPolicies)).EndInit();
            this.gbxMiscellaneous.ResumeLayout(false);
            this.gbxMiscellaneous.PerformLayout();
            this.gbxBudget.ResumeLayout(false);
            this.gbxBudget.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBudget)).EndInit();
            this.gbxEnterOtherCriteria.ResumeLayout(false);
            this.gbxEnterOtherCriteria.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.gbxYearType.ResumeLayout(false);
            this.gbxYearType.PerformLayout();
            this.gbxPickWorkTypes.ResumeLayout(false);
            this.gbxPickStructures.ResumeLayout(false);
            this.gbxPickStructures.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tpgAdmin.ResumeLayout(false);
            this.tctAdmin.ResumeLayout(false);
            this.tpgWorkAction.ResumeLayout(false);
            this.gbxUpdateWorkCriteria.ResumeLayout(false);
            this.gbxUpdateWorkCriteria.PerformLayout();
            this.tpgDeterioration.ResumeLayout(false);
            this.tpgDeterioration.PerformLayout();
            this.gbxUpdateDeteriorationCurves.ResumeLayout(false);
            this.gbxUpdateDeteriorationCurves.PerformLayout();
            this.tpgFiips.ResumeLayout(false);
            this.tpgFiips.PerformLayout();
            this.tpgMiscReports.ResumeLayout(false);
            this.tpgMiscReports.PerformLayout();
            this.tpgAssetManagement.ResumeLayout(false);
            this.tpgAssetManagement.PerformLayout();
            this.tpgTimesheet.ResumeLayout(false);
            this.tpgTimesheet.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.gbxCreateReports.ResumeLayout(false);
            this.gbxCreateReports.PerformLayout();
            this.gbxPickDatabase.ResumeLayout(false);
            this.gbxPickDatabase.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tctMain;
        private System.Windows.Forms.TabPage tpgNeedsAnalysis;
        private System.Windows.Forms.TextBox tbxRegions;
        private System.Windows.Forms.TextBox tbxEndYear;
        private System.Windows.Forms.TextBox tbxStartYear;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbxStructureIds;
        private System.Windows.Forms.GroupBox gbxPickStructures;
        private System.Windows.Forms.RadioButton rbtStructuresById;
        private System.Windows.Forms.RadioButton rbtStructuresByRegion;
        private System.Windows.Forms.CheckBox chkLocalBridges;
        private System.Windows.Forms.CheckBox chkStateBridges;
        private System.Windows.Forms.CheckBox chkDebug;
        private System.Windows.Forms.GroupBox gbxPickWorkTypes;
        private System.Windows.Forms.GroupBox gbxEnterOtherCriteria;
        private System.Windows.Forms.LinkLabel lbkOpenOutputFile;
        private System.Windows.Forms.TabPage tpgAssetManagement;
        private System.Windows.Forms.LinkLabel lbkAssetManagementOpenOutputFile;
        private System.Windows.Forms.Label lblAssetManagementExcelOutputFile;
        private System.Windows.Forms.TextBox tbxAssetManagementExcelOutputFilePath;
        private System.Windows.Forms.CheckBox chkDeteriorateOlayDefects;
        private System.Windows.Forms.GroupBox gbxPickDatabase;
        private System.Windows.Forms.RadioButton rbtWisamDbDev;
        private System.Windows.Forms.RadioButton rbtWisamDbTest;
        private System.Windows.Forms.RadioButton rbtWisamDbProd;
        private System.Windows.Forms.TabPage tpgAdmin;
        private System.Windows.Forms.TabControl tctAdmin;
        private System.Windows.Forms.TabPage tpgWorkAction;
        private System.Windows.Forms.TabPage tpgDeterioration;
        private System.Windows.Forms.Button btnUpdateOverlaysCombinedWorkActions;
        private System.Windows.Forms.GroupBox gbxUpdateWorkCriteria;
        private System.Windows.Forms.Button btnAddNewWorkRule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxRuleSequence;
        private System.Windows.Forms.Label lblRuleSequence;
        private System.Windows.Forms.Label lblActive;
        private System.Windows.Forms.ComboBox cbxWorkAction;
        private System.Windows.Forms.Label lblWorkAction;
        private System.Windows.Forms.CheckBox chkRuleActive;
        private System.Windows.Forms.TextBox tbxRuleNotes;
        private System.Windows.Forms.ListBox lbxRuleCategory;
        private System.Windows.Forms.Label lblRuleNotes;
        private System.Windows.Forms.Label lblRuleCategory;
        private System.Windows.Forms.Label lblRuleCriteria;
        private System.Windows.Forms.Label lblRuleId;
        private System.Windows.Forms.TextBox tbxRuleCriteria;
        private System.Windows.Forms.ListBox lbxRuleId;
        private System.Windows.Forms.Button btnUpdateWorkActionCriteria;
        private System.Windows.Forms.TabPage tpgFiips;
        private System.Windows.Forms.Button btnCalculateNbiDeteriorationRates;
        private System.Windows.Forms.LinkLabel lbkOpenFiipsBridgeListFile;
        private System.Windows.Forms.Button btnCreateFiipsBridgeList;
        private System.Windows.Forms.Button btnBrowsePonModDeterExcelInputFile;
        private System.Windows.Forms.Label lblPonModDeterExcelInputFilePath;
        private System.Windows.Forms.TextBox tbxPonModDeterExcelInputFilePath;
        private System.Windows.Forms.Button btnUpdatePonModDeter;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnDeleteWorkActionCriteria;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblCurrentDb;
        private System.Windows.Forms.Button btnBrowseExcelInputFile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbxExcelInputFilePath;
        private System.Windows.Forms.Button btnUpdatePmic;
        private System.Windows.Forms.GroupBox gbxUpdateDeteriorationCurves;
        private System.Windows.Forms.Label lblNbiComponentDeteriorationFormula;
        private System.Windows.Forms.Label lblNbiComponent;
        private System.Windows.Forms.TextBox tbxNbiComponentDeteriorationFormula;
        private System.Windows.Forms.ListBox lbxNbiComponent;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbxNbiComponentDeterioratedRatings;
        private System.Windows.Forms.Button btnSaveNbiComponentDeterioration;
        private System.Windows.Forms.Button btnRecalcNbiComponentDeterioration;
        private System.Windows.Forms.TabPage tpgMiscReports;
        private System.Windows.Forms.Button btnCreateRulesTable;
        private System.Windows.Forms.Button btnCreateElementDeteriorationReport;
        private System.Windows.Forms.Button btnUpdateCorridorCodes;
        private System.Windows.Forms.Button btnUpdateHighClearanceRoutes;
        private System.Windows.Forms.CheckBox chkPiFactors;
        private System.Windows.Forms.LinkLabel lbkOpenMiscReport;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox tbxMiscReportsOutputFilePath;
        private System.Windows.Forms.TextBox tbxFirstYear;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox tbxLastYear;
        private System.Windows.Forms.Button btnGenerateAssetManagementReport;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckedListBox checkedListBoxWorkActions;
        private System.Windows.Forms.CheckBox chkInterpolateNbi;
        private System.Windows.Forms.CheckBox chkIncludeCStructures;
        private System.Windows.Forms.Button btnCreateBidItemsReport;
        private System.Windows.Forms.TextBox tbxStartLetYear;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox tbxEndLetYear;
        private System.Windows.Forms.Button btnCreateDesignBillableReport;
        private System.Windows.Forms.TextBox tbxStartMonthBillableReport;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox tbxStartYearBillableReport;
        private System.Windows.Forms.TextBox tbxEndMonthBillableReport;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox tbxEndYearBillableReport;
        private System.Windows.Forms.TextBox tbxRuleWorkActionNotes;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button btnUnselectAllWorkActions;
        private System.Windows.Forms.Button btnSelectAllWorkActions;
        private System.Windows.Forms.TabPage tpgTimesheet;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox tbxTimesheetDataFile;
        private System.Windows.Forms.Button btnImportTimesheet;
        private System.Windows.Forms.TextBox tbxMonthWeekEndingDate;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox tbxYearWeekEndingDate;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox tbxImportResults;
        private System.Windows.Forms.Button btnBrowseTimesheetDataFile;
        private System.Windows.Forms.Button btnBrowseAccessDatabase;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox tbxAccessDatabase;
        private System.Windows.Forms.LinkLabel lbkOpenBillableReport;
        private System.Windows.Forms.Button btnCreateStructureProgramReport;
        private System.Windows.Forms.CheckBox chkCountTpo;
        private System.Windows.Forms.LinkLabel lbkOpenDebugFile;
        private System.Windows.Forms.GroupBox gbxBudget;
        private System.Windows.Forms.CheckBox chkBigBucket;
        private System.Windows.Forms.DataGridView dgvBudget;
        private System.Windows.Forms.GroupBox gbxMiscellaneous;
        private System.Windows.Forms.GroupBox gbxYearType;
        private System.Windows.Forms.RadioButton rbtCalendarYear;
        private System.Windows.Forms.RadioButton rbtFederalFiscalYear;
        private System.Windows.Forms.RadioButton rbtStateFiscalYear;
        private System.Windows.Forms.GroupBox gbxPolicy;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox cboAnalysisTypes;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button btnRunNeedsAnalysis;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkNorthwest;
        private System.Windows.Forms.CheckBox chkNorthcentral;
        private System.Windows.Forms.CheckBox chkNortheast;
        private System.Windows.Forms.CheckBox chkSoutheast;
        private System.Windows.Forms.CheckBox chkSouthwest;
        private System.Windows.Forms.GroupBox gbxComments;
        private System.Windows.Forms.TextBox tbxComments;
        private System.Windows.Forms.TabPage tpgSettings;
        private System.Windows.Forms.DataGridView dgvPolicies;
        private System.Windows.Forms.Label lblPriorityScoreInfo;
        private System.Windows.Forms.ToolStrip tsrNeedsAnalysis;
        private System.Windows.Forms.ToolStripButton tsbNeedsAnalysisRun;
        private System.Windows.Forms.ToolStripButton tsbNeedsAnalysisOpenFile;
        private System.Windows.Forms.ToolStripButton tsbNeedsAnalysisSaveInput;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Label lblPiCategories;
        private System.Windows.Forms.TextBox tbxMaxPriorityScore;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckPolicy;
        private System.Windows.Forms.DataGridViewTextBoxColumn Policy;
        private System.Windows.Forms.DataGridViewTextBoxColumn Criteria;
        private System.Windows.Forms.DataGridViewTextBoxColumn PriorityScoreEffect;
        private System.Windows.Forms.ToolStripButton tsbNeedsAnalysisSaveAs;
        private System.Windows.Forms.DataGridViewTextBoxColumn budgetYear;
        private System.Windows.Forms.DataGridViewTextBoxColumn budgetAmount;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox gbxCreateReports;
        private System.Windows.Forms.Button btnCreateMetaReport;
        private System.Windows.Forms.Button btnGetCoreData;
        private System.Windows.Forms.Button btnAnalyzeFlexibleScenario;
        private System.Windows.Forms.Button btnAnalyzeStrDeckReplacements;
        private System.Windows.Forms.TextBox tbxCaiId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnGenerateRegionNeedsReport;
        private System.Windows.Forms.Button btnGetNbiDeterioration;
        private System.Windows.Forms.Button btnGenerateLocalBridgeProgramReport;
        private System.Windows.Forms.Button btnGenerateStatePmdssAndNeedsReport;
        private System.Windows.Forms.Button btnGenerateFiipsReport;
        private System.Windows.Forms.Button btnGenerateAllCurrentNeeds;
        private System.Windows.Forms.Button btnGenerateDebugReport;
        private System.Windows.Forms.Button btnGenerateStatePmdssReport;
        private System.Windows.Forms.Button btnGenerateStateNeedsReport;
        private System.Windows.Forms.Button btnGenerateStateFiipsReport;
        private System.Windows.Forms.Button btnAnalyzeRegionNeeds;
        private System.Windows.Forms.CheckBox chkApplyBudget;
        private System.Windows.Forms.GroupBox gbxResults;
        private System.Windows.Forms.TextBox tbxExcelOuputFilePath;
        private System.Windows.Forms.TextBox tbxLeastCost;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.TextBox tbxMaxNumToAnalyze;
        private System.Windows.Forms.RadioButton rbtStructuresByFunding;
        private System.Windows.Forms.CheckedListBox checkedListBoxFundings;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.CheckBox chkBridgeInventory;
        private System.Windows.Forms.TextBox tbxElements;
        private System.Windows.Forms.Button btnCreateLetDatesReport;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Button btnSetAnnualBudget;
        private System.Windows.Forms.TextBox tbxAnnualBudget;
        private System.Windows.Forms.CheckBox chkApplyRegionSelections;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button buttonGetStructuresDataForGis;
        private System.Windows.Forms.ComboBox comboBoxQualifiedDeterioration;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.CheckBox checkBoxQualifiedDeterioration;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.TextBox textBoxQualifiedExpression;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButtonBrmDeterioration;
        private System.Windows.Forms.RadioButton radioButtonOldDeterioration;
    }
}

