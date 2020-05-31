using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BlazorPlus;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.Razor;
using Newtonsoft.Json;

namespace BlazorCefApp.Demos.RazorLive
{
	public partial class RazorLive
	{
		const string DEMO_CODE = @"

@code
{

	void SayHello()
	{
		BlazorPlus.BlazorSession.Current.Alert(""Hello"",""Cooooollll"");
	}

}

<button class='btn btn-primary' @onclick=""SayHello"">Hello</button>

";


		string Normalize(string code)
		{
			return code.Replace("\r", "");
		}

		[Inject]
		public IWebHostEnvironment WHE { get; set; }

		Microsoft.CodeAnalysis.PortableExecutableReference[] refs;
		TagHelperDescriptor[] tagHelpers;
		List<RazorExtension> exts;

		byte[] CompileCode(string code)
		{
			typeof(EventHandlers).ToString();
			typeof(EditContext).ToString();//ensure more assemblies..

			if (refs == null)
			{
				refs = AppDomain.CurrentDomain.GetAssemblies().Select(v =>
				{
					try
					{
						return Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(v.Location);
					}
					catch
					{
						return null;
					}
				}).Where(v => v != null).ToArray();
			}

			if (exts == null)
			{
				exts = new List<RazorExtension>();
				exts.Add(new AssemblyExtension("ComponentBase", typeof(ComponentBase).Assembly));
				exts.Add(new AssemblyExtension("ComponentWeb", typeof(BindAttributes).Assembly));

				foreach (AssemblyName asm in this.GetType().Assembly.GetReferencedAssemblies())
				{
					exts.Add(new AssemblyExtension(asm.FullName, AppDomain.CurrentDomain.Load(asm.FullName)));
				}
			}

			var config = RazorConfiguration.Create(RazorLanguageVersion.Version_3_0, "Default", exts);

			var proj = new MyProject();

			if (tagHelpers == null)
			{
				var rpe1 = RazorProjectEngine.Create(config, proj, b =>
				{
					//Check Microsoft.AspNetCore.Razor.Tools.DiscoverCommand
					b.Features.Add(new DefaultMetadataReferenceFeature
					{
						References = refs
					});
					b.Features.Add(new CompilationTagHelperFeature());
					b.Features.Add(new DefaultTagHelperDescriptorProvider());
					CompilerFeatures.Register(b);
				});

				tagHelpers = rpe1.Engine.Features.OfType<ITagHelperFeature>().Single().GetDescriptors().ToArray();
			}

			if (code == null)
				return null;

			var engine = RazorProjectEngine.Create(config, proj, b =>
			{
				var mf = new MyFeatureV2() { TagHelpers = tagHelpers };
				b.Features.Add(mf);
			});

			var projitem = new MyProjectItem() { Name = "MyComponent.razor", Code = code };

			var razorDoc = engine.Process(projitem);

			var csdoc = razorDoc.GetCSharpDocument();

			foreach (var diag in csdoc.Diagnostics)
			{
				switch (diag.Id)
				{
					case "RZ9980":
					case "RZ9981":
						throw new Exception("Error : " + diag.Id);
				}
			}

			string targetCode = csdoc.GeneratedCode;

			if (string.IsNullOrEmpty(targetCode) && csdoc.Diagnostics.Count != 0)
				return null;

			var invalidAttrChars = new char[] { '@', '/', '\\' };
			foreach (string eachline in targetCode.Split('\n'))
			{
				var line = eachline.Trim();
				if (line.StartsWith("__builder.AddAttribute"))
				{
					int p1 = line.IndexOf('"') + 1;
					int p2 = line.IndexOf('"', p1);
					var attrname = line.Substring(p1, p2 - p1);

					if (attrname.IndexOfAny(invalidAttrChars) != -1)
						throw new Exception("Invalid attribute name [" + attrname + "]");

				}
			}

			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
			watch.Start();


			var st = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(csdoc.GeneratedCode);
			var csc = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create("new" + DateTime.Now.Ticks)
				.WithOptions(new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary))
				.AddSyntaxTrees(st)
				.AddReferences(refs);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			var result = csc.Emit(ms);
			if (!result.Success)
				throw new Exception(result.Diagnostics.First(v => v.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error).ToString());

			return ms.ToArray();
			

		}
		

		class MyFeatureV2 : IConfigureRazorCodeGenerationOptionsFeature, ITagHelperFeature
		{
			public int Order => 1;

			public RazorEngine Engine { get; set; }

			public void Configure(RazorCodeGenerationOptionsBuilder options)
			{
				options.RootNamespace = "HelloAssembly";
			}

			public IReadOnlyList<TagHelperDescriptor> TagHelpers { get; set; }

			public IReadOnlyList<TagHelperDescriptor> GetDescriptors()
			{
				return TagHelpers;

			}

		}


		class MyProjectItem : RazorProjectItem
		{
			public override string FileKind => FileKinds.Component;

			public override string BasePath => "/";

			public override string FilePath => "/" + Name;

			public override string PhysicalPath => "/" + Name;

			public override bool Exists => true;

			public string Name;

			public string Code;

			public override Stream Read()
			{
				return new MemoryStream(Encoding.UTF8.GetBytes(Code));
			}

		}


		class MyProject : RazorProjectFileSystem
		{
			public override IEnumerable<RazorProjectItem> EnumerateItems(string basePath)
			{
				throw new NotImplementedException();
			}

			[Obsolete]
			public override RazorProjectItem GetItem(string path)
			{
				throw new NotImplementedException();
			}

			public override RazorProjectItem GetItem(string path, string fileKind)
			{
				if (path == "/_Imports.razor")
					return new MyProjectItem() { Name = "_Imports.razor", Code = @"

@using Microsoft.AspNetCore.Authorization
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop

" };


				throw new NotImplementedException(fileKind + ":" + path);
			}
		}
	}
}
