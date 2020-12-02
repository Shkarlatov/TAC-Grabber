namespace TAC_Grabber.IMEI
{

    public class ReportingBodyIdentifierLimit : ReportingBodyIdentifier
    {

        public ReportingBodyIdentifierLimit(int rBI, int start_mm, int stop_mm, string usedBy, Band band = Band.All) : base(rBI, usedBy, band)
        {
            this.start_mm = start_mm;
            this.stop_mm = stop_mm;
        }
    }
}
