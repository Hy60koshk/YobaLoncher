using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YobaLoncher {
	class SettingsDialog : YobaDialog {
		private TextBox gamePath;

		public string GamePath => gamePath.Text;

		public SettingsDialog() : base(new Size(440, 280), new UIElement[] {
			new UIElement() {
				Caption = Locale.Get("Cancel")
				, Result = DialogResult.Cancel
			}
			, new UIElement() {
				Caption = Locale.Get("Apply")
				, Result = DialogResult.OK
			}
		}) {
			Text = Locale.Get("SettingsTitle");
			SuspendLayout();
			Label gamePathLabel = new Label();
			gamePathLabel.Text = Locale.Get("SettingsGamePath");
			gamePathLabel.Font = new Font("Tahoma", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			gamePathLabel.Location = new Point(20, 30);
			gamePathLabel.AutoSize = true;
			gamePath = new TextBox();
			gamePath.Text = Program.GamePath;
			gamePath.Font = new Font("Consolas", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			gamePath.Location = new Point(20, 52);
			gamePath.Size = new Size(400, 20);
			gamePath.BackColor = Color.FromArgb(32, 33, 34);
			gamePath.BorderStyle = BorderStyle.FixedSingle;
			gamePath.ForeColor = Color.White;
			Controls.Add(gamePathLabel);
			Controls.Add(gamePath);
			ResumeLayout();
		}
	}
}
