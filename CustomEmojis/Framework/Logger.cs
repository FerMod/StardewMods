
using StardewModdingAPI;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace CustomEmojis.Framework {

	internal class Logger : IDisposable {

		private static string FilePath { get; set; } = Directory.GetCurrentDirectory() + "\\Mods";
		private static IMonitor ModMonitor { get; set; }
		private static StreamWriter Stream { get; set; }

		public static void InitLogger(string filePath, bool append = true, IMonitor monitor = null) {
			SetFilePath(filePath, append);
			if(monitor != null) {
				SetMonitor(monitor);
			}
		}

		public static void SetMonitor(IMonitor monitor) {
			ModMonitor = monitor;
		}

		public static void SetFilePath(string filePath, bool append = true) {
			string folderPath = Path.GetDirectoryName(FilePath);
			if(folderPath == null) {
				throw new ArgumentException($"Log path '{FilePath}' not valid.");
			}
			Directory.CreateDirectory(folderPath);
			FilePath = filePath;
			Stream = new StreamWriter(FilePath, append) {
				AutoFlush = true
			};
		}

		public static void LogTrace(string message,
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

			WriteLine($"{time} {sb.ToString()}");
		}

		public static void Log(params string[] messages) {

			string time = "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
			int timeLength = time.Length + 1;

			WriteLine($"{time} {messages[0]}");
			ModMonitor?.Log(messages[0]);
			for(int i = 1; i < messages.Length; i++) {
				WriteLine($"{messages[i].PadLeft(messages[i].Length + timeLength)}");
				ModMonitor?.Log(messages[i]);
			}

		}

		public static void WriteLine(string message) {
			Write(message + "\r\n");
		}

		public static void Write(string message) {
			Stream.Write(message);
		}

		public void Dispose() {
			Stream.Dispose();
		}
	}

}
