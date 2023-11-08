using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel
{
    public class MainDataModel
    {
		//Свойства
		public string? NumberCounter { get; set; }                  //Номер счетчика

        public bool CheckErrorConection { get; set; } = false;      //Статус связи со счетчиком.
        public string? HashCheckErrorConection { get; set; }        //Хэш статуса связи со счетчиком.

        public string? DescriptionCounter { get; set; }             //Наименование счетчика.

		public string? Pressure_ch1 { get; set; }                   //Давление канала №1.
		public string? HashPressure_ch1 { get; set; }               //Хэш давления канала №1.

        public string? Temperature_ch1 { get; set; }                //Температура канала №1.
        public string? HashTemperature_ch1 { get; set; }            //Хэш температуры канала №1.

        public string? Flow_ch1 { get; set; }                       //Расход канала №1.
        public string? HashFlow_ch1 { get; set; }                   //Хэш расхода канала №1

        public string? Pressure_ch2 { get; set; }                   //Давление канала №2.
        public string? HashPressure_ch2 { get; set; }               //Хэш давления канала №2.

        public string? Temperature_ch2 { get; set; }                //Температура канала №2.
        public string? HashTemperature_ch2 { get; set; }            //Хэш температуры канала №2.

        public string? Flow_ch2 { get; set; }                       //Расход канала №2.
        public string? HashFlow_ch2 { get; set; }                   //Хэш расхода канала №2.
    }
}
