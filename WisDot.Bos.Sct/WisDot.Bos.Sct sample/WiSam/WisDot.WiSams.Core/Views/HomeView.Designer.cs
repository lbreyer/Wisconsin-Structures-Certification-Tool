namespace WisDot.WiSams.Core.Views
{
    partial class HomeView
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
            this.groupBoxNbiDeterioration = new System.Windows.Forms.GroupBox();
            this.listBoxNbiComponent = new System.Windows.Forms.ListBox();
            this.textBoxDeteriorationFormula = new System.Windows.Forms.TextBox();
            this.buttonSaveDeterioration = new System.Windows.Forms.Button();
            this.buttonCalculateDeterioration = new System.Windows.Forms.Button();
            this.groupBoxNbiDeterioration.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxNbiDeterioration
            // 
            this.groupBoxNbiDeterioration.Controls.Add(this.buttonCalculateDeterioration);
            this.groupBoxNbiDeterioration.Controls.Add(this.buttonSaveDeterioration);
            this.groupBoxNbiDeterioration.Controls.Add(this.textBoxDeteriorationFormula);
            this.groupBoxNbiDeterioration.Controls.Add(this.listBoxNbiComponent);
            this.groupBoxNbiDeterioration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxNbiDeterioration.Location = new System.Drawing.Point(0, 0);
            this.groupBoxNbiDeterioration.Name = "groupBoxNbiDeterioration";
            this.groupBoxNbiDeterioration.Size = new System.Drawing.Size(932, 442);
            this.groupBoxNbiDeterioration.TabIndex = 0;
            this.groupBoxNbiDeterioration.TabStop = false;
            this.groupBoxNbiDeterioration.Text = "NBI Deterioration";
            // 
            // listBoxNbiComponent
            // 
            this.listBoxNbiComponent.FormattingEnabled = true;
            this.listBoxNbiComponent.ItemHeight = 16;
            this.listBoxNbiComponent.Location = new System.Drawing.Point(193, 51);
            this.listBoxNbiComponent.Name = "listBoxNbiComponent";
            this.listBoxNbiComponent.Size = new System.Drawing.Size(120, 84);
            this.listBoxNbiComponent.TabIndex = 0;
            // 
            // textBoxDeteriorationFormula
            // 
            this.textBoxDeteriorationFormula.Location = new System.Drawing.Point(421, 51);
            this.textBoxDeteriorationFormula.Multiline = true;
            this.textBoxDeteriorationFormula.Name = "textBoxDeteriorationFormula";
            this.textBoxDeteriorationFormula.Size = new System.Drawing.Size(451, 84);
            this.textBoxDeteriorationFormula.TabIndex = 1;
            // 
            // buttonSaveDeterioration
            // 
            this.buttonSaveDeterioration.Location = new System.Drawing.Point(741, 182);
            this.buttonSaveDeterioration.Name = "buttonSaveDeterioration";
            this.buttonSaveDeterioration.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveDeterioration.TabIndex = 2;
            this.buttonSaveDeterioration.Text = "Save";
            this.buttonSaveDeterioration.UseVisualStyleBackColor = true;
            // 
            // buttonCalculateDeterioration
            // 
            this.buttonCalculateDeterioration.Location = new System.Drawing.Point(492, 182);
            this.buttonCalculateDeterioration.Name = "buttonCalculateDeterioration";
            this.buttonCalculateDeterioration.Size = new System.Drawing.Size(209, 23);
            this.buttonCalculateDeterioration.TabIndex = 3;
            this.buttonCalculateDeterioration.Text = "Recalculate Deterioration";
            this.buttonCalculateDeterioration.UseVisualStyleBackColor = true;
            // 
            // HomeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 442);
            this.Controls.Add(this.groupBoxNbiDeterioration);
            this.Name = "HomeView";
            this.Text = "Home";
            this.groupBoxNbiDeterioration.ResumeLayout(false);
            this.groupBoxNbiDeterioration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxNbiDeterioration;
        private System.Windows.Forms.Button buttonSaveDeterioration;
        private System.Windows.Forms.TextBox textBoxDeteriorationFormula;
        private System.Windows.Forms.ListBox listBoxNbiComponent;
        private System.Windows.Forms.Button buttonCalculateDeterioration;
    }
}