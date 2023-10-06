using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel
{
    public class MainDataModel
    {
        //Properties
        public bool CheckErrorConection { get; set; }
        public string NumberCounter { get; set; }
        public string DescriptionCounter { get; set; }
        public string Pressure_ch1 { get; set; }
        public string Temperature_ch1 { get; set; }
        public string Flow_ch1 { get; set; }
        public string Pressure_ch2 { get; set; }
        public string Temperature_ch2 { get; set; }
        public string Flow_ch2 { get; set; }
    }
}
