
namespace ClearCase2Git
{
    partial class cc2git_addRepo
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
            this.GitRepoChkBx = new System.Windows.Forms.CheckBox();
            this.FolderPathLbl = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.FolderPathTxtBx = new System.Windows.Forms.TextBox();
            this.ProjectNameTxtBx = new System.Windows.Forms.TextBox();
            this.ProjectNameLbl = new System.Windows.Forms.Label();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.CreateBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // GitRepoChkBx
            // 
            this.GitRepoChkBx.AutoSize = true;
            this.GitRepoChkBx.Checked = true;
            this.GitRepoChkBx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.GitRepoChkBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GitRepoChkBx.Location = new System.Drawing.Point(445, 48);
            this.GitRepoChkBx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.GitRepoChkBx.Name = "GitRepoChkBx";
            this.GitRepoChkBx.Size = new System.Drawing.Size(78, 21);
            this.GitRepoChkBx.TabIndex = 0;
            this.GitRepoChkBx.Text = "Git repo";
            this.GitRepoChkBx.UseVisualStyleBackColor = true;
            // 
            // FolderPathLbl
            // 
            this.FolderPathLbl.AutoSize = true;
            this.FolderPathLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FolderPathLbl.Location = new System.Drawing.Point(13, 16);
            this.FolderPathLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FolderPathLbl.Name = "FolderPathLbl";
            this.FolderPathLbl.Size = new System.Drawing.Size(80, 17);
            this.FolderPathLbl.TabIndex = 1;
            this.FolderPathLbl.Text = "Folder path";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(445, 11);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "Navigate";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FolderPathTxtBx
            // 
            this.FolderPathTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FolderPathTxtBx.Location = new System.Drawing.Point(136, 12);
            this.FolderPathTxtBx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.FolderPathTxtBx.Name = "FolderPathTxtBx";
            this.FolderPathTxtBx.Size = new System.Drawing.Size(300, 23);
            this.FolderPathTxtBx.TabIndex = 3;
            this.FolderPathTxtBx.TextChanged += new System.EventHandler(this.FolderPathTxtBx_TextChanged);
            // 
            // ProjectNameTxtBx
            // 
            this.ProjectNameTxtBx.AcceptsReturn = true;
            this.ProjectNameTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectNameTxtBx.Location = new System.Drawing.Point(136, 48);
            this.ProjectNameTxtBx.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ProjectNameTxtBx.Name = "ProjectNameTxtBx";
            this.ProjectNameTxtBx.Size = new System.Drawing.Size(300, 23);
            this.ProjectNameTxtBx.TabIndex = 5;
            this.ProjectNameTxtBx.TextChanged += new System.EventHandler(this.ProjectNameTxtBx_TextChanged);
            // 
            // ProjectNameLbl
            // 
            this.ProjectNameLbl.AutoSize = true;
            this.ProjectNameLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectNameLbl.Location = new System.Drawing.Point(13, 52);
            this.ProjectNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ProjectNameLbl.Name = "ProjectNameLbl";
            this.ProjectNameLbl.Size = new System.Drawing.Size(91, 17);
            this.ProjectNameLbl.TabIndex = 4;
            this.ProjectNameLbl.Text = "Project name";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.Location = new System.Drawing.Point(15, 91);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(264, 31);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // CreateBtn
            // 
            this.CreateBtn.Enabled = false;
            this.CreateBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateBtn.Location = new System.Drawing.Point(281, 91);
            this.CreateBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CreateBtn.Name = "CreateBtn";
            this.CreateBtn.Size = new System.Drawing.Size(264, 31);
            this.CreateBtn.TabIndex = 7;
            this.CreateBtn.Text = "Create";
            this.CreateBtn.UseVisualStyleBackColor = true;
            this.CreateBtn.Click += new System.EventHandler(this.CreateBtn_Click);
            // 
            // cc2git_addRepo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 132);
            this.Controls.Add(this.CreateBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.ProjectNameTxtBx);
            this.Controls.Add(this.ProjectNameLbl);
            this.Controls.Add(this.FolderPathTxtBx);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.FolderPathLbl);
            this.Controls.Add(this.GitRepoChkBx);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "cc2git_addRepo";
            this.Text = "cc2git_addRepo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox GitRepoChkBx;
        private System.Windows.Forms.Label FolderPathLbl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox FolderPathTxtBx;
        private System.Windows.Forms.TextBox ProjectNameTxtBx;
        private System.Windows.Forms.Label ProjectNameLbl;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button CreateBtn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}