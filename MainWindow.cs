using System;
using System.Windows.Forms;
using System.Data;
using Gtk;
using Winux.Tables;
using Winux.Data;
using Winux.Dialogs;

public partial class wndMain : Gtk.Window
{
	public TableAdapter _atblMain;

	public wndMain() : base(Gtk.WindowType.Toplevel)
	{
		Build();
	}

	protected void OnDeleteEvent(object sender, DeleteEventArgs a)
	{
		Gtk.Application.Quit();
		a.RetVal = true;
	}
	protected void OnActCreateTableActivated(object sender, EventArgs e)
	{
		// Only one comment
		WinuxDB.wCreateTable wct = new WinuxDB.wCreateTable();
		wct.ShowAll();
	}

	public void ShowTable(Gtk.TreeView Table, string tableName)
	{
		SqliteCompact scp = new SqliteCompact(WinuxDB.Config.connString);
		DataRowCollection drcColumns = scp.GetTableInfo(tableName);
		CellRendererText render;
		System.Type[] types = new System.Type[drcColumns.Count];
		ListStore ls;
		string[] columns = new string[drcColumns.Count];
		int TypeIndex = 0;

		// How clear all columns before adding new columns
		foreach (DataRow dr in drcColumns) {
			System.Windows.Forms.Application.DoEvents();
			render = new CellRendererText();
			Table.AppendColumn(dr["name"].ToString(), render, "text", TypeIndex);
			types.SetValue(typeof(string), TypeIndex);
			TypeIndex++;
		}
		ls = new ListStore(types);

		DataRowCollection drcRows = scp.Query("SELECT * FROM "+tableName);
		foreach (DataRow dr in drcRows)
		{
			System.Windows.Forms.Application.DoEvents();
			for (int i = 0; i < drcColumns.Count; i++)
			{ 
				System.Windows.Forms.Application.DoEvents();
				columns.SetValue(dr[i].ToString(), i);
			}
			ls.AppendValues(columns);
		}
		Table.Model = ls;
		Table.ShowAll();
	}

	protected void OnActRefreshTableActivated(object sender, EventArgs e)
	{
		try
		{
			this.ShowTable(tblMain, "itbl_test");
		}
		catch (Exception err){
			ExceptReport.Details(err);
		}
	}
}                       