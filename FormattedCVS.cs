namespace TAC_Grabber
{
    public class FormattedCVS
    {

        public string Host { get; set; }
        public string TAC { get; set; }
        public string Result { get; set; }

        public override string ToString()
        {
            return $"{Host}\t{TAC}\t{Result}";
        }
    }
}
