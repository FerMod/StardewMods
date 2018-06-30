
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

public class ModData {

	[JsonIgnore]
	public bool IsDataSaved { get; set; } = false;

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
		return FilesChanged || !IsDataSaved;
	}

	public bool ShouldGenerateTexture() {
		return FilesChanged;
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
			/*
			ModEntry.ModMonitor.Log($"Found Files: ");
			foreach(var item in fileEnumeration.ToList()) {
				ModEntry.ModMonitor.Log($"{item}");
			}
			*/
			//IEnumerable<string> differentValues = FilesChecksums.FirstKeys.Except(fileEnumeration.Select(x => ModUtilities.GetRelativePath(ModFolder, x)));
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
				if(path != filePath) {
					FilesChecksums.RemoveByFirst(filePath);
					IsDataSaved = false; // Mark as the file path changed
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
			FilesChecksums.TryAdd(path, fileHash);

		} else {

			// If it isnt a directory nor a file, and its in the checksum dictionary, remove from it
			string relativePath = ModUtilities.GetRelativePath(ModFolder, absolutePath);
			if(FilesChecksums.TryRemoveByFirst(relativePath)) {
				FilesChanged = true;
			}

		}

		return FilesChanged;
	}

}
