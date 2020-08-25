//using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;
using static System.Console;

namespace CompareVSInstallations {
	/// <summary>Compares two visual studio .vsconfig files in terms of installed components difference.</summary>
	class Program {

		static void Main(string[] args) {
			var ic = new VSInstallationsComperarer(args[0], args[1]);
			WriteLine($@"Comparing {args[0]}");
			WriteLine($@"with {args[1]}");
			WriteLine($@"...");
			var r = ic.compare().Result;
			printDifferences(r);
			ReadKey();
		}

		private static void printDifferences((VSInstalConfig ae, VSInstalConfig be) r) {
			var (a, b) = r;
			printComponets("first", a.components, ConsoleColor.Green);
			printComponets("second", b.components, ConsoleColor.Red);
			if (a.components.Count > 0 | b.components.Count > 0) return;
			var pc = ForegroundColor; ForegroundColor = ConsoleColor.Green;
			WriteLine("No difference found between config files");
			ForegroundColor = pc;
		}

		private static void printComponets(string name, HashSet<string> components, ConsoleColor color) {
			if (components.Count == 0) {
				WriteLine($@"No exlusive components found in {name} config");
				return;
			}
			WriteLine($@"Found {components.Count} components exclusive only to {name} config:");
			var pc = ForegroundColor; ForegroundColor = color;
			foreach (var c in components) WriteLine($@"	{c}");
			ForegroundColor = pc;
		}

	}

	/// <summary>Compares installation files of two visuals studios (.vsconfig files exported from Visual studio installer).</summary>
	public class VSInstallationsComperarer {
		private static JsonSerializerOptions ops = new JsonSerializerOptions() {
			Converters = { new VersionConverter() }
		};
		/// <summary>First file to compare to second.</summary>
		public string fileA { get; set; }
		/// <summary>Second file to compare.</summary>
		public string fileB { get; set; }

		/// <summary>Creating new comparer, setting initial file paths.</summary>
		public VSInstallationsComperarer(string fa, string fb) {
			fileA = fa;
			fileB = fb;
		}

		/// <summary>Compares both files <see cref="fileA"/> and <see cref="fileB"/> and returns two config objects, each containing only compoents that are exclusive to the cofiguration file.
		/// If <see cref="VSInstalConfig.version"/> is the same for both configs it will be set to null.</summary>
		/// <returns></returns>
		public async Task<(VSInstalConfig ae, VSInstalConfig be)> compare() {
			string fa; string fb;
			lock (ops) { fa = fileA; fb = fileB;}
			var t = Task.Run(() => {
				var a = JsonSerializer.Deserialize<VSInstalConfig>(File.ReadAllText(fa), ops);
				var b = JsonSerializer.Deserialize<VSInstalConfig>(File.ReadAllText(fb), ops);
				difference(a, b);
				return (a, b);
			});
			await t; return t.Result;
		}

		/// <summary>Modifies both configurations so that each will containin only compoents that are exclusive to it.
		/// If <see cref="VSInstalConfig.version"/> is the same for both configs it will be set to null.</summary>
		public void difference(VSInstalConfig a, VSInstalConfig b) {
			var acc = new HashSet<string>(a.components);
			a.components.ExceptWith(b.components);
			b.components.ExceptWith(acc);
			if (a.version == b.version) a.version = b.version = null;
		}
	}

	/// <summary>Represents installation confiuguration of the Visual Studio IDE.</summary>
	public class VSInstalConfig {
		/// <summary>??</summary>
		public Version version { get; set; }
		/// <summary>List of individual components that can be selected to install.</summary>
		public HashSet<string> components { get; set; }
	}

	/// <summary>Converter for <see cref="Version"/> class.</summary>
	public class VersionConverter : JsonConverter<Version> {
		/// <inheritdoc/>
		public override Version Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
			return Version.Parse(reader.GetString());
		}

		/// <inheritdoc/>
		public override void Write(Utf8JsonWriter writer, Version value, JsonSerializerOptions options) {
			writer.WriteStringValue(value.ToString());
		}
	}



}
