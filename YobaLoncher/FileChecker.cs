using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YobaLoncher {
	class CheckResult {
		public bool IsAllOk = true;
		public LinkedList<FileInfo> InvalidFiles = new LinkedList<FileInfo>();
	}
	class FileCheckedEventArgs : EventArgs {
		public FileInfo File {
			get; 
		}
		public FileCheckedEventArgs(FileInfo file) {
			File = file;
		}
	}
	class FileChecker {
		private static MD5 md5_;

		public static MD5 MD5 {
			get => md5_ == null ? (md5_ = MD5.Create()) : md5_;
		}

		public static async Task<CheckResult> CheckFiles(List<FileInfo> files) {
			return await CheckFiles(Program.GamePath, files, null);
		}
		public static async Task<CheckResult> CheckFiles(string root, List<FileInfo> files) {
			return await CheckFiles(root, files, null);
		}
		public static async Task<CheckResult> CheckFiles(List<FileInfo> files, EventHandler<FileCheckedEventArgs> checkEventHandler) {
			return await CheckFiles(Program.GamePath, files, checkEventHandler);
		}
		public static async Task<CheckResult> CheckFiles(string root, List<FileInfo> files, EventHandler<FileCheckedEventArgs> checkEventHandler) {
			CheckResult result = new CheckResult();
			foreach (FileInfo file in files) {
				if (!(file.IsOK = CheckFileMD5(root, file)) && YU.stringHasText(file.Url)) {
					result.InvalidFiles.AddLast(file);
					result.IsAllOk = false;
					WebRequest webRequest = WebRequest.Create(file.Url);
					webRequest.Method = "HEAD";

					using (WebResponse webResponse = await webRequest.GetResponseAsync()) {
						string fileSize = webResponse.Headers.Get("Content-Length");
						file.Size = Convert.ToUInt32(fileSize);
					}
					checkEventHandler?.Invoke(null, new FileCheckedEventArgs(file));
				}
			}
			return result;
		}
		public static CheckResult CheckFilesOffline(List<FileInfo> files) {
			CheckResult result = new CheckResult();
			foreach (FileInfo file in files) {
				if (!(file.IsOK = CheckFileMD5(Program.GamePath, file))) {
					result.InvalidFiles.AddLast(file);
					result.IsAllOk = false;
				}
			}
			return result;
		}

		public static string GetFileMD5(string path) {
			byte[] hash;
			using (FileStream stream = File.OpenRead(path)) {
				hash = MD5.ComputeHash(stream);
			}
			StringBuilder hashSB = new StringBuilder(hash.Length);
			for (int i = 0; i < hash.Length; i++) {
				hashSB.Append(hash[i].ToString("X2"));
			}
			return hashSB.ToString();
		}
		public static bool CheckFileMD5(FileInfo file) {
			return CheckFileMD5(Program.GamePath, file);
		}
		public static bool CheckFileMD5(string root, FileInfo file) {
			if (file.Path == null || file.Path.Length == 0) {
				throw new Exception(Locale.Get("FileCheckNoFilePath"));
			}
			if (file.Path.Contains(':') || file.Path.Contains('?') || file.Path.Contains('*') || file.Path.Contains("\\\\") || file.Path.Contains("//")) {
				throw new Exception(string.Format(Locale.Get("FileCheckNoFilePath"), file.Path));
			}
			string filepath = root + file.Path;
			if (File.Exists(filepath) && (new System.IO.FileInfo(filepath).Length > 0)) {
				file.IsPresent = true;
				if (file.Hashes == null || file.Hashes.Count == 0) {
					return true;
				}
				string strHash = GetFileMD5(filepath);
				foreach (string correctHash in file.Hashes) {
					if (correctHash == null || correctHash.Length == 0
						|| correctHash.ToUpper().Equals(strHash)) {
						return true;
					}
				}
			}
			return false;
		}

		public static bool CheckFileMD5(string path, string correctHash) {
			if (File.Exists(path)) {
				if (correctHash is null || correctHash.Length == 0) {
					return true;
				}
				string strHash = GetFileMD5(path);
				if (correctHash.ToUpper().Equals(strHash)) {
					return true;
				}
			}
			return false;
		}
	}
}
