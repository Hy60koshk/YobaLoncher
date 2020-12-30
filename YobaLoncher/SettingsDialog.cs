using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace YobaLoncher {
	class SettingsDialog : YobaDialog {
		private TextBox gamePath;
		private FolderBrowserDialog folderBrowserDialog1;
		private YobaComboBox openingPanelCB;
		private CheckBox launchViaGalaxy;
		private CheckBox offlineMode;
		private CheckBox closeLauncherOnLaunch;
		//private YobaButton openingPanelCB;

		public string GamePath => gamePath.Text;
		public StartPageEnum OpeningPanel => (StartPageEnum)openingPanelCB.SelectedIndex;
		public bool LaunchViaGalaxy => launchViaGalaxy.Checked;
		public bool OfflineMode => offlineMode.Checked;
		public bool CloseOnLaunch => closeLauncherOnLaunch.Checked;
		private CommonOpenFileDialog folderBrowserDialog;

		public SettingsDialog() : base(new Size(480, 313), new UIElement[] {
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

			folderBrowserDialog1 = new FolderBrowserDialog();

			folderBrowserDialog = new CommonOpenFileDialog() {
				IsFolderPicker = true
			};

			ToolTip theToolTip = new ToolTip();
			theToolTip.AutoPopDelay = 10000;
			theToolTip.InitialDelay = 200;
			theToolTip.ReshowDelay = 100;

			Label gamePathLabel = new Label();
			gamePathLabel.Text = Locale.Get("SettingsGamePath");
			gamePathLabel.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			gamePathLabel.Location = new Point(18, 22);
			gamePathLabel.Size = new Size(444, 40);

			gamePath = new TextBox();
			gamePath.Text = Program.GamePath;
			gamePath.Font = new Font("Lucida Sans Unicode", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			gamePath.Location = new Point(2, 4);
			gamePath.Size = new Size(359, 20);
			gamePath.BackColor = Color.FromArgb(32, 33, 34);
			gamePath.BorderStyle = BorderStyle.None;
			gamePath.ForeColor = Color.White;
			YU.assertLucida(gamePath);

			YobaButton browseButton = new YobaButton();
			browseButton.Location = new Point(385, 44);
			browseButton.Name = "browseButton";
			browseButton.Size = new Size(75, 27);
			browseButton.Text = Locale.Get("Browse");
			browseButton.UseVisualStyleBackColor = false;
			browseButton.Click += new System.EventHandler(browseButton_Click);

			Panel fieldBackground = new Panel();
			fieldBackground.BackColor = Color.FromArgb(32, 33, 34);
			fieldBackground.BorderStyle = BorderStyle.FixedSingle;
			fieldBackground.Controls.Add(gamePath);
			fieldBackground.Location = new Point(20, 44);
			fieldBackground.Name = "fieldBackground";
			fieldBackground.Size = new Size(361, 27);

			launchViaGalaxy = new CheckBox();
			launchViaGalaxy.Text = Locale.Get("SettingsGogGalaxy");
			launchViaGalaxy.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			launchViaGalaxy.Location = new Point(20, 86);
			launchViaGalaxy.Size = new Size(440, 24);
			launchViaGalaxy.Checked = LauncherConfig.LaunchFromGalaxy;
			launchViaGalaxy.BackColor = Color.Transparent;
			launchViaGalaxy.Enabled = LauncherConfig.GalaxyDir != null;
			
			offlineMode = new CheckBox();
			offlineMode.Text = Locale.Get("SettingsOfflineMode");
			offlineMode.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			offlineMode.Location = new Point(20, 119);
			offlineMode.Size = new Size(440, 24);
			offlineMode.Checked = LauncherConfig.StartOffline;
			offlineMode.BackColor = Color.Transparent;

			theToolTip.SetToolTip(offlineMode, Locale.Get("SettingsOfflineModeTooltip"));

			closeLauncherOnLaunch = new CheckBox();
			closeLauncherOnLaunch.Text = Locale.Get("SettingsCloseOnLaunch");
			closeLauncherOnLaunch.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			closeLauncherOnLaunch.Location = new Point(20, 152);
			closeLauncherOnLaunch.Size = new Size(440, 24);
			closeLauncherOnLaunch.Checked = LauncherConfig.CloseOnLaunch;
			closeLauncherOnLaunch.BackColor = Color.Transparent;

			Label openingPanelLabel = new Label();
			openingPanelLabel.Text = Locale.Get("SettingsOpeningPanel");
			openingPanelLabel.Font = new Font("Tahoma", 12F, FontStyle.Regular, GraphicsUnit.Pixel, 204);
			openingPanelLabel.Location = new Point(18, 187);
			openingPanelLabel.Size = new Size(444, 40);

			openingPanelCB = new YobaComboBox();
			openingPanelCB.Location = new Point(20, 208);
			openingPanelCB.Name = "openingPanel";
			openingPanelCB.Size = new Size(440, 22);
			openingPanelCB.DataSource = new string[] {
				Locale.Get("SettingsOpeningPanelChangelog")
				, Locale.Get("SettingsOpeningPanelStatus")
				, Locale.Get("SettingsOpeningPanelLinks")
				//, Locale.Get("SettingsOpeningPanelMods")
			};
			/*openingPanelCB = new YobaButton();
			openingPanelCB.Location = new Point(20, 141);
			openingPanelCB.Name = "openingPanel";

			YobaButton opt1 = new YobaComboBox();
			opt1.Location = new Point(20, 141);
			Size = new Size(440, 28);

			Panel cbDD = new Panel();
			cbDD.BackColor = Color.FromArgb(40, 40, 41);
			cbDD.BorderStyle = BorderStyle.FixedSingle;
			cbDD.Controls.Add(gamePath);
			cbDD.Location = new Point(20, 141 + openingPanelCB.Height);
			cbDD.Name = "cbDD";
			cbDD.Size = new Size(361, openingPanelCB.Height * 3);
			*/

			gamePath.TabIndex = 1;
			browseButton.TabIndex = 2;
			launchViaGalaxy.TabIndex = 3;
			offlineMode.TabIndex = 4;
			closeLauncherOnLaunch.TabIndex = 5;
			openingPanelCB.TabIndex = 10;
			
			Controls.Add(fieldBackground);
			Controls.Add(browseButton);
			Controls.Add(launchViaGalaxy);
			Controls.Add(offlineMode);
			Controls.Add(closeLauncherOnLaunch);
			Controls.Add(openingPanelCB);

			Controls.Add(openingPanelLabel);
			Controls.Add(gamePathLabel);

			Load += new EventHandler((object o, EventArgs a) => {
				openingPanelCB.SelectedIndex = (int)LauncherConfig.StartPage;
			});
			ResumeLayout();
		}

		private void browseButton_Click(object sender, EventArgs e) {
			folderBrowserDialog.InitialDirectory = gamePath.Text;
			if (folderBrowserDialog.ShowDialog() == CommonFileDialogResult.Ok) {
				gamePath.Text = folderBrowserDialog.FileName;
			}
		}

		private void openingPanelCB_DrawItem(object sender, DrawItemEventArgs e) {
			int index = e.Index >= 0 ? e.Index : 0;
			using (SolidBrush brush = new SolidBrush(openingPanelCB.BackColor)) {
				e.DrawBackground();
				e.Graphics.DrawString(openingPanelCB.Items[index].ToString(), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
				e.DrawFocusRectangle();
			}
		}
	}
}
