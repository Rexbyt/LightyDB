//using System;
//using System.Collections.Generic;

//namespace Winux.Data
//{
//	public enum DbType : int { OleDb = 1, Sql = 2, Sqlite = 3 };

//	public class DbCompact
//	{
//		public object DB = null;

//		public DbCompact(DbType dtype, string connectionString)
//		{
//			switch (dtype)
//			{
//				case DbType.OleDb:
//					this.DB = new OleDbCompact(connectionString);
//					break;
//				case DbType.Sql:
//					this.DB = new SqlCompact(connectionString);
//					break;
//				case DbType.Sqlite:
//					this.DB = new SqliteCompact(connectionString);
//					break;
//				default:
//					this.DB = null;
//					break;
//			}
//		}
//	}
//}
