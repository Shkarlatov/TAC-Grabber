using System.Collections.Generic;

namespace TAC_Grabber.IMEI
{
    // GSM Association Non-confidential
    // Official Document TS.06 - IMEI Allocation and Approval Process
    // Annex A. Reporting Body Identifier List
    // https://www.gsma.com/newsroom/wp-content/uploads//TS.06-v17.0.pdf

    public static class ReportingBodyIdentifierSourceData
    {
        public static IEnumerable<ReportingBodyIdentifier> GetReportingBodyIdentifiers()
        {
            return new[]
            {
                new ReportingBodyIdentifier(01,"CTIA"),
                new ReportingBodyIdentifier(35,"TUV SUD BABT"),
                new ReportingBodyIdentifier(86,"TAF (China)"),
                new ReportingBodyIdentifierLimit(98,100,7800,"Reserved"),
                new ReportingBodyIdentifier(99,"GHA"),

                new ReportingBodyIdentifier(10,"DECT PP",Band.DECT),
                new ReportingBodyIdentifier(30,"Iridium",Band.GSM_Satellite),
                new ReportingBodyIdentifier(33,"DGPT / ART",Band.GSM_900_1800),
                new ReportingBodyIdentifier(44,"BABT",Band.GSM_900_1800),
                new ReportingBodyIdentifier(45,"NTA",Band.GSM_900_1800),
                new ReportingBodyIdentifier(49,"BZT/BAPT/Reg TP",Band.GSM_900_1800),
                new ReportingBodyIdentifier(50,"BZT ETS",Band.GSM_900_1800),
                new ReportingBodyIdentifier(51,"Cetecom ICT Services",Band.GSM_900_1800),
                new ReportingBodyIdentifier(52,"CETECOM",Band.GSM_900_1800),
                new ReportingBodyIdentifier(53,"TUV",Band.GSM_900_1800),
                new ReportingBodyIdentifier(54,"PHOENIX TEST-LAB",Band.GSM_900_1800),

                // Effective 29 April 2019, the 91 TAC RB identifier will be suspended
                // and not used for any new TAC allocations.
                new ReportingBodyIdentifier(91,"MSAI",Band.All),

                // Test IMEI allocating bodies
                //new ReportingBodyIdentifier(00,"Test ME"),
                new ReportingBodyIdentifierLimit(00, 10_01_00, 10_18_00,"CTTIA"),
                new ReportingBodyIdentifierLimit(00, 44_00_00, 45_00_00,"TUV SUD BABT"),
                new ReportingBodyIdentifierLimit(00, 86_00_00, 87_00_00,"TUV SUD BABT"),

            };
        }
    }
}
