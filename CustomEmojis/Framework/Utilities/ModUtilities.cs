
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace CustomEmojis.Framework.Utilities {

	public static class ModUtilities {

		// Takes same patterns, and executes in parallel
		public static IEnumerable<string> GetFiles(string path, string[] searchPatterns, SearchOption searchOption = SearchOption.TopDirectoryOnly) {
			return searchPatterns.AsParallel().SelectMany(searchPattern => Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => s.EndsWith(searchPattern)));
		}

		public static string CalculateFileHash(string filePath) {

			using(HashAlgorithm hashAlgorithm = SHA256.Create()) {

				using(FileStream stream = File.OpenRead(filePath)) {
					byte[] hash = hashAlgorithm.ComputeHash(stream);
					return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}

			}

		}

		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static String GetRelativePath(String fromPath, String toPath) {

			if(String.IsNullOrEmpty(fromPath)) {
				throw new ArgumentNullException("fromPath");
			}
			if(String.IsNullOrEmpty(toPath)) {
				throw new ArgumentNullException("toPath");
			}

			fromPath += Path.DirectorySeparatorChar;

			Uri fromUri = new Uri(fromPath);
			Uri toUri = new Uri(toPath);

			if(fromUri.Scheme != toUri.Scheme) { // path can't be made relative.
				return toPath;
			}

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if(toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase)) {
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}

	}

}
