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
			ModInfo modInfo = lastFileInfo.LastFileOfMod ?? lastFileInfo.LastFileOfModToUpdate;
			updateProgressBar.Value = 100;
			updateLabelText.Text = Locale.Get("StatusCopyingFiles");
			try {
				LinkedListNode<FileInfo> currentMod = modFilesToUpload_.First;
				bool gotLast = false;
				while (!gotLast && currentMod != null) {
					MoveUploadedFile(currentMod.Value);
					gotLast = currentMod.Value == lastFileInfo;
					currentMod = currentMod.Next;
					modFilesToUpload_.RemoveFirst();
				}
				lastFileInfo.LastFileOfModToUpdate = null;
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

		private void DownloadNextMod() {
			if (currentFile_ is null) {
				launchGameButton.Enabled = false;
				currentFile_ = modFilesToUpload_.First;
				downloadProgressTracker_ = new DownloadProgressTracker(50, TimeSpan.FromMilliseconds(500));
			}
			else if (currentFile_.Value.LastFileOfMod != null || currentFile_.Value.LastFileOfModToUpdate != null) {
				if (!FinalizeModDownload(currentFile_.Value)) {
					FinishModDownload();
					return;
				}
				currentFile_ = modFilesToUpload_.First;
			}
			else {
				currentFile_ = currentFile_.Next;
			}
			if (currentFile_ != null) {
				if (currentFile_.Value.IsOK) {
					DownloadNextMod();
				}
				else {
					DownloadFile(currentFile_.Value);
				}
			}
			else {
				updateLabelText.Text = Locale.Get("ModInstallationDone");
				FinishModDownload();
			}
		}

		private void FinishModDownload() {
			currentFile_ = null;
			modFilesToUpload_ = null;
			foreach (ModInfo mi in Program.LoncherSettings.Mods) {
				mi.DlInProgress = false;
			}
			UpdateModsWebView();
			CheckReady();
			launchGameButton.Enabled = true;
		}
	}
}