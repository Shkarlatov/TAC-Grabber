using System.Collections.Generic;

namespace TAC_Grabber.IMEI
{
    public class ReportingBodyIdentifier
    {
        public ReportingBodyIdentifier(int rBI, string usedBy, Band band = Band.All)
        {
            RBI = rBI;
            UsedBy = usedBy;
            Band = band;
        }

        public int RBI { get; set; }
        public string UsedBy { get; set; }
        public Band Band { get; set; }

        protected int start_mm = 0;
        protected int stop_mm = length;

        private const int length = 1_000_000;

        public IEnumerable<int> GetAvalibleTAC()
        {
            const int length = 1_000_000;

            for (int i = start_mm; i < stop_mm; i++)
            {
                yield return RBI * length + i;
            }
        }

    }
}
