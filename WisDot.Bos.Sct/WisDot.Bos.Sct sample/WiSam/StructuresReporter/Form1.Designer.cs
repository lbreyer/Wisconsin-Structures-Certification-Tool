namespace StructuresReporter
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonBidItems = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonInHouseDesign = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonFiips = new System.Windows.Forms.ToolStripButton();
            this.groupBoxFilter = new System.Windows.Forms.GroupBox();
            this.groupBoxResults = new System.Windows.Forms.GroupBox();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMain
            // 
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonBidItems,
            this.toolStripButtonInHouseDesign,
            this.toolStripButtonFiips});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(770, 27);
            this.toolStripMain.TabIndex = 0;
            this.toolStripMain.Text = "Reports";
            // 
            // toolStripButtonBidItems
            // 
            this.toolStripButtonBidItems.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBidItems.Image")));
            this.toolStripButtonBidItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBidItems.Name = "toolStripButtonBidItems";
            this.toolStripButtonBidItems.Size = new System.Drawing.Size(95, 24);
            this.toolStripButtonBidItems.Text = "Bid Items";
            // 
            // toolStripButtonInHouseDesign
            // 
            this.toolStripButtonInHouseDesign.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonInHouseDesign.Image")));
            this.toolStripButtonInHouseDesign.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonInHouseDesign.Name = "toolStripButtonInHouseDesign";
            this.toolStripButtonInHouseDesign.Size = new System.Drawing.Size(143, 24);
            this.toolStripButtonInHouseDesign.Text = "In-House Design";
            // 
            // toolStripButtonFiips
            // 
            this.toolStripButtonFiips.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonFiips.Image")));
            this.toolStripButtonFiips.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonFiips.Name = "toolStripButtonFiips";
            this.toolStripButtonFiips.Size = new System.Drawing.Size(64, 24);
            this.toolStripButtonFiips.Text = "FIIPS";
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Location = new System.Drawing.Point(13, 31);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Size = new System.Drawing.Size(745, 100);
            this.groupBoxFilter.TabIndex = 1;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "Filter";
            // 
            // groupBoxResults
            // 
            this.groupBoxResults.Location = new System.Drawing.Point(13, 138);
            this.groupBoxResults.Name = "groupBoxResults";
            this.groupBoxResults.Size = new System.Drawing.Size(745, 204);
            this.groupBoxResults.TabIndex = 2;
            this.groupBoxResults.TabStop = false;
            this.groupBoxResults.Text = "Results";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 354);
            this.Controls.Add(this.groupBoxResults);
            this.Controls.Add(this.groupBoxFilter);
            this.Controls.Add(this.toolStripMain);
            this.Name = "Form1";
            this.Text = "Structures Reporter";
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonBidItems;
        private System.Windows.Forms.ToolStripButton toolStripButtonInHouseDesign;
        private System.Windows.Forms.ToolStripButton toolStripButtonFiips;
        private System.Windows.Forms.GroupBox groupBoxFilter;
        private System.Windows.Forms.GroupBox groupBoxResults;
    }
}

