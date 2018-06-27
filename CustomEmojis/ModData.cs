
using CustomEmojis;
using CustomEmojis.Framework.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ModData {

	public string ModFolder { get; set; }
	public bool FilesChanged { get; set; }
	public int EmojisAdded { get; set; }
	public List<string> WatchedPaths { get; set; }
	public Dictionary<string, string> FilesChecksums { get; set; }

	public ModData() {

		FilesChanged = true;
		EmojisAdded = 0;
		WatchedPaths = new List<string>();
		FilesChecksums = new Dictionary<string, string>();

	}

	//public void UpdateFilesChecksum(string modFolder, string[] fileExtensions) {

	//	if(ModFolder != modFolder) {
	//		ModFolder = modFolder;
	//	}

	//	if(WatchedPaths != null) {
	//		UpdateFilesChecksum(modFolder, fileExtensions, WatchedPaths);
	//	}
	//}

	//public void UpdateFilesChecksum(string modFolder, string[] imageExtensions, List<string> watchedFiles, bool watchedSubfolder = false) {

	//	foreach(string file in watchedFiles.ToList()) {
	//		string absolutePath = Path.Combine(modFolder, file);
	//		if(File.Exists(absolutePath) || Directory.Exists(absolutePath)) {

	//			if(File.GetAttributes(absolutePath).HasFlag(FileAttributes.Directory)) {
	//				string relativePath = ModUtilities.GetRelativePath(modFolder, absolutePath);
	//				if(WatchedPaths.Contains(relativePath)) {
	//					IEnumerable<string> files = ModUtilities.GetFiles(Path.Combine(modFolder, file), imageExtensions, SearchOption.AllDirectories);
	//					UpdateFilesChecksum(modFolder, imageExtensions, files.ToList(), true);
	//				}
	//			} else {
	//				//FilesChecksums.Remove(relativePath);
	//				string relativePath = ModUtilities.GetRelativePath(modFolder, file);
	//				if(watchedSubfolder || WatchedPaths.Contains(relativePath)) {
	//					FilesChecksums[relativePath] = ModUtilities.CalculateFileHash(absolutePath);
	//				} else {
	//					FilesChecksums.Remove(relativePath);
	//				}
	//			}
	//		} else {
	//			WatchedPaths.Remove(file);
	//			string relativePath = ModUtilities.GetRelativePath(modFolder, absolutePath);
	//			if(FilesChecksums.ContainsKey(relativePath)) {
	//				FilesChecksums.Remove(relativePath);
	//			}
	//		}
	//	}

	//	FilesChanged = false;
	//}

	public bool Checksum(string[] imageExtensions) {
		if(!FilesChanged) {
			foreach(string path in WatchedPaths) {
				FilesChanged = Checksum(path, imageExtensions);
			}
		}
		return FilesChanged;
	}

	public bool Checksum(string path, string[] imageExtensions) {

		string absolutePath = Path.Combine(ModFolder, path);

		if(Directory.Exists(absolutePath)) {

			IEnumerable<string> fileEnumeration = ModUtilities.GetFiles(absolutePath, imageExtensions, SearchOption.AllDirectories);
			ModEntry.ModMonitor.Log($"Found Files: ");
			foreach(var item in fileEnumeration.ToList()) {
				ModEntry.ModMonitor.Log($"{item}");
			}
			IEnumerable<string> differentValues = FilesChecksums.Keys.ToList().Except(fileEnumeration.Select(x => ModUtilities.GetRelativePath(ModFolder, x)));
			ModEntry.ModMonitor.Log($"For each different value:");
			foreach(string key in differentValues.ToList()) {
				ModEntry.ModMonitor.Log($"{key}");
				if(FilesChecksums.ContainsKey(key)) {
					FilesChecksums.Remove(key);
				}
			}

			foreach(string file in fileEnumeration.ToList()) {
				string relativePath = ModUtilities.GetRelativePath(ModFolder, file);
				FilesChanged = Checksum(relativePath, imageExtensions);
			}

		} else if(File.Exists(absolutePath)) {

			string fileHash = ModUtilities.CalculateFileHash(absolutePath);

			string key;
			if(FilesChecksums.ContainsValue(fileHash)) {
				key = FilesChecksums.FirstOrDefault(x => x.Value == fileHash).Key;
				if(path != key) {
					FilesChecksums.Remove(key);
				}
			}

			if(FilesChecksums.TryGetValue(path, out string hash)) {

				if(fileHash != hash) {
					FilesChecksums[path] = fileHash;
					FilesChanged = true;
				}

			} else {
				FilesChecksums[path] = fileHash;
				FilesChanged = true;
			}

		} else {

			string relativePath = ModUtilities.GetRelativePath(ModFolder, absolutePath);
			if(FilesChecksums.ContainsKey(relativePath)) {
				FilesChecksums.Remove(relativePath);
				FilesChanged = true;
			}

		}

		return FilesChanged;
	}

	//public bool Checksum() {

	//	for(int i = 0; i < WatchedPaths.Count; i++) {
	//		KeyValuePair<string, string> entry = FilesChecksums.ElementAt(i);
	//		IEnumerable<string> files = ModUtilities.GetFiles(Path.Combine(ModFolder, entry.Key), WatchedFilesExtension, SearchOption.AllDirectories);
	//		foreach(string file in files) {
	//			string absolutePath = Path.Combine(ModFolder, file);
	//			string relativePath = ModUtilities.GetRelativePath(ModFolder, absolutePath);
	//			if(WatchedPaths.Contains(relativePath)) {
	//			}
	//		}
	//	}
	//	/*
	//	for(int i = 0; i < FilesChecksums.Count; i++) {
	//		KeyValuePair<string, string> entry = FilesChecksums.ElementAt(i);
	//		string absolutePath = Path.Combine(ModFolder, entry.Key);
	//		if(!File.Exists(absolutePath)) {
	//			FilesChanged = true;
	//			FilesChecksums.Remove(entry.Key);
	//		} else {
	//			string fileHash = ModUtilities.CalculateFileHash(absolutePath);
	//			string relativePath = ModUtilities.GetRelativePath(ModFolder, absolutePath);
	//			if(entry.Value != fileHash || !WatchedFiles.Contains(relativePath)) {
	//				FilesChanged = true;
	//				FilesChecksums[entry.Key] = fileHash;
	//			}
	//		}

	//	}
	//	*/
	//	return FilesChanged;
	//}

}
