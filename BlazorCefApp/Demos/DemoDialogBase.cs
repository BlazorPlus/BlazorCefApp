using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPlus;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.DataProtection;

namespace BlazorCefApp.Demos
{
	public abstract class DemoDialogBase : ComponentBase
	{
		BlazorDialog _dlg;
		[CascadingParameter]
		public BlazorDialog OwnerDialog
		{
			get
			{
				return _dlg;
			}
			set
			{
				_dlg = value;
				_dlg.CancelHandler = mode =>
				{
					//mode : "ESC" or "BACK"
					OnDialogCancel(mode);
				};
				_dlg.CloseCalled += _dlg_CloseCalled;
			}

		}

		private void _dlg_CloseCalled(object sender, EventArgs e)
		{
			OnDialogClosed();
		}

		protected virtual void OnDialogClosed()
		{

		}

		public void Close()
		{
			_dlg.CloseAnimate("dialog-slide-out", 200);
		}
		public void CloseWithoutAnimate()
		{
			_dlg.Close();
		}

		protected virtual void OnDialogCancel(string mode)
		{
			Close();
		}

	}
}
