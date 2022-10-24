namespace WiSam.StructuresProgram
{
    partial class FormAddWorkConceptToProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAddWorkConceptToProject));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAddNewWorkConcept = new System.Windows.Forms.Button();
            this.comboBoxWorkConcept = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.textBoxNewWorkConceptStructureId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonNewStructure = new System.Windows.Forms.RadioButton();
            this.radioButtonRecommend = new System.Windows.Forms.RadioButton();
            this.radioButtonEvaluate = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonCancel);
            this.groupBox1.Controls.Add(this.buttonAddNewWorkConcept);
            this.groupBox1.Controls.Add(this.comboBoxWorkConcept);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.textBoxNewWorkConceptStructureId);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.radioButtonNewStructure);
            this.groupBox1.Controls.Add(this.radioButtonRecommend);
            this.groupBox1.Controls.Add(this.radioButtonEvaluate);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 179);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonCancel.Location = new System.Drawing.Point(198, 128);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(128, 30);
            this.buttonCancel.TabIndex = 49;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonAddNewWorkConcept
            // 
            this.buttonAddNewWorkConcept.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAddNewWorkConcept.Location = new System.Drawing.Point(54, 128);
            this.buttonAddNewWorkConcept.Name = "buttonAddNewWorkConcept";
            this.buttonAddNewWorkConcept.Size = new System.Drawing.Size(128, 30);
            this.buttonAddNewWorkConcept.TabIndex = 48;
            this.buttonAddNewWorkConcept.Text = "Submit";
            this.buttonAddNewWorkConcept.UseVisualStyleBackColor = true;
            this.buttonAddNewWorkConcept.Click += new System.EventHandler(this.buttonAddNewWorkConcept_Click);
            // 
            // comboBoxWorkConcept
            // 
            this.comboBoxWorkConcept.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWorkConcept.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxWorkConcept.FormattingEnabled = true;
            this.comboBoxWorkConcept.Location = new System.Drawing.Point(112, 78);
            this.comboBoxWorkConcept.Name = "comboBoxWorkConcept";
            this.comboBoxWorkConcept.Size = new System.Drawing.Size(251, 21);
            this.comboBoxWorkConcept.TabIndex = 47;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(200, 41);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(188, 17);
            this.label18.TabIndex = 45;
            this.label18.Text = "(7 or 11 char, e.g. B010002)";
            this.label18.Visible = false;
            // 
            // textBoxNewWorkConceptStructureId
            // 
            this.textBoxNewWorkConceptStructureId.Location = new System.Drawing.Point(313, 16);
            this.textBoxNewWorkConceptStructureId.Name = "textBoxNewWorkConceptStructureId";
            this.textBoxNewWorkConceptStructureId.Size = new System.Drawing.Size(80, 22);
            this.textBoxNewWorkConceptStructureId.TabIndex = 44;
            this.textBoxNewWorkConceptStructureId.Text = "BXXYYYY";
            this.textBoxNewWorkConceptStructureId.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-28, 118);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 17);
            this.label3.TabIndex = 43;
            this.label3.Text = "Str:";
            // 
            // radioButtonNewStructure
            // 
            this.radioButtonNewStructure.AutoSize = true;
            this.radioButtonNewStructure.Location = new System.Drawing.Point(191, 17);
            this.radioButtonNewStructure.Name = "radioButtonNewStructure";
            this.radioButtonNewStructure.Size = new System.Drawing.Size(118, 21);
            this.radioButtonNewStructure.TabIndex = 2;
            this.radioButtonNewStructure.TabStop = true;
            this.radioButtonNewStructure.Text = "New Structure";
            this.radioButtonNewStructure.UseVisualStyleBackColor = true;
            this.radioButtonNewStructure.Visible = false;
            // 
            // radioButtonRecommend
            // 
            this.radioButtonRecommend.AutoSize = true;
            this.radioButtonRecommend.Location = new System.Drawing.Point(29, 78);
            this.radioButtonRecommend.Name = "radioButtonRecommend";
            this.radioButtonRecommend.Size = new System.Drawing.Size(82, 21);
            this.radioButtonRecommend.TabIndex = 1;
            this.radioButtonRecommend.Text = "Propose";
            this.radioButtonRecommend.UseVisualStyleBackColor = true;
            // 
            // radioButtonEvaluate
            // 
            this.radioButtonEvaluate.AutoSize = true;
            this.radioButtonEvaluate.Checked = true;
            this.radioButtonEvaluate.Location = new System.Drawing.Point(29, 44);
            this.radioButtonEvaluate.Name = "radioButtonEvaluate";
            this.radioButtonEvaluate.Size = new System.Drawing.Size(84, 21);
            this.radioButtonEvaluate.TabIndex = 0;
            this.radioButtonEvaluate.TabStop = true;
            this.radioButtonEvaluate.Text = "Evaluate";
            this.radioButtonEvaluate.UseVisualStyleBackColor = true;
            // 
            // FormAddWorkConceptToProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 179);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddWorkConceptToProject";
            this.Text = "Evaluate or Propose";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonNewStructure;
        private System.Windows.Forms.RadioButton radioButtonRecommend;
        private System.Windows.Forms.RadioButton radioButtonEvaluate;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBoxNewWorkConceptStructureId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxWorkConcept;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonAddNewWorkConcept;
    }
}