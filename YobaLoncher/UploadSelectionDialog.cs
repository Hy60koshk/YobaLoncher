using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace YobaLoncher {
	class UploadSelectionDialog : YobaDialog {
		private CheckBox[] checkBoxes;

		//public string GamePath => gamePath.Text;

		public UploadSelectionDialog() : base(new Size(500, 480), new UIElement[] {
			new UIElement() {
				Caption = "Cancel"
			}
			, new UIElement() {
				Caption = "Apply"
				, Result = DialogResult.OK
			}
		}) {
			SuspendLayout();
			/*gamePath = new TextBox();
			gamePath.Text = Program.GamePath;
			gamePath.Location = new Point(20, 200);
			gamePath.Size = new Size(400, 20);
			Controls.Add(gamePath);*/
			ResumeLayout();
		}
	}
}
