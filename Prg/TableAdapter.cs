// This Adapter for automatic show table and data in in table. For correctly working a script, required 
//specify TreeView and table name from database SQLite.

using System;
using System.Data;
using Winux.Dialogs;
using Winux.Data;
using Gtk;

namespace WinuxDB
{
	public class TableAdapter
	{
		private ListStore rows;
		private bool TableShown = false;
		private Gtk.TreeView table;
		private string dbTableName_;

		public string SetTableName { 
			get {
				return dbTableName_;
			}

			set {
				dbTableName_ = value;
			}
		}

		public TableAdapter(Gtk.TreeView Table)
		{
			table = Table;
		}

		public void ShowTable()
		{
			if (TableShown)
			{
				this.UpdateTable();
				return;
			}

			SqliteCompact scp = new SqliteCompact(Config.connString);
			DataRowCollection drcColumns = scp.GetTableInfo(this.dbTableName_);
			CellRendererText render;

			// Whether we need a table
			if (drcColumns == null || drcColumns.Count <= 0) { 
				MsgBox.Error(
							string.Format(Config.Lang("msgErrorGetTableInfo",
					                                  "The required table ({0}) does not exist!"),
										  this.dbTableName_), Config.Lang("titleError", "Error"));
				return;
			}

			System.Type[] types = new System.Type[drcColumns.Count];

			string[] columns = new string[drcColumns.Count];
			int TypeIndex = 0;

			// Get all headers from table opt_headers if they exists else null
			Headers headers = Table.Headers(this.dbTableName_);

			// How clear all columns before adding new columns
			foreach (DataRow dr in drcColumns)
			{
				System.Windows.Forms.Application.DoEvents();
				render = new CellRendererText();
				string colName = "";
				if (headers != null && headers.column.ContainsKey(dr["name"].ToString()))
					colName = headers.column[dr["name"].ToString()];
				else
					colName = dr["name"].ToString();

				this.table.AppendColumn(colName, render, "text", TypeIndex);
				types.SetValue(typeof(string), TypeIndex);
				TypeIndex++;
			}
			rows = new ListStore(types);

			DataRowCollection drcRows = scp.Query("SELECT * FROM " + this.dbTableName_);
			foreach (DataRow dr in drcRows)
			{
				System.Windows.Forms.Application.DoEvents();
				for (int i = 0; i < rows.NColumns; i++)
				{
					System.Windows.Forms.Application.DoEvents();
					columns.SetValue(dr[i].ToString(), i);
				}
				rows.AppendValues(columns);
			}
			this.table.Model = rows;
			this.table.ShowAll();
			this.TableShown = true;
		}

		public void UpdateTable()
		{
			this.rows.Clear();
			SqliteCompact scp = new SqliteCompact(WinuxDB.Config.connString);
			DataRowCollection drcRows = scp.Query("SELECT * FROM " + this.dbTableName_);
			string[] columns = new string[rows.NColumns];

			foreach (DataRow dr in drcRows)
			{
				System.Windows.Forms.Application.DoEvents();
				for (int i = 0; i < rows.NColumns; i++)
				{
					System.Windows.Forms.Application.DoEvents();
					columns.SetValue(dr[i].ToString(), i);
				}
				rows.AppendValues(columns);
			}
			this.table.Model = rows;
			this.table.ShowAll();
		}
	}
}
