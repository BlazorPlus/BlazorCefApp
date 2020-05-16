using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using BlazorPlus;
using CefLite;


namespace BlazorCefApp
{

	static public class BlazorSessionExt
	{
		static public ICefWinBrowser FindBrowser(this BlazorSession session)
		{
			WebCustomizeSession wcs = (WebCustomizeSession)session;
			long CefWinBrowserId;
			if (!long.TryParse(wcs.HttpContextAccessor.HttpContext.Request.Cookies["CefWinBrowserId"], out CefWinBrowserId))
			{
				//BlazorSession.Current.Toast("Failed to get browserid");
				return CefWin.MainBrowser;
			}
			ICefWinBrowser browser = CefWin.FindBrowser(CefWinBrowserId);
			if (browser == null)
			{
				//BlazorSession.Current.Toast("Failed to find browser : " + CefWinBrowserId);
				return CefWin.MainBrowser;
			}
			return browser;
		}

		static public void RunBrowser(this BlazorSession session, Action<ICefWinBrowser> action)
		{
			var browser = FindBrowser(session);
			CefWin.PostToAppThread(delegate
			{
				action(browser);
			});
		}

		static public void ShowDevTools(this BlazorSession session)
		{
			RunBrowser(session, browser =>
			{
				browser.Agent.ShowDevTools();
			});
		}

		static public void CloseBrowser(this BlazorSession session)
		{
			RunBrowser(session, browser =>
			{
				browser.Agent.Dispose();
			});
		}
	}




}
