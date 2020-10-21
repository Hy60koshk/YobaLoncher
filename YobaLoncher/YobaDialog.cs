using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YobaLoncher {
	public partial class YobaDialog : Form {
		public YobaDialog(string message) {
			startInit();
			label1.Text = message;
			initButtons(null);
		}
		internal YobaDialog(string message, Size size) {
			startInit();
			Size = size;
			MinimumSize = size;
			MaximumSize = size;
			initButtons(null);
		}
		internal YobaDialog(string message, Size size, UIElement[] buttons) : this(message, size) {
			initButtons(buttons);
		}
		internal YobaDialog(string message, UIElement[] buttons) {
			startInit();
			label1.Text = message;
			initButtons(buttons);
		}
		private void startInit() {
			InitializeComponent();
			Icon = Program.launcherData != null ? (Program.launcherData.Icon ?? Resource1.yIcon) : Resource1.yIcon;
			MinimumSize = Size;
			MaximumSize = Size;
		}
		private void initButtons(UIElement[] buttons) {
			SuspendLayout();
			if (buttons == null) {
				YobaButton button = new YobaButton();
				button.Location = new Point((int)((double)Size.Width / 2 - (double)button.Width / 2), Size.Height - 46);
				button.Name = "okButton";
				button.Text = "OK";
				button.TabIndex = 1;
				Controls.Add(button);
				button.Click += new EventHandler(onBtnClick);
			}
			else {
				int minwidth = 38;
				int maxheight = 1;
				byte btnnum = 0;
				YobaButton[] ybuttons = new YobaButton[buttons.Length];
				foreach (UIElement btnInfo in buttons) {
					YobaButton button = new YobaButton();
					button.Name = "button" + btnnum;
					ybuttons[btnnum] = button;
					button.TabIndex = 1 + btnnum;
					if (btnInfo.Size != null) {
						button.Size = new Size(btnInfo.Size.X, btnInfo.Size.Y);
					}
					if (btnInfo.Caption != null) {
						button.Text = btnInfo.Caption;
					}
					button.DialogResult = btnInfo.Result;
					minwidth += 18 + button.Size.Width;
					maxheight = Math.Max(maxheight, button.Size.Height);
					Controls.Add(button);
					btnnum++;
					button.Click += new EventHandler(onBtnClick);
				}
				Size = MinimumSize = MaximumSize = new Size(Math.Max(minwidth, Size.Width), maxheight > 28 ? Size.Height + maxheight - 28 : Size.Height);
				if (ybuttons.Length == 1) {
					YobaButton btn = ybuttons[0];
					btn.Location = new Point((int)((double)Size.Width / 2 - (double)btn.Width / 2), Size.Height - maxheight - 18);
				}
				else {
					int space = (Size.Width - minwidth) / (btnnum - 1);
					int bottomY = Size.Height - maxheight - 18;
					int offsetX = 28;
					foreach (YobaButton btn in ybuttons) {
						btn.Location = new Point(offsetX, bottomY);
						offsetX += 18 + btn.Size.Width + space;
					}
				}
			}
			label1.Size = new Size(Size.Width - 56, Size.Height - 84);
			closeButton.Location = new Point(Size.Width - 30, -3);
			ResumeLayout(false);
		}
		private void onBtnClick(object s, EventArgs args) {
			YobaButton _this = (YobaButton)s;
			((YobaDialog)_this.Parent).DialogResult = _this.DialogResult;
		}
	}

	public class YobaButton : Button {
		public override void NotifyDefault(bool value) {
			base.NotifyDefault(false);
		}
		protected override bool ShowFocusCues {
			get {
				return false;
			}
		}
		public string Url = null;
		public DialogResult DialogResult = DialogResult.OK;

		public YobaButton() : base() {
			BackColor = Color.FromArgb(48, 48, 50);
			FlatAppearance.BorderColor = Color.Gray;
			FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 10, 11);
			FlatStyle = FlatStyle.Flat;
			Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			Margin = new Padding(0);
			Size = new Size(138, 28);
			UseVisualStyleBackColor = false;
		}

		public YobaButton(string url) : this() {
			Url = url;
		}
	}

	public class YobaCloseButton : Button {
		public override void NotifyDefault(bool value) {
			base.NotifyDefault(false);
		}
		protected override bool ShowFocusCues {
			get {
				return false;
			}
		}
		public YobaCloseButton() : base() {
			BackColor = Color.FromArgb(62, 63, 64);
			FlatAppearance.BorderColor = Color.Gray;
			FlatAppearance.MouseDownBackColor = Color.FromArgb(10, 10, 11);
			FlatStyle = FlatStyle.Flat;
			Font = new Font("Lucida Console", 16F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			Location = new Point(-2, -3);
			Margin = new Padding(0);
			Name = "closeButton";
			Size = new Size(32, 24);
			TabIndex = 100;
			Text = "X";
			UseVisualStyleBackColor = false;
			Click += new EventHandler(closeButton_Click);
		}

		private void closeButton_Click(object sender, EventArgs e) {
			((Form)Parent).DialogResult = DialogResult.Abort;
		}

		public void updateLocation() {
			Size formsize = ((Form)Parent).Size;
			Location = new Point(formsize.Width - 30, -3);
		}
	}
}
