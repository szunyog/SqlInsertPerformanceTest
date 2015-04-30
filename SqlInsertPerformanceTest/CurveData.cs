using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInsertPerformanceTest
{
public class CurveData
{
    public CurveType CurveId { get; set; }
    public DateTime TimeStamp { get; set; }
    public Decimal Value { get; set; }
}

public enum CurveType
{
    Type1,
    Type2,
    TypeN
}

}
