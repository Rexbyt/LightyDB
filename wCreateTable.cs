using System;
using Winux.Dialogs;
using Winux.Data;
using System.Data;
using Gtk;
namespace WinuxDB
{
	public class CustomCellRendererText : CellRendererText
	{ 
		public int ColumnIndex { get; set; }
		public string TypeCell { get; set; }
	}

	public class CustomCellRendererCombo : CellRendererCombo
	{
		public int ColumnIndex { get; set; }
		public string TypeCell { get; set; }
	}

	public class CustomCellRendererToggle : CellRendererToggle
	{
		public int ColumnIndex { get; set; }
		public string TypeCell { get; set; }
	}

	public partial class wCreateTable : Gtk.Window
	{
		private ListStore rows;

		public wCreateTable() :
				base(Gtk.WindowType.Toplevel)
		{
			Build();

			try
			{
				this.CreateTableColumns();
			}
			catch (Exception err)
			{
				ExceptReport.Details(err);
			}
		}

		public void CreateTableColumns() { 
			CustomCellRendererText ColumnNameCell = new CustomCellRendererText();
			ColumnNameCell.Editable = true;
			ColumnNameCell.ColumnIndex = 0;
			ColumnNameCell.TypeCell = "Text";
			ColumnNameCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn("ColumnName", ColumnNameCell, "text", ColumnNameCell.ColumnIndex);

			CustomCellRendererText ColumnTitleCell = new CustomCellRendererText();
			ColumnTitleCell.Editable = true;
			ColumnTitleCell.ColumnIndex = 1;
			ColumnTitleCell.TypeCell = "Text";
			ColumnTitleCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn("ColumnTitle", ColumnTitleCell, "text", ColumnTitleCell.ColumnIndex);

			CustomCellRendererCombo TypeCell = new CustomCellRendererCombo();
			ListStore listType = new ListStore(typeof(string));
			TypeCell.Editable = true;
			TypeCell.Mode = CellRendererMode.Editable;
			TypeCell.ColumnIndex = 2;
			TypeCell.TypeCell = "Combo";
			TypeCell.TextColumn = 0;
			TypeCell.HasEntry = false;
			TypeCell.Model = listType;
			listType.AppendValues("Integer");
				listType.AppendValues("Text");
				listType.AppendValues("Date");
				listType.AppendValues("Time");
				listType.AppendValues("Money");
			TypeCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn("Type", TypeCell, "text", TypeCell.ColumnIndex);

			CustomCellRendererText SizeCell = new CustomCellRendererText();
			SizeCell.Editable = true;
			SizeCell.Mode = CellRendererMode.Editable;
			SizeCell.ColumnIndex = 3;
			SizeCell.TypeCell = "Double";
			SizeCell.Edited += new EditedHandler(OnEdt);
			tblColumns.AppendColumn("Size", SizeCell, "text", SizeCell.ColumnIndex);

			CustomCellRendererToggle UniqCell = new CustomCellRendererToggle();
			UniqCell.Activatable = true;
			UniqCell.Toggled += new ToggledHandler(OnToggled);
			UniqCell.ColumnIndex = 4;
			tblColumns.AppendColumn("Uniq", UniqCell, "bool", UniqCell.ColumnIndex);

			rows = new ListStore(typeof(string), typeof(string), typeof(string), typeof(double), typeof(bool));
			this.rows.AppendValues("id", "ID", "Integer", 0, true);
			tblColumns.Model = rows;

			tblColumns.ShowAll();
		}

		void OnToggled(object o, ToggledArgs args)
		{
			TreeIter iter;
			TreeModel store = tblColumns.Model;

			CustomCellRendererToggle chb = o as CustomCellRendererToggle;

			if (store.GetIterFromString(out iter, args.Path))
			{
				bool val = (bool)store.GetValue(iter, chb.ColumnIndex);
				store.SetValue(iter, chb.ColumnIndex, !val);
			}
			else {
				MsgBox.Error("Лажааа!!", "");
			}
		}

		void OnEdt(object o, EditedArgs args)
		{
			try
			{
				TreeIter iter;
				TreeModel model;

				tblColumns.Selection.GetSelected(out model, out iter);
				if (o.ToString().Contains("RendererText"))
				{
					if (((CustomCellRendererText)o).TypeCell == "Text")
					{
						model.SetValue(iter, ((CustomCellRendererText)o).ColumnIndex, args.NewText);
					}
					else if (((CustomCellRendererText)o).TypeCell == "Double")
					{
						double newvalue = double.Parse(args.NewText.Replace(".", ","));
						model.SetValue(iter, ((CustomCellRendererText)o).ColumnIndex, newvalue);
					}
				}
				else if (o.ToString().Contains("RendererCombo")){
					model.SetValue(iter, ((CustomCellRendererCombo)o).ColumnIndex, args.NewText);
				}
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		protected void OnActAddColumnActivated(object sender, EventArgs e)
		{
			try
			{
				//Create new row for column table
				this.rows.AppendValues("...", "...", "...", 0);
				tblColumns.Model = this.rows;
				tblColumns.ShowAll();
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		protected void OnActDeleteActivated(object sender, EventArgs e)
		{
			this.tblColumnRowDelete();
		}

		private void tblColumnRowDelete() { 
			// Delete selected row
			TreeSelection ts = tblColumns.Selection;
			TreeIter iter;
			if (ts.GetSelected(out iter))
			{
				if (ts.CountSelectedRows() == 1)
				{
					this.rows.Remove(ref iter);
				}
				else {
					this.rows.Clear();
				}
				tblColumns.Model = this.rows;
				tblColumns.ShowAll();
			}
		}

		protected void OnTblColumnsKeyPressEvent(object o, KeyPressEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Delete)
			{
				this.tblColumnRowDelete();
			}
		}

		private string ColumnsSQL = "";
		protected void OnActCreateActivated(object sender, EventArgs e)
		{
			try
			{
				// Collect main request for create table
				string tblName = txtTableName.Text;
				string tblTitle = txtTableTitle.Text;
				// Prefix itbl_ helps to distinguish table info of user
				// For auto select from sqlite_master
				this.ColumnsSQL = "CREATE TABLE itbl_" + txtTableName.Text.Trim() + "(";
				tblColumns.Model.Foreach(ForeachModelOftblColumns);

				this.ColumnsSQL = this.ColumnsSQL.Trim().Substring(0, (this.ColumnsSQL.Trim().Length - 1))+");";

				SqliteCompact scp = new SqliteCompact(XData.connString);
				scp.Open();
				scp.QueryScalar(this.ColumnsSQL);
				scp.Close();
				MsgBox.Info("Table created successfully", "Информация");
			}
			catch (Exception err){
				ExceptReport.Details(err);
			}
		}

		//Collect request for columns of new table
		bool ForeachModelOftblColumns(TreeModel model, TreePath path, TreeIter iter)
		{
			string colName = model.GetValue(iter, 0).ToString().Trim();
			string colTitle = model.GetValue(iter, 1).ToString().Trim();
			string colType = model.GetValue(iter, 2).ToString().Trim();
			double colSize = Convert.ToDouble(model.GetValue(iter, 3));
			string size = "";
			string toSqlite3Type = "";
			if (colType.Trim().ToLower() == "money")
				toSqlite3Type = "double";
			else if (colType.Trim().ToLower() == "text" && colSize > 0.0)
				toSqlite3Type = "varchar";
			else
				toSqlite3Type = colType.Trim().ToLower();

			if (colSize > 0.0)
				size = "("+colSize.ToString()+")";

			if(colName.ToLower() == "id")
				this.ColumnsSQL += colName+" "+toSqlite3Type+size+" NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE,";
			else
				this.ColumnsSQL += colName + " " + toSqlite3Type+ size + " NOT NULL,";
			return true;
		}
}
}
