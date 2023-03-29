using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace SchedulerMonitor.Utility
{
	public class DataServiceAssign
	{
        /// <summary>
        /// Отримання з БД назву підрозділу і години роботи
        /// </summary>
        /// <param name="pointID"></param>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static Models.WorkTime GetPointName(int pointID)
        {
            string connString = Properties.Settings.Default.connectionString;
            string script = "SELECT Name, OpenTime, CloseTime FROM dbo.tbServicePoint WHERE ID = $pointID$";
            script = script.Replace("$pointID$", pointID.ToString());
            Models.WorkTime res = new Models.WorkTime ();

            using (SqlConnection connection = new SqlConnection (connString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand (script, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res = new Models.WorkTime
                                {
                                    Start = GetIntegerSqlDef(reader, "OpenTime", 0),
                                    Name = GetStringSqlDef(reader, "Name", string.Empty),
                                    End = GetIntegerSqlDef(reader, "CloseTime", 0)
                                };
                            }
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Отримання даних з бд
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <param name="pointID"></param>
        /// <returns></returns>
        public static List<Models.ServiceAssign> GetServiceAssignData(int pointID)
        {
            List<Models.ServiceAssign> res = new List<Models.ServiceAssign> ();
            string connString = Properties.Settings.Default.connectionString;

            string script = @"SELECT sa.PointID	--айді точки
		                            , Sp.Name AS PointName	--назва точки
		                            , sa.Start	--початок сеансу
		                            , sa.Finish	--кінець сеансу
		                            , (DATEDIFF(second, sa.Start, sa.Finish) / 60) Length	-- Тривалість
		                            , (p.LastName + ' ' + p.FirstName + ' ' + p.Patronymic) Performer	--ПІБ виконавця
		                            , PD.PersonName		--ПІБ замовника
		                            , T.ArticleName -- назва послуги
		                            , sa.Description	--опис процедури
                            FROM dbo.tbServiceAssign sa 
                            LEFT JOIN dbo.tbServicePoint SP ON SP.ID = SA.PointID
                            LEFT JOIN dbo.vwTarifItem T ON T.ID = SA.TarifitemID
                            LEFT JOIN dbo.vwPersonDiscount PD ON PD.ID = sa.CardID
                            LEFT JOIN dbo.tbServiceExecutor se on se.ID = sa.ExecutorID
                            JOIN dbo.tbPerson p on p.id = se.PersonID
                            WHERE sa.PointID = $pointID$
                            AND CAST(sa.Start AS DATE) = CAST(GETDATE() AS DATE)
                            ORDER BY sa.Start
                            ";

            script = script.Replace("$pointID$", pointID.ToString());
            using (SqlConnection connection = new SqlConnection (connString))
            {
				connection.Open();
				using (SqlCommand command = new SqlCommand (script, connection))
				{
                command.CommandTimeout = 600;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                res.Add(new Models.ServiceAssign()
                                {
                                    PointId = GetIntegerSqlDef(reader, "PointID", 0),
                                    PointName = GetStringSqlDef(reader, "PointName", string.Empty),
                                    Start = GetDateTimeSql(reader, "Start") ?? DateTime.MinValue,
                                    End = GetDateTimeSql(reader, "Finish") ?? DateTime.MinValue,
                                    Length = Properties.Settings.Default.Length == true 
                                        ? GetStringSqlDef(reader, "Length", string.Empty) 
                                        : string.Empty,
                                    Performer = Properties.Settings.Default.Performer == true
                                        ? GetStringSqlDef(reader, "Performer", string.Empty)
                                        : string.Empty,
                                    PersonName = Properties.Settings.Default.PersonName == true 
                                        ? GetStringSqlDef(reader, "PersonName", string.Empty) 
                                        : string.Empty,
                                    ArticleName = Properties.Settings.Default.ArticleName == true 
                                        ? GetStringSqlDef(reader, "ArticleName", string.Empty) 
                                        : string.Empty,
                                    Description = Properties.Settings.Default.Comment == true 
                                        ? GetStringSqlDef(reader, "Description", string.Empty)
                                        : string.Empty,
                                });
                            }
                        }
                    }
				}
            }

            foreach(var a in res)
            {
                a.StartStr = a.Start.ToString("yyyy-MM-ddTHH:mm:ss");
                a.EndStr = a.End.ToString("yyyy-MM-ddTHH:mm:ss");
                a.Performer = string.IsNullOrEmpty(a.Performer) ? "" : "Виконавець: " + a.Performer + ", ";
                a.Length = string.IsNullOrEmpty(a.Length) ? "" : "Тривалість: " + a.Length + "хв., ";
                a.ArticleName = string.IsNullOrEmpty(a.ArticleName) ? "" : "Назва процедури: " + a.ArticleName + ", ";
                a.PersonName = string.IsNullOrEmpty(a.PersonName) ? "" : "Замовник: " + a.PersonName + ", ";
                a.Description = string.IsNullOrEmpty(a.Description) ? "" : "Опис: " + a.Description;
            }
            return res;
        }

        public static DateTime? GetDateTimeSql(SqlDataReader reader, string v)
        {
            int idx = -1;
            try
            {
                idx = reader.GetOrdinal(v);
            }
            catch
            {
                idx = -1;
            }
            if (idx >= 0 && !reader.IsDBNull(idx))
            {
                return reader.GetDateTime(idx);
            }
            else
                return null;
        }

        private static DateTime GetDateTimeSqlDef(DataRow dr, string fldName, DateTime val_def)
        {
            var val = DateTime.MinValue;
            if (!dr.Table.Columns.Contains(fldName) || !DateTime.TryParse(dr[fldName].ToString(), out val))
                val = val_def;
            return val;
        }

        public static int GetIntegerSqlDef(SqlDataReader reader, string v, int val_def)
        {
            return GetIntegerSql(reader, v) ?? val_def;
        }

        private static Int32? GetIntegerSql(SqlDataReader reader, string v)
        {
            int idx = -1;
            try
            {
                idx = reader.GetOrdinal(v);
            }
            catch
            {
                idx = -1;
            }
            if (idx >= 0 && !reader.IsDBNull(idx))
            {
                var dotNetType = reader.GetFieldType(idx);
                if (dotNetType == typeof(Int16) || dotNetType == typeof(Int32))
                    return (Int32)reader.GetValue(idx);
                else
                if (dotNetType == typeof(Byte))
                    return reader.GetByte(idx);
            }
            return null;
        }

        public static decimal GetDecimalSqlDef(SqlDataReader reader, string v, decimal val_def)
        {
            return GetDecimalSql(reader, v) ?? val_def;
        }

        public static Decimal? GetDecimalSql(SqlDataReader reader, string v)
        {
            int idx = -1;
            try
            {
                idx = reader.GetOrdinal(v);
            }
            catch
            {
                idx = -1;
            }
            if (idx >= 0 && !reader.IsDBNull(idx))
            {
                var dotNetType = reader.GetFieldType(idx);
                if (dotNetType == typeof(Double))
                    return (Decimal)reader.GetDouble(idx);
                else
                if (dotNetType == typeof(Decimal))
                    return reader.GetDecimal(idx);
            }
            return null;
        }

        public static String GetStringSql(SqlDataReader reader, string v)
        {

            int idx = -1;
            try
            {
                idx = reader.GetOrdinal(v);
            }
            catch
            {
                idx = -1;
            }
            if (idx >= 0 && !reader.IsDBNull(idx))
            {
                var dotNetType = reader.GetFieldType(idx);
                if (dotNetType == typeof(Int16) || dotNetType == typeof(Int32) || dotNetType == typeof(Int64))
                    return Convert.ToString(reader.GetInt32(idx));
                else
                if (dotNetType == typeof(bool) || dotNetType == typeof(Boolean))
                    return Convert.ToString(reader.GetBoolean(idx) ? 1 : 0);
                else
                    return (string)reader[v];
            }

            return null;
        }

        public static string GetStringSqlDef(SqlDataReader reader, string v, string val_def)
        {
            return GetStringSql(reader, v) ?? val_def;
        }

        public static Boolean? GetBooleanSql(SqlDataReader reader, string v)
        {
            int idx = -1;
            try
            {
                idx = reader.GetOrdinal(v);
            }
            catch
            {
                idx = -1;
            }
            if (idx >= 0 && !reader.IsDBNull(idx))
                return (Boolean)reader[v];

            return null;
        }

        public static bool GetBooleanSqlDef(SqlDataReader reader, string v, bool val_def)
        {
            return GetBooleanSql(reader, v) ?? val_def;
        }

        public static String GetDateTimeStrSql(SqlDataReader reader, string v)
        {
            int idx = -1;
            try
            {
                idx = reader.GetOrdinal(v);
            }
            catch
            {
                idx = -1;
            }
            if (idx >= 0 && !reader.IsDBNull(idx))
            {
                DateTime res = reader.GetDateTime(idx);
                return DateTimeToStr(res);
            }
            else
                return null;
        }

        public static String DateTimeToStr(DateTime? dt)
        {
            if (dt == null)
                return string.Empty;
            else
                return (dt ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
