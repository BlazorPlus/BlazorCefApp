using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorPlus;

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
					OnCancel(mode);
				};
			}

		}

		public void Close()
		{
			_dlg.CloseAnimate("dialog-slide-out", 200);
		}
		public void CloseWithoutAnimate()
		{
			_dlg.Close();
		}

		public virtual void OnCancel(string mode)
		{
			Close();
		}

	}
}
