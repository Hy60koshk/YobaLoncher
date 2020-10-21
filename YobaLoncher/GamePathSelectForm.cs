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
		public string ThePath = "";

		public GamePathSelectForm() {
			InitializeComponent();
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

		/*private void closeButton_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Abort;
		}*/
	}
}
