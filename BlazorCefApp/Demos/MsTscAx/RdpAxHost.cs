using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using mstscax_interop;

namespace BlazorCefApp.Demos.MsTscAx
{
	public class RdpAxHost : AxHost
	{
		//https://docs.microsoft.com/en-us/windows/win32/termserv/remote-desktop-activex-control 

		static string _FindNewestClsid()
		{
			using var key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("CLSID");

			//string[] guids = { "1DF7C823-B2D4-4B54-975A-F2AC5D7CF8B8"	//MsTscAx.MsTscAx.12
			//		,"A0C63C30-F08D-4AB4-907C-34905D770C7D"		//MsTscAx.MsTscAx.11
			//		,"8B918B82-7985-4C24-89DF-C33AD2BBFBCD"		//MsTscAx.MsTscAx.10
			//		,"A3BC03A0-041D-42E3-AD22-882B7865C9C5"		//MsTscAx.MsTscAx.9
			//		,"54D38BF7-B1EF-4479-9674-1BD6EA465258"		//MsTscAx.MsTscAx.8
			//};

			//foreach (string guid in guids)
			//{
			//	try
			//	{
			//		using var subkey = key.OpenSubKey("{" + guid + "}");
			//		return guid;
			//	}
			//	catch
			//	{

			//	}
			//}

			//8B918B82-7985-4C24-89DF-C33AD2BBFBCD
			return Type.GetTypeFromProgID("MsTscAx.MsTscAx").GUID.ToString();

			//return "54D38BF7-B1EF-4479-9674-1BD6EA465258";	//MsTscAx.MsTscAx.8   //Windows 7, Windows Server 2008
		}

		public static readonly string CLSID = _FindNewestClsid();

		IMsTscAxEvents _events;
		public RdpAxHost(IMsTscAxEvents eventHandler) : base(CLSID)
		{
			_events = eventHandler;
		}

		IMsRdpClient7 _ax;
		public IMsRdpClient7 RdpClient
		{
			get
			{
				if (_ax == null)
					_ax = (IMsRdpClient7)base.GetOcx();
				return _ax;
			}
		}

		ConnectionPointCookie cookie;
		protected override void CreateSink()
		{
			cookie = new ConnectionPointCookie(RdpClient, _events, typeof(IMsTscAxEvents));
		}

	}
}
