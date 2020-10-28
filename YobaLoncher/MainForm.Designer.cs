namespace YobaLoncher {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.updateProgressBar = new System.Windows.Forms.ProgressBar();
			this.updateLabelText = new System.Windows.Forms.Label();
			this.draggingPanel = new System.Windows.Forms.Panel();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.statusPanel = new System.Windows.Forms.Panel();
			this.linksPanel = new System.Windows.Forms.Panel();
			this.theToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.changelogMenuBtn = new YobaLoncher.YobaButton();
			this.checkResultMenuBtn = new YobaLoncher.YobaButton();
			this.linksMenuBtn = new YobaLoncher.YobaButton();
			this.settingsButton = new YobaLoncher.YobaButton();
			this.closeButton = new YobaLoncher.YobaCloseButton();
			this.launchGameBtn = new YobaLoncher.YobaButton();
			this.minimizeButton = new YobaLoncher.YobaCloseButton();
			this.SuspendLayout();
			// 
			// updateProgressBar
			// 
			this.updateProgressBar.Location = new System.Drawing.Point(19, 399);
			this.updateProgressBar.Name = "updateProgressBar";
			this.updateProgressBar.Size = new System.Drawing.Size(602, 23);
			this.updateProgressBar.TabIndex = 0;
			// 
			// updateLabelText
			// 
			this.updateLabelText.AutoSize = true;
			this.updateLabelText.BackColor = System.Drawing.Color.Transparent;
			this.updateLabelText.ForeColor = System.Drawing.Color.White;
			this.updateLabelText.Location = new System.Drawing.Point(20, 375);
			this.updateLabelText.Name = "updateLabelText";
			this.updateLabelText.Size = new System.Drawing.Size(87, 13);
			this.updateLabelText.TabIndex = 1;
			this.updateLabelText.Text = "updateLabelText";
			// 
			// draggingPanel
			// 
			this.draggingPanel.BackColor = System.Drawing.Color.Transparent;
			this.draggingPanel.Location = new System.Drawing.Point(0, 0);
			this.draggingPanel.Margin = new System.Windows.Forms.Padding(0);
			this.draggingPanel.Name = "draggingPanel";
			this.draggingPanel.Size = new System.Drawing.Size(715, 24);
			this.draggingPanel.TabIndex = 103;
			this.draggingPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.draggingPanel_MouseDown);
			// 
			// webBrowser1
			// 
			this.webBrowser1.Location = new System.Drawing.Point(153, 25);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(610, 330);
			this.webBrowser1.TabIndex = 104;
			// 
			// statusPanel
			// 
			this.statusPanel.Location = new System.Drawing.Point(120, 165);
			this.statusPanel.Name = "statusPanel";
			this.statusPanel.Size = new System.Drawing.Size(345, 138);
			this.statusPanel.TabIndex = 107;
			this.statusPanel.Visible = false;
			// 
			// linksPanel
			// 
			this.linksPanel.Location = new System.Drawing.Point(23, 200);
			this.linksPanel.Name = "linksPanel";
			this.linksPanel.Size = new System.Drawing.Size(200, 100);
			this.linksPanel.TabIndex = 108;
			this.linksPanel.Visible = false;
			// 
			// theToolTip
			// 
			this.theToolTip.AutoPopDelay = 0;
			this.theToolTip.InitialDelay = 200;
			this.theToolTip.ReshowDelay = 100;
			// 
			// changelogMenuBtn
			// 
			this.changelogMenuBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.changelogMenuBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.changelogMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.changelogMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.changelogMenuBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.changelogMenuBtn.ForeColor = System.Drawing.Color.White;
			this.changelogMenuBtn.Location = new System.Drawing.Point(19, 25);
			this.changelogMenuBtn.Margin = new System.Windows.Forms.Padding(0);
			this.changelogMenuBtn.Name = "changelogMenuBtn";
			this.changelogMenuBtn.Size = new System.Drawing.Size(124, 28);
			this.changelogMenuBtn.TabIndex = 106;
			this.changelogMenuBtn.Text = "Changelog";
			this.changelogMenuBtn.UseVisualStyleBackColor = false;
			this.changelogMenuBtn.Click += new System.EventHandler(this.changelogMenuBtn_Click);
			// 
			// checkResultMenuBtn
			// 
			this.checkResultMenuBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.checkResultMenuBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.checkResultMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.checkResultMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.checkResultMenuBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.checkResultMenuBtn.ForeColor = System.Drawing.Color.White;
			this.checkResultMenuBtn.Location = new System.Drawing.Point(19, 67);
			this.checkResultMenuBtn.Margin = new System.Windows.Forms.Padding(0);
			this.checkResultMenuBtn.Name = "checkResultMenuBtn";
			this.checkResultMenuBtn.Size = new System.Drawing.Size(124, 28);
			this.checkResultMenuBtn.TabIndex = 105;
			this.checkResultMenuBtn.Text = "Check Results";
			this.checkResultMenuBtn.UseVisualStyleBackColor = false;
			this.checkResultMenuBtn.Click += new System.EventHandler(this.checkResultMenuBtn_Click);
			// 
			// linksMenuBtn
			// 
			this.linksMenuBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.linksMenuBtn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.linksMenuBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.linksMenuBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.linksMenuBtn.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.linksMenuBtn.ForeColor = System.Drawing.Color.White;
			this.linksMenuBtn.Location = new System.Drawing.Point(19, 109);
			this.linksMenuBtn.Margin = new System.Windows.Forms.Padding(0);
			this.linksMenuBtn.Name = "linksMenuBtn";
			this.linksMenuBtn.Size = new System.Drawing.Size(124, 28);
			this.linksMenuBtn.TabIndex = 102;
			this.linksMenuBtn.Text = "Links";
			this.linksMenuBtn.UseVisualStyleBackColor = false;
			this.linksMenuBtn.Click += new System.EventHandler(this.linksMenuBtn_Click);
			// 
			// settingsButton
			// 
			this.settingsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.settingsButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.settingsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.settingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.settingsButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.settingsButton.ForeColor = System.Drawing.Color.White;
			this.settingsButton.Location = new System.Drawing.Point(19, 329);
			this.settingsButton.Margin = new System.Windows.Forms.Padding(0);
			this.settingsButton.Name = "settingsButton";
			this.settingsButton.Size = new System.Drawing.Size(124, 26);
			this.settingsButton.TabIndex = 101;
			this.settingsButton.Text = "Settings";
			this.settingsButton.UseVisualStyleBackColor = false;
			this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.closeButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.closeButton.Font = new System.Drawing.Font("Lucida Console", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.closeButton.ForeColor = System.Drawing.Color.White;
			this.closeButton.Location = new System.Drawing.Point(750, -2);
			this.closeButton.Margin = new System.Windows.Forms.Padding(0);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(32, 24);
			this.closeButton.TabIndex = 100;
			this.closeButton.Text = "X";
			this.closeButton.UseVisualStyleBackColor = false;
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// launchGameBtn
			// 
			this.launchGameBtn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.launchGameBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.launchGameBtn.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.launchGameBtn.ForeColor = System.Drawing.Color.White;
			this.launchGameBtn.Location = new System.Drawing.Point(639, 364);
			this.launchGameBtn.Margin = new System.Windows.Forms.Padding(0);
			this.launchGameBtn.Name = "launchGameBtn";
			this.launchGameBtn.Size = new System.Drawing.Size(124, 59);
			this.launchGameBtn.TabIndex = 2;
			this.launchGameBtn.Text = "Launch!";
			this.launchGameBtn.UseVisualStyleBackColor = true;
			this.launchGameBtn.Click += new System.EventHandler(this.launchGameBtn_Click);
			// 
			// minimizeButton
			// 
			this.minimizeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.minimizeButton.DialogResult = System.Windows.Forms.DialogResult.Abort;
			this.minimizeButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.minimizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.minimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.minimizeButton.Font = new System.Drawing.Font("Lucida Console", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.minimizeButton.ForeColor = System.Drawing.Color.White;
			this.minimizeButton.Location = new System.Drawing.Point(720, -2);
			this.minimizeButton.Margin = new System.Windows.Forms.Padding(0);
			this.minimizeButton.Name = "minimizeButton";
			this.minimizeButton.Size = new System.Drawing.Size(32, 24);
			this.minimizeButton.TabIndex = 100;
			this.minimizeButton.Text = "_";
			this.minimizeButton.UseVisualStyleBackColor = false;
			this.minimizeButton.Click += new System.EventHandler(this.minimizeButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(780, 440);
			this.Controls.Add(this.statusPanel);
			this.Controls.Add(this.linksPanel);
			this.Controls.Add(this.changelogMenuBtn);
			this.Controls.Add(this.checkResultMenuBtn);
			this.Controls.Add(this.webBrowser1);
			this.Controls.Add(this.draggingPanel);
			this.Controls.Add(this.linksMenuBtn);
			this.Controls.Add(this.settingsButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.launchGameBtn);
			this.Controls.Add(this.updateLabelText);
			this.Controls.Add(this.updateProgressBar);
			this.Controls.Add(this.minimizeButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Yoba Loncher";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ProgressBar updateProgressBar;
		private System.Windows.Forms.Label updateLabelText;
		private YobaButton launchGameBtn;
		private YobaCloseButton closeButton;
		private YobaCloseButton minimizeButton;
		private YobaButton settingsButton;
		private YobaButton linksMenuBtn;
		private System.Windows.Forms.Panel draggingPanel;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private YobaButton checkResultMenuBtn;
		private YobaButton changelogMenuBtn;
		private System.Windows.Forms.Panel statusPanel;
		private System.Windows.Forms.Panel linksPanel;
		private System.Windows.Forms.ToolTip theToolTip;
	}
}