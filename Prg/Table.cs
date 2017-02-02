using System;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using Winux.Data;
using Winux.Dialogs;

namespace WinuxDB
{
	public static class Table
	{
		public static Headers Headers(string tblName) {
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
	}

	public class Headers
	{
		public string table = "";
		public Dictionary<string, string> column;
	}
}
