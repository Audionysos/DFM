using com.audionysos.data.items;
using com.audionysos.files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.IO.Path;
using static System.IO.File;
using HtmlAgilityPack;
using Markdig;
using com.audionysos.generics.diagnostinc;

namespace com.audionysos.data.dfm.docFiles {

	public class DocFiles : DFMModule {

		public override ItemProvider itemProvider => new SDocItemsProvider();

		public DocFiles(DFM sytem) : base(sytem) {

		}

		#region Extras
		public List<Issue> Diagnose() {
			var iss = new List<Issue>();
			foreach (var t in top.Enum<Topic>()) {
				if (!t.doc.mainMd.exist) iss.Add(new Issue($"{t} does not specify it main source file."));
				t.examineLinks();
				Console.WriteLine(t.ToString());
			}
			return iss;
		}

		public void GenrateDocs() {
			foreach (var t in top.Enum<Topic>()) {
				t.ToHTML();
			}
		}
		#endregion

	}


	public class Topic : ItemInterface {
		public static MarkdownPipeline mdPipie = new MarkdownPipelineBuilder()
			.UseAdvancedExtensions().Build();

		/// <summary>Documentation folder for DFM.</summary>
		public DocPah doc { get; private set; }

		public Topic(Item i) : base(i) {
			//doc = i.path.Add("doc");
			doc = new DocPah(i.path);
		}

		public void examineLinks() {
			var hd = new HtmlDocument(); hd.Load(doc.mainHTML);
			var b = hd.DocumentNode;
			var iss = new List<Issue>();
			foreach (var n in b.DescendantsAndSelf()) {
				foreach (var a in n.ChildAttributes("href")) {
					Console.WriteLine("Attribute: " + a.Value);
					var lnk = a.Value;
					var p = new Pah(lnk);
					if (!p.exist) iss.Add(
						new Issue($@"Could not find entry of path ""{p}"" specfied in ""{doc.mainHTML}"" document.", Impact.IMPORTANT));
					else if (p.isAbsolute) iss.Add(
						new Issue($@"The path ""{p}"" specfied in ""{doc.mainHTML}"" document is absolute. This may cause problems when moving to another data system."));
				}
			}
		}

		public void ToHTML() {
			var rawHtml = Markdown.ToHtml(doc.mainMd.text(), mdPipie);
			embedInBasePage(rawHtml).Save(doc.mainHTML);
			//File.WriteAllText(doc.mainHTML, rawHtml);
		}

		private HtmlDocument embedInBasePage(string rawHtml) {
			var dp = item.ip.get<SDocItemsProvider>();
			var hd = new HtmlDocument();
			hd.LoadHtml(dp.baseHTML);
			var bd = hd.DocumentNode.SelectSingleNode("//body/div");
			bd.InnerHtml = rawHtml;
			var css = hd.DocumentNode.SelectSingleNode("//head/link");
			var cssR = doc.routeTo(dp.defaultCSS);
			css.SetAttributeValue("href", cssR);
			return hd;
		}

	}

	public class DocPah : Pah {
		/// <summary>Path to main document in markdown format.</summary>
		public Pah mainMd { get; private set; }
		/// <summary>Path to main document in HTML format.</summary>
		public Pah mainHTML { get; private set; }

		public DocPah(string root) : base(root) { }
		public DocPah(Pah parent) : base(parent, "doc") {
			mainMd = Add(parent.name + ".md");
			mainHTML = Add(parent.name + ".html");
		}
	}

	public class SDocItemsProvider : ItemProvider {
		/// <summary>Base html page for doc embeding.</summary>
		public string baseHTML { get; set; }
		/// <summary>Path to defualt style file.</summary>
		public Pah defaultCSS { get; set; }

		public SDocItemsProvider(ItemProvider root = null) : base(root) {
			Add(new SystemFileItemsProvider(this));
		}

		protected override Item createItem(Pah p) {
			return isDocScheme(p) ? new Item<Topic>(p, root) : null;
		}

		protected override I create<I>(Pah p, ItemInfResolveContext c) {
			//if(c.get<FilE>())
				//this.
			return null;
		}

		public static bool isDocScheme(string dir) {
			if (!Directory.Exists(dir)) return false;
			if (!Exists(Combine(dir, "doc", "dfm_System"))) return false;
			return true;
		}

	}
}
