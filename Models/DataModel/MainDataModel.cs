﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel
{
    public class MainDataModel
    {
		//Properties
		public string? NumberCounter { get; set; }

        public bool CheckErrorConection { get; set; } = false;
        public string? HashCheckErrorConection { get; set; }

        public string? DescriptionCounter { get; set; }

		public string? Pressure_ch1 { get; set; }
		public string? HashPressure_ch1 { get; set; }

        public string? Temperature_ch1 { get; set; }
        public string? HashTemperature_ch1 { get; set; }

        public string? Flow_ch1 { get; set; }
        public string? HashFlow_ch1 { get; set; }

        public string? Pressure_ch2 { get; set; }
        public string? HashPressure_ch2 { get; set; }

        public string? Temperature_ch2 { get; set; }
        public string? HashTemperature_ch2 { get; set; }

        public string? Flow_ch2 { get; set; }
        public string? HashFlow_ch2 { get; set; }
    }
}
