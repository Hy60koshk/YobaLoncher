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

		public SettingsDialog() : base(new Size(500, 480), new UIElement[] {
			new UIElement() {
				Caption = Locale.Get("Cancel")
			}
			, new UIElement() {
				Caption = Locale.Get("Apply")
				, Result = DialogResult.OK
			}
		}) {
			Text = Locale.Get("SettingsTitle");
			SuspendLayout();
			gamePath = new TextBox();
			gamePath.Text = Program.GamePath;
			gamePath.Location = new Point(20, 200);
			gamePath.Size = new Size(400, 20);
			Controls.Add(gamePath);
			ResumeLayout();
		}
	}
}
