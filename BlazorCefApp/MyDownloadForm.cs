using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using CefLite;

namespace BlazorCefApp
{
	public partial class MyDownloadForm : Form
	{
		static public string TextFormTitle = "MyDownloads";
		static public string TextUrl = "Url";
		static public string TextId = "Id";
		static public string TextFileName = "File Name";
		static public string TextProgress = "Progress";
		static public string TextProgressCompleted = "Completed";
		static public string TextProgressCanceled = "Canceled";
		static public string TextConfirmCancelTitle = "Cancel";
		static public string TextConfirmCancelMessage = "Cancel download file?";

		public MyDownloadForm()
		{
			this.Size = new Size(800, 500);
			this.MinimumSize = new Size(360, 360);

			if (CefWin.ApplicationIcon != null)
				this.Icon = CefWin.ApplicationIcon;

			this.Text = TextFormTitle ?? CefWin.ApplicationTitle;

			this.Controls.Add(ListView);

			ListView.Activation = ItemActivation.Standard;
			ListView.ItemActivate += ListView_ItemActivate;

			ListView.Columns.Add(new ColumnHeader() { Name = "Id", Text = TextId, Width = 50 });
			ListView.Columns.Add(new ColumnHeader() { Name = "Url", Text = TextUrl, Width = 360 });
			ListView.Columns.Add(new ColumnHeader() { Name = "FileName", Text = TextFileName, Width = 260 });
			ListView.Columns.Add(new ColumnHeader() { Name = "Progress", Text = TextProgress, Width = 100 });
		}

		private void ListView_ItemActivate(object sender, EventArgs e)
		{
			foreach (ListViewItem lvi in ListView.SelectedItems)
			{
				DownloadItem lvitem = (DownloadItem)lvi.Tag;
				if (lvitem.IsComplete)
				{
					System.Diagnostics.Process.Start("Explorer", "/select, " + lvitem.FullPath);
				}
				else if (lvitem.IsInProgress)
				{
					if (MessageBox.Show(TextConfirmCancelMessage, TextConfirmCancelTitle, MessageBoxButtons.OKCancel) == DialogResult.OK)
					{
						lvitem.Cancel();
						lvitem.RemoveFromList();
					}
				}
			}
		}

		public ListView ListView { get; } = new ListView() { Dock = DockStyle.Fill, View = View.Details, HideSelection = false, FullRowSelect = true };


		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			DownloadItem.DataVersionUpdated += DownloadItem_DataVersionUpdated;
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);
			DownloadItem.DataVersionUpdated -= DownloadItem_DataVersionUpdated;
		}


		private void DownloadItem_DataVersionUpdated()
		{
			try
			{
				RefreshList();
			}
			catch (Exception x)
			{
				Console.WriteLine(x);
			}
		}

		long _ldv = 0;
		void RefreshList()
		{
			if (_ldv == DownloadItem.DataVersion)
				return;
			_ldv = DownloadItem.DataVersion;

			ListView.BeginUpdate();

			var items = DownloadItem.Items.ToList();
			foreach (ListViewItem lvi in new System.Collections.ArrayList(ListView.Items))
			{
				DownloadItem lvitem = (DownloadItem)lvi.Tag;
				if (!items.Contains(lvitem))
				{
					ListView.Items.Remove(lvi);
					continue;
				}
				Update(lvitem, lvi);
				items.Remove(lvitem);
			}
			foreach (DownloadItem item in items)
			{
				ListViewItem lvi = new ListViewItem();
				for (int i = 0; i < ListView.Columns.Count; i++)
					lvi.SubItems.Add("");
				Update(item, lvi);
				ListView.Items.Add(lvi);
			}

			ListView.EndUpdate();
		}

		void Update(DownloadItem item, ListViewItem lvi)
		{
			string progress = "";
			if (item.IsInProgress)
			{
				if (item.TotalBytes > 0)
				{
					progress = Math.Floor(100.0 * item.ReceivedBytes / (double)item.TotalBytes) + "% " + FormatSize(item.ReceivedBytes);
				}
				else
				{
					progress = FormatSize(item.ReceivedBytes);
				}
			}
			else if (item.IsComplete)
			{
				progress = TextProgressCompleted;
			}
			else if (item.IsCanceled)
			{
				progress = TextProgressCanceled;
			}
			else if (!item.IsValid)
			{
				progress = "Invalid";
			}
			else
			{

			}

			lvi.Tag = item;
			lvi.Text = item.Id.ToString();

			lvi.SubItems[1].Text = item.Url;
			lvi.SubItems[2].Text = Path.GetFileName(item.FullPath);

			lvi.SubItems[3].Text = progress;

		}

		static string FormatSize(long size)
		{
			if (size > 1024 * 1024)
				return size / 1024 / 1024 + " MB";
			if (size > 1024)
				return size / 1024 + "KB";
			return size + " Bytes";
		}
	}
}
