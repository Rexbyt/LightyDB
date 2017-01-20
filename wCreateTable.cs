using System;
using Winux.Dialogs;
using Gtk;
namespace WinuxDB
{
	public class CustomCellRenderText : CellRendererText
	{ 
		public int ColumnIndex { get; set; }
	}

	public partial class wCreateTable : Gtk.Window
	{
		ListStore rows;

		public wCreateTable() :
				base(Gtk.WindowType.Toplevel)
		{
			Build();

			try
			{
				CustomCellRenderText EditableCell = new CustomCellRenderText();
				EditableCell.Editable = true;
				EditableCell.ColumnIndex = 0;
				EditableCell.Edited += new EditedHandler(OnEdt);
				tblColumns.AppendColumn("Parameter", EditableCell, "text", EditableCell.ColumnIndex);

				CustomCellRenderText EditableCell1 = new CustomCellRenderText();
				EditableCell1.Editable = true;
				EditableCell1.ColumnIndex = 1;
				EditableCell1.Edited += new EditedHandler(OnEdt);
				tblColumns.AppendColumn("Value", EditableCell1, "text", EditableCell1.ColumnIndex);

				rows = new ListStore(typeof(string), typeof(string));
				tblColumns.Model = rows;

				tblColumns.ShowAll();
			}
			catch (Exception err)
			{
				ExceptReport.Details(err);
			}
		}

		void OnEdt(object o, EditedArgs args)
		{
			TreeIter iter;

			tblColumns.Selection.GetSelected(out iter);
			tblColumns.Model.SetValue(iter, ((CustomCellRenderText)o).ColumnIndex, args.NewText);
		}

		protected void OnActAddColumnActivated(object sender, EventArgs e)
		{
			this.rows.AppendValues("...", "...");
			tblColumns.Model = this.rows;
			tblColumns.ShowAll();
		}

		protected void OnActDeleteActivated(object sender, EventArgs e)
		{
			this.tblColumnRowDelete();
		}

		private void tblColumnRowDelete() { 
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
	}
}
