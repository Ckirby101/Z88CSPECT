using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Z88CSPECT
{
	class Program
	{

		public class MapSymbol
		{
			public string label;
			public string address;
			public string type;
			public string local;
			public string section;
			public string other;
			public string compiler;
			public string path;
		}

		/// -------------------------------------------------------------------------------------------------
		/// <summary> Main entry-point for this application. </summary>
		///
		/// <remarks> 05/11/2017. </remarks>
		///
		/// <param name="args"> An array of command-line argument strings. </param>
		/// -------------------------------------------------------------------------------------------------
		static void Main(string[] args)
		{
			Console.WriteLine("Covert Z88DK map file to SNASM format.");


			if (args.Length == 2)
			{
				ParseFile(args[0], args[1]);
			}

		}

		/// -------------------------------------------------------------------------------------------------
		/// <summary> Parse file. </summary>
		///
		/// <remarks> 05/11/2017. </remarks>
		///
		/// <param name="infile">  The infile. </param>
		/// <param name="outfile"> The outfile. </param>
		/// -------------------------------------------------------------------------------------------------
		static void ParseFile(string infile, string outfile)
		{
			Console.WriteLine("Parsing");

			Regex reg = new Regex(
				@"(?<label>\w+)\s+= \$(?<address>[a-fA-F0-9]+) ; (?<type>[a-zA-Z]*), (?<local>[a-zA-Z]*), (?<other>\w*), (?<section>\w*), (?<compiler>\w*), (?<path>[a-zA-Z:\\0-9.)_]*)");

			List<MapSymbol> mapSymbols = new List<MapSymbol>();
			string[] maplines = File.ReadAllLines(infile);

			if (maplines.Length <= 0)
			{
				Console.WriteLine(infile + " empty");
				return;
			}

			foreach (string s in maplines)
			{
				if (!string.IsNullOrEmpty(s))
				{
					var match = reg.Match(s);

					MapSymbol ms = new MapSymbol();
					ms.label = match.Groups["label"].ToString();
					ms.address = match.Groups["address"].ToString();
					ms.type = match.Groups["type"].ToString();
					ms.local = match.Groups["local"].ToString();
					ms.other = match.Groups["other"].ToString();
					ms.section = match.Groups["section"].ToString();
					ms.path = match.Groups["path"].ToString();


					mapSymbols.Add(ms);
				}
			}


			Console.WriteLine("Found "+mapSymbols.Count+" symbols");

			System.IO.StreamWriter file = new System.IO.StreamWriter(outfile);


			foreach (MapSymbol ms in mapSymbols)
			{
				if (ms.path.Contains(".c:"))
				{
					file.WriteLine("0000"+ms.address.ToUpper()+" "+ms.label);

				}

			}

			file.Close();

		}
	}
}
