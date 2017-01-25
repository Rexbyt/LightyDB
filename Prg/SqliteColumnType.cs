using System;
namespace WinuxDB
{
	public static class SqliteColumnType
	{
		public static string[] GetTypes()
		{
			return new string[]{ "Interger", "Text", "Date", "Time", "Money" };
		}

		public static string WinuxDBTypeToSqliteType(string WinuxDBType)
		{
			if (WinuxDBType.Trim().ToLower() == "money")
			{
				return "double";
			}
			else {
				return WinuxDBType.Trim().ToLower();
			}
		}
	}
}
