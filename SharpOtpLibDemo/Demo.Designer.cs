namespace SharpOtpLibDemo
{
    partial class Demo
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
            this.picQrCode = new System.Windows.Forms.PictureBox();
            this.txtIdent = new System.Windows.Forms.TextBox();
            this.txtCurCode = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.txtCounter = new System.Windows.Forms.TextBox();
            this.rdoHotp = new System.Windows.Forms.RadioButton();
            this.rdoTotp = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdCheckCode = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtOffset = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numWindow = new System.Windows.Forms.NumericUpDown();
            this.txtSecret = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmdGenerateSecret = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdLoad = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtThrottling = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.chkLocked = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // picQrCode
            // 
            this.picQrCode.Location = new System.Drawing.Point(13, 13);
            this.picQrCode.Name = "picQrCode";
            this.picQrCode.Size = new System.Drawing.Size(200, 200);
            this.picQrCode.TabIndex = 0;
            this.picQrCode.TabStop = false;
            // 
            // txtIdent
            // 
            this.txtIdent.Location = new System.Drawing.Point(308, 13);
            this.txtIdent.Name = "txtIdent";
            this.txtIdent.Size = new System.Drawing.Size(160, 20);
            this.txtIdent.TabIndex = 1;
            this.txtIdent.Text = "ident";
            this.txtIdent.TextChanged += new System.EventHandler(this.txtIdent_TextChanged);
            // 
            // txtCurCode
            // 
            this.txtCurCode.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtCurCode.Location = new System.Drawing.Point(308, 65);
            this.txtCurCode.Name = "txtCurCode";
            this.txtCurCode.ReadOnly = true;
            this.txtCurCode.Size = new System.Drawing.Size(79, 20);
            this.txtCurCode.TabIndex = 1;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(308, 91);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(79, 20);
            this.txtCode.TabIndex = 1;
            // 
            // txtCounter
            // 
            this.txtCounter.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtCounter.Location = new System.Drawing.Point(308, 39);
            this.txtCounter.Name = "txtCounter";
            this.txtCounter.ReadOnly = true;
            this.txtCounter.Size = new System.Drawing.Size(79, 20);
            this.txtCounter.TabIndex = 1;
            // 
            // rdoHotp
            // 
            this.rdoHotp.AutoSize = true;
            this.rdoHotp.Checked = true;
            this.rdoHotp.Location = new System.Drawing.Point(413, 167);
            this.rdoHotp.Name = "rdoHotp";
            this.rdoHotp.Size = new System.Drawing.Size(55, 17);
            this.rdoHotp.TabIndex = 2;
            this.rdoHotp.TabStop = true;
            this.rdoHotp.Text = "HOTP";
            this.rdoHotp.UseVisualStyleBackColor = true;
            this.rdoHotp.CheckedChanged += new System.EventHandler(this.rdoHotp_CheckedChanged);
            // 
            // rdoTotp
            // 
            this.rdoTotp.AutoSize = true;
            this.rdoTotp.Location = new System.Drawing.Point(353, 167);
            this.rdoTotp.Name = "rdoTotp";
            this.rdoTotp.Size = new System.Drawing.Size(54, 17);
            this.rdoTotp.TabIndex = 2;
            this.rdoTotp.TabStop = true;
            this.rdoTotp.Text = "TOTP";
            this.rdoTotp.UseVisualStyleBackColor = true;
            this.rdoTotp.CheckedChanged += new System.EventHandler(this.rdoTotp_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(220, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Ident";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(219, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Counter";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(220, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Current Code";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(220, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Verify Code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(220, 192);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Window";
            // 
            // cmdCheckCode
            // 
            this.cmdCheckCode.Location = new System.Drawing.Point(393, 89);
            this.cmdCheckCode.Name = "cmdCheckCode";
            this.cmdCheckCode.Size = new System.Drawing.Size(75, 23);
            this.cmdCheckCode.TabIndex = 5;
            this.cmdCheckCode.Text = "Check Code";
            this.cmdCheckCode.UseVisualStyleBackColor = true;
            this.cmdCheckCode.Click += new System.EventHandler(this.cmdCheckCode_Click);
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtResult.Location = new System.Drawing.Point(308, 141);
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(160, 20);
            this.txtResult.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(220, 144);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Result";
            // 
            // txtOffset
            // 
            this.txtOffset.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtOffset.Location = new System.Drawing.Point(434, 39);
            this.txtOffset.Name = "txtOffset";
            this.txtOffset.ReadOnly = true;
            this.txtOffset.Size = new System.Drawing.Size(34, 20);
            this.txtOffset.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(393, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Offset";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(220, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Type";
            // 
            // numWindow
            // 
            this.numWindow.Location = new System.Drawing.Point(348, 190);
            this.numWindow.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numWindow.Name = "numWindow";
            this.numWindow.Size = new System.Drawing.Size(120, 20);
            this.numWindow.TabIndex = 6;
            this.numWindow.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numWindow.ValueChanged += new System.EventHandler(this.numWindow_ValueChanged);
            // 
            // txtSecret
            // 
            this.txtSecret.Location = new System.Drawing.Point(93, 219);
            this.txtSecret.Name = "txtSecret";
            this.txtSecret.Size = new System.Drawing.Size(375, 20);
            this.txtSecret.TabIndex = 7;
            this.txtSecret.Text = "YUEZENG7WK6HRA4YW3GKWXQRF42BTVXD";
            this.txtSecret.TextChanged += new System.EventHandler(this.txtSecret_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 222);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 13);
            this.label9.TabIndex = 4;
            this.label9.Text = "Secret";
            // 
            // cmdGenerateSecret
            // 
            this.cmdGenerateSecret.Location = new System.Drawing.Point(205, 245);
            this.cmdGenerateSecret.Name = "cmdGenerateSecret";
            this.cmdGenerateSecret.Size = new System.Drawing.Size(97, 23);
            this.cmdGenerateSecret.TabIndex = 5;
            this.cmdGenerateSecret.Text = "Generate Secret";
            this.cmdGenerateSecret.UseVisualStyleBackColor = true;
            this.cmdGenerateSecret.Click += new System.EventHandler(this.cmdGenerateSecret_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(393, 245);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 5;
            this.cmdSave.Text = "Save DB";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdLoad
            // 
            this.cmdLoad.Location = new System.Drawing.Point(308, 245);
            this.cmdLoad.Name = "cmdLoad";
            this.cmdLoad.Size = new System.Drawing.Size(75, 23);
            this.cmdLoad.TabIndex = 5;
            this.cmdLoad.Text = "Load DB";
            this.cmdLoad.UseVisualStyleBackColor = true;
            this.cmdLoad.Click += new System.EventHandler(this.cmdLoad_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(221, 118);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(51, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Throttling";
            // 
            // txtThrottling
            // 
            this.txtThrottling.BackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.txtThrottling.Location = new System.Drawing.Point(308, 115);
            this.txtThrottling.Name = "txtThrottling";
            this.txtThrottling.ReadOnly = true;
            this.txtThrottling.Size = new System.Drawing.Size(79, 20);
            this.txtThrottling.TabIndex = 1;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(387, 118);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(88, 13);
            this.label11.TabIndex = 3;
            this.label11.Text = "(next delay in ms)";
            // 
            // chkLocked
            // 
            this.chkLocked.AutoCheck = false;
            this.chkLocked.AutoSize = true;
            this.chkLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkLocked.Location = new System.Drawing.Point(406, 65);
            this.chkLocked.Name = "chkLocked";
            this.chkLocked.Size = new System.Drawing.Size(62, 17);
            this.chkLocked.TabIndex = 8;
            this.chkLocked.Text = "Locked";
            this.chkLocked.UseVisualStyleBackColor = true;
            // 
            // Demo
            // 
            this.AcceptButton = this.cmdCheckCode;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 274);
            this.Controls.Add(this.chkLocked);
            this.Controls.Add(this.txtSecret);
            this.Controls.Add(this.numWindow);
            this.Controls.Add(this.cmdGenerateSecret);
            this.Controls.Add(this.cmdLoad);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.cmdCheckCode);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rdoTotp);
            this.Controls.Add(this.rdoHotp);
            this.Controls.Add(this.txtCounter);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.txtThrottling);
            this.Controls.Add(this.txtOffset);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.txtCurCode);
            this.Controls.Add(this.txtIdent);
            this.Controls.Add(this.picQrCode);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Demo";
            this.Text = "Demo";
            this.Load += new System.EventHandler(this.Demo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picQrCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWindow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picQrCode;
        private System.Windows.Forms.TextBox txtIdent;
        private System.Windows.Forms.TextBox txtCurCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.TextBox txtCounter;
        private System.Windows.Forms.RadioButton rdoHotp;
        private System.Windows.Forms.RadioButton rdoTotp;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cmdCheckCode;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtOffset;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numWindow;
        private System.Windows.Forms.TextBox txtSecret;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button cmdGenerateSecret;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdLoad;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtThrottling;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox chkLocked;

    }
}