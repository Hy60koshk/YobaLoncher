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
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		public static DialogResult ShowDialog(string msg) {
			return new YobaDialog(msg).ShowDialog();
		}
		internal static DialogResult ShowDialog(string msg, UIElement[] buttons) {
			return new YobaDialog(msg, buttons).ShowDialog();
		}

		internal static UIElement[] YesNoBtns = new UIElement[] {
			new UIElement() {
				Caption = Locale.Get("No"), Result = DialogResult.No
			}, new UIElement() {
				Caption = Locale.Get("Yes"), Result = DialogResult.Yes
			}
		};

		public YobaDialog() : this("", null) { }
		public YobaDialog(string message) : this(message, null) { }
		internal YobaDialog(string message, UIElement[] buttons) {
			startInit();
			label1.Text = message;
			initButtons(buttons);
		}
		internal YobaDialog(Size size) : this("", size, null) { }
		internal YobaDialog(string message, Size size) : this(message, size, null) { }
		internal YobaDialog(Size size, UIElement[] buttons) : this("", size, buttons) { }
		internal YobaDialog(string message, Size size, UIElement[] buttons) {
			startInit();
			label1.Text = message;
			Size = size;
			MinimumSize = size;
			MaximumSize = size;
			initButtons(buttons);
		}

		private void startInit() {
			InitializeComponent();
			Icon = Program.LoncherSettings != null ? (Program.LoncherSettings.Icon ?? Resource1.yIcon) : Resource1.yIcon;
			MinimumSize = Size;
			MaximumSize = Size;
		}

		private void initButtons(UIElement[] buttons) {
			SuspendLayout();
			if (buttons == null) {
				YobaButton button = new YobaButton();
				button.Location = new Point((int)((double)Size.Width / 2 - (double)button.Width / 2), Size.Height - 46);
				button.Name = "okButton";
				button.Text = Locale.Get("OK");
				button.TabIndex = 1;
				button.DialogResult = DialogResult.OK;
				Controls.Add(button);
				//button.Click += new EventHandler(onBtnClick);
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
			if (label1.Text.Length > 0) {
				label1.Size = new Size(Size.Width - 56, Size.Height - 84);
			}
			else {
				Controls.Remove(label1);
			}
			closeButton.UpdateLocation();//Location = new Point(Size.Width - 30, -3);
			draggingPanel.Size = new Size(Size.Width - 32, 24);
			ResumeLayout(false);
		}
		private void onBtnClick(object s, EventArgs args) {
			YobaButton _this = (YobaButton)s;
			((YobaDialog)_this.Parent).DialogResult = _this.DialogResult;
		}

		private void draggingPanel_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}
	}

	internal abstract class YobaButtonAbs : Button {
		protected static class BtnColors {
			public static Color Fore = Color.White;
			public static Color Back = Color.FromArgb(48, 48, 50);
			public static Color MouseDown = Color.FromArgb(10, 10, 11);
			public static Color MouseOver = Color.FromArgb(62, 63, 64);
			public static Color Border = Color.Gray;
		}

		public override void NotifyDefault(bool value) {
			base.NotifyDefault(false);
		}
		protected override bool ShowFocusCues {
			get {
				return false;
			}
		}
		public YobaButtonAbs() : base() {
			BackColor = BtnColors.Back;
			ForeColor = BtnColors.Fore;
			FlatAppearance.BorderColor = BtnColors.Border;
			FlatAppearance.MouseDownBackColor = BtnColors.MouseDown;
			FlatAppearance.MouseOverBackColor = BtnColors.MouseOver;
			FlatStyle = FlatStyle.Flat;
			Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			Margin = new Padding(0);
			Size = new Size(138, 28);
			UseVisualStyleBackColor = false;
		}

		public virtual void ApplyUIStyles(UIElement styleInfo) {
			UseVisualStyleBackColor = true;
			if (styleInfo.Position != null) {
				Location = new Point(styleInfo.Position.X, styleInfo.Position.Y);
			}
			if (styleInfo.Size != null) {
				Size = new Size(styleInfo.Size.X, styleInfo.Size.Y);
			}
			if (styleInfo.Caption != null) {
				Text = styleInfo.Caption;
			}
			if (styleInfo.CustomStyle) {
				FlatStyle = FlatStyle.Flat;
				ForeColor = YU.colorFromString(styleInfo.Color, BtnColors.Fore);
				FlatAppearance.BorderColor = YU.colorFromString(styleInfo.BorderColor, BtnColors.Border);
			}
			if (styleInfo.BgImage != null && YU.stringHasText(styleInfo.BgImage.Path)) {
				if (YU.stringHasText(styleInfo.BgImage.Layout)) {
					try {
						Enum.Parse(typeof(ImageLayout), styleInfo.BgImage.Layout);
					}
					catch {
						BackgroundImageLayout = ImageLayout.Stretch;
					}
				}
				else {
					BackgroundImageLayout = ImageLayout.Stretch;
				}
				BackgroundImage = new Bitmap(PreloaderForm.IMGPATH + styleInfo.BgImage.Path);
			}
		}
	}

	internal class YobaButton : YobaButtonAbs {
		public string Url = null;

		public YobaButton() : base() { }

		public YobaButton(string url) : this() {
			Url = url;
		}

		public override void ApplyUIStyles(UIElement styleInfo) {
			base.ApplyUIStyles(styleInfo);

			YU.setFont(this, styleInfo.Font, styleInfo.FontSize);
			if (styleInfo.CustomStyle) {
				BackColor = YU.colorFromString(styleInfo.BgColor, BtnColors.Back);
				FlatAppearance.BorderSize = styleInfo.BorderSize > -1 ? styleInfo.BorderSize : 0;
				FlatAppearance.MouseOverBackColor = YU.colorFromString(styleInfo.BgColorHover, BtnColors.MouseOver);
				FlatAppearance.MouseDownBackColor = YU.colorFromString(styleInfo.BgColorDown, BtnColors.MouseDown);
			}
		}
	}

	internal class YobaCloseButton : YobaButtonAbs {
		public YobaCloseButton() : base() {
			BackColor = Color.FromArgb(62, 63, 64);
			FlatAppearance.MouseOverBackColor = BtnColors.Back;
			Font = new Font("Lucida Console", 16F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			Location = new Point(-2, -3);
			Name = "closeButton";
			Size = new Size(32, 24);
			TabIndex = 100;
			Text = "X";

			DialogResult = DialogResult.Abort;
		}

		public void UpdateLocation() {
			Size formsize = ((Form)Parent).Size;
			Location = new Point(formsize.Width - 30, -3);
		}

		public override void ApplyUIStyles(UIElement styleInfo) {
			base.ApplyUIStyles(styleInfo);

			if (YU.stringHasText(styleInfo.FontSize)) {
				YU.setFont(this, styleInfo.Font, styleInfo.FontSize);
			}
			if (styleInfo.CustomStyle) {
				BackColor = YU.colorFromString(styleInfo.BgColor, Color.FromArgb(62, 63, 64));
				FlatAppearance.BorderSize = styleInfo.BorderSize > -1 ? styleInfo.BorderSize : 1;
				FlatAppearance.MouseOverBackColor = YU.colorFromString(styleInfo.BgColorHover, BtnColors.Back);
				FlatAppearance.MouseDownBackColor = YU.colorFromString(styleInfo.BgColorDown, BtnColors.MouseDown);
			}
		}
	}
}