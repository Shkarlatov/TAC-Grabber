using System.Collections.Generic;
using System.Linq;

namespace TAC_Grabber.IMEI
{
    public class IMEIGenerator
    {
        public IEnumerable<string> GetIMEI(int skip=0)
        {
            return ReportingBodyIdentifierSourceData.GetReportingBodyIdentifiers()
              .SelectMany(x => x.GetAvalibleTAC())
              .Skip(skip).Select(x =>string.Format("{0}{1}", x.ToString("00000000"), "0000000" ));  // IMEI=  8 TAC + 7 MM
        }

        public IEnumerable<string> GetValidIMEI(int skip=0)
        {
            return GetIMEI(skip).Select(x => x.Remove(x.Length - 1).AppendCheckDigit());
        }
    }
}
