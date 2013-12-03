namespace Inferno_Login_Agent_562
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.headLabel = new System.Windows.Forms.Label();
            this.maintainanceCheckBox = new System.Windows.Forms.CheckBox();
            this.lsStatusLabel = new System.Windows.Forms.Label();
            this.lsStatus = new System.Windows.Forms.Label();
            this.footerLabel = new System.Windows.Forms.Label();
            this.lsTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // headLabel
            // 
            this.headLabel.AutoSize = true;
            this.headLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.headLabel.Font = new System.Drawing.Font("Segoe Script", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headLabel.ForeColor = System.Drawing.Color.Olive;
            this.headLabel.Location = new System.Drawing.Point(52, 9);
            this.headLabel.Name = "headLabel";
            this.headLabel.Size = new System.Drawing.Size(291, 33);
            this.headLabel.TabIndex = 31;
            this.headLabel.Text = "Inferno Login Agent 562";
            // 
            // maintainanceCheckBox
            // 
            this.maintainanceCheckBox.AutoSize = true;
            this.maintainanceCheckBox.Font = new System.Drawing.Font("Segoe Script", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maintainanceCheckBox.Location = new System.Drawing.Point(76, 92);
            this.maintainanceCheckBox.Name = "maintainanceCheckBox";
            this.maintainanceCheckBox.Size = new System.Drawing.Size(247, 35);
            this.maintainanceCheckBox.TabIndex = 32;
            this.maintainanceCheckBox.Text = "Server Maintainance";
            this.maintainanceCheckBox.UseVisualStyleBackColor = true;
            this.maintainanceCheckBox.CheckedChanged += new System.EventHandler(this.maintainanceCheckBox_CheckedChanged);
            // 
            // lsStatusLabel
            // 
            this.lsStatusLabel.AutoSize = true;
            this.lsStatusLabel.Font = new System.Drawing.Font("Segoe Script", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsStatusLabel.ForeColor = System.Drawing.Color.Fuchsia;
            this.lsStatusLabel.Location = new System.Drawing.Point(20, 53);
            this.lsStatusLabel.Name = "lsStatusLabel";
            this.lsStatusLabel.Size = new System.Drawing.Size(133, 33);
            this.lsStatusLabel.TabIndex = 33;
            this.lsStatusLabel.Text = "LS Status :";
            // 
            // lsStatus
            // 
            this.lsStatus.AutoSize = true;
            this.lsStatus.Font = new System.Drawing.Font("Segoe Script", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsStatus.ForeColor = System.Drawing.Color.Black;
            this.lsStatus.Location = new System.Drawing.Point(159, 53);
            this.lsStatus.Name = "lsStatus";
            this.lsStatus.Size = new System.Drawing.Size(164, 33);
            this.lsStatus.TabIndex = 35;
            this.lsStatus.Text = "Disconnected";
            // 
            // footerLabel
            // 
            this.footerLabel.AutoSize = true;
            this.footerLabel.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.footerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.footerLabel.Location = new System.Drawing.Point(114, 135);
            this.footerLabel.Name = "footerLabel";
            this.footerLabel.Size = new System.Drawing.Size(162, 20);
            this.footerLabel.TabIndex = 36;
            this.footerLabel.Text = "~ Made by Karthik P ~";
            // 
            // lsTimer
            // 
            this.lsTimer.Interval = 500;
            this.lsTimer.Tick += new System.EventHandler(this.lsTimer_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 164);
            this.Controls.Add(this.footerLabel);
            this.Controls.Add(this.lsStatus);
            this.Controls.Add(this.lsStatusLabel);
            this.Controls.Add(this.maintainanceCheckBox);
            this.Controls.Add(this.headLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(412, 202);
            this.Name = "Main";
            this.Text = "Inferno Login Agent 562";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headLabel;
        private System.Windows.Forms.CheckBox maintainanceCheckBox;
        private System.Windows.Forms.Label lsStatusLabel;
        private System.Windows.Forms.Label lsStatus;
        private System.Windows.Forms.Label footerLabel;
        private System.Windows.Forms.Timer lsTimer;
    }
}

