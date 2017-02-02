using System;
using System.Data;
using System.Collections.Generic;
using Winux.Data;

namespace WinuxDB
{
	public static class Options
	{
		public static string Get(string param)
		{
			return Options.Get("program", param);
		}

		public static string Get(string optionName, string param)
		{
			SqliteCompact scp = new SqliteCompact(Config.connString);
			scp.Open();
			int exists = Convert.ToInt32(
				scp.QueryScalar("SELECT COUNT(parameter) FROM opt_" + optionName + " WHERE LOWER(parameter) = '" +
								param.Trim().ToLower() + "'"));

			if (exists > 0)
			{
				string val = scp.QueryScalar("SELECT value FROM opt_" + optionName + " WHERE LOWER(parameter) = '" +
								   param.Trim().ToLower() + "'").ToString().Trim();
				scp.Close();
				return val;
			}
			else {
				scp.Close();
				return "";
			}
		}

		public static bool Set(string param, string value, bool rewrite)
		{
			return Options.Set("program", param, value, rewrite);
		}

		public static bool Set(string optionName, string param, string value, bool rewrite)
		{
			SqliteCompact scp = new SqliteCompact(Config.connString);
			scp.Open();
			int exists = Convert.ToInt32(
				scp.QueryScalar("SELECT COUNT(parameter) FROM opt_" + optionName + " WHERE LOWER(parameter) = '" +
								param.Trim().ToLower() + "'"));
			int res = 0;


			if (exists <= 0)
			{
				Dictionary<string, object> parameters = new Dictionary<string, object>() {
					{ "parameter", param },
					{ "value", value }
				};
				res = scp.Insert("opt_" + optionName, parameters);
				scp.Close();
				if (res > 0)
					return true;
				else
					return false;
			}
			else {
				if (rewrite)
				{
					Dictionary<string, object> parameters = new Dictionary<string, object>() {
						{ "value", value }
					};
					res = scp.Update("opt_" + optionName, parameters, "WHERE LOWER(parameter) = '" +
									 param.Trim().ToLower() + "'");
					scp.Close();
					if (res > 0)
						return true;
					else
						return false;
				}
				else {
					return false;
				}
			}
		}
	}
}
