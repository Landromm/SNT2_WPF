using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel.DataControl
{
    class Data_RTC
    {
        delegate string InvertedString(string? input);

		#region Поля.
		private DateTime _dateTime;
		static private double bufferDoubleData;
		#endregion

		#region Свойства.
		public int Id
		{
			get; set;
		}
		public DateTime DateTimes
		{
			get => _dateTime;
			set => _dateTime = value;
		}
		#endregion

		#region Поля и свойства Data_NV
		private string? _timeAndDate;                    //Текущее время и дата.
		[MaxLength(20)]
		public string? TimeAndDate
		{
			get => _timeAndDate;
			set
			{
				string tempStr = invertedString(value);
				StringBuilder strBuilder = new StringBuilder();
				if (tempStr != null)
				{
					strBuilder.Append(tempStr[0]);
					strBuilder.Append(tempStr[1]);
					strBuilder.Append("-");
					strBuilder.Append(tempStr[2]);
					strBuilder.Append(tempStr[3]);
					strBuilder.Append("-");
					strBuilder.Append(tempStr[4]);
					strBuilder.Append(tempStr[5]);
					strBuilder.Append(" ");
					strBuilder.Append(tempStr[10]);
					strBuilder.Append(tempStr[11]);
					strBuilder.Append(":");
					strBuilder.Append(tempStr[14]);
					strBuilder.Append(tempStr[15]);
					strBuilder.Append(":");
					strBuilder.Append(tempStr[18]);
					strBuilder.Append(tempStr[19]);

					_timeAndDate = strBuilder.ToString();
				}
			}
		}

		private string? _serialNumber;                   //Заводской номер.
		[MaxLength(20)]
		public string? SerialNumber
		{
			get => _serialNumber;
			set => _serialNumber = invertedString(value);
		}

		private string? _accessCode;                     //Код доступа.
		[MaxLength(20)]
		public string? AccessCode
		{
			get => _accessCode;
			set => _accessCode = invertedString(value);
		}

		private string? _configurator;                   //Конфигуратор.
		[MaxLength(20)]
		public string? Configurator
		{
			get => _configurator;
			set => _configurator = invertedString(value);
		}

		private string? _interface;                      //Интерфейс.
		[MaxLength(20)]
		public string? Interface
		{
			get => _interface;
			set => _interface = invertedString(value);
		}

		private string? _levelSignalU1Channel1;          //Уровень сингнала U1 - Канала №1.
		[MaxLength(20)]
		public string? LevelSignalU1Channel1
		{
			get => _levelSignalU1Channel1;
			set => _levelSignalU1Channel1 = value;
		}

		private string? _levelSignalU2Channel1;          //Уровень сингнала U2 - Канала №1.
		[MaxLength(20)]
		public string? LevelSignalU2Channel1
		{
			get => _levelSignalU2Channel1;
			set => _levelSignalU2Channel1 = value;
		}

		private string? _levelSignalU3Channel1;          //Уровень сингнала U3 - Канала №1.
		[MaxLength(20)]
		public string? LevelSignalU3Channel1
		{
			get => _levelSignalU3Channel1;
			set => _levelSignalU3Channel1 = value;
		}

		private string? _levelSignalU1Channel2;          //Уровень сингнала U1 - Канала №2.
		[MaxLength(20)]
		public string? LevelSignalU1Channel2
		{
			get => _levelSignalU1Channel2;
			set => _levelSignalU1Channel2 = value;
		}

		private string? _levelSignalU2Channel2;          //Уровень сингнала U2 - Канала №2.
		[MaxLength(20)]
		public string? LevelSignalU2Channel2
		{
			get => _levelSignalU2Channel2;
			set => _levelSignalU2Channel2 = value;
		}

		private string? _levelSignalU3Channel2;          //Уровень сингнала U3 - Канала №2.
		[MaxLength(20)]
		public string? LevelSignalU3Channel2
		{
			get => _levelSignalU3Channel2;
			set => _levelSignalU3Channel2 = value;
		}

		private string? _errorConection;                 //Ошибка связи со счетчиком.
		[MaxLength(20)]
		public string? ErrorConection
		{
			get => _errorConection;
			set => _errorConection = value;
		}

		private string? _errorChannel1;                  //Ошибки канала №1.
		[MaxLength(20)]
		public string? ErrorChannel1
		{
			get => _errorChannel1;
			set => _errorChannel1 = value;
		}

		private string? _errorChannel2;                  //Ошибки канала №2.
		[MaxLength(20)]
		public string? ErrorChannel2
		{
			get => _errorChannel2;
			set => _errorChannel2 = value;
		}

		private string? _errorSystem;                    //Ошибки системы.
		[MaxLength(20)]
		public string? ErrorSystem
		{
			get => _errorSystem;
			set => _errorSystem = value;
		}

		private string? _temperatureChanel1;             //Температура канал №1.
		[MaxLength(20)]
		public string? TemperatureChanel1
		{
			get => _temperatureChanel1;
            set => _temperatureChanel1 = invertedString(value);
		}

		private string? _temperatureChanel2;             //Температура канал №2.
		[MaxLength(20)]
		public string? TemperatureChanel2
		{
			get => _temperatureChanel2;
			set => _temperatureChanel2 = invertedString(value);
		}

		private string? _temperature_T3;                 //Температура Т3.
		[MaxLength(20)]
		public string? Temperature_T3
		{
			get => _temperature_T3;
			set => _temperature_T3 = invertedString(value);
		}

		private string? _temperature_T4_T5;              //Температура Т4/T5.
		[MaxLength(20)]
		public string? Temperature_T4_T5
		{
			get => _temperature_T4_T5;
			set => _temperature_T4_T5 = invertedString(value);
		}

		private string? _commissioning;                  //Ввод в эксплуатацию.
		[MaxLength(20)]
		public string? Commissioning
		{
			get => _commissioning;
			set => _commissioning = invertedString(value);
		}

		private string? _lowDifferenceTemp;              //Малая разность температур.
		[MaxLength(20)]
		public string? LowDifferenceTemp
		{
			get => _lowDifferenceTemp;
			set => _lowDifferenceTemp = invertedString(value);
		}

		private string? _pressure_P3;                    //Давление Р3.
		[MaxLength(20)]
		public string? Pressure_P3
		{
			get => _pressure_P3;
			set => _pressure_P3 = invertedString(value);
		}

		private string? _pressure_P4_P5;                 //Давление Р4/P5.
		[MaxLength(20)]
		public string? Pressure_P4_P5
		{
			get => _pressure_P4_P5;
			set => _pressure_P4_P5 = invertedString(value);
		}

		private string? _downtime;                       //Время простоя.
        [MaxLength(20)]
        public string? Downtime
        {
            get => _downtime;
            set => _downtime = invertedString(value);
        }

        private string? _runningTime;                    //Время наработки.
        [MaxLength(20)]
        public string? RunningTime
        {
            get => _runningTime;
            set => _runningTime = invertedString(value);
        }

        private string? _irregularFlowTime;              //Ненармированный расход (время).
		[MaxLength(20)]
		public string? IrregularFlowTime
		{
			get => _irregularFlowTime;
			set => _irregularFlowTime = invertedString(value);
		}

		private string? _excessFlowTime;                 //Превышение расхода (время).
		[MaxLength(20)]
		public string? ExcessFlowTime
		{
			get => _excessFlowTime;
			set => _excessFlowTime = invertedString(value);
		}

		private string? _noFlowTime;                     //Отсутствие расхода (время).
		[MaxLength(20)]
		public string? NoFlowTime
		{
			get => _noFlowTime;
			set => _noFlowTime = invertedString(value);
		}

		private string? _negativeFlowTime;               //Отрицательный расход (время).
		[MaxLength(20)]
		public string? NegativeFlowTime
		{
			get => _negativeFlowTime;
			set => _negativeFlowTime = invertedString(value);
		}

		private string? _defectTime;                     //Неисправность (время).
		[MaxLength(20)]
		public string? DefectTime
		{
			get => _defectTime;
			set => _defectTime = invertedString(value);
		}

		private string? _noPowerTime;                    //Отсутствие питания (время).
		[MaxLength(20)]
		public string? NoPowerTime
		{
			get => _noPowerTime;
			set => _noPowerTime = invertedString(value);
		}

		private string? _maxSensorsPressure;             //Максимум датчиков давления.
		[MaxLength(20)]
		public string? MaxSensorsPressure
		{
			get => _maxSensorsPressure;
			set => _maxSensorsPressure = invertedString(value);
		}
		#endregion

		/// <summary>
		/// Инверсия полученной строки в обратном порядке по байтам.
		/// </summary>
		InvertedString invertedString = inputStr =>
		{
			string tempstr = "";
			for (int i = 0; i < inputStr.Length; i += 2)
			{
				tempstr = tempstr.Insert(0, inputStr.Substring(i, 2));
			}
			if (double.TryParse(tempstr, out bufferDoubleData))
				return tempstr!;

			return null!;
		};
	}
}
