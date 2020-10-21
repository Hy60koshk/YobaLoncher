namespace YobaLoncher {
	partial class GamePathSelectForm {
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new YobaButton();
			this.button2 = new YobaButton();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.closeButton = new YobaCloseButton();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(41)))));
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.textBox1.ForeColor = System.Drawing.Color.White;
			this.textBox1.HideSelection = false;
			this.textBox1.Location = new System.Drawing.Point(34, 69);
			this.textBox1.MaxLength = 1000;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(356, 27);
			this.textBox1.TabIndex = 0;
			this.textBox1.WordWrap = false;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(393, 69);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 27);
			this.button1.TabIndex = 1;
			this.button1.Text = "Browse...";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.button2.Location = new System.Drawing.Point(184, 116);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(130, 30);
			this.button2.TabIndex = 2;
			this.button2.Text = "Proceed";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(43, 42);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(162, 14);
			this.label1.TabIndex = 4;
			this.label1.Text = "Enter the path to the game";
			// 
			// GamePathSelectForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(500, 170);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.ForeColor = System.Drawing.Color.White;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(500, 170);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 170);
			this.Name = "GamePathSelectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select the game folder";
			this.Shown += new System.EventHandler(this.GamePathSelectForm_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private YobaButton button1;
		private YobaButton button2;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private YobaCloseButton closeButton;
		private System.Windows.Forms.Label label1;
	}
}