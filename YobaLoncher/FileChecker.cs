using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YobaLoncher {
	class CheckResult {
		public bool IsAllOk = true;
		public LinkedList<FileInfo> InvalidFiles = new LinkedList<FileInfo>();
	}
	class FileChecker {
		private static MD5 md5_;

		public static MD5 MD5 {
			get => md5_ == null ? (md5_ = MD5.Create()) : md5_;
		}

		public static CheckResult CheckFiles(List<FileInfo> files) {
			return CheckFiles(Program.GamePath, files);
		}
		public static CheckResult CheckFiles(string root, List<FileInfo> files) {
			CheckResult result = new CheckResult();
			foreach (FileInfo file in files) {
				if (!(file.IsOK = CheckFileMD5(root, file))) {
					result.InvalidFiles.AddLast(file);
					result.IsAllOk = false;
					WebRequest webRequest = WebRequest.Create(file.Url);
					webRequest.Method = "HEAD";

					using (WebResponse webResponse = webRequest.GetResponse()) {
						string fileSize = webResponse.Headers.Get("Content-Length");
						file.Size = Convert.ToUInt32(fileSize);
					}
				}
			}
			return result;
		}

		public static bool CheckFileMD5(FileInfo file) {
			return CheckFileMD5(Program.GamePath, file);
		}
		public static bool CheckFileMD5(string root, FileInfo file) {
			if (file.Path == null || file.Path.Length == 0) {
				throw new Exception("No file path provided.\r\nContact the guy who set the launcher up.");
			}
			if (File.Exists(root + file.Path)) {
				if (file.Hashes == null || file.Hashes.Count == 0) {
					return true;
				}
				byte[] hash;
				using (FileStream stream = File.OpenRead(root + file.Path)) {
					hash = MD5.ComputeHash(stream);
				}
				StringBuilder hashSB = new StringBuilder(hash.Length);
				for (int i = 0; i < hash.Length; i++) {
					hashSB.Append(hash[i].ToString("X2"));
				}
				string strHash = hashSB.ToString();
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
				byte[] hash;
				using (FileStream stream = File.OpenRead(path)) {
					hash = MD5.ComputeHash(stream);
				}
				StringBuilder hashSB = new StringBuilder(hash.Length);
				for (int i = 0; i < hash.Length; i++) {
					hashSB.Append(hash[i].ToString("X2"));
				}
				string strHash = hashSB.ToString();
				if (correctHash.ToUpper().Equals(strHash)) {
					return true;
				}
			}
			return false;
		}
	}
}
