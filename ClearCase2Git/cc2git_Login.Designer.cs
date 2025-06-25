
namespace ClearCase2Git
{
    partial class cc2git_Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(cc2git_Login));
            this.LogoPicBx = new System.Windows.Forms.PictureBox();
            this.LoginUserIDLbl = new System.Windows.Forms.Label();
            this.LoginUserTxtBx = new System.Windows.Forms.TextBox();
            this.UserLoginPasswordTxtBx = new System.Windows.Forms.TextBox();
            this.LoginUserPasswrodLbl = new System.Windows.Forms.Label();
            this.NewUserChkBx = new System.Windows.Forms.CheckBox();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.LoginBtn = new System.Windows.Forms.Button();
            this.FullNameTxtBx = new System.Windows.Forms.TextBox();
            this.FullNameLbl = new System.Windows.Forms.Label();
            this.EmailTxtBx = new System.Windows.Forms.TextBox();
            this.EmailLbl = new System.Windows.Forms.Label();
            this.ProjectCmbBx = new System.Windows.Forms.ComboBox();
            this.ProjectsLbl = new System.Windows.Forms.Label();
            this.ProjectTxtBx = new System.Windows.Forms.TextBox();
            this.AddProjBtn = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.CCServerLbl = new System.Windows.Forms.Label();
            this.CCSrvCmbBx = new System.Windows.Forms.ComboBox();
            this.AddCCSrvTxtBx = new System.Windows.Forms.TextBox();
            this.GitUrlTxtBx = new System.Windows.Forms.TextBox();
            this.GitUrlLbl = new System.Windows.Forms.Label();
            this.gitUrlCmbBx = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPicBx)).BeginInit();
            this.SuspendLayout();
            // 
            // LogoPicBx
            // 
            this.LogoPicBx.Image = global::ClearCase2Git.Properties.Resources.CC2Git_Logo;
            this.LogoPicBx.InitialImage = ((System.Drawing.Image)(resources.GetObject("LogoPicBx.InitialImage")));
            this.LogoPicBx.Location = new System.Drawing.Point(-2, -1);
            this.LogoPicBx.Name = "LogoPicBx";
            this.LogoPicBx.Size = new System.Drawing.Size(325, 132);
            this.LogoPicBx.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.LogoPicBx.TabIndex = 0;
            this.LogoPicBx.TabStop = false;
            // 
            // LoginUserIDLbl
            // 
            this.LoginUserIDLbl.AutoSize = true;
            this.LoginUserIDLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginUserIDLbl.Location = new System.Drawing.Point(4, 140);
            this.LoginUserIDLbl.Name = "LoginUserIDLbl";
            this.LoginUserIDLbl.Size = new System.Drawing.Size(53, 17);
            this.LoginUserIDLbl.TabIndex = 1;
            this.LoginUserIDLbl.Text = "User Id";
            // 
            // LoginUserTxtBx
            // 
            this.LoginUserTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginUserTxtBx.Location = new System.Drawing.Point(87, 137);
            this.LoginUserTxtBx.Name = "LoginUserTxtBx";
            this.LoginUserTxtBx.Size = new System.Drawing.Size(156, 23);
            this.LoginUserTxtBx.TabIndex = 1;
            this.LoginUserTxtBx.TextChanged += new System.EventHandler(this.LoginUserTxtBx_TextChanged);
            // 
            // UserLoginPasswordTxtBx
            // 
            this.UserLoginPasswordTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserLoginPasswordTxtBx.Location = new System.Drawing.Point(87, 166);
            this.UserLoginPasswordTxtBx.Name = "UserLoginPasswordTxtBx";
            this.UserLoginPasswordTxtBx.Size = new System.Drawing.Size(208, 23);
            this.UserLoginPasswordTxtBx.TabIndex = 2;
            this.UserLoginPasswordTxtBx.UseSystemPasswordChar = true;
            this.UserLoginPasswordTxtBx.TextChanged += new System.EventHandler(this.UserLoginPasswordTxtBx_TextChanged);
            // 
            // LoginUserPasswrodLbl
            // 
            this.LoginUserPasswrodLbl.AutoSize = true;
            this.LoginUserPasswrodLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginUserPasswrodLbl.Location = new System.Drawing.Point(4, 169);
            this.LoginUserPasswrodLbl.Name = "LoginUserPasswrodLbl";
            this.LoginUserPasswrodLbl.Size = new System.Drawing.Size(69, 17);
            this.LoginUserPasswrodLbl.TabIndex = 3;
            this.LoginUserPasswrodLbl.Text = "Password";
            // 
            // NewUserChkBx
            // 
            this.NewUserChkBx.AutoSize = true;
            this.NewUserChkBx.Location = new System.Drawing.Point(249, 140);
            this.NewUserChkBx.Name = "NewUserChkBx";
            this.NewUserChkBx.Size = new System.Drawing.Size(71, 17);
            this.NewUserChkBx.TabIndex = 5;
            this.NewUserChkBx.Text = "New user";
            this.NewUserChkBx.UseVisualStyleBackColor = true;
            this.NewUserChkBx.CheckedChanged += new System.EventHandler(this.NewUserChkBx_CheckedChanged);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelBtn.Location = new System.Drawing.Point(6, 344);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(101, 25);
            this.CancelBtn.TabIndex = 6;
            this.CancelBtn.Text = "Exit";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // LoginBtn
            // 
            this.LoginBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoginBtn.Location = new System.Drawing.Point(213, 344);
            this.LoginBtn.Name = "LoginBtn";
            this.LoginBtn.Size = new System.Drawing.Size(106, 25);
            this.LoginBtn.TabIndex = 7;
            this.LoginBtn.Text = "Check";
            this.LoginBtn.UseVisualStyleBackColor = true;
            this.LoginBtn.Click += new System.EventHandler(this.LoginBtn_Click);
            // 
            // FullNameTxtBx
            // 
            this.FullNameTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullNameTxtBx.HideSelection = false;
            this.FullNameTxtBx.Location = new System.Drawing.Point(87, 195);
            this.FullNameTxtBx.Name = "FullNameTxtBx";
            this.FullNameTxtBx.Size = new System.Drawing.Size(233, 23);
            this.FullNameTxtBx.TabIndex = 3;
            this.FullNameTxtBx.WordWrap = false;
            this.FullNameTxtBx.TextChanged += new System.EventHandler(this.FullNameTxtBx_TextChanged);
            // 
            // FullNameLbl
            // 
            this.FullNameLbl.AutoSize = true;
            this.FullNameLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FullNameLbl.Location = new System.Drawing.Point(4, 198);
            this.FullNameLbl.Name = "FullNameLbl";
            this.FullNameLbl.Size = new System.Drawing.Size(71, 17);
            this.FullNameLbl.TabIndex = 8;
            this.FullNameLbl.Text = "Full Name";
            // 
            // EmailTxtBx
            // 
            this.EmailTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailTxtBx.HideSelection = false;
            this.EmailTxtBx.Location = new System.Drawing.Point(87, 224);
            this.EmailTxtBx.Name = "EmailTxtBx";
            this.EmailTxtBx.Size = new System.Drawing.Size(233, 23);
            this.EmailTxtBx.TabIndex = 4;
            this.EmailTxtBx.WordWrap = false;
            this.EmailTxtBx.TextChanged += new System.EventHandler(this.EmailTxtBx_TextChanged);
            // 
            // EmailLbl
            // 
            this.EmailLbl.AutoSize = true;
            this.EmailLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmailLbl.Location = new System.Drawing.Point(4, 227);
            this.EmailLbl.Name = "EmailLbl";
            this.EmailLbl.Size = new System.Drawing.Size(42, 17);
            this.EmailLbl.TabIndex = 10;
            this.EmailLbl.Text = "Email";
            // 
            // ProjectCmbBx
            // 
            this.ProjectCmbBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectCmbBx.FormattingEnabled = true;
            this.ProjectCmbBx.Location = new System.Drawing.Point(87, 313);
            this.ProjectCmbBx.Name = "ProjectCmbBx";
            this.ProjectCmbBx.Size = new System.Drawing.Size(233, 24);
            this.ProjectCmbBx.TabIndex = 12;
            this.ProjectCmbBx.SelectedIndexChanged += new System.EventHandler(this.ProjectCmbBx_SelectedIndexChanged);
            // 
            // ProjectsLbl
            // 
            this.ProjectsLbl.AutoSize = true;
            this.ProjectsLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectsLbl.Location = new System.Drawing.Point(4, 316);
            this.ProjectsLbl.Name = "ProjectsLbl";
            this.ProjectsLbl.Size = new System.Drawing.Size(74, 17);
            this.ProjectsLbl.TabIndex = 13;
            this.ProjectsLbl.Text = "Git M.Proj.";
            // 
            // ProjectTxtBx
            // 
            this.ProjectTxtBx.Enabled = false;
            this.ProjectTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProjectTxtBx.HideSelection = false;
            this.ProjectTxtBx.Location = new System.Drawing.Point(87, 313);
            this.ProjectTxtBx.Name = "ProjectTxtBx";
            this.ProjectTxtBx.Size = new System.Drawing.Size(232, 23);
            this.ProjectTxtBx.TabIndex = 14;
            this.ProjectTxtBx.TabStop = false;
            this.ProjectTxtBx.WordWrap = false;
            this.ProjectTxtBx.TextChanged += new System.EventHandler(this.ProjectTxtBx_TextChanged);
            // 
            // AddProjBtn
            // 
            this.AddProjBtn.Enabled = false;
            this.AddProjBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddProjBtn.Location = new System.Drawing.Point(107, 344);
            this.AddProjBtn.Name = "AddProjBtn";
            this.AddProjBtn.Size = new System.Drawing.Size(106, 25);
            this.AddProjBtn.TabIndex = 15;
            this.AddProjBtn.Text = "Add Project";
            this.AddProjBtn.UseVisualStyleBackColor = true;
            this.AddProjBtn.Click += new System.EventHandler(this.AddProjBtn_Click);
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(296, 166);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 24);
            this.button1.TabIndex = 16;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CCServerLbl
            // 
            this.CCServerLbl.AutoSize = true;
            this.CCServerLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CCServerLbl.Location = new System.Drawing.Point(4, 287);
            this.CCServerLbl.Name = "CCServerLbl";
            this.CCServerLbl.Size = new System.Drawing.Size(58, 17);
            this.CCServerLbl.TabIndex = 18;
            this.CCServerLbl.Text = "CC Srvs";
            // 
            // CCSrvCmbBx
            // 
            this.CCSrvCmbBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CCSrvCmbBx.FormattingEnabled = true;
            this.CCSrvCmbBx.Location = new System.Drawing.Point(87, 285);
            this.CCSrvCmbBx.Name = "CCSrvCmbBx";
            this.CCSrvCmbBx.Size = new System.Drawing.Size(233, 24);
            this.CCSrvCmbBx.TabIndex = 17;
            this.CCSrvCmbBx.SelectedIndexChanged += new System.EventHandler(this.CCSrvCmbBx_SelectedIndexChanged);
            // 
            // AddCCSrvTxtBx
            // 
            this.AddCCSrvTxtBx.Enabled = false;
            this.AddCCSrvTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddCCSrvTxtBx.HideSelection = false;
            this.AddCCSrvTxtBx.Location = new System.Drawing.Point(87, 285);
            this.AddCCSrvTxtBx.Name = "AddCCSrvTxtBx";
            this.AddCCSrvTxtBx.Size = new System.Drawing.Size(232, 23);
            this.AddCCSrvTxtBx.TabIndex = 19;
            this.AddCCSrvTxtBx.TabStop = false;
            this.AddCCSrvTxtBx.Visible = false;
            this.AddCCSrvTxtBx.WordWrap = false;
            // 
            // GitUrlTxtBx
            // 
            this.GitUrlTxtBx.Enabled = false;
            this.GitUrlTxtBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GitUrlTxtBx.HideSelection = false;
            this.GitUrlTxtBx.Location = new System.Drawing.Point(87, 254);
            this.GitUrlTxtBx.Name = "GitUrlTxtBx";
            this.GitUrlTxtBx.Size = new System.Drawing.Size(232, 23);
            this.GitUrlTxtBx.TabIndex = 22;
            this.GitUrlTxtBx.TabStop = false;
            this.GitUrlTxtBx.Visible = false;
            this.GitUrlTxtBx.WordWrap = false;
            this.GitUrlTxtBx.TextChanged += new System.EventHandler(this.GitUrlTxtBx_TextChanged);
            // 
            // GitUrlLbl
            // 
            this.GitUrlLbl.AutoSize = true;
            this.GitUrlLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GitUrlLbl.Location = new System.Drawing.Point(4, 256);
            this.GitUrlLbl.Name = "GitUrlLbl";
            this.GitUrlLbl.Size = new System.Drawing.Size(59, 17);
            this.GitUrlLbl.TabIndex = 21;
            this.GitUrlLbl.Text = "Git Web";
            // 
            // gitUrlCmbBx
            // 
            this.gitUrlCmbBx.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gitUrlCmbBx.FormattingEnabled = true;
            this.gitUrlCmbBx.Location = new System.Drawing.Point(87, 254);
            this.gitUrlCmbBx.Name = "gitUrlCmbBx";
            this.gitUrlCmbBx.Size = new System.Drawing.Size(233, 24);
            this.gitUrlCmbBx.TabIndex = 20;
            this.gitUrlCmbBx.SelectedIndexChanged += new System.EventHandler(this.gitUrlCmbBx_SelectedIndexChanged);
            // 
            // cc2git_Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 376);
            this.Controls.Add(this.GitUrlLbl);
            this.Controls.Add(this.gitUrlCmbBx);
            this.Controls.Add(this.CCServerLbl);
            this.Controls.Add(this.CCSrvCmbBx);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.AddProjBtn);
            this.Controls.Add(this.ProjectsLbl);
            this.Controls.Add(this.ProjectCmbBx);
            this.Controls.Add(this.EmailTxtBx);
            this.Controls.Add(this.EmailLbl);
            this.Controls.Add(this.FullNameTxtBx);
            this.Controls.Add(this.FullNameLbl);
            this.Controls.Add(this.LoginBtn);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.NewUserChkBx);
            this.Controls.Add(this.UserLoginPasswordTxtBx);
            this.Controls.Add(this.LoginUserPasswrodLbl);
            this.Controls.Add(this.LoginUserTxtBx);
            this.Controls.Add(this.LoginUserIDLbl);
            this.Controls.Add(this.LogoPicBx);
            this.Controls.Add(this.ProjectTxtBx);
            this.Controls.Add(this.AddCCSrvTxtBx);
            this.Controls.Add(this.GitUrlTxtBx);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "cc2git_Login";
            this.Text = "cc2git_Login";
            ((System.ComponentModel.ISupportInitialize)(this.LogoPicBx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox LogoPicBx;
        private System.Windows.Forms.Label LoginUserIDLbl;
        private System.Windows.Forms.TextBox LoginUserTxtBx;
        private System.Windows.Forms.TextBox UserLoginPasswordTxtBx;
        private System.Windows.Forms.Label LoginUserPasswrodLbl;
        private System.Windows.Forms.CheckBox NewUserChkBx;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Button LoginBtn;
        private System.Windows.Forms.TextBox FullNameTxtBx;
        private System.Windows.Forms.Label FullNameLbl;
        private System.Windows.Forms.TextBox EmailTxtBx;
        private System.Windows.Forms.Label EmailLbl;
        private System.Windows.Forms.ComboBox ProjectCmbBx;
        private System.Windows.Forms.Label ProjectsLbl;
        private System.Windows.Forms.TextBox ProjectTxtBx;
        private System.Windows.Forms.Button AddProjBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label CCServerLbl;
        private System.Windows.Forms.ComboBox CCSrvCmbBx;
        private System.Windows.Forms.TextBox AddCCSrvTxtBx;
        private System.Windows.Forms.TextBox GitUrlTxtBx;
        private System.Windows.Forms.Label GitUrlLbl;
        private System.Windows.Forms.ComboBox gitUrlCmbBx;
    }
}