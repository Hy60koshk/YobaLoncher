namespace YobaLoncher {
	partial class YobaDialog {
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
			this.closeButton = new YobaLoncher.YobaCloseButton();
			this.messageLabel = new System.Windows.Forms.Label();
			this.draggingPanel = new System.Windows.Forms.Panel();
			this.SuspendLayout();
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
			this.closeButton.Location = new System.Drawing.Point(381, -3);
			this.closeButton.Margin = new System.Windows.Forms.Padding(0);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(32, 24);
			this.closeButton.TabIndex = 0;
			this.closeButton.Text = "X";
			this.closeButton.UseVisualStyleBackColor = false;
			// 
			// label1
			// 
			this.messageLabel.Font = new System.Drawing.Font("Verdana", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.messageLabel.Location = new System.Drawing.Point(28, 28);
			this.messageLabel.Name = "label1";
			this.messageLabel.Size = new System.Drawing.Size(342, 92);
			this.messageLabel.TabIndex = 2;
			this.messageLabel.Text = "label1";
			// 
			// draggingPanel
			// 
			this.draggingPanel.ForeColor = System.Drawing.Color.Transparent;
			this.draggingPanel.Location = new System.Drawing.Point(1, 1);
			this.draggingPanel.Name = "draggingPanel";
			this.draggingPanel.Size = new System.Drawing.Size(378, 24);
			this.draggingPanel.TabIndex = 300;
			this.draggingPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.draggingPanel_MouseDown);
			// 
			// YobaDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(411, 173);
			this.Controls.Add(this.draggingPanel);
			this.Controls.Add(this.messageLabel);
			this.Controls.Add(this.closeButton);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "YobaDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);

		}

		#endregion

		private YobaCloseButton closeButton;
		private System.Windows.Forms.Label messageLabel;
		private System.Windows.Forms.Panel draggingPanel;
	}
}