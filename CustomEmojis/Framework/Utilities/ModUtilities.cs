
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace CustomEmojis.Framework.Utilities {

	public class ModUtilities {

		public static string GetParentFolder(string path) {
			string parentFolder = "";
			try {
				parentFolder = GetParentFolder(Path.GetDirectoryName(path), path);
			} catch(ArgumentException) {
			}
			return parentFolder;
		}

		private static string GetParentFolder(string path, string lastPath) {
			if(!String.IsNullOrWhiteSpace(path)) {
				lastPath = path;
				return GetParentFolder(Path.GetDirectoryName(path), lastPath);
			} else {
				return lastPath;
			}
		}

		/// <summary>
		///  Return a <code>IEnumerable<string></code> of file path that matches at least one of the patters. The search is executed in parallel.
		/// </summary>
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

		/*
		/// <summary>
		/// Construct a derived class of from a base class
		/// </summary>
		/// <typeparam name="F">Type of base class</typeparam>
		/// <typeparam name="T">Type of class you want</typeparam>
		/// <param name="baseClass">the instance of the base class</param>
		/// <returns></returns>
		public static T Construct<T>(Type baseClassType, object baseClassInstance) where T : new() {

			// Create derived instance
			T derived = new T();

			if(baseClassInstance.GetType().IsSubclassOf(baseClassType)) {

				// Get all base class properties
				PropertyInfo[] properties = baseClassInstance.GetType().GetProperties();

				foreach(PropertyInfo basePropertyInfo in properties) {

					// Get derived matching property
					PropertyInfo derivedPropertyInfo = typeof(T).GetProperty(basePropertyInfo.Name, basePropertyInfo.PropertyType);

					// this property must not be index property
					if(derivedPropertyInfo != null && derivedPropertyInfo.GetSetMethod() != null && basePropertyInfo.GetIndexParameters().Length == 0 && derivedPropertyInfo.GetIndexParameters().Length == 0) {
						derivedPropertyInfo.SetValue(derived, derivedPropertyInfo.GetValue(baseClassInstance, null), null);
					}
				}

			}

			return derived;
		}
		*/

		/// <summary>
		/// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T CloneJson<T>(T source) {

			// Don't serialize a null object, simply return the default for that object
			if(source == null) {
				return default(T);
			}

			// initialize inner objects individually
			// for example in default constructor some list property initialized with some values,
			// but in 'source' these items are cleaned -
			// without ObjectCreationHandling.Replace default constructor values will be added to result
			var deserializeSettings = new JsonSerializerSettings {
				ObjectCreationHandling = ObjectCreationHandling.Replace
			};

			return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
		}

		// Source: https://www.codeproject.com/Articles/42221/Constructing-an-instance-class-from-its-base-class
		/// <summary>
		/// Construct a derived class of from a base class
		/// </summary>
		/// <typeparam name="F">Type of base class</typeparam>
		/// <typeparam name="T">Type of class you want</typeparam>
		/// <param name="baseClass">the instance of the base class</param>
		/// <returns></returns>
		public static T Construct<F, T>(F baseClass) where T : F, new() {

			// Create derived instance
			T derived = new T();

			// Get all base class properties
			PropertyInfo[] properties = typeof(F).GetProperties();

			foreach(PropertyInfo basePropertyInfo in properties) {

				// Get derived matching property
				PropertyInfo derivedPropertyInfo = typeof(T).GetProperty(basePropertyInfo.Name, basePropertyInfo.PropertyType);

				// this property must not be index property
				if(derivedPropertyInfo != null && derivedPropertyInfo.GetSetMethod() != null && basePropertyInfo.GetIndexParameters().Length == 0 && derivedPropertyInfo.GetIndexParameters().Length == 0) {
					derivedPropertyInfo.SetValue(derived, derivedPropertyInfo.GetValue(baseClass, null), null);
				}
			}

			return derived;
		}

	}

}
