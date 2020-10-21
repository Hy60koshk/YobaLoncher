using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YobaLoncher {
	class DownloadProgressTracker {
		private long totalFileSize_;
		private readonly int sampleSize_;
		private readonly TimeSpan valueDelay_;

		private DateTime lastUpdateCalculated_;
		private long previousProgress_;

		private double cachedSpeed_;

		private Queue<Tuple<DateTime, long>> changes_ = new Queue<Tuple<DateTime, long>>();

		public DownloadProgressTracker(int sampleSize, TimeSpan valueDelay) {
			lastUpdateCalculated_ = DateTime.Now;
			sampleSize_ = sampleSize;
			valueDelay_ = valueDelay;
		}

		public void Reset() {
			previousProgress_ = 0;
		}

		public void SetProgress(long bytesReceived, long totalBytesToReceive) {
			totalFileSize_ = totalBytesToReceive;

			long diff = bytesReceived - previousProgress_;
			if (diff <= 0)
				return;

			previousProgress_ = bytesReceived;

			changes_.Enqueue(new Tuple<DateTime, long>(DateTime.Now, diff));
			while (changes_.Count > sampleSize_)
				changes_.Dequeue();
		}

		public double GetProgress() {
			return previousProgress_ / (double)totalFileSize_;
		}

		public string GetProgressString() {
			return String.Format("{0:P0}", GetProgress());
		}

		public string GetBytesPerSecondString() {
			double speed = GetBytesPerSecond();
			string[] prefix = new string[] { "", "K", "M", "G" };

			int index = 0;
			while (speed > 1024 && index < prefix.Length - 1) {
				speed /= 1024;
				index++;
			}

			int intLen = ((int)speed).ToString().Length;
			int decimals = 3 - intLen;
			if (decimals < 0)
				decimals = 0;

			string format = String.Format("{{0:F{0}}}", decimals) + "{1}B/s";

			return String.Format(format, speed, prefix[index]);
		}

		public double GetBytesPerSecond() {
			if (DateTime.Now >= lastUpdateCalculated_ + valueDelay_) {
				lastUpdateCalculated_ = DateTime.Now;
				cachedSpeed_ = GetRateInternal();
			}

			return cachedSpeed_;
		}

		private double GetRateInternal() {
			if (changes_.Count == 0)
				return 0;

			TimeSpan timespan = changes_.Last().Item1 - changes_.First().Item1;
			long bytes = changes_.Sum(t => t.Item2);

			double rate = bytes / timespan.TotalSeconds;

			if (double.IsInfinity(rate) || double.IsNaN(rate))
				return 0;

			return rate;
		}
	}
}
