using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using CefLite;

namespace BlazorCefApp
{
	public class Program
	{

		[STAThread]
		static public void Main(string[] args)
		{
			//TODO:Change the project type to "Windows Application" to hide the console
			//If you start the app via Visual Studio , the VS Command Prompt will always show
			CefWin.PrintDebugInformation = true;  //show debug information in console

			CefWin.ApplicationTitle = "MyBlazorApp"; //as the Default Title

			CefWin.ShowSplashScreen("wwwroot/splash.jpg");  //or show System.Drawing.Image from embedded resource 

			if (CefWin.ActivateExistingApp())   // Optional, only allow one instance running
			{
				Console.WriteLine("Anoter instance is running , So this instance quit.");
				return;
			}

			//CefWin.SettingAutoSetUserDataStoragePath = false;
			//CefWin.SettingAutoSetCacheStoragePath = false;

			CefWin.SetEnableHighDPISupport();

			CefWin.SearchLibCefSubPathList.Add("chromium");         // search ./chromium/ for libcef.dll
			CefInitState initState = CefWin.SearchAndInitialize();

			if (initState != CefInitState.Initialized)
			{
				if (initState == CefInitState.Failed)
				{
					System.Windows.MessageBox.Show("Failed to start application\r\nCheck the github page about how to deploy the libcef.dll", "Error"
					   , System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				}
				return;
			}

			using IHost host = CreateHostBuilder(args).Build();
			try
			{
				host.Start();
			}
			catch (Exception x)
			{
				Console.WriteLine(x);
				System.Windows.MessageBox.Show("Failed to start service. Please try again. \r\n" + x.Message, "Error"
					, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
				CefWin.CefShutdown();
				return;
			}

			CefWin.ApplicationHost = host;
			CefWin.ApplicationTask = host.WaitForShutdownAsync(CefWin.ApplicationCTS.Token);

			ShowMainForm();

			CefWin.RunApplication();

		}

		static void ShowMainForm()
		{
			string startUrl = aspnetcoreUrls.Split(';')[0];
			DefaultBrowserForm form = CefWin.OpenBrowser(startUrl);
			form.Width = 1120;
			form.Height = 777;
			form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			//CefWin.CenterForm(form);
			//form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		}

		static string aspnetcoreUrls = "http://127.12.34.56:7890";
		//static string aspnetcoreUrls = "http://127.12.34.56:7890;https://127.12.34.56:7891";
		//static string aspnetcoreUrls = "https://127.12.34.56:7891";       //Force to SSL , not so useful , just a test
		//static string aspnetcoreUrls = CefWin.MakeFixedLocalHostUrl();    //make fixed url by user name , so each user can open 1 instance
		//static string aspnetcoreUrls = CefWin.MakeRandomLocalHostUrl();   //random url allow multiple instance of this app , but cookie/localStorage will lost when open app again.


		static public IHostBuilder CreateHostBuilder(string[] args)
		{
			var builder = Host.CreateDefaultBuilder(args);

			builder.ConfigureWebHostDefaults(webBuilder =>
			{
				Console.WriteLine("aspnetcoreUrls : " + aspnetcoreUrls);
				webBuilder.UseUrls(aspnetcoreUrls);
				webBuilder.UseStartup<Startup>();
			});

			return builder;
		}


		static void OtherIdeas_ProxyServer()
		{
			//modify the CefAdditionArguments before  CefWin.SearchAndInitialize() 

			//If you want to add custom server and override system's setting , use this line :
			CefWin.CefAdditionArguments.Add("--proxy-server=http://127.0.0.1:8080");  //works..
		}


	}
}
