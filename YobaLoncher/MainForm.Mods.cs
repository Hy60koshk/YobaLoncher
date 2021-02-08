using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace YobaLoncher {
	public partial class MainForm {
		private void MoveUploadedFile(FileInfo fileInfo) {
			if (fileInfo.IsOK) {
				return;
			}
			string filename = ThePath + fileInfo.Path.Replace('/', '\\');
			string dirpath = filename.Substring(0, filename.LastIndexOf('\\'));
			Directory.CreateDirectory(dirpath);
			if (File.Exists(filename)) {
				File.Delete(filename);
			}
			File.Move(PreloaderForm.UPDPATH + fileInfo.UploadAlias, filename);
			fileInfo.IsOK = true;
			fileInfo.IsPresent = true;
		}
		private bool FinalizeModDownload(FileInfo lastFileInfo) {
			string filename = "";
			bool success = false;
			ModInfo modInfo = lastFileInfo.LastFileOfMod;
			updateProgressBar.Value = 100;
			updateLabelText.Text = Locale.Get("StatusCopyingFiles");
			try {
				LinkedListNode<FileInfo> currentMod = modsToUpload_.First;
				while (currentMod.Value != lastFileInfo) {
					MoveUploadedFile(currentMod.Value);
				}
				MoveUploadedFile(lastFileInfo);
				modInfo.Install();
				success = true;
			}
			catch (UnauthorizedAccessException ex) {
				ShowDownloadError(string.Format(Locale.Get("DirectoryAccessDenied"), filename) + ":\r\n" + ex.Message);
			}
			catch (Exception ex) {
				ShowDownloadError(string.Format(Locale.Get("CannotMoveFile"), filename) + ":\r\n" + ex.Message);
			}
			modInfo.DlInProgress = false;
			UpdateModsWebView();
			return success;
		}

		private void StartDownloadMods() {
			launchGameButton.Enabled = false;
			currentFile_ = modsToUpload_.First;

			while (currentFile_ != null) {
				if (currentFile_.Value.IsOK) {
					if (!FinalizeModDownload(currentFile_.Value)) {
						return;
					}
					currentFile_ = currentFile_.Next;
				}
				else {
					break;
				}
			}
			if (currentFile_ != null) {
				downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
				DownloadFile(currentFile_.Value);
			}
			else {
				if (!modsToUpload_.Last.Value.IsOK) {
					if (!FinalizeModDownload(currentFile_.Value)) {
						return;
					}
				}
				updateLabelText.Text = Locale.Get("ModInstallationDone");
				CheckReady();
			}
		}

		private void DownloadNextMod() {
			if (currentFile_.Value.LastFileOfMod != null) {
				if (!FinalizeModDownload(currentFile_.Value)) {
					return;
				}
			}
			currentFile_ = currentFile_.Next;
			while (currentFile_ != null) {
				if (currentFile_.Value.IsOK) {
					if (!FinalizeModDownload(currentFile_.Value)) {
						return;
					}
					currentFile_ = currentFile_.Next;
				}
				else {
					break;
				}
			}
			if (currentFile_ != null) {
				DownloadFile(currentFile_.Value);
			}
			else {
				if (!modsToUpload_.Last.Value.IsOK) {
					if (!FinalizeModDownload(currentFile_.Value)) {
						return;
					}
				}
				updateLabelText.Text = Locale.Get("ModInstallationDone");
				CheckReady();
			}
		}
	}
}