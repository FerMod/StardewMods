
using CustomEmojis.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using CustomEmojis.Framework.Types;
using Newtonsoft.Json;
using CustomEmojis;
using System.Diagnostics;
using CustomEmojis.Framework;

public class ModData {

	[JsonIgnore]
	public bool DataChanged { get; set; } = false;

	public string ModFolder { get; set; } = Directory.GetCurrentDirectory();
	public bool FilesChanged { get; set; } = true;
	public List<string> WatchedPaths { get; set; } = new List<string>();
	public BiDictionary<string, string> FilesChecksums { get; set; } = new BiDictionary<string, string>();

	public ModData() {
	}

	public ModData(string modFolder) {
		ModFolder = modFolder;
	}

	public bool ShouldSaveData() {
		return FilesChanged || DataChanged;
	}

	public bool ShouldGenerateTexture() {
		return FilesChanged;
	}

	public void ShouldGenerateTexture(bool generate) {
		FilesChanged = generate;
	}

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
			fileEnumeration = fileEnumeration.OrderBy(s => s);

			/*
			ModEntry.ModMonitor.Log($"Found Files: ");
			foreach(var item in fileEnumeration.ToList()) {
				ModEntry.ModMonitor.Log($"{item}");
			}
			*/
			IEnumerable<string> temp = fileEnumeration.Select(x => x = Path.Combine(ModFolder, x)).Except(FilesChecksums.FirstKeys);
			List<string> t = temp.ToList();
			foreach(var item in temp.ToList()) {
				Logger.Log($"File checksum not found: {item}");
			}
			//IEnumerable<string> differentValues = FilesChecksums.FirstKeys.Except());
			//foreach(var item in differentValues.ToList()) {
			//	Logger.Log(item);
			//}

			//ModEntry.ModMonitor.Log($"For each different file:");
			//foreach(string filePath in differentValues.ToList()) {
			//	bool removed = FilesChecksums.TryRemoveByFirst(filePath);
			//	ModEntry.ModMonitor.Log($"File(removed? {removed}): {filePath}");
			//	//if(FilesChecksums.TryGetByFirst(filePath, out string hash)) {
			//	//	FilesChecksums.RemoveByFirst(filePath);
			//	//}
			//}

			foreach(string file in fileEnumeration.ToList()) {
				string relativePath = ModUtilities.GetRelativePath(ModFolder, file);
				FilesChanged = Checksum(relativePath, imageExtensions);
			}

		} else if(File.Exists(absolutePath)) {

			string fileHash = ModUtilities.CalculateFileHash(absolutePath);

			// Check if the calculated hash is already associated with a file path
			if(FilesChecksums.TryGetBySecond(fileHash, out string filePath)) {
				// If is different, remove the outdated path
				if(path != filePath && !File.Exists(Path.Combine(ModFolder, filePath))) {
					FilesChecksums.RemoveByFirst(filePath);
					DataChanged = true; // Mark as the file path changed
				}
			}

			// Check if the file path is already associated with a hash 
			if(FilesChecksums.TryGetByFirst(path, out string hash)) {
				// If the obtained hash is different than the calculated the file changed, remove the outdated hash
				if(fileHash != hash) {
					FilesChecksums.RemoveBySecond(hash);
					FilesChanged = true; // Mark as the files changed
				}
			}

			// Add the file to the checksum dictionary
			if(FilesChecksums.TryAdd(path, fileHash)) {
				DataChanged = true;
				FilesChanged = true;
			} else {
				Logger.Log($"not added: {path} {fileHash}");
			}

		} else {
			// If it isnt a directory nor a file, and its in the checksum dictionary, try removing from it
			string relativePath = ModUtilities.GetRelativePath(ModFolder, absolutePath);
			if(FilesChecksums.TryRemoveByFirst(relativePath)) {
				FilesChanged = true;
			}

		}

		return FilesChanged;
	}

}
