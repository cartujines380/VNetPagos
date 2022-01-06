using System.Collections.Generic;

namespace VisaNet.Presentation.Administration.Models.Chart
{
    public class Chart
    {
        public ICollection<Column> cols { get; set; }
        public ICollection<Row> rows { get; set; }
    }
}