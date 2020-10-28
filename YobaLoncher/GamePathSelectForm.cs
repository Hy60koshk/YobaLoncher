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
	public partial class GamePathSelectForm : Form {
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		public string ThePath = "";

		public GamePathSelectForm() {
			InitializeComponent();
			button1.Text = Locale.Get("Browse");
			button2.Text = Locale.Get("Proceed");
			label1.Text = Locale.Get("EnterThePath");
			Text = Locale.Get("GamePathSelectionTitle");
			closeButton.UpdateLocation();
		}

		private void button1_Click(object sender, EventArgs e) {
			folderBrowserDialog1.SelectedPath = textBox1.Text;
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
				textBox1.Text = folderBrowserDialog1.SelectedPath;
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
