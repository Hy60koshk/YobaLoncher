using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace YobaLoncher {
	public partial class GamePathSelectForm : Form {
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		public string ThePath = "";
		private CommonOpenFileDialog folderBrowserDialog;

		public GamePathSelectForm() {
			InitializeComponent();

			folderBrowserDialog = new CommonOpenFileDialog() {
				IsFolderPicker = true
			};

			int style = NativeWinAPI.GetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE);
			style |= NativeWinAPI.WS_EX_COMPOSITED;
			NativeWinAPI.SetWindowLong(this.Handle, NativeWinAPI.GWL_EXSTYLE, style);

			YU.assertLucida(textBox1);

			button1.Text = Locale.Get("Browse");
			button2.Text = Locale.Get("Proceed");
			label1.Text = Locale.Get("EnterThePath");
			Text = Locale.Get("GamePathSelectionTitle");

			closeButton.UpdateLocation();
		}

		private void button1_Click(object sender, EventArgs e) {
			folderBrowserDialog.InitialDirectory = textBox1.Text;
			if (folderBrowserDialog.ShowDialog() == CommonFileDialogResult.Ok) {
				textBox1.Text = folderBrowserDialog.FileName;
			}
		}

		private void GamePathSelectForm_Shown(object sender, EventArgs e) {
			textBox1.Text = ThePath;
		}

		private void button2_Click(object sender, EventArgs e) {
			ThePath = textBox1.Text;
			DialogResult = DialogResult.Yes;
		}

		private void draggingPanel_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}
	}
}
