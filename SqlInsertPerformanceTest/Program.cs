using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInsertPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new InsertTest();
            var startDate = DateTime.Now;
            var endDate = startDate.AddYears(1);
            var data = GenerateTestData(startDate, endDate);

            
            test.ClearTable();
            test.ConstructedSQLWithoutParamters(data);
            test.ClearTable();
            test.ConstructedSQLWithParamters(data);
            test.ClearTable();
            test.StoredProcedure(data);
            test.ClearTable();
            test.ConstructedSQLWithSeveralValues(data);
            test.ClearTable();
            test.StoredProcedureList(data);
            test.ClearTable();
            test.InsertListTestSqlDataRecord(data);
            test.ClearTable();
            test.StoredProcedureXml(data);
            
            Console.ReadLine();
        }

        private static List<CurveData> GenerateTestData(DateTime startDate, DateTime endDate)
        {
            List<CurveData> result = new List<CurveData>();
            DateTime date = startDate.Date.ToUniversalTime();
            endDate = endDate.Date.ToUniversalTime();
            Random random = new Random();
            while (date < endDate)
            {
                result.Add(new CurveData()
                {
                    CurveId = CurveType.Type1,
                    TimeStamp = date,
                    Value = System.Convert.ToDecimal(random.NextDouble())
                });

                date = date.AddMinutes(15);
            }
            return result;

        }
    }
}
