using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInsertPerformanceTest
{
    public class InsertTest
    {
        private const string ConnectionString = "Data Source=.\\sqlexpress;Initial Catalog=InsertTest;Integrated Security=True";

        public void ConstructedSQLWithoutParamters(List<CurveData> data)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    foreach (var item in data)
                    {
                        insertCommand.CommandText = string.Format("INSERT INTO dbo.CurveData ([CurveId], [TimeStamp], [Value]) VALUES({0},'{1}', {2})", (int)item.CurveId, item.TimeStamp, item.Value);
                        insertCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("ConstructedSQLWithoutParamters: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void ConstructedSQLWithParamters(List<CurveData> data)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandText = "INSERT INTO dbo.CurveData ([CurveId], [TimeStamp], [Value]) VALUES(@CurveId, @TimeStamp, @Value)";
                    insertCommand.CommandType = System.Data.CommandType.Text;

                    var curveIdParamter = new SqlParameter("CurveId", SqlDbType.Int);
                    var timeStampParamter = new SqlParameter("TimeStamp", SqlDbType.DateTime);
                    var valueParamter = new SqlParameter("Value", SqlDbType.Decimal);
                    insertCommand.Parameters.Add(curveIdParamter);
                    insertCommand.Parameters.Add(timeStampParamter);
                    insertCommand.Parameters.Add(valueParamter);

                    foreach (var item in data)
                    {
                        curveIdParamter.Value = (int)item.CurveId;
                        timeStampParamter.Value = item.TimeStamp;
                        valueParamter.Value = item.Value;
                        insertCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("ConstructedSQLWithParamters: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void ConstructedSQLWithSeveralValues(List<CurveData> data)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandType = System.Data.CommandType.Text;
                    StringBuilder insertStatement = new StringBuilder();

                    // max 1000 items/statement
                    int counter = 0;
                    foreach (var item in data)
                    {
                        if (counter == 0)
                        {
                            insertStatement.Append("INSERT INTO dbo.CurveData ([CurveId], [TimeStamp], [Value]) Values ");
                        }
                        insertStatement.Append(string.Format("({0}, '{1}', {2}),", (int)item.CurveId, item.TimeStamp, item.Value));

                        counter++;
                        if (counter == 100)
                        {
                            //remove last coma
                            insertStatement.Length--;
                            insertCommand.CommandText = insertStatement.ToString();
                            insertCommand.ExecuteNonQuery();
                            insertStatement.Clear();
                            counter = 0;
                        }
                    }

                    if (counter > 0)
                    {
                        //remove last coma
                        insertStatement.Length--;
                        insertCommand.CommandText = insertStatement.ToString();
                        insertCommand.ExecuteNonQuery();
                        insertStatement.Clear();
                        counter = 0;
                    }
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("ConstructedSQLWithSeveralValues: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void StoredProcedure(List<CurveData> data)
        {

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandText = "dbo.InsertCurveData";
                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    var curveIdParamter = new SqlParameter("CurveId", SqlDbType.Int);
                    var timeStampParamter = new SqlParameter("TimeStamp", SqlDbType.DateTime);
                    var valueParamter = new SqlParameter("Value", SqlDbType.Decimal);
                    insertCommand.Parameters.Add(curveIdParamter);
                    insertCommand.Parameters.Add(timeStampParamter);
                    insertCommand.Parameters.Add(valueParamter);

                    foreach (var item in data)
                    {
                        curveIdParamter.Value = item.CurveId;
                        timeStampParamter.Value = item.TimeStamp;
                        valueParamter.Value = item.Value;
                        insertCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("StoredProcedure: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void StoredProcedureList(List<CurveData> data)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {

                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandText = "dbo.InsertCurveDataList";
                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    DataTable parameterTable = new DataTable();
                    parameterTable.Columns.Add("CurveId", typeof(int));
                    parameterTable.Columns.Add("[TimeStamp]", typeof(DateTime));
                    parameterTable.Columns.Add("[Value]", typeof(decimal));

                    foreach (var item in data)
                    {
                        parameterTable.Rows.Add(item.CurveId, item.TimeStamp, item.Value);
                    }

                    Console.WriteLine("StoredProcedureList: {0} items data converted in {1} ms.", data.Count, watch.ElapsedMilliseconds);

                    SqlParameter tvparam = insertCommand.Parameters.AddWithValue("@List", parameterTable);
                    tvparam.SqlDbType = System.Data.SqlDbType.Structured;
                    insertCommand.ExecuteNonQuery();
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("StoredProcedureList: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void StoredProcedureXml(List<CurveData> data)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {

                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandText = "dbo.InsertCurveDataXml";
                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;

                    DataTable parameterTable = new DataTable("data");
                    parameterTable.Columns.Add("CurveId", typeof(int));
                    parameterTable.Columns.Add("TimeStamp", typeof(DateTime));
                    parameterTable.Columns.Add("Value", typeof(decimal));
                    parameterTable.Columns[1].DateTimeMode = DataSetDateTime.Utc;
                    foreach (var item in data)
                    {
                        parameterTable.Rows.Add(item.CurveId, item.TimeStamp, item.Value);
                    }
                    var test = parameterTable.Select("TimeStamp = 'Mar 27 2016  1:15AM'").ToList();

                    using (StringWriter sw = new StringWriter())
                    {
                        parameterTable.WriteXml(sw);
                        sw.Flush();
                        SqlParameter tvparam = insertCommand.Parameters.AddWithValue("@data", sw.ToString());
                        tvparam.SqlDbType = System.Data.SqlDbType.Xml;
                    }

                    Console.WriteLine("StoredProcedureXml: {0} items data converted in {1} ms.", data.Count, watch.ElapsedMilliseconds);

                    insertCommand.ExecuteNonQuery();
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("InsertListTestXml: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void InsertListTestSqlDataRecord(List<CurveData> data)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlMetaData[] meta = new SqlMetaData[] {
            new SqlMetaData("CurveId", SqlDbType.Int),
            new SqlMetaData("TimeStamp", SqlDbType.DateTime),
            new SqlMetaData("Value", SqlDbType.Decimal),
        };
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    var insertCommand = connection.CreateCommand();
                    insertCommand.Transaction = tran;
                    insertCommand.CommandText = "dbo.InsertCurveDataList";
                    insertCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    List<SqlDataRecord> records = new List<SqlDataRecord>();

                    foreach (var item in data)
                    {
                        SqlDataRecord r = new SqlDataRecord(meta);
                        r.SetValues(item.CurveId, item.TimeStamp, item.Value);
                        records.Add(r);
                    }

                    Console.WriteLine("InsertListTestSqlDataRecord: {0} items data converted in {1} ms.", data.Count, watch.ElapsedMilliseconds);

                    SqlParameter tvparam = insertCommand.Parameters.AddWithValue("@List", records);
                    tvparam.SqlDbType = System.Data.SqlDbType.Structured;
                    insertCommand.ExecuteNonQuery();
                    tran.Commit();
                }
            }
            watch.Stop();
            Console.WriteLine("InsertListTestSqlDataRecord: {0} items saved in {1} ms.", data.Count, watch.ElapsedMilliseconds);
        }
        public void ClearTable()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var deleteCommand = connection.CreateCommand();
                deleteCommand.CommandText = "delete CurveData";
                deleteCommand.CommandType = System.Data.CommandType.Text;
                deleteCommand.ExecuteNonQuery();

            }

        }
    }
}
