using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using BlazorPlus;


namespace BlazorCefApp
{
	public class WebCustomizeSession : BlazorSession
	{
		public IWebHostEnvironment WebHostEnvironment { get; private set; }

		public IHttpContextAccessor HttpContextAccessor { get; private set; }

		public WebCustomizeSession(Microsoft.JSInterop.IJSRuntime jsr,NavigationManager nav
			, IWebHostEnvironment whe
			, IHttpContextAccessor hca)
			: base(jsr,nav)
		{
			WebHostEnvironment = whe;
			HttpContextAccessor = hca;
		}

		public override Type TypeGetUIDialogAlert(UIDialogOption option)
		{
			return typeof(CustomizeUI.UIDialogAlert);
		}
		public override Type TypeGetUIDialogConfirm(UIDialogOption option)
		{
			return typeof(CustomizeUI.UIDialogConfirm);
		}
		public override Type TypeGetUIDialogPrompt(UIDialogOption option)
		{
			return typeof(CustomizeUI.UIDialogPrompt);
		}

		//protected override bool IsValidBrowserUniqueID(string uid)
		//{
		//	return base.IsValidBrowserUniqueID(uid);
		//}
		//protected override string GenerateBrowserUniqueID()
		//{
		//	return base.GenerateBrowserUniqueID();
		//}

		//public override string TranslateTemplate(string code)
		//{
		//	return base.TranslateTemplate(code);
		//}

		//string GetJsonFilePath()
		//{
		//	//by default , the Browser.UniqueID is base64 string of 
		//	byte[] data = Convert.FromBase64String(this.Browser.UniqueID);
		//	string group = ((int)data[0]).ToString();
		//	string fname = BitConverter.ToString(data).Replace("-", "");
		//	string folder = System.IO.Path.Combine(WebHostEnvironment.ContentRootPath, "BrowserStringItems", group, fname);
		//	if (!System.IO.Directory.Exists(folder)) System.IO.Directory.CreateDirectory(folder);
		//	return System.IO.Path.Combine(folder, "stringitems.json");
		//}
		//protected override async Task<string> GetBrowserStringItemAsync(string key)
		//{
		//	string filepath = GetJsonFilePath();
		//	if (!System.IO.File.Exists(filepath))
		//		return null;
		//	string jsontext = await System.IO.File.ReadAllTextAsync(filepath);
		//	var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsontext);
		//	string val;
		//	dict.TryGetValue(key, out val);
		//	return val;
		//}
		//protected override async Task SetBrowserStringItemAsync(string key, string value)
		//{
		//	string filepath = GetJsonFilePath();
		//	string jsontext;
		//	Dictionary<string, string> dict;
		//	if (!System.IO.File.Exists(filepath))
		//	{
		//		dict = new Dictionary<string, string>();
		//	}
		//	else
		//	{
		//		jsontext = await System.IO.File.ReadAllTextAsync(filepath);
		//		dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsontext);
		//	}
		//	dict[key] = value;
		//	jsontext = System.Text.Json.JsonSerializer.Serialize(dict);
		//	await System.IO.File.WriteAllTextAsync(filepath, jsontext);
		//}
	}




}
