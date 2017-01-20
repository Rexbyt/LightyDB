using System;
using Gtk;

namespace Winux.Tables
{
	public class TableAdapter
	{
		public TreeView _Table;
		private ListStore _Rows;
		private TreeIter _Iter;
		private Type[] _Types;

		public TableAdapter(TreeView treeView)
		{
			_Table = treeView;
		}

		public void AddColumn(string title, CellRenderer cellRender, params object[] attrs) {
			_Table.AppendColumn(title, cellRender, attrs);
		}
		public void AddColumn(TreeViewColumn treeviewcolumn)
		{
			_Table.AppendColumn(treeviewcolumn);
		}

		public void SetColumnsType(params Type[] types) {
			_Types = types;
			_Rows = new ListStore(_Types);
		}

		public void SetSortColumnId(int sort_id_column, SortType order) {
			_Rows.SetSortColumnId(sort_id_column, order);
		}

		public TreeIter Expander(object title)
		{
			return _Rows.AppendValues(title);
		}

		public void AddRow(params object[] values) {
			_Rows.AppendValues(values);
		}

		public object GetSelectedRow(int column) {
			_Table.Selection.GetSelected(out _Iter);
			return _Table.Model.GetValue(_Iter, column);
		}

		public int RemoveColumn(int column) {
			return _Table.RemoveColumn(_Table.Columns[column]);
		}

		public void ClearRows() {
			_Rows = new ListStore(_Types);
			_Table.Model = _Rows;
		}

		public void ShowAll() { 
			_Table.Model = _Rows;
			_Table.ShowAll();
		}
	}
}
