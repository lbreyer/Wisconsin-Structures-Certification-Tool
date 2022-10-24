namespace WiSam.StructuresProgram
{
    partial class FormRequestRecertification
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRequestRecertification));
            this.textBoxRecertificationReason = new System.Windows.Forms.TextBox();
            this.buttonSubmitRecertificationRequest = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.buttonClearRecertificationReason = new System.Windows.Forms.Button();
            this.buttonGrantRecertificationRequest = new System.Windows.Forms.Button();
            this.buttonRejectRecertificationRequest = new System.Windows.Forms.Button();
            this.textBoxRecertificationComments = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxRecertificationReason
            // 
            this.textBoxRecertificationReason.Location = new System.Drawing.Point(21, 30);
            this.textBoxRecertificationReason.MaxLength = 2000;
            this.textBoxRecertificationReason.Multiline = true;
            this.textBoxRecertificationReason.Name = "textBoxRecertificationReason";
            this.textBoxRecertificationReason.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxRecertificationReason.Size = new System.Drawing.Size(757, 162);
            this.textBoxRecertificationReason.TabIndex = 96;
            // 
            // buttonSubmitRecertificationRequest
            // 
            this.buttonSubmitRecertificationRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSubmitRecertificationRequest.Location = new System.Drawing.Point(35, 436);
            this.buttonSubmitRecertificationRequest.Name = "buttonSubmitRecertificationRequest";
            this.buttonSubmitRecertificationRequest.Size = new System.Drawing.Size(168, 30);
            this.buttonSubmitRecertificationRequest.TabIndex = 94;
            this.buttonSubmitRecertificationRequest.Text = "Submit Recert Request";
            this.buttonSubmitRecertificationRequest.UseVisualStyleBackColor = true;
            this.buttonSubmitRecertificationRequest.Click += new System.EventHandler(this.buttonSubmitRecertificationRequest_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(21, 10);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(139, 17);
            this.label21.TabIndex = 95;
            this.label21.Text = "Reason for Request:";
            // 
            // buttonClearRecertificationReason
            // 
            this.buttonClearRecertificationReason.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonClearRecertificationReason.Location = new System.Drawing.Point(222, 436);
            this.buttonClearRecertificationReason.Name = "buttonClearRecertificationReason";
            this.buttonClearRecertificationReason.Size = new System.Drawing.Size(107, 30);
            this.buttonClearRecertificationReason.TabIndex = 97;
            this.buttonClearRecertificationReason.Text = "Clear Reason";
            this.buttonClearRecertificationReason.UseVisualStyleBackColor = true;
            this.buttonClearRecertificationReason.Click += new System.EventHandler(this.buttonClearRecertificationReason_Click);
            // 
            // buttonGrantRecertificationRequest
            // 
            this.buttonGrantRecertificationRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonGrantRecertificationRequest.Location = new System.Drawing.Point(406, 436);
            this.buttonGrantRecertificationRequest.Name = "buttonGrantRecertificationRequest";
            this.buttonGrantRecertificationRequest.Size = new System.Drawing.Size(181, 30);
            this.buttonGrantRecertificationRequest.TabIndex = 98;
            this.buttonGrantRecertificationRequest.Text = "Grant Recert Request";
            this.buttonGrantRecertificationRequest.UseVisualStyleBackColor = true;
            this.buttonGrantRecertificationRequest.Click += new System.EventHandler(this.buttonGrantRecertificationRequest_Click);
            // 
            // buttonRejectRecertificationRequest
            // 
            this.buttonRejectRecertificationRequest.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRejectRecertificationRequest.Location = new System.Drawing.Point(593, 436);
            this.buttonRejectRecertificationRequest.Name = "buttonRejectRecertificationRequest";
            this.buttonRejectRecertificationRequest.Size = new System.Drawing.Size(181, 30);
            this.buttonRejectRecertificationRequest.TabIndex = 99;
            this.buttonRejectRecertificationRequest.Text = "Reject Recert Request";
            this.buttonRejectRecertificationRequest.UseVisualStyleBackColor = true;
            this.buttonRejectRecertificationRequest.Click += new System.EventHandler(this.buttonRejectRecertificationRequest_Click);
            // 
            // textBoxRecertificationComments
            // 
            this.textBoxRecertificationComments.Location = new System.Drawing.Point(21, 221);
            this.textBoxRecertificationComments.MaxLength = 2000;
            this.textBoxRecertificationComments.Multiline = true;
            this.textBoxRecertificationComments.Name = "textBoxRecertificationComments";
            this.textBoxRecertificationComments.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxRecertificationComments.Size = new System.Drawing.Size(757, 162);
            this.textBoxRecertificationComments.TabIndex = 101;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 201);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 17);
            this.label1.TabIndex = 100;
            this.label1.Text = "BOS Comments:";
            // 
            // FormRequestRecertification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 485);
            this.Controls.Add(this.textBoxRecertificationComments);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonRejectRecertificationRequest);
            this.Controls.Add(this.buttonGrantRecertificationRequest);
            this.Controls.Add(this.buttonClearRecertificationReason);
            this.Controls.Add(this.textBoxRecertificationReason);
            this.Controls.Add(this.buttonSubmitRecertificationRequest);
            this.Controls.Add(this.label21);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormRequestRecertification";
            this.Text = "Request Recertification";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxRecertificationReason;
        private System.Windows.Forms.Button buttonSubmitRecertificationRequest;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button buttonClearRecertificationReason;
        private System.Windows.Forms.Button buttonGrantRecertificationRequest;
        private System.Windows.Forms.Button buttonRejectRecertificationRequest;
        private System.Windows.Forms.TextBox textBoxRecertificationComments;
        private System.Windows.Forms.Label label1;
    }
}