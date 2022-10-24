namespace WiSam.StructuresProgram
{
    partial class FormLogin
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.buttonOpenHelp = new System.Windows.Forms.Button();
            this.buttonExitApplication = new System.Windows.Forms.Button();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.comboBoxDatabase = new System.Windows.Forms.ComboBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.buttonEmailUs = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxProjectEdit = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProjectEdit)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOpenHelp
            // 
            this.buttonOpenHelp.Location = new System.Drawing.Point(41, 235);
            this.buttonOpenHelp.Name = "buttonOpenHelp";
            this.buttonOpenHelp.Size = new System.Drawing.Size(67, 30);
            this.buttonOpenHelp.TabIndex = 25;
            this.buttonOpenHelp.Text = "Help";
            this.buttonOpenHelp.UseVisualStyleBackColor = true;
            this.buttonOpenHelp.Visible = false;
            this.buttonOpenHelp.Click += new System.EventHandler(this.buttonOpenHelp_Click);
            // 
            // buttonExitApplication
            // 
            this.buttonExitApplication.Location = new System.Drawing.Point(196, 194);
            this.buttonExitApplication.Name = "buttonExitApplication";
            this.buttonExitApplication.Size = new System.Drawing.Size(82, 30);
            this.buttonExitApplication.TabIndex = 24;
            this.buttonExitApplication.Text = "Cancel";
            this.buttonExitApplication.UseVisualStyleBackColor = true;
            this.buttonExitApplication.Click += new System.EventHandler(this.buttonExitApplication_Click);
            // 
            // buttonLogin
            // 
            this.buttonLogin.Location = new System.Drawing.Point(113, 194);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(63, 30);
            this.buttonLogin.TabIndex = 23;
            this.buttonLogin.Text = "OK";
            this.buttonLogin.UseVisualStyleBackColor = true;
            this.buttonLogin.Click += new System.EventHandler(this.buttonLogin_Click);
            // 
            // comboBoxDatabase
            // 
            this.comboBoxDatabase.FormattingEnabled = true;
            this.comboBoxDatabase.Items.AddRange(new object[] {
            "Prod",
            "Test",
            "Dev"});
            this.comboBoxDatabase.Location = new System.Drawing.Point(12, 164);
            this.comboBoxDatabase.Name = "comboBoxDatabase";
            this.comboBoxDatabase.Size = new System.Drawing.Size(54, 24);
            this.comboBoxDatabase.TabIndex = 21;
            this.comboBoxDatabase.Visible = false;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(202, 144);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(165, 22);
            this.textBoxPassword.TabIndex = 20;
            this.textBoxPassword.Text = "dotswr";
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPassword_KeyDown);
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(202, 104);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(165, 22);
            this.textBoxUserName.TabIndex = 19;
            this.textBoxUserName.Text = "dotswr";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(120, 147);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 17);
            this.label5.TabIndex = 17;
            this.label5.Text = "Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(116, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 16;
            this.label4.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(149, 323);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "Build Date: June 2021";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(170, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Structures Certification Tool";
            // 
            // buttonEmailUs
            // 
            this.buttonEmailUs.Location = new System.Drawing.Point(298, 194);
            this.buttonEmailUs.Name = "buttonEmailUs";
            this.buttonEmailUs.Size = new System.Drawing.Size(75, 30);
            this.buttonEmailUs.TabIndex = 27;
            this.buttonEmailUs.Text = "Email Us";
            this.buttonEmailUs.UseVisualStyleBackColor = true;
            this.buttonEmailUs.Click += new System.EventHandler(this.buttonEmailUs_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::WiSam.StructuresProgram.Properties.Resources.scot_logo;
            this.pictureBox2.Location = new System.Drawing.Point(61, 12);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(117, 66);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 28;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::WiSam.StructuresProgram.Properties.Resources.BOS_Logo_240;
            this.pictureBox1.Location = new System.Drawing.Point(120, 245);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(240, 50);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBoxProjectEdit
            // 
            this.pictureBoxProjectEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBoxProjectEdit.Image = global::WiSam.StructuresProgram.Properties.Resources.ajax_loader;
            this.pictureBoxProjectEdit.Location = new System.Drawing.Point(396, 130);
            this.pictureBoxProjectEdit.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxProjectEdit.Name = "pictureBoxProjectEdit";
            this.pictureBoxProjectEdit.Size = new System.Drawing.Size(33, 31);
            this.pictureBoxProjectEdit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxProjectEdit.TabIndex = 78;
            this.pictureBoxProjectEdit.TabStop = false;
            this.pictureBoxProjectEdit.Visible = false;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 368);
            this.Controls.Add(this.pictureBoxProjectEdit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.buttonEmailUs);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonOpenHelp);
            this.Controls.Add(this.buttonExitApplication);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.comboBoxDatabase);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.FormLogin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxProjectEdit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenHelp;
        private System.Windows.Forms.Button buttonExitApplication;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.ComboBox comboBoxDatabase;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button buttonEmailUs;
        private System.Windows.Forms.PictureBox pictureBox2;
        protected System.Windows.Forms.PictureBox pictureBoxProjectEdit;
    }
}

