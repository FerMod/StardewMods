
using CustomEmojis.Framework.Utilities;
using MultiplayerEmotes;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class ModData {

	public string ModFolder { get; set; }
	public bool FilesChanged { get; set; }
	public int EmojisAdded { get; set; }
	public List<string> WatchedFiles { get; set; }
	public Dictionary<string, string> FilesChecksums { get; set; }

	public ModData() {

		FilesChanged = true;

		WatchedFiles = new List<string> {
			"vanillaEmojis.png",
			"sprites"
		};

		FilesChecksums = new Dictionary<string, string>();

		EmojisAdded = 0;

	}

	public void UpdateFilesChecksum(string modFolder, string[] imageExtensions) {

		if (ModFolder != modFolder) {
			ModFolder = modFolder;
		}

		if (WatchedFiles != null) {
			UpdateFilesChecksum(modFolder, imageExtensions, WatchedFiles);
		}
	}

	public void UpdateFilesChecksum(string modFolder, string[] imageExtensions, List<string> watchedFiles) {

		foreach (string file in watchedFiles) {

			if (File.GetAttributes(Path.Combine(modFolder, file)).HasFlag(FileAttributes.Directory)) {
				IEnumerable<string> files = ModUtilities.GetFiles(Path.Combine(modFolder, file), imageExtensions, SearchOption.AllDirectories);
				UpdateFilesChecksum(modFolder, imageExtensions, files.ToList());
			} else {
				string filePath = Path.Combine(modFolder, file);
				string relativePath = ModUtilities.GetRelativePath(modFolder, filePath);
				if (File.Exists(filePath)) {
					FilesChecksums[relativePath] = ModUtilities.CalculateFileHash(filePath);
				}
			}

		}

	}

	public bool Checksum() {
		FilesChanged = false;

		foreach (KeyValuePair<string, string> entry in FilesChecksums) {
			string relativePath = Path.Combine(ModFolder, entry.Key);
			if (!File.Exists(relativePath)) {
				FilesChanged = true;
			} else {
				string key = ModUtilities.CalculateFileHash(relativePath);
				if (!FilesChecksums.ContainsKey(key) || entry.Key != key) {
					FilesChanged = true;
				}
			}
		}
		return FilesChanged;
	}

}
