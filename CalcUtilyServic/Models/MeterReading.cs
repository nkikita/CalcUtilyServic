using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalcUtilyServic.Models
{
    public class MeterReading
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public int ResidentsCount { get; set; }

        public decimal HWS_Prev { get; set; }
        public decimal HWS_Curr { get; set; }

        public decimal GWS_Prev { get; set; }
        public decimal GWS_Curr { get; set; }

        public decimal Elec_Day_Prev { get; set; }
        public decimal Elec_Day_Curr { get; set; }

        public decimal Elec_Night_Prev { get; set; }
        public decimal Elec_Night_Curr { get; set; }

        public decimal TotalAmount { get; set; }
    }

}
