using CefLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using mstscax_interop;

namespace BlazorCefApp.Demos.MsTscAx
{
	public class RdpPanel : Panel, IMsTscAxEvents
	{

		void QuitPanel()
		{
			Disconnected?.Invoke(this, EventArgs.Empty);
			this.Dispose();
		}

		public event EventHandler Disconnected;

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
		}

		RdpAxHost axhost;

		public void Connect(string serverAndPort)
		{
			//MessageBox.Show(RdpAxHost.CLSID);

			if (axhost != null) axhost.Dispose();
			axhost = new RdpAxHost(this);
			axhost.Dock = DockStyle.Fill;
			this.Controls.Add(axhost);

			string[] pair = serverAndPort.Split(':');

			CefWin.SetTimeout(100, delegate
			{
				var rc = axhost.RdpClient;
				rc.Server = pair[0];

				//rc.Domain = "";

				rc.DesktopWidth = this.ClientSize.Width;
				rc.DesktopHeight = this.ClientSize.Height;
				rc.ConnectingText = "playing....";
				var adv = rc.AdvancedSettings8;
				adv.Compress = 1;
				adv.BitmapPeristence = 1;

				if (pair.Length > 1 && int.TryParse(pair[1], out int port))
				{
					adv.RDPPort = port;
				}

				//rc.UserName = "Administrator";
				//adv.ClearTextPassword = "********";
				adv.AuthenticationLevel = 2;
				adv.EnableCredSspSupport = true;

				rc.Connect();
			});
		}



		void IMsTscAxEvents.OnConnecting()
		{
			//CefLite.CefWin.InvokeInAppThread(delegate
			//{
			//	MessageBox.Show("Connecting");
			//});
		}

		void IMsTscAxEvents.OnConnected()
		{
			//CefLite.CefWin.InvokeInAppThread(delegate
			//{
			//	MessageBox.Show("OnConnected");
			//});
		}

		void IMsTscAxEvents.OnLoginComplete()
		{

		}

		void IMsTscAxEvents.OnDisconnected(int discReason)
		{
			//7943 no username/password ? 

			//2825 The remote computer requires Network Level Authentication, which your computer does not support.
			//		set		EnableCredSspSupport = true;

			//

			CefLite.CefWin.InvokeInAppThread(delegate
			{
				//MessageBox.Show("OnDisconnected " + discReason);
				QuitPanel();
			});
		}

		void IMsTscAxEvents.OnEnterFullScreenMode()
		{

		}

		void IMsTscAxEvents.OnLeaveFullScreenMode()
		{

		}

		void IMsTscAxEvents.OnChannelReceivedData(string chanName, string data)
		{

		}

		void IMsTscAxEvents.OnRequestGoFullScreen()
		{

		}

		void IMsTscAxEvents.OnRequestLeaveFullScreen()
		{

		}

		void IMsTscAxEvents.OnFatalError(int errorCode)
		{

		}

		void IMsTscAxEvents.OnWarning(int warningCode)
		{

		}

		void IMsTscAxEvents.OnRemoteDesktopSizeChange(int width, int height)
		{

		}

		void IMsTscAxEvents.OnIdleTimeoutNotification()
		{

		}

		void IMsTscAxEvents.OnRequestContainerMinimize()
		{

		}

		void IMsTscAxEvents.OnConfirmClose(out bool pfAllowClose)
		{
			pfAllowClose = true;
		}

		void IMsTscAxEvents.OnReceivedTSPublicKey(string publicKey, out bool pfContinueLogon)
		{
			pfContinueLogon = true;
		}

		void IMsTscAxEvents.OnAutoReconnecting(int disconnectReason, int attemptCount, out AutoReconnectContinueState pArcContinueStatus)
		{
			pArcContinueStatus = AutoReconnectContinueState.autoReconnectContinueAutomatic;
		}

		void IMsTscAxEvents.OnAuthenticationWarningDisplayed()
		{

		}

		void IMsTscAxEvents.OnAuthenticationWarningDismissed()
		{

		}

		void IMsTscAxEvents.OnRemoteProgramResult(string bstrRemoteProgram, RemoteProgramResult lError, bool vbIsExecutable)
		{

		}

		void IMsTscAxEvents.OnRemoteProgramDisplayed(bool vbDisplayed, uint uDisplayInformation)
		{

		}

		void IMsTscAxEvents.OnRemoteWindowDisplayed(bool vbDisplayed, ref _RemotableHandle hwnd, RemoteWindowDisplayedAttribute windowAttribute)
		{

		}

		void IMsTscAxEvents.OnLogonError(int lError)
		{
			//CefLite.CefWin.InvokeInAppThread(delegate
			//{
			//	MessageBox.Show("OnLogonError " + lError);
			//});
		}

		void IMsTscAxEvents.OnFocusReleased(int iDirection)
		{

		}

		void IMsTscAxEvents.OnUserNameAcquired(string bstrUserName)
		{
			//TODO: remember last user name ?

			//CefLite.CefWin.InvokeInAppThread(delegate
			//{
			//	MessageBox.Show("OnUserNameAcquired " + bstrUserName);
			//});
		}

		void IMsTscAxEvents.OnMouseInputModeChanged(bool fMouseModeRelative)
		{

		}

		void IMsTscAxEvents.OnServiceMessageReceived(string serviceMessage)
		{

		}

		void IMsTscAxEvents.OnConnectionBarPullDown()
		{

		}

		void IMsTscAxEvents.OnNetworkStatusChanged(uint qualityLevel, int bandwidth, int rtt)
		{

		}

		void IMsTscAxEvents.OnDevicesButtonPressed()
		{

		}

		void IMsTscAxEvents.OnAutoReconnected()
		{

		}

		void IMsTscAxEvents.OnAutoReconnecting2(int disconnectReason, bool networkAvailable, int attemptCount, int maxAttemptCount)
		{

		}
	}
}
