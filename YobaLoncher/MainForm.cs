using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YobaLoncher {
	public partial class MainForm : Form {
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		private DownloadProgressTracker downloadProgressTracker_;
		public string ThePath = "";
		private LinkedList<FileInfo> filesToUpload_;
		private LinkedList<FileInfo> modFilesToUpload_;
		private LinkedListNode<FileInfo> currentFile_ = null;
		private bool ReadyToGo_ = false;
		private volatile bool UpdateInProgress_ = false;

		private long lastDlstringUpdate_ = -1;
		private string statusListClass_ = "";

		private Panel[] uiPanels_;

		private Dictionary<WebBrowser, bool> allowUrlsInEmbeddedBrowser = new Dictionary<WebBrowser, bool>();

		WebClient wc_;

		[ComVisible(true)]
		public class StatusController {
			public class SFileInfo {
				public string Title;
				public bool IsOk = false;

				public SFileInfo(string title, bool isok) {
					Title = title;
					IsOk = isok;
				}
			}

			private static StatusController _instance = null;
			internal MainForm Form = null;
			internal Dictionary<string, FileInfo> FileMap;

			public void UncheckFile(string id) {
				FileInfo fi = FileMap[id];
				if (fi.Importance > 0) {
					fi.IsCheckedToDl = false;
					Form.CheckReady();
				}
			}
			public void CheckFile(string id) {
				FileMap[id].IsCheckedToDl = true;
				Form.SetReady(false);
			}

			public static StatusController Instance {
				get {
					if (_instance is null) {
						_instance = new StatusController();
					}
					return _instance;
				}
			}
		}

		[ComVisible(true)]
		public class ModsController {
			public class SFileInfo {
				public string Title;
				public bool IsOk = false;

				public SFileInfo(string title, bool isok) {
					Title = title;
					IsOk = isok;
				}
			}

			private static ModsController _instance = null;
			private static string[] sizeUnits = new string[] { "B", "KB", "MB", "GB", "TB", "<size unit error>" };
			internal MainForm Form = null;
			internal Dictionary<string, ModInfo> ModMap;

			public void InstallMod(string id) {
				InstallModAsync(id);
			}
			private async Task InstallModAsync(string id) {
				ModInfo mi = ModMap[id];
				uint size = 0;
				if (mi.CurrentVersionFiles[0].Size == 0) {
					await FileChecker.CheckFiles(mi.CurrentVersionFiles);
				}
				foreach (FileInfo fi in mi.CurrentVersionFiles) {
					if (!fi.IsOK) {
						size += fi.Size;
					}
				}
				int sizePow = 0;
				while (size > 2000) {
					size /= 1024;
					sizePow++;
				}
				if (sizePow > 5) {
					sizePow = 5;
				}
				if (DialogResult.Yes == YobaDialog.ShowDialog(String.Format(Locale.Get("AreYouSureInstallMod"), mi.Name, size, sizeUnits[sizePow]), YobaDialog.YesNoBtns)) {
					if (Form.modFilesToUpload_ is null) {
						Form.modFilesToUpload_ = new LinkedList<FileInfo>(mi.CurrentVersionFiles);
						mi.DlInProgress = true;
						Form.UpdateModsWebView();
						if (!Form.UpdateInProgress_) {
							Form.DownloadNextMod();
						}
					}
					else {
						foreach (FileInfo fi in mi.CurrentVersionFiles) {
							Form.modFilesToUpload_.AddLast(fi);
						}
						mi.DlInProgress = true;
						Form.UpdateModsWebView();
					}
				}
			}
			public void UninstallMod(string id) {
				ModInfo mi = ModMap[id];
				if (DialogResult.Yes == YobaDialog.ShowDialog(String.Format(Locale.Get("AreYouSureUninstallMod"), mi.Name), YobaDialog.YesNoBtns)) {
					mi.Delete();
					Form.UpdateModsWebView();
				}
			}
			public void DisableMod(string id) {
				ModInfo mi = ModMap[id];
				mi.Disable();
				Form.UpdateModsWebView();
			}
			public void EnableMod(string id) {
				ModInfo mi = ModMap[id];
				try {
					mi.Enable();
				}
				catch (Exception ex) {
					YobaDialog.ShowDialog(String.Format(Locale.Get("CannotEnableMod"), mi.Name) + "\r\n\r\n" + ex.Message);
				}
				Form.UpdateModsWebView();
			}

			public static ModsController Instance {
				get {
					if (_instance is null) {
						_instance = new ModsController();
					}
					return _instance;
				}
			}
		}

		public MainForm() {
			ThePath = Program.GamePath;

			InitializeComponent();

			int winstyle = NativeWinAPI.GetWindowLong(basePanel.Handle, NativeWinAPI.GWL_EXSTYLE);
			NativeWinAPI.SetWindowLong(basePanel.Handle, NativeWinAPI.GWL_EXSTYLE, winstyle | NativeWinAPI.WS_EX_COMPOSITED);

			SuspendLayout();
			
			wc_ = new WebClient { Encoding = Encoding.UTF8 };
			wc_.DownloadProgressChanged += new DownloadProgressChangedEventHandler(OnDownloadProgressChanged);
			wc_.DownloadFileCompleted += new AsyncCompletedEventHandler(OnDownloadCompleted);
			downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));

			int missingFilesCount = Program.GameFileCheckResult.InvalidFiles.Count;

			if (missingFilesCount > 0) {
				filesToUpload_ = Program.GameFileCheckResult.InvalidFiles;
				bool allowRun = true;
				foreach (FileInfo fi in filesToUpload_) {
					if (fi.Importance < 2 && (!fi.IsPresent || (!fi.IsOK && fi.Importance < 1))) {
						allowRun = false;
					}
					else {
						missingFilesCount--;
					}
				}
				SetReady(allowRun);
			}
			else {
				SetReady(true);
			}
			updateLabelText.Text = ReadyToGo_ ? Locale.Get("AllFilesIntact") : String.Format(Locale.Get("FilesMissing"), missingFilesCount);

			changelogMenuBtn.Text = Locale.Get("ChangelogBtn");
			linksButton.Text = Locale.Get("LinksBtn");
			settingsButton.Text = Locale.Get("SettingsBtn");
			faqButton.Text = Locale.Get("FAQBtn");
			statusButton.Text = Locale.Get("StatusBtn");
			modsButton.Text = Locale.Get("ModsBtn");
			Text = Locale.Get("MainFormTitle");

			theToolTip.SetToolTip(closeButton, Locale.Get("Close"));
			theToolTip.SetToolTip(minimizeButton, Locale.Get("Minimize"));
			theToolTip.SetToolTip(changelogMenuBtn, Locale.Get("ChangelogTooltip"));
			theToolTip.SetToolTip(statusButton, Locale.Get("StatusTooltip"));
			theToolTip.SetToolTip(linksButton, Locale.Get("LinksTooltip"));
			theToolTip.SetToolTip(settingsButton, Locale.Get("SettingsTooltip"));
			theToolTip.SetToolTip(faqButton, Locale.Get("FAQTooltip"));
			theToolTip.SetToolTip(modsButton, Locale.Get("ModsTooltip"));

			uiPanels_ = new Panel[] { changelogPanel, statusPanel, linksPanel, modsPanel, faqPanel };

			foreach (Panel p in uiPanels_) {
				p.Location = new Point(0, 0);
				p.Size = new Size(610, 330);
			}
			foreach (WebBrowser wb in new WebBrowser[] { statusBrowser, modsBrowser, faqBrowser, changelogBrowser }) {
				wb.Navigate("about:blank");
				wb.Document.Write(String.Empty);
				allowUrlsInEmbeddedBrowser.Add(wb, false);
			}

			

			//allowUrlsInEmbeddedBrowser.Add(statusBrowser, false);
			//allowUrlsInEmbeddedBrowser.Add(modsBrowser, false);
			//allowUrlsInEmbeddedBrowser.Add(faqBrowser, false);
			//allowUrlsInEmbeddedBrowser.Add(changelogBrowser, false);

			UpdateInfoWebViews();

			statusBrowser.Size = new Size(610, 330);
			statusBrowser.Location = new Point(0, 0);
			modsBrowser.Size = new Size(610, 330);
			modsBrowser.Location = new Point(0, 0);

			statusBrowser.ObjectForScripting = StatusController.Instance;
			StatusController.Instance.Form = this;
			StatusController.Instance.FileMap = new Dictionary<string, FileInfo>();
			modsBrowser.ObjectForScripting = ModsController.Instance;
			ModsController.Instance.Form = this;

			UpdateStatusWebView();
			UpdateModsWebView();

			if (Program.LoncherSettings.UI.ContainsKey("UpdateLabel")) {
				UIElement updateLabelInfo = Program.LoncherSettings.UI["UpdateLabel"];
				if (updateLabelInfo != null) {
					updateLabelText.BackColor = YU.colorFromString(updateLabelInfo.BgColor, Color.Transparent);
					updateLabelText.ForeColor = YU.colorFromString(updateLabelInfo.Color, Color.White);
					YU.setFont(updateLabelText, updateLabelInfo.Font, updateLabelInfo.FontSize);
				}
			}
			string[] menuScreenKeys = new string[] {
				"BasePanel", "StatusPanel", "LinksPanel", "ModsPanel", "ChangelogPanel", "FAQPanel"
			};
			string[] menuBtnKeys = new string[] {
				"LaunchButton", "SettingsButton", "StatusButton", "LinksButton", "ChangelogButton", "ModsButton", "FAQButton", "CloseButton", "MinimizeButton", "HelpButton"
			};
			string[] menuBtnControlKeys = new string[] {
				"launchGameButton", "settingsButton", "statusButton", "linksButton", "changelogMenuBtn", "modsButton", "faqButton", "closeButton", "minimizeButton", "helpButton"
			};
			for (int i = 0; i < menuBtnKeys.Length; i++) {
				string menuBtnKey = menuBtnKeys[i];
				if (Program.LoncherSettings.UI.ContainsKey(menuBtnKey)) {
					UIElement launchButtonInfo = Program.LoncherSettings.UI[menuBtnKey];
					if (launchButtonInfo != null) {
						Control[] ctrls = Controls.Find(menuBtnControlKeys[i], true);
						if (ctrls.Length > 0) {
							((YobaButtonAbs)ctrls[0]).ApplyUIStyles(launchButtonInfo);
						}
					}
				}
			}
			foreach (string menuScreenKey in menuScreenKeys) {
				if (Program.LoncherSettings.UI.ContainsKey(menuScreenKey)) {
					UIElement uiInfo = Program.LoncherSettings.UI[menuScreenKey];
					if (uiInfo != null) {
						Control[] ctrls = Controls.Find(menuScreenKey, true);
						if (ctrls.Length > 0) {
							Panel panel = (Panel)ctrls[0];
							if (uiInfo.Position != null) {
								panel.Location = new Point(uiInfo.Position.X, uiInfo.Position.Y);
							}
							if (uiInfo.Size != null) {
								panel.Size = new Size(uiInfo.Size.X, uiInfo.Size.Y);
							}
							if (YU.stringHasText(uiInfo.Color)) {
								panel.ForeColor = YU.colorFromString(uiInfo.Color, Color.White);
							}
							if (YU.stringHasText(uiInfo.BgColor)) {
								panel.BackColor = YU.colorFromString(uiInfo.BgColor, Color.DimGray);
							}
							if (uiInfo.BgImage != null && YU.stringHasText(uiInfo.BgImage.Path)) {
								if (YU.stringHasText(uiInfo.BgImage.Layout)) {
									try {
										Enum.Parse(typeof(ImageLayout), uiInfo.BgImage.Layout);
									}
									catch {
										panel.BackgroundImageLayout = ImageLayout.Stretch;
									}
								}
								else {
									panel.BackgroundImageLayout = ImageLayout.Stretch;
								}
								panel.BackgroundImage = YU.readBitmap(PreloaderForm.IMGPATH + uiInfo.BgImage.Path);
							}
						}
					}
				}
			}

			for (int i = 0; i < Program.LoncherSettings.Buttons.Count; i++) {
				LinkButton lbtn = Program.LoncherSettings.Buttons[i];
				if (lbtn != null) {
					YobaButton linkButton = new YobaButton(lbtn.Url);
					linkButton.Name = "linkBtn" + (i + 1);
					linkButton.TabIndex = 10 + i;
					linkButton.UseVisualStyleBackColor = true;
					linkButton.ApplyUIStyles(lbtn);
					if (YU.stringHasText(lbtn.Caption)) {
						linkButton.Text = "";
						theToolTip.SetToolTip(linkButton, lbtn.Caption);
					}
					linkButton.Click += new EventHandler((object o, EventArgs a) => {
						string url = ((YobaButton)o).Url;
						if (YU.stringHasText(url)) {
							Process.Start(url);
						}
					});
					linksPanel.Controls.Add(linkButton);
				}
			}

			BackgroundImageLayout = ImageLayout.Stretch;
			BackgroundImage = Program.LoncherSettings.Background;

			switch (LauncherConfig.StartPage) {
				case StartPageEnum.Changelog:
					changelogPanel.Visible = true;
					break;
				case StartPageEnum.Mods:
					modsPanel.Visible = true;
					break;
				case StartPageEnum.Status:
					statusPanel.Visible = true;
					break;
				case StartPageEnum.Links:
					linksPanel.Visible = true;
					break;
				case StartPageEnum.FAQ:
					faqPanel.Visible = true;
					break;
			}

			List<ModInfo> outdatedMods = new List<ModInfo>();
			LinkedList<FileInfo> outdatedModFiles = new LinkedList<FileInfo>();
			LinkedList<FileInfo> outdatedAlteredModFiles = new LinkedList<FileInfo>();

			foreach (ModInfo mi in Program.LoncherSettings.Mods) {
				if (mi.CurrentVersionFiles != null) {
					if ((mi.ModConfigurationInfo != null) && mi.ModConfigurationInfo.Active) {
						bool hasit = false;
						foreach (FileInfo mif in mi.CurrentVersionFiles) {
							if (!mif.IsOK) {
								outdatedModFiles.AddLast(mif);
								if (!hasit) {
									outdatedMods.Add(mi);
									hasit = true;
								}
								if (mi.ModConfigurationInfo.Altered) {
									outdatedAlteredModFiles.AddLast(mif);
								}
							}
						}
						if (hasit) {
							outdatedModFiles.Last.Value.LastFileOfModToUpdate = mi;
						}
					}
				}
			}
			if (outdatedMods.Count > 0) {
				string outdatedmods = "";
				string alteredmods = "";
				List<ModInfo> alteredOutdatedMods = new List<ModInfo>();
				ulong outdatedmodssize = 0;
				bool comma = false;
				bool altcomma = false;
				foreach (ModInfo mi in outdatedMods) {
					if (!comma) {
						comma = true;
					}
					else {
						outdatedmods += ", ";
					}
					outdatedmods += mi.CurrentVersionData.Name ?? mi.Name;
					if (mi.ModConfigurationInfo.Altered) {
						if (!altcomma) {
							altcomma = true;
						}
						else {
							alteredmods += ", ";
						}
						alteredOutdatedMods.Add(mi);
						alteredmods += mi.CurrentVersionData.Name ?? mi.Name;
					}
				}
				foreach (FileInfo mif in outdatedModFiles) {
					outdatedmodssize += mif.Size;
				}
				if (DialogResult.Yes == YobaDialog.ShowDialog(String.Format(Locale.Get("YouHaveOutdatedMods"), outdatedmods, YU.formatFileSize(outdatedmodssize)), YobaDialog.YesNoBtns)) {
					modFilesToUpload_ = outdatedModFiles;
					foreach (ModInfo mi in outdatedMods) {
						mi.DlInProgress = true;
					}
					UpdateModsWebView();
					if (!UpdateInProgress_) {
						DownloadNextMod();
					}
				}
				else {
					if (alteredOutdatedMods.Count > 0) {
						ulong alteredmodssize = 0;
						foreach (FileInfo mif in outdatedAlteredModFiles) {
							alteredmodssize += mif.Size;
						}
						if (DialogResult.Yes == YobaDialog.ShowDialog(String.Format(Locale.Get("YouHaveAlteredMods"), alteredmods, YU.formatFileSize(alteredmodssize)), YobaDialog.YesNoBtns)) {
							modFilesToUpload_ = outdatedAlteredModFiles;
							foreach (ModInfo mi in alteredOutdatedMods) {
								mi.DlInProgress = true;
							}
							UpdateModsWebView();
							if (!UpdateInProgress_) {
								DownloadNextMod();
							}
						}
					}
				}
			}
			PerformLayout();
		}

		private void UpdateInfoWebViews() {
			bool hasChangelog = false, hasFaq = false;
			allowUrlsInEmbeddedBrowser[changelogBrowser] = true;
			allowUrlsInEmbeddedBrowser[faqBrowser] = true;
			LauncherData ls = Program.LoncherSettings;
			if (Program.OfflineMode) {
				if (Program.LoncherSettings.Changelog.Html is null) {
					changelogLoncherIsOfflineLable.Text = Locale.Get("LauncherIsInOfflineMode");
				}
				else {
					changelogBrowser.DocumentText = Program.LoncherSettings.Changelog.Html;
					hasChangelog = true;
				}
				if (Program.LoncherSettings.FAQ.Html is null) {
					faqLoncherIsOfflineLable.Text = Locale.Get("LauncherIsInOfflineMode");
				}
				else {
					faqBrowser.DocumentText = Program.LoncherSettings.FAQ.Html;
					hasFaq = true;
				}
			}
			else {
				if (Program.LoncherSettings.Changelog.Error != null) {
					changelogLoncherIsOfflineLable.Text = Locale.Get(Program.LoncherSettings.Changelog.Error);
				}
				else {
					if (Program.LoncherSettings.Changelog.Html != null) {
						changelogBrowser.DocumentText = Program.LoncherSettings.Changelog.Html.Substring(1);//"data:text/html;charset=UTF-8," + 
						hasChangelog = true;
					}
					else if (Program.LoncherSettings.Changelog.Site.Length > 0) {
						changelogBrowser.Url = new Uri(Program.LoncherSettings.Changelog.Site);
						hasChangelog = true;
					}
					else {
						changelogLoncherIsOfflineLable.Text = Locale.Get("ChangelogFileUnavailable");
					}
				}
				if (Program.LoncherSettings.FAQ.Error != null) {
					faqLoncherIsOfflineLable.Text = Locale.Get(Program.LoncherSettings.FAQ.Error);
				}
				else {
					if (Program.LoncherSettings.FAQ.Html != null) {
						faqBrowser.DocumentText = Program.LoncherSettings.FAQ.Html;//"data:text/html;charset=UTF-8," + 
						hasFaq = true;
					}
					else if (Program.LoncherSettings.FAQ.Site.Length > 0) {
						faqBrowser.Url = new Uri(Program.LoncherSettings.FAQ.Site);
						hasFaq = true;
					}
					else {
						faqLoncherIsOfflineLable.Text = Locale.Get("FAQFileUnavailable");
					}
				}
			}

			if (hasChangelog) {
				//changelogPanel.Controls.Remove(changelogLoncherIsOfflineLable);
				changelogLoncherIsOfflineLable.Visible = false;
				changelogBrowser.Location = new Point(0, 0);
				changelogBrowser.Size = new Size(610, 330);
			}
			else {
				//changelogPanel.Controls.Remove(changelogBrowser);
				changelogBrowser.Visible = false;
			}
			if (hasFaq) {
				faqLoncherIsOfflineLable.Visible = false;
				faqBrowser.Location = new Point(0, 0);
				faqBrowser.Size = new Size(610, 330);
			}
			else {
				faqBrowser.Visible = false;
			}
		}

		private void UpdateStatusWebView() {
			StringBuilder statusSb = new StringBuilder();
			GameVersion gameVersion = Program.LoncherSettings.GameVersion;

			int finum = 0;

			void appendFiles(StringBuilder sb, List<FileInfo> files) {
				foreach (FileInfo fi in files) {
					string id = "fileNumber" + finum++;
					StatusController.Instance.FileMap[id] = fi;
					string tooltip = fi.Tooltip;
					string statusText = null;
					sb.Append("<div class='fileEntry ");
					if (fi.IsOK) {
						sb.Append("exists");
						fi.IsCheckedToDl = false;
						statusText = Locale.Get("StatusListDownloadedFile");
						if (tooltip is null) {
							tooltip = Locale.Get("StatusListDownloadedFileTooltip");
						}
					}
					else if (fi.Importance == 1 && fi.IsPresent) {
						sb.Append("recommended");
						statusText = Locale.Get("StatusListRecommendedFile");
						if (tooltip is null) {
							tooltip = Locale.Get("StatusListRecommendedFileTooltip");
						}
					}
					else if (fi.Importance > 1) {
						sb.Append("optional");
						statusText = Locale.Get("StatusListOptionalFile");
						if (tooltip is null) {
							tooltip = Locale.Get("StatusListOptionalFileTooltip");
						}
					}
					else {
						sb.Append("forced");
						statusText = Locale.Get("StatusListRequiredFile");
						fi.IsCheckedToDl = true;
						if (tooltip is null) {
							tooltip = Locale.Get("StatusListRequiredFileTooltip");
						}
					}
					if (fi.IsCheckedToDl) {
						sb.Append(" checked");
					}
					sb.Append("' id='").Append(id).Append("' title='").Append(tooltip).Append("'> ").Append(fi.Description ?? fi.Path).Append("</div>");
					//<span class='statusIndicator'>").Append(statusText).Append("</span>
				}
			}

			foreach (FileGroup fg in gameVersion.FileGroups) {
				statusSb.Append("<div class='group-spoiler-button'><span class='spoilersym'>- </span>").Append(fg.Name).Append("</div><div class='group-spoiler'>");
				appendFiles(statusSb, fg.Files);
				statusSb.Append("</div><div class='spoilerdash'></div>");
			}
			appendFiles(statusSb, gameVersion.Files);
			string template = Resource1.status_template.Replace("[[[DOWNLOAD]]]", Locale.Get("StatusComboboxDownload"))
				.Replace("[[[NODOWNLOAD]]]", Locale.Get("StatusComboboxNoDownload"))
				.Replace("[[[UPDATE]]]", Locale.Get("StatusComboboxUpdate"))
				.Replace("[[[UPDATEFORCED]]]", Locale.Get("StatusComboboxUpdateForced"))
				.Replace("[[[DOWNLOADFORCED]]]", Locale.Get("StatusComboboxDownloadForced"))
				.Replace("[[[NOUPDATE]]]", Locale.Get("StatusComboboxNoUpdate"))
				.Replace("[[[STATUS]]]", statusSb.ToString());
			statusBrowser.DocumentText = template; //Program.LoncherSettings.ChangelogHtml;//"data:text/html;charset=UTF-8," + 
		}

		private void UpdateModsWebView() {
			StringBuilder statusSb = new StringBuilder();
			ModsController.Instance.ModMap = new Dictionary<string, ModInfo>();
			int finum = 0;

			foreach (ModInfo mi in Program.LoncherSettings.Mods) {
				if (mi.CurrentVersionFiles != null) {
					string id = "modId" + finum++;
					ModsController.Instance.ModMap.Add(id, mi);
					string buttons;
					statusSb.Append("<div class='modEntry");
					if (mi.DlInProgress) {
						buttons = String.Format("<div class='loading'>{0}</div>", Locale.Get("ModInstallationInProgress"));
					}
					else if (mi.ModConfigurationInfo is null) {
						buttons = String.Format("<div class='modControlButton install'>{0}</div>", Locale.Get("InstallMod"));
					}
					else if (mi.ModConfigurationInfo.Active) {
						statusSb.Append(" installed active");
						buttons = String.Format("<div class='modControlButton disable'>{0}</div><div class='modControlButton uninstall'>{1}</div>"
							, Locale.Get("DisableMod"), Locale.Get("UninstallMod"));
					}
					else {
						statusSb.Append(" installed");
						buttons = String.Format("<div class='modControlButton enable'>{0}</div><div class='modControlButton uninstall'>{1}</div>"
							, Locale.Get("EnableMod"), Locale.Get("UninstallMod"));
					}
					statusSb.Append("' id='").Append(id).Append("'><div class='modTitle'>").Append(mi.CurrentVersionData.Name ?? mi.Name)
							.Append("</div><div class='modDesc'>").Append(mi.CurrentVersionData.Description ?? mi.Description)
							.Append("</div><div class='modControls'>").Append(buttons).Append("</div></div>");
				}
			}
			if (finum == 0) {
				statusSb.Append("<div class='noMods'>").Append(Locale.Get("NoModsForThisVersion")).Append("</div>");
			}
			string template = Resource1.mods_template
				.Replace("[[[MODS]]]", statusSb.ToString());
			modsBrowser.DocumentText = template;
		}

		private async void DownloadFile(FileInfo fileInfo) {
			if (!YU.stringHasText(fileInfo.UploadAlias)) {
				fileInfo.UploadAlias = fileInfo.Hashes.Count > 0 ? fileInfo.Hashes[0] : null;
				if (!YU.stringHasText(fileInfo.UploadAlias)) {
					int lios = Math.Max(fileInfo.Path.LastIndexOf('\\'), fileInfo.Path.LastIndexOf('/'));
					fileInfo.UploadAlias = lios > -1 ? fileInfo.Path.Substring(lios + 1, fileInfo.Path.Length) : fileInfo.Path;
				}
			}
			string uploadFilename = PreloaderForm.UPDPATH + fileInfo.UploadAlias;
			if (File.Exists(uploadFilename)) {
				if (FileChecker.CheckFileMD5(PreloaderForm.UPDPATH, fileInfo)) {
					if (UpdateInProgress_) {
						DownloadNext();
					}
					else {
						DownloadNextMod();
					}
					return;
				}
				else {
					File.Delete(uploadFilename);
				}
			}
			try {
				updateProgressBar.Value = 0;
				updateLabelText.Text = string.Format(
					Locale.Get("DLRate")
					, YU.formatFileSize(0)
					, YU.formatFileSize(fileInfo.Size)
					, ""
					, currentFile_.Value.Description
				);
				await wc_.DownloadFileTaskAsync(new Uri(fileInfo.Url), uploadFilename);
			}
			catch (Exception ex) {
				ShowDownloadError(string.Format(Locale.Get("CannotDownloadFile"), fileInfo.Path) + "\r\n" + ex.Message);
			}
		}

		private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
			downloadProgressTracker_.SetProgress(e.BytesReceived, e.TotalBytesToReceive);
			if (DateTime.Now.Ticks > lastDlstringUpdate_ + 500000L) {
				lastDlstringUpdate_ = DateTime.Now.Ticks;
				updateProgressBar.Value = e.ProgressPercentage;
				string desc = currentFile_ is null ? "" : currentFile_.Value.Description;
				updateLabelText.Text = string.Format(
					Locale.Get("DLRate")
					, YU.formatFileSize(e.BytesReceived)
					, YU.formatFileSize(e.TotalBytesToReceive)
					, downloadProgressTracker_.GetBytesPerSecondString()
					, desc
				);
			}
		}

		private void OnDownloadCompleted(object sender, AsyncCompletedEventArgs e) {
			downloadProgressTracker_.Reset();
			if (UpdateInProgress_) {
				DownloadNext();
			}
			else {
				DownloadNextMod();
			}
		}

		private void DownloadNext() {
			do {
				currentFile_ = currentFile_.Next;
			}
			while ((currentFile_ != null) && !currentFile_.Value.IsCheckedToDl);
			
			if (currentFile_ != null) {
				DownloadFile(currentFile_.Value);
			}
			else {
				string filename = "";
				updateProgressBar.Value = 100;
				updateLabelText.Text = Locale.Get("StatusCopyingFiles");
				try {
					foreach (FileInfo fileInfo in filesToUpload_) {
						if (!fileInfo.IsCheckedToDl) {
							continue;
						}
						filename = ThePath + fileInfo.Path.Replace('/', '\\');
						string dirpath = filename.Substring(0, filename.LastIndexOf('\\'));
						Directory.CreateDirectory(dirpath);
						if (File.Exists(filename)) {
							File.Delete(filename);
						}
						File.Move(PreloaderForm.UPDPATH + fileInfo.UploadAlias, filename);
						fileInfo.IsOK = true;
						fileInfo.IsPresent = true;
					}
					
					updateLabelText.Text = Locale.Get("StatusUpdatingDone");
					UpdateStatusWebView();
					if (modFilesToUpload_ != null) {
						UpdateInProgress_ = false;
						DownloadNextMod();
					}
					else {
						SetReady(true);
						launchGameButton.Enabled = true;
						UpdateInProgress_ = false;
						if (YobaDialog.ShowDialog(Locale.Get("UpdateSuccessful"), YobaDialog.YesNoBtns) == DialogResult.Yes) {
							launch();
						}
					}
				}
				catch (UnauthorizedAccessException ex) {
					ShowDownloadError(string.Format(Locale.Get("DirectoryAccessDenied"), filename) + ":\r\n" + ex.Message);
				}
				catch (Exception ex) {
					ShowDownloadError(string.Format(Locale.Get("CannotMoveFile"), filename) + ":\r\n" + ex.Message);
				}
				UpdateInProgress_ = false;
			}
		}

		private void ShowDownloadError(string error) {
			YobaDialog.ShowDialog(error);
			launchGameButton.Enabled = true;
			updateProgressBar.Value = 0;
			updateLabelText.Text = Locale.Get("StatusDownloadError");
			UpdateStatusWebView();
		}

		private void SetReady(bool isReady) {
			if (isReady) {
				ReadyToGo_ = true;
				launchGameButton.Text = Locale.Get("LaunchBtn");
			}
			else {
				ReadyToGo_ = false;
				launchGameButton.Text = Locale.Get("UpdateBtn");
			}
		}
		private void CheckReady() {
			if (filesToUpload_ != null) {
				foreach (FileInfo fi in filesToUpload_) {
					if (fi.IsCheckedToDl) {
						return;
					}
				}
			}
			SetReady(true);
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
			Application.Exit();
		}

		private void launchGameBtn_Click(object sender, EventArgs e) {
			if (ReadyToGo_) {
				launch();
			}
			else {
				HtmlElement statusList = statusBrowser.Document.GetElementById("articleContent");
				statusListClass_ = statusList.GetAttribute("class");
				statusList.SetAttribute("class", statusListClass_ + " disabled");
				launchGameButton.Enabled = false;
				UpdateInProgress_ = true;
				currentFile_ = filesToUpload_.First;
				while ((currentFile_ != null) && !currentFile_.Value.IsCheckedToDl) {
					currentFile_ = currentFile_.Next;
				}
				DownloadFile(currentFile_.Value);
			}
		}

		private void launch() {
			string args = "/C \"" + ThePath + Program.LoncherSettings.ExeName + "\"";
			if (LauncherConfig.LaunchFromGalaxy) {
				args = string.Format("/command=runGame /gameId={1} /path=\"{0}\"", ThePath, Program.LoncherSettings.GogID);
				Process.Start(new ProcessStartInfo { Arguments = args, FileName = LauncherConfig.GalaxyDir });
			}
			else {
				if (ThePath.Contains("steamapps")) {
					args = "/C explorer steam://run/" + Program.LoncherSettings.SteamID;
				}
				Process.Start(new ProcessStartInfo { Arguments = args, FileName = "cmd", WindowStyle = ProcessWindowStyle.Hidden });
			}
			YU.Log(args);
			launchGameButton.Enabled = false;
			System.Threading.Thread.Sleep(1800);
			if (LauncherConfig.CloseOnLaunch) {
				Application.Exit();
			}
			else {
				launchGameButton.Enabled = true;
			}
		}

		private void settingsButton_Click(object sender, EventArgs e) {
			SettingsDialog settingsDialog = new SettingsDialog(this);
			settingsDialog.Icon = Program.LoncherSettings.Icon;
			if (settingsDialog.ShowDialog(this) == DialogResult.OK) {
				LauncherConfig.StartPage = settingsDialog.OpeningPanel;
				LauncherConfig.GameDir = settingsDialog.GamePath;
				LauncherConfig.LaunchFromGalaxy = settingsDialog.LaunchViaGalaxy;
				bool prevOffline = LauncherConfig.StartOffline;
				LauncherConfig.StartOffline = settingsDialog.OfflineMode;
				LauncherConfig.CloseOnLaunch = settingsDialog.CloseOnLaunch;
				LauncherConfig.Save();
				settingsDialog.Dispose();
				if (LauncherConfig.GameDir != Program.GamePath) {
					Hide();
					new PreloaderForm(this).Show();
				}
				else if (prevOffline != LauncherConfig.StartOffline) {
					if (YobaDialog.ShowDialog(Locale.Get(LauncherConfig.StartOffline ? "OfflineModeSet" : "OnlineModeSet"), YobaDialog.YesNoBtns) == DialogResult.Yes) {
						Hide();
						new PreloaderForm(this).Show();
					}
				}
			}
			this.Focus();
		}

		private void changelogMenuBtn_Click(object sender, EventArgs e) {
			showUIPanel(changelogPanel);
		}

		private void checkResultMenuBtn_Click(object sender, EventArgs e) {
			showUIPanel(statusPanel);
		}

		private void linksMenuBtn_Click(object sender, EventArgs e) {
			showUIPanel(linksPanel);
		}

		private void modsButton_Click(object sender, EventArgs e) {
			showUIPanel(modsPanel);
		}
		
		private void faqButton_Click(object sender, EventArgs e) {
			showUIPanel(faqPanel);
		}

		private void showUIPanel(Panel panelToShow) {
			foreach (Panel p in uiPanels_) {
				p.Visible = p == panelToShow;
			}
		}

		private void closeButton_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void minimizeButton_Click(object sender, EventArgs e) {
			WindowState = FormWindowState.Minimized;
		}

		private void draggingPanel_MouseDown(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left) {
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}

		private async Task<LauncherData.StaticTabData> getStaticTabData(string uiKey, string url, string quoteToEscape, string replacePlaceholder) {
			LauncherData.StaticTabData staticTabData = new LauncherData.StaticTabData();
			staticTabData.Site = url;
			try {
				if (Program.LoncherSettings.UIStyle.TryGetValue(uiKey, out FileInfo fileInfo)) {
					if (fileInfo != null && YU.stringHasText(fileInfo.Url)) {
						using (WebClient wc = new WebClient { Encoding = Encoding.UTF8 }) {
							string template = (await wc.DownloadStringTaskAsync(new Uri(fileInfo.Url)));
							string cl = "";
							if (url != null && url.Length > 0) {
								cl = (await wc.DownloadStringTaskAsync(new Uri(url)));
								if (quoteToEscape != null && quoteToEscape.Length > 0) {
									string quote = quoteToEscape;
									cl = cl.Replace("\\", "\\\\").Replace(quote, "\\" + quote);
									if (cl.Contains("\r")) {
										cl = cl.Replace("\r\n", "\\\r\n");
									}
									else {
										cl = cl.Replace("\n", "\\\n");
									}
								}
							}
							staticTabData.Html = template.Replace(replacePlaceholder, cl);
						}
					}
				}
			}
			catch (Exception ex) {
				staticTabData.Error = ex.Message;
			}
			return staticTabData;
		}

		private async void refreshButton_Click(object sender, EventArgs e) {
			if (!Program.OfflineMode) {
				LauncherData.LauncherDataRaw launcherDataRaw = Program.LoncherSettings.RAW;
				string quote = launcherDataRaw.QuoteToEscape;
				Program.LoncherSettings.Changelog = await getStaticTabData("Changelog", launcherDataRaw.Changelog, quote, "[[[CHANGELOG]]]");
				Program.LoncherSettings.FAQ = await getStaticTabData("FAQ", launcherDataRaw.FAQFile, quote, "[[[FAQTEXT]]]");
				UpdateInfoWebViews();
			}
		}

		private void helpButton_Click(object sender, EventArgs e) {
			YU.ShowHelpDialog();
		}

		private void MainForm_Shown(object sender, EventArgs e) {
			if (!(Program.FirstRun || Program.OfflineMode)
					&& Program.LoncherSettings.Survey != null
					&& YU.stringHasText(Program.LoncherSettings.Survey.Text)
					&& YU.stringHasText(Program.LoncherSettings.Survey.Url)
					&& YU.stringHasText(Program.LoncherSettings.Survey.ID)
					&& (LauncherConfig.LastSurveyId is null || LauncherConfig.LastSurveyId != Program.LoncherSettings.Survey.ID)) {
				int showSurvey = new Random().Next(0, 100);
				string discardId = "-0" + Program.LoncherSettings.Survey.ID;
				bool wasDiscarded = discardId.Equals(LauncherConfig.LastSurveyId);
				if ((!wasDiscarded && showSurvey > 70) || (showSurvey > 7 && showSurvey < 10)) {
					DialogResult result = YobaDialog.ShowDialog(Program.LoncherSettings.Survey.Text, YobaDialog.YesNoBtns);
					if (result == DialogResult.Yes) {
						Process.Start(Program.LoncherSettings.Survey.Url);
						LauncherConfig.LastSurveyId = Program.LoncherSettings.Survey.ID;
						LauncherConfig.Save();
					}
					else if (result == DialogResult.No) {
						LauncherConfig.LastSurveyId = discardId;
						LauncherConfig.Save();
					}
				}
			}
		}

		private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
			if (allowUrlsInEmbeddedBrowser[(WebBrowser)sender]) {
				allowUrlsInEmbeddedBrowser[(WebBrowser)sender] = false;
				return;
			}
			string url = e.Url.ToString();
			string urlstart = url.Substring(0, 7);
			switch (urlstart) {
				case "http://":
				case "https:/":
					Process.Start(url);
					e.Cancel = true;
					break;
			}
		}
	}
}