
using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace CustomEmojis.Framework {

	internal class Logger {

		private static string FilePath { get; set; } = Directory.GetCurrentDirectory() + "\\Mods";
		private static IMonitor ModMonitor { get; set; }

		public static void SetOutput(string filePath, IMonitor monitor = null) {
			SetFilePath(filePath);
			if(monitor != null) {
				SetMonitor(monitor);
			}
		}

		public static void SetMonitor(IMonitor monitor) {
			ModMonitor = monitor;
		}

		public static void SetFilePath(string filePath) {
			string folderPath = Path.GetDirectoryName(FilePath);
			if(folderPath == null) {
				throw new ArgumentException($"Log path '{FilePath}' not valid.");
			}
			Directory.CreateDirectory(folderPath);
			FilePath = filePath;
		}

		public static void LogTrace(string message, bool append = true,
		[CallerMemberName] string memberName = "",
		[CallerFilePath] string sourceFilePath = "",
		[CallerLineNumber] int sourceLineNumber = 0) {

			string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
			int timeLength = time.Length + 1;
			string callerMemberName = "member name: " + memberName;
			string callerFilePath = "source file path: " + sourceFilePath;
			string callerLineNumber = "source line number: " + sourceLineNumber;

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(message);
			sb.AppendLine(callerMemberName.PadLeft(callerMemberName.Length));
			sb.AppendLine(callerFilePath.PadLeft(callerFilePath.Length));
			sb.Append(callerLineNumber.PadLeft(callerLineNumber.Length));

			ModMonitor?.Log(sb.ToString());

			sb.Clear();

			sb.AppendLine(message);
			sb.AppendLine(callerMemberName.PadLeft(callerMemberName.Length + timeLength));
			sb.AppendLine(callerFilePath.PadLeft(callerFilePath.Length + timeLength));
			sb.Append(callerLineNumber.PadLeft(callerLineNumber.Length + timeLength));

			WriteLine($"{time} {sb.ToString()}", append);
		}

		public static void Log(string message, bool append = true) {
			string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
			WriteLine($"{time} {message}", append);
			ModMonitor?.Log(message);
		}

		public static void WriteLine(string message, bool append = true) {
			Write(message + "\r\n", append);
		}

		public static void Write(string message, bool append = true) {
			using(StreamWriter stream = new StreamWriter(FilePath, append)) {
				stream.AutoFlush = true;
				stream.Write(message);
			}
		}

	}

}
