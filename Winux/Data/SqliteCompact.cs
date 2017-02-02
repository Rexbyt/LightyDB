using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Xml;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Winux.Data
{
    class SqliteCompact
    {
        public SQLiteConnection Connection = null;
        public string sqlString = "";
        private string connstr = "";

        /// <summary>
        /// Ошибка выполенния работы с БД
        /// </summary>
        public string errors = "";

        /// <summary>
        /// Конструктор для подключения к базе данных
        /// </summary>
        /// <param name="connectionString">Строка соединения</param>
        public SqliteCompact(string connectionString) {
            this.Connection = new SQLiteConnection(connectionString);
            this.connstr = connectionString;
        }

		/// <summary>
		/// Конструктор для создания БД с последующим подключением к БД
		/// </summary>
		/// <param name="connectionString">Строка соединения</param>
		/// <param name="create">Создать базу данных по указанному пути</param>
		public SqliteCompact(string connectionString, bool create)
		{
			SQLiteConnection.CreateFile(connectionString);
			this.Connection = new SQLiteConnection(connectionString);
			this.connstr = connectionString;
		}

		public DataRowCollection GetTableInfo(string tableName)
		{
			return this.Query("PRAGMA table_info('"+tableName+"')");
		}

        /// <summary>
        /// Получаем autoincrement id внутри сессии
        /// </summary>
        /// <returns>Автоматически присвоены ID</returns>
        public int getAutoincrementID()
        {
            SQLiteCommand cmd = new SQLiteCommand("SELECT SCOPE_IDENTITY()", this.Connection);
            object val = cmd.ExecuteScalar();
            return Convert.ToInt32(val);
        }

        public object QueryScalar(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection);
            object val = cmd.ExecuteScalar();
            return val;
        }

        public void Execute(string command)
        {
            SQLiteCommand cmd = new SQLiteCommand(command, this.Connection);
            cmd.CommandType = CommandType.Text;
			cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Открытие соединения
        /// </summary>
        public void Open() {
            this.Connection.Open();
        }

        /// <summary>
        /// Закрытие соединения
        /// </summary>
        public void Close() {
            this.Connection.Close();
        }

        /// <summary>
        /// Компактное добавление данных для избежания путаницы
        /// </summary>
        /// <param name="tblName">Имя таблици добавдения</param>
        /// <param name="param">Перечисляем все поля и их значения в Dictionary</param>
        /// <returns>Вернет количество добавленных строк</returns>
        public int Insert(string tblName, Dictionary<string,object> param) {
            string sql = "INSERT INTO " + tblName + " (";
            string values = "";
            int intVar = 0;
            // Формируем список заполняемых полей
            foreach (KeyValuePair<string,object> kp in param) {
                sql += kp.Key + ",";
                try
                {
                    intVar = Convert.ToInt32(kp.Value);
                    values += kp.Value + ",";
                }
                catch {
                    values += "'" + kp.Value + "',";
                }
            }
            sql = sql.Trim().Substring(0, (sql.Length - 1));
            values = values.Trim().Substring(0,(values.Trim().Length - 1));
            sql += ") VALUES (" + values + ")";
            
			this.sqlString = sql;
            SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection);
            int res = cmd.ExecuteNonQuery(); 
            return res;
        }

        /// <summary>
        /// Компактное альтернативное добавление данных для избежания путаницы.
        /// </summary>
        /// <param name="tblName">Имя таблици добавдения</param>
        /// <param name="param">Перечисляем все поля и их значения в Dictionary. 
        /// Его ключи должны соответствовать названиям полей целевой таблицы в БД!</param>
        /// <param name="fields">Перечисляем нужные поля из Dictionary</param>
        /// <returns>Вернет количество добавленных строк</returns>
        public int Insert(string tblName, Dictionary<string, object> param, string fields)
        {
            string sql = "INSERT INTO " + tblName + " (";
            string values = "";
            int intVar = 0;
            // Формируем список заполняемых полей
            if (fields.Trim().Length > 0) { 
                string[] field = fields.Trim().Split(",".ToCharArray());
                foreach (string str in field) {
                    Application.DoEvents();
                    if (param.Keys.Contains(str.Trim()))
                    {
                        sql += str.Trim() + ",";
                        try
                        {
                            intVar = Convert.ToInt32(param[str.Trim()]);
                            values += param[str.Trim()] + ",";
                        }
                        catch
                        {
                            values += "'" + param[str.Trim()].ToString().Trim() + "',";
                        }
                    }
                    else {
                        this.errors = "Список параметров не содержит поле ["+str+"]!";
                        return 0;
                    }
                }
            }
            else
            { 
                this.errors = "Список полей не должен быть пустым при альтернативном инсерте!";
                return 0;
            }

            sql = sql.Trim().Substring(0, (sql.Length - 1));
            values = values.Trim().Substring(0, (values.Trim().Length - 1));
            sql += ") VALUES (" + values + ")";
            
			this.sqlString = sql;
            SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection);
            int res = cmd.ExecuteNonQuery();
            return res;
        }

        /// <summary>
        /// Обновление базы данных
        /// </summary>
        /// <param name="tblName">Название таблицы</param>
        /// <param name="param">Название столбцов и их значения в виде массива Dictionary</param>
        /// <param name="filter">Вильтр WHERE типа string. Пример указания: "id=1,name=\"Alex\"", с WHERE</param>
        /// <returns>Вернет количество измененных строк</returns>
        public int Update(string tblName,Dictionary<string,object> param, string filter) {
            string sql = "UPDATE " + tblName + " SET ";
            string paramString = "";
            int intVar = 0;
            foreach(KeyValuePair<string,object> kp in param){
                try
                {
                    intVar = Convert.ToInt32(kp.Value);
                    paramString += kp.Key + "=" + kp.Value + ",";
                }
                catch {
                    paramString += kp.Key + "='" + kp.Value + "',";
                }
            }
            paramString = paramString.Trim().Substring(0, (paramString.Trim().Length - 1));
            sql += paramString;
            if (filter.Trim().Length > 0) {
                sql += " " + filter.Trim();
            }
            
			this.sqlString = sql;
            SQLiteCommand cmd = new SQLiteCommand(sql,this.Connection);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Альтенативная функция обновления
        /// </summary>
        /// <param name="tblName">Название таблицы</param>
        /// <param name="param">Название столбцов и их значения в виде массива Dictionary</param>
        /// <param name="filter">Вильтр WHERE типа string. Пример указания: "id=1,name=\"Alex\"", без WHERE</param>
        /// <param name="fields">Список полее, которые нужно обновлять</param>
        /// <returns>Вернет количество измененных строк</returns>
        public int Update(string tblName, Dictionary<string, object> param, string filter, string fields)
        {
            string sql = "UPDATE " + tblName + " SET ";
            string paramString = "";
            int intVar = 0;

            // Формируем список заполняемых полей
            if (fields.Trim().Length > 0)
            {
                string[] field = fields.Trim().Split(",".ToCharArray());
                foreach (string str in field)
                {
                    Application.DoEvents();
                    if (param.Keys.Contains(str.Trim()))
                    {
                        try
                        {
                            intVar = Convert.ToInt32(param[str.Trim()]);
                            paramString += str.Trim() + "=" + param[str.Trim()] + ",";
                        }
                        catch
                        {
                            paramString += str.Trim() + "='" + param[str.Trim()] + "',";
                        }   
                    }
                    else
                    {
                        this.errors = "Список параметров не содержит поле [" + str + "]!";
                        return 0;
                    }
                }
            }
            else
            {
                this.errors = "Список полей не должен быть пустым при альтернативном обновление!";
                return 0;
            }

            paramString = paramString.Trim().Substring(0, (paramString.Trim().Length - 1));
            sql += paramString;
            if (filter.Trim().Length > 0)
            {
                sql += " " + filter.Trim();
            }
            
			this.sqlString = sql;
            SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection);
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаление нужной строки
        /// </summary>
        /// <param name="tblName">Имя таблицы в которой производим удаление</param>
        /// <param name="filter">Фильтрация используя WHERE</param>
        /// <returns></returns>
        public int Delete(string tblName,string filter) {
            string sql = "DELETE FROM " + tblName;
            if (filter.Trim().Length > 0)
            {
            	sql += " " + filter;
            }

            this.sqlString = sql;
            SQLiteCommand cmd = new SQLiteCommand(sql, this.Connection);
            int res = cmd.ExecuteNonQuery();
            if (res <= 0)
            {
                this.errors = "Удаление не произведено!";
            }
            return res;
        }

        /// <summary>
        /// Селекторный запрос в стандартном формате SQL
        /// </summary>
        /// <param name="sql">Строка запроса</param>
        /// <returns>Вернет колекцию строк соответствующих запросу</returns>
        public DataRowCollection Query(string sql) {
			DataSet ds = new DataSet();
            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, this.Connection);
            da.Fill(ds,"result");
			if (ds.Tables["result"] != null)
				return ds.Tables["result"].Rows;
			else
				return null;
        }
    }
}
