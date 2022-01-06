namespace VisaNet.Domain.EntitiesDtos
{
    public class BinFileProcessResultDto
    {
        public int LinesProcessed { get; set; }
        public int Inserts { get; set; }
        public int Updates { get; set; }
        public int Deletes { get; set; }
        public int Errors { get; set; }
    }
}
