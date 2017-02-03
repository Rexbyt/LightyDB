using System;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Winux.Data;
using Winux.Dialogs;

namespace WinuxDB
{
	public static class Table
	{
		public static Headers Headers(string tblName) 
		{
			SqliteCompact scp = new SqliteCompact(Config.connString);
			DataRowCollection drc = scp.Query("SELECT value FROM opt_headers WHERE parameter = '"+tblName+"'");

			if (drc.Count == 1)
			{
				string headers = drc[0]["value"].ToString().Trim();
				if (headers.Length > 0)
				{
					try
					{
						return (Headers)JsonConvert.DeserializeObject(headers, typeof(Headers));
					}
					catch (Exception err){
						ExceptReport.Details(err);
						return null;
					}
				}
				else
				{
					return null;
				}
			}
			else if (drc.Count < 1)
			{
				return null;
			}
			else {
				throw new Exception("Conflict settings solutions. The table opt_headers more than one entry!");
			}
		}

		/// <summary>
		/// Gets the all exists tables from database.
		/// </summary>
		/// <returns>Collection ID table, Header table</returns>
		/// <param name="tblname">Empty row for get all tables, or SQL LIKE expression, or full table name</param>
		public static DataRowCollection GetTablesName(string tblname) 
		{
			SqliteCompact scp = new SqliteCompact(Config.connString);
			string tblfilter = " AND tbl_name LIKE '"+tblname+"'";
			if (tblname.Trim().Length <= 0)
				tblfilter = "";

			return scp.Query("SELECT tbl_name FROM sqlite_master WHERE [type] = 'table'" + tblfilter);
		}

		public static DataRowCollection GetTableColumn(string tblname)
		{
			SqliteCompact scp = new SqliteCompact(Config.connString);
			DataSet ds = new DataSet();
			ds.Tables.Add();
			ds.Tables[0].Columns.Add("name");
			ds.Tables[0].Columns.Add("title");
			ds.Tables[0].Columns.Add("type");
			ds.Tables[0].Columns.Add("size");
			ds.Tables[0].Columns.Add("unique");
			ds.Tables[0].Columns.Add("primary");
			ds.Tables[0].Columns.Add("autoincrement");
			DataRowCollection drc = scp.Query("SELECT sql FROM sqlite_master WHERE [type] = 'table' AND tbl_name = '"+tblname+"'");
			string sql = "";
			if (drc.Count == 1)
			{
				sql = drc[0]["sql"].ToString().Trim();
				sql = sql.Remove(0, sql.IndexOf('(') + 1);
				sql = sql.Substring(0, sql.Length - 1);
				string[] sqlColumns = sql.Split(",".ToCharArray());

				if (sqlColumns.Length > 0)
				{
					foreach (string col in sqlColumns)				
					{
						// Collect rows
						Match mtch = Regex.Match(col.Trim(), "^([a-z\\d]+_?)\\s+([a-z]+)(\\(\\d+\\))?\\s*(NOT NULL)?\\s*"+
						                         "(PRIMARY KEY)?\\s*(AUTOINCREMENT)?\\s*(UNIQUE)?$", RegexOptions.IgnoreCase);

						// Get Size
						string fieldSize = "";
						if (mtch.Groups[3].Value.Trim().Length > 0)
							fieldSize = mtch.Groups[3].Value.Trim().Substring(1, mtch.Groups[3].Value.Trim().Length - 2);
						
						// Get header
						Headers header = Table.Headers(tblname);
						string fieldTitle = "";
						string fieldName = mtch.Groups[1].Value.Trim();
						if (header != null && header.column != null)
						{
							if (header.column.ContainsKey(fieldName))
								fieldTitle = header.column[fieldName];
						}

						// Add new row
						ds.Tables[0].Rows.Add(
							fieldName, // Name
							fieldTitle, 				 // Title (edit later)!
							mtch.Groups[2].Value.Trim(), // Type
							fieldSize.Trim(),			 // Size
							mtch.Groups[7].Value.Trim().Length > 0 ? 1 : 0, // Unique
							mtch.Groups[5].Value.Trim().Length > 0 ? 1 : 0, // Primary
							mtch.Groups[6].Value.Trim().Length > 0 ? 1 : 0  // Autoincrement
						);
					}
				}
			}

			return ds.Tables[0].Rows;
		}
	}

	public class Headers
	{
		public string table = "";
		public Dictionary<string, string> column;
	}
}
