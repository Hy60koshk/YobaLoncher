using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using IWshRuntimeLibrary;
using IOFile = System.IO.File;
using System.Runtime.InteropServices;
using System.IO;

namespace YobaLoncher {
	class SettingsDialog : YobaDialog {
		private readonly MainForm mainForm_;
		private TextBox gamePath;
		private YobaComboBox openingPanelCB;
		private CheckBox launchViaGalaxy;
		private CheckBox offlineMode;
		private CheckBox closeLauncherOnLaunch;
		private CommonOpenFileDialog folderBrowserDialog;
		//private YobaButton openingPanelCB;

		public string GamePath => gamePath.Text;
		public StartPageEnum OpeningPanel => (StartPageEnum)openingPanelCB.SelectedIndex;
		public bool LaunchViaGalaxy => launchViaGalaxy.Checked;
		public bool OfflineMode => offlineMode.Checked;
		public bool CloseOnLaunch => closeLauncherOnLaunch.Checked;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		extern static bool DestroyIcon(IntPtr handle);

		public SettingsDialog(MainForm mainForm) : base(new Size(480, 460), new UIElement[] {
			new UIElement() {
				Caption = Locale.Get("Cancel")
				, Result = DialogResult.Cancel
			}
			, new UIElement() {
				Caption = Locale.Get("Apply")
				, Result = DialogResult.OK
			}
		}) {
			mainForm_ = mainForm;
			Text = Locale.Get("SettingsTitle");

			SuspendLayout();

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
			YobaButton makeBackupBtn = new YobaButton();
			makeBackupBtn.MouseClick += MakeBackupBtn_MouseClick;
			makeBackupBtn.Location = new Point(20, 246);
			makeBackupBtn.Size = new Size(240, 24);
			makeBackupBtn.Text = Locale.Get("SettingsMakeBackup");

			YobaButton createShortcutBtn = new YobaButton();
			createShortcutBtn.MouseClick += CreateShortcutBtn_MouseClick;
			createShortcutBtn.Location = new Point(20, 286);
			createShortcutBtn.Size = new Size(240, 24);
			createShortcutBtn.Text = Locale.Get("SettingsCreateShortcut");

			/*YobaButton uninstallRussifierBtn = new YobaButton();
			createShortcutBtn.MouseClick += CreateShortcutBtn_MouseClick;
			createShortcutBtn.Location = new Point(20, 366);
			createShortcutBtn.Size = new Size(240, 24);
			createShortcutBtn.Text = Locale.Get("SettingsUninstallMainProduct");

			YobaButton uninstallLoncherBtn = new YobaButton();
			createShortcutBtn.MouseClick += CreateShortcutBtn_MouseClick;
			createShortcutBtn.Location = new Point(20, 406);
			createShortcutBtn.Size = new Size(240, 24);
			createShortcutBtn.Text = Locale.Get("SettingsUninstallRussifier");*/

			gamePath.TabIndex = 1;
			browseButton.TabIndex = 2;
			launchViaGalaxy.TabIndex = 3;
			offlineMode.TabIndex = 4;
			closeLauncherOnLaunch.TabIndex = 5;
			
			openingPanelCB.TabIndex = 10;
			makeBackupBtn.TabIndex = 15;
			createShortcutBtn.TabIndex = 16;

			Controls.Add(fieldBackground);
			Controls.Add(browseButton);
			Controls.Add(launchViaGalaxy);
			Controls.Add(offlineMode);
			Controls.Add(closeLauncherOnLaunch);
			Controls.Add(openingPanelCB);
			Controls.Add(makeBackupBtn);
			Controls.Add(createShortcutBtn);

			Controls.Add(openingPanelLabel);
			Controls.Add(gamePathLabel);

			Load += new EventHandler((object o, EventArgs a) => {
				openingPanelCB.SelectedIndex = (int)LauncherConfig.StartPage;
			});
			ResumeLayout();
		}

		private void CreateShortcutBtn_MouseClick(object sender, MouseEventArgs e) {
			try {
				string filename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Запускатр Боевых Братьев.lnk";
				if (!IOFile.Exists(filename)) {
					WshShell wsh = new WshShell();
					IWshShortcut shortcut = wsh.CreateShortcut(filename) as IWshShortcut;
					shortcut.Arguments = "";
					shortcut.TargetPath = Application.ExecutablePath;
					shortcut.WorkingDirectory = Program.LoncherPath;
					shortcut.WindowStyle = 1;
					string iconFile = Program.LoncherDataPath + "shortcutIcon.ico";
					bool validIconFile = IOFile.Exists(iconFile);
					if (!validIconFile) {
						string exename = Program.GamePath + Program.LoncherSettings.ExeName;

						if (IOFile.Exists(PreloaderForm.ICON_FILE)) {
							PngIconConverter.Convert(PreloaderForm.ICON_FILE, iconFile);
							validIconFile = true;
						}
						else if (IOFile.Exists(exename) && exename.EndsWith(".exe")) {
							Icon exeIcon = Icon.ExtractAssociatedIcon(exename);
							if (exeIcon != null) {
								Bitmap exeBmp = exeIcon.ToBitmap();
								PngIconConverter.Convert(exeBmp, iconFile);
								validIconFile = true;
							}
						}
					}
					if (validIconFile) {
						shortcut.IconLocation = iconFile;
					}
					shortcut.Save();
				}
			} catch (Exception ex) {
				YobaDialog.ShowDialog(ex.Message);
			}
		}

		private void MakeBackupBtn_MouseClick(object sender, MouseEventArgs e) {
			string bkpdir = Program.GamePath + "_loncher_backups\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "\\";
			if (DialogResult.Yes == YobaDialog.ShowDialog(String.Format(Locale.Get("SettingsMakeBackupInfo"), bkpdir), YesNoBtns)) {
				try {
					string origDir = Program.GamePath;
					if (!Directory.Exists(bkpdir)) {
						Directory.CreateDirectory(bkpdir);
					}

					List<string> dirs = new List<string>();

					void backupFile(FileInfo fi) {
						string path = fi.Path.Replace('/', '\\');
						int fileNameStart = path.LastIndexOf('\\');
						if (fileNameStart > 0) {
							string dir = path.Substring(0, fileNameStart);
							if (!dirs.Contains(dir)) {
								if (!Directory.Exists(bkpdir + dir)) {
									Directory.CreateDirectory(bkpdir + dir);
								}
								dirs.Add(dir);
							}
						}
						if (IOFile.Exists(origDir + path)) {
							IOFile.Copy(origDir + path, bkpdir + path);
						}
					}

					GameVersion gameVersion = Program.LoncherSettings.GameVersion;
					foreach (FileGroup fg in gameVersion.FileGroups) {
						foreach (FileInfo fi in fg.Files) {
							backupFile(fi);
						}
					}
					foreach (FileInfo fi in gameVersion.Files) {
						backupFile(fi);
					}
					YobaDialog.ShowDialog(String.Format(Locale.Get("SettingsMakeBackupDone"), bkpdir));
				}
				catch (Exception ex) {
					YobaDialog.ShowDialog(ex.Message);
				}
			}
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
