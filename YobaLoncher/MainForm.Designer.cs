﻿namespace YobaLoncher {
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
			this.updateProgressBar = new YobaProgressBar();
			this.updateLabelText = new System.Windows.Forms.Label();
			this.draggingPanel = new System.Windows.Forms.Panel();
			this.changelogBrowser = new System.Windows.Forms.WebBrowser();
			this.statusPanel = new System.Windows.Forms.Panel();
			this.statusBrowser = new System.Windows.Forms.WebBrowser();
			this.linksPanel = new System.Windows.Forms.Panel();
			this.theToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.changelogMenuBtn = new YobaLoncher.YobaButton();
			this.statusButton = new YobaLoncher.YobaButton();
			this.linksButton = new YobaLoncher.YobaButton();
			this.settingsButton = new YobaLoncher.YobaButton();
			this.closeButton = new YobaLoncher.YobaCloseButton();
			this.launchGameButton = new YobaLoncher.YobaButton();
			this.minimizeButton = new YobaLoncher.YobaCloseButton();
			this.basePanel = new System.Windows.Forms.Panel();
			this.changelogPanel = new System.Windows.Forms.Panel();
			this.loncherIsOfflineLable = new System.Windows.Forms.Label();
			this.modsPanel = new System.Windows.Forms.Panel();
			this.modsBrowser = new System.Windows.Forms.WebBrowser();
			this.refreshButton = new YobaLoncher.YobaButton();
			this.modsButton = new YobaLoncher.YobaButton();
			this.statusPanel.SuspendLayout();
			this.basePanel.SuspendLayout();
			this.changelogPanel.SuspendLayout();
			this.modsPanel.SuspendLayout();
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
			// changelogBrowser
			// 
			this.changelogBrowser.Location = new System.Drawing.Point(36, 29);
			this.changelogBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.changelogBrowser.Name = "changelogBrowser";
			this.changelogBrowser.Size = new System.Drawing.Size(254, 147);
			this.changelogBrowser.TabIndex = 104;
			// 
			// statusPanel
			// 
			this.statusPanel.BackColor = System.Drawing.Color.Transparent;
			this.statusPanel.Controls.Add(this.statusBrowser);
			this.statusPanel.Location = new System.Drawing.Point(323, 16);
			this.statusPanel.Name = "statusPanel";
			this.statusPanel.Size = new System.Drawing.Size(263, 138);
			this.statusPanel.TabIndex = 107;
			this.statusPanel.Visible = false;
			// 
			// statusBowser
			// 
			this.statusBrowser.Location = new System.Drawing.Point(29, 29);
			this.statusBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.statusBrowser.Name = "statusBowser";
			this.statusBrowser.Size = new System.Drawing.Size(221, 67);
			this.statusBrowser.TabIndex = 0;
			// 
			// linksPanel
			// 
			this.linksPanel.BackColor = System.Drawing.Color.Transparent;
			this.linksPanel.Location = new System.Drawing.Point(22, 236);
			this.linksPanel.Name = "linksPanel";
			this.linksPanel.Size = new System.Drawing.Size(282, 72);
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
			// statusButton
			// 
			this.statusButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.statusButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.statusButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.statusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.statusButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.statusButton.ForeColor = System.Drawing.Color.White;
			this.statusButton.Location = new System.Drawing.Point(19, 67);
			this.statusButton.Margin = new System.Windows.Forms.Padding(0);
			this.statusButton.Name = "statusButton";
			this.statusButton.Size = new System.Drawing.Size(124, 28);
			this.statusButton.TabIndex = 105;
			this.statusButton.Text = "Check Results";
			this.statusButton.UseVisualStyleBackColor = false;
			this.statusButton.Click += new System.EventHandler(this.checkResultMenuBtn_Click);
			// 
			// linksButton
			// 
			this.linksButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.linksButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.linksButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.linksButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.linksButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.linksButton.ForeColor = System.Drawing.Color.White;
			this.linksButton.Location = new System.Drawing.Point(19, 109);
			this.linksButton.Margin = new System.Windows.Forms.Padding(0);
			this.linksButton.Name = "linksButton";
			this.linksButton.Size = new System.Drawing.Size(124, 28);
			this.linksButton.TabIndex = 102;
			this.linksButton.Text = "Links";
			this.linksButton.UseVisualStyleBackColor = false;
			this.linksButton.Click += new System.EventHandler(this.linksMenuBtn_Click);
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
			// launchGameButton
			// 
			this.launchGameButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.launchGameButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.launchGameButton.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.launchGameButton.ForeColor = System.Drawing.Color.White;
			this.launchGameButton.Location = new System.Drawing.Point(639, 364);
			this.launchGameButton.Margin = new System.Windows.Forms.Padding(0);
			this.launchGameButton.Name = "launchGameButton";
			this.launchGameButton.Size = new System.Drawing.Size(124, 59);
			this.launchGameButton.TabIndex = 2;
			this.launchGameButton.Text = "Launch!";
			this.launchGameButton.UseVisualStyleBackColor = true;
			this.launchGameButton.Click += new System.EventHandler(this.launchGameBtn_Click);
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
			// basePanel
			// 
			this.basePanel.Controls.Add(this.changelogPanel);
			this.basePanel.Controls.Add(this.modsPanel);
			this.basePanel.Controls.Add(this.linksPanel);
			this.basePanel.Controls.Add(this.statusPanel);
			this.basePanel.Location = new System.Drawing.Point(153, 25);
			this.basePanel.Name = "basePanel";
			this.basePanel.Size = new System.Drawing.Size(610, 330);
			this.basePanel.TabIndex = 109;
			// 
			// changelogPanel
			// 
			this.changelogPanel.BackColor = System.Drawing.Color.Transparent;
			this.changelogPanel.Controls.Add(this.loncherIsOfflineLable);
			this.changelogPanel.Controls.Add(this.changelogBrowser);
			this.changelogPanel.Location = new System.Drawing.Point(22, 16);
			this.changelogPanel.Name = "changelogPanel";
			this.changelogPanel.Size = new System.Drawing.Size(282, 199);
			this.changelogPanel.TabIndex = 110;
			this.changelogPanel.Visible = false;
			// 
			// loncherIsOfflineLable
			// 
			this.loncherIsOfflineLable.AutoSize = true;
			this.loncherIsOfflineLable.Font = new System.Drawing.Font("Verdana", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.loncherIsOfflineLable.ForeColor = System.Drawing.Color.White;
			this.loncherIsOfflineLable.Location = new System.Drawing.Point(24, 26);
			this.loncherIsOfflineLable.Name = "loncherIsOfflineLable";
			this.loncherIsOfflineLable.Size = new System.Drawing.Size(46, 16);
			this.loncherIsOfflineLable.TabIndex = 105;
			this.loncherIsOfflineLable.Text = "label1";
			// 
			// modsPanel
			// 
			this.modsPanel.BackColor = System.Drawing.Color.Transparent;
			this.modsPanel.Controls.Add(this.modsBrowser);
			this.modsPanel.Location = new System.Drawing.Point(323, 179);
			this.modsPanel.Name = "modsPanel";
			this.modsPanel.Size = new System.Drawing.Size(254, 129);
			this.modsPanel.TabIndex = 109;
			this.modsPanel.Visible = false;
			// 
			// modsBrowser
			// 
			this.modsBrowser.Location = new System.Drawing.Point(81, 46);
			this.modsBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.modsBrowser.Name = "modsBrowser";
			this.modsBrowser.Size = new System.Drawing.Size(173, 92);
			this.modsBrowser.TabIndex = 105;
			// 
			// refreshButton
			// 
			this.refreshButton.BackColor = System.Drawing.Color.Transparent;
			this.refreshButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.refreshButton.FlatAppearance.BorderSize = 0;
			this.refreshButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.refreshButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.refreshButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.refreshButton.ForeColor = System.Drawing.Color.White;
			this.refreshButton.Location = new System.Drawing.Point(112, 204);
			this.refreshButton.Margin = new System.Windows.Forms.Padding(0);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(31, 28);
			this.refreshButton.TabIndex = 110;
			this.refreshButton.UseVisualStyleBackColor = false;
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// modsButton
			// 
			this.modsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(50)))));
			this.modsButton.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
			this.modsButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
			this.modsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.modsButton.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.modsButton.ForeColor = System.Drawing.Color.White;
			this.modsButton.Location = new System.Drawing.Point(19, 151);
			this.modsButton.Margin = new System.Windows.Forms.Padding(0);
			this.modsButton.Name = "modsButton";
			this.modsButton.Size = new System.Drawing.Size(124, 28);
			this.modsButton.TabIndex = 111;
			this.modsButton.Text = "Mods";
			this.modsButton.UseVisualStyleBackColor = false;
			this.modsButton.Click += new System.EventHandler(this.modsButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(780, 440);
			this.Controls.Add(this.modsButton);
			this.Controls.Add(this.refreshButton);
			this.Controls.Add(this.changelogMenuBtn);
			this.Controls.Add(this.statusButton);
			this.Controls.Add(this.draggingPanel);
			this.Controls.Add(this.linksButton);
			this.Controls.Add(this.settingsButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.launchGameButton);
			this.Controls.Add(this.updateLabelText);
			this.Controls.Add(this.updateProgressBar);
			this.Controls.Add(this.minimizeButton);
			this.Controls.Add(this.basePanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Yoba Loncher";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.statusPanel.ResumeLayout(false);
			this.basePanel.ResumeLayout(false);
			this.changelogPanel.ResumeLayout(false);
			this.changelogPanel.PerformLayout();
			this.modsPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private YobaProgressBar updateProgressBar;
		private System.Windows.Forms.Label updateLabelText;
		private YobaButton launchGameButton;
		private YobaCloseButton closeButton;
		private YobaCloseButton minimizeButton;
		private YobaButton settingsButton;
		private YobaButton linksButton;
		private System.Windows.Forms.Panel draggingPanel;
		private System.Windows.Forms.WebBrowser changelogBrowser;
		private YobaButton statusButton;
		private YobaButton changelogMenuBtn;
		private System.Windows.Forms.Panel statusPanel;
		private System.Windows.Forms.Panel linksPanel;
		private System.Windows.Forms.ToolTip theToolTip;
		private System.Windows.Forms.Panel basePanel;
		private System.Windows.Forms.Panel changelogPanel;
		private System.Windows.Forms.Panel modsPanel;
		private YobaButton refreshButton;
		private System.Windows.Forms.WebBrowser statusBrowser;
		private System.Windows.Forms.Label loncherIsOfflineLable;
		private YobaButton modsButton;
		private System.Windows.Forms.WebBrowser modsBrowser;
	}
}