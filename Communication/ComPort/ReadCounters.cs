using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SNT2_WPF.Communication.IniData;
using SNT2_WPF.Communication.Logger;
using SNT2_WPF.Models.DataConEF;
using SNT2_WPF.Models.DataModel.DataControl;
using SNT2_WPF.Models.DBModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SNT2_WPF.Communication.ComPort;
internal class ReadCounters : IReadCounters
{
	static int timeoutSend;
	static int timeoutRead;
	static int okResultProcedure;
	static int errorCount;
	static int limitErrorCom;
	//--------------------------
	private static string? temp_PortName;
	private static string? temp_BaudRate;
	private static string? temp_Parity;
	private static string? temp_StopBits;
	private static string? temp_DataBits;
	//--------------------------
	static bool checkSumCRC;
	private static string? tempStr;
	//--------------------------
	private static readonly CommunicationManager comm = new();
	private static SendMessage sendMsg = new();
	private static LogWriter logWriter = new();
	private static ProjectObject projectObject = new();
	private static Dictionary<int, List<int>> dictionary = new();

	public ReadCounters() 
	{
		timeoutSend = 250;
		timeoutRead = 1000;
		okResultProcedure = 0;
		errorCount = 0;
		limitErrorCom = 0;
		//--------------------------
		checkSumCRC = false;
		tempStr = string.Empty;
	}

	public Task StartReadCounters()
	{
		if(sendMsg.CountNumberCounter != 0)
			InitializationDB(sendMsg.CountNumberCounter);

		#region Цикл опроса счетчика.

		for (int i = 0; i < sendMsg.CountNumberCounter; i++)
		{
			Console.WriteLine("Отправляемые пакеты данных для счетчика #{0}:", sendMsg.NumbersCounters[i]);
			Console.WriteLine(sendMsg.SendStartSessionHex[i] + " - сообщение инициализации обмена.");
			Console.WriteLine(sendMsg.SendWritePage128Hex[i] + " - сообщение записи страницы 128 байт данных.");
			Console.WriteLine(sendMsg.SendWritePage256Hex[i] + " - сообщение записи страницы 256 байт данных.");
			Console.WriteLine(sendMsg.SendReadDataHex[i] + " - сообщение чтения данных со страницы.");
			Console.WriteLine("\n");
		}
		
		int countReadData = 0;
		ParamFromConfiguration_Load();
		OpenComPort();

		while (true)
		{
			for (int i = 0; i < sendMsg.CountNumberCounter; i++)
			{
				countReadData = 0;
				do
				{
					try
					{
						comm.WriteData(sendMsg.SendStartSessionHex[i]);      //Инициализации обмена данными со счетчиком.
						Thread.Sleep(timeoutSend);
						WriteDataRTC(sendMsg.SendWritePage128Hex[i], sendMsg.SendReadDataHex[i]);

						if (comm.DataByteList.Count != 0)
							checkSumCRC = CheckSumCRC(comm.DataByteList);

						if (!checkSumCRC)
						{
							countReadData++;
						}
						else
						{   //Начало опроса.
							countReadData = 0;
							FillingAnObject_RTC(i + 1);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						//errorCount++;
						continue;
					}
				}
				while (!checkSumCRC && countReadData < 3);

				countReadData = 0;
				do
				{
					try
					{
						comm.WriteData(sendMsg.SendStartSessionHex[i]);      //Инициализации обмена данными со счетчиком.
						Thread.Sleep(timeoutSend);
						WriteDataNV(sendMsg.SendWritePage256Hex[i], sendMsg.SendReadDataHex[i]);

						if (comm.DataByteList.Count != 0)
							checkSumCRC = CheckSumCRC(comm.DataByteList);

						if (!checkSumCRC)
						{
							countReadData++;
						}
						else
						{   //Начало опроса.
							countReadData = 0;
							FillingAnObject_NV(i + 1);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						continue;
					}
				}
				while (!checkSumCRC && countReadData < 3);
			}
		}
		#endregion
	}

	//Метод чтения параметров COM-порт из ini. файла
	private void ParamFromConfiguration_Load()
	{
		try
		{
			IniFile INI = new(@ConfigurationManager.AppSettings["pathConfig"]);
			temp_PortName = INI.ReadINI("COMportSettings", "PortName");
			temp_BaudRate = INI.ReadINI("COMportSettings", "BaudRate");
			temp_Parity = INI.ReadINI("COMportSettings", "Parity");
			temp_StopBits = INI.ReadINI("COMportSettings", "StopBits");
			temp_DataBits = INI.ReadINI("COMportSettings", "DataBits");
			timeoutRead = Convert.ToInt32(INI.ReadINI("COMportSettings", "Timeout"));
			limitErrorCom = Convert.ToInt32(INI.ReadINI("SNTConfig", "LimitErrorCom"));

			logWriter.LoadFlagLog();
		}
		catch (Exception ex)
		{
			Console.WriteLine("Ошибка чтения config.ini файла!\n" + ex);
		}
	}

	//Метод открытия COM-порта.
	private void OpenComPort()
	{
		comm.OpenPort(	_baudRate: temp_BaudRate, 
						_dataBits: temp_DataBits, 
						_stopBits: temp_StopBits, 
						_parity: temp_Parity, 
						_portName: temp_PortName);

		if (comm.ComPortIsOpen())
		{
			string info = "Параметры COM-порта:\n" +
				 "Четность: " + temp_Parity + "\n" +
				 "Стоповые биты: " + temp_StopBits + "\n" +
				 "Биты данных: " + temp_DataBits + "\n" +
				 "Бит в секунду: " + temp_BaudRate + "\n" +
				 "Таймаут: " + timeoutRead + "\n";

			Console.WriteLine(info);
			logWriter.WriteInformation(info);
		}
	}

	//Метод опроса счетчика - пакет данных 128 байт - RTC
	private void WriteDataRTC(string writePageMsg, string readDataMsg)
	{
		comm.WriteData(writePageMsg);  //Записи данных на страницу '0' в счетчике (128 байт).

		Thread.Sleep(timeoutSend);

		comm.Count = 0;
		comm.DataByteList.Clear();
		comm.WriteData(readDataMsg);   //Чтения данных со счетчика.            
		Console.WriteLine("---------------------------------------------");

		Thread.Sleep(timeoutRead);
	}

	//Метод опроса счетчика - пакет данных 256 байт - RTC
	private void WriteDataNV(string writePageMsg, string readDataMsg)
	{
		comm.WriteData(writePageMsg);  //Записи данных на страницу '0' в счетчике (128 байт).

		Thread.Sleep(timeoutSend);

		comm.Count = 0;
		comm.DataByteList.Clear();
		comm.WriteData(readDataMsg);   //Чтения данных со счетчика.            
		Console.WriteLine("---------------------------------------------");

		Thread.Sleep(timeoutRead);
	}

	//Метод подсчета CRC - пакета данных 128 байт.
	private bool CheckSumCRC(List<string> dataList)
	{
		int resultCRC = 0x00;
		for (int i = 0; i < dataList.Count - 1; i++)
		{
			resultCRC ^= Convert.ToInt32(dataList[i], 16);
		}
		resultCRC ^= 0xA5;
		//Console.WriteLine("Контрольная сумма результат:\t" + resultCRC);

		if (resultCRC == Convert.ToInt32(dataList.Last(), 16))
		{
			Console.Write("Чтение данных - OK.");
		}
		else
		{
			Console.Write("Контрольная сумма не совпадает!");
		}

		Console.ResetColor();
		return resultCRC == Convert.ToInt32(dataList.Last(), 16) ? true : false;
	}

	//Метод выборки байт из массива и преобразования их в строку по индексам.
	private static string FormatData(Range range)
	{
		tempStr = string.Empty;
		;
		string[] buffArray = new string[comm.DataByteList.Count];
		int count = 0;
		foreach (var item in comm.DataByteList)
		{
			buffArray[count++] = item.ToString();
		}
		foreach (var item in buffArray[range])
		{
			tempStr += item;
		}
		return tempStr;
	}

	//Метод заполнения объекта данных 128 байт.
	private void FillingAnObject_RTC(int indexCount)
	{
		Data_RTC data_RTC = new();

		try
		{
			data_RTC.DateTimes = DateTime.Now;
			data_RTC.TimeAndDate = FormatData(2..12);
			data_RTC.SerialNumber = FormatData(17..20);
			data_RTC.AccessCode = FormatData(20..23);
			data_RTC.Configurator = FormatData(23..25);
			data_RTC.Interface = FormatData(25..28);
			data_RTC.LevelSignalU1Channel1 = FormatData(45..46);
			data_RTC.LevelSignalU2Channel1 = FormatData(44..45);
			data_RTC.LevelSignalU3Channel1 = FormatData(43..44);
			data_RTC.LevelSignalU1Channel2 = FormatData(48..49);
			data_RTC.LevelSignalU2Channel2 = FormatData(47..48);
			data_RTC.LevelSignalU3Channel2 = FormatData(46..47);
			data_RTC.ErrorConection = "0"; //Переопределен как ошибка связи со счетчиком.
			data_RTC.ErrorChannel1 = FormatData(63..64);
			data_RTC.ErrorChannel2 = FormatData(64..65);
			data_RTC.ErrorSystem = FormatData(65..66);
			data_RTC.TemperatureChanel1 = FormatData(66..69);
			data_RTC.TemperatureChanel2 = FormatData(69..72);
			data_RTC.Temperature_T3 = FormatData(72..75);
			data_RTC.Temperature_T4_T5 = FormatData(75..78);
			data_RTC.Commissioning = FormatData(78..82);
			data_RTC.LowDifferenceTemp = FormatData(89..92);
			data_RTC.Pressure_P3 = FormatData(92..94);
			data_RTC.Pressure_P4_P5 = FormatData(94..96);
			data_RTC.Downtime = FormatData(99..102);
			data_RTC.RunningTime = FormatData(103..106);
			data_RTC.IrregularFlowTime = FormatData(107..110);
			data_RTC.ExcessFlowTime = FormatData(111..114);
			data_RTC.NoFlowTime = FormatData(115..118);
			data_RTC.NegativeFlowTime = FormatData(119..122);
			data_RTC.DefectTime = FormatData(123..126);
			data_RTC.NoPowerTime = FormatData(126..129);
			data_RTC.MaxSensorsPressure = FormatData(129..130);

			Console.WriteLine($"Время:{data_RTC.DateTimes}");
			SetValueDB_RTC(data_RTC, indexCount);
		}
		catch (Exception ex)
		{
			using (var context = new DataContext())
			{
				GetSqlProcedure(context, dictionary[indexCount][11], "1", DateTime.Now);
			}
			if (errorCount >= limitErrorCom)
			{
				errorCount = 0;
				comm.ClosePort();
				comm.OpenPort(	_baudRate: temp_BaudRate, 
								_dataBits: temp_DataBits, 
								_stopBits: temp_StopBits, 
								_parity: temp_Parity, 
								_portName: temp_PortName);
			}
			else
			{
				errorCount++;
				string error = $"Количество накопительных ошибок: {errorCount} из {limitErrorCom}";
				//OutputConsole_Error("\n\nНе получены данные со счетчика!\n\n");
				Console.WriteLine(error);
				logWriter.WriteError($"Не получены данные со счетчика!\t" + error);
			}
		}
	}
	//Метод заполнения объекта данных 256 байт.
	private void FillingAnObject_NV(int indexCount)
	{
		Data_NV data_NV = new();
		try
		{
			data_NV.DateTimes = DateTime.Now;
			//-ОБЩИЕ--------------------------------------------------------------------------------------------
			data_NV.FlowVolume_Sum_Diff = FormatData(78..81);
			data_NV.FlowVolume_Sum_Diff_JunTetrad = FormatData(81..82);
			data_NV.Volume_Sum_Diff = FormatData(206..210);
			//-КАНАЛ №1------------------------------------------------------------------------------------------
			data_NV.DiameterRU = FormatData(2..5);
			data_NV.BaseL = FormatData(5..8);
			data_NV.CutoffThreshold = FormatData(8..10);
			data_NV.ParameterU = FormatData(13..14);
			data_NV.ParameterZ = FormatData(11..13);
			data_NV.ParameterI = FormatData(10..11);
			data_NV.ZeroOffset = FormatData(14..18);
			data_NV.DelayTime = FormatData(20..24);
			data_NV.FlowMax = FormatData(24..27);
			data_NV.FlowBoundary = FormatData(27..30);
			data_NV.TotalTime = FormatData(30..34);
			data_NV.DifferenceTime = FormatData(34..38);
			data_NV.FlowVolume = FormatData(38..42);
			data_NV.ThermalPowerMW = FormatData(42..45);
			data_NV.ThermalPowerGcall = FormatData(45..48);
			data_NV.FlowMass = FormatData(56..60);
			data_NV.Volume = FormatData(60..64);
			data_NV.Mass = FormatData(64..68);
			data_NV.ThermalEnergyGJ = FormatData(68..72);
			data_NV.Pressure = FormatData(82..84);
			data_NV.WeightPulseInput = FormatData(95..97);
			data_NV.LengthRU = FormatData(97..100);
			data_NV.ThermalPowerGJ = FormatData(107..110);
			data_NV.FlowPulseInput = FormatData(114..117);
			data_NV.VolumePulseInput = FormatData(117..121);
			data_NV.Viscosity = FormatData(125..127);
			data_NV.ThermalEnergyGCall = FormatData(196..200);
			//-КАНАЛ №2------------------------------------------------------------------------------------------
			data_NV.DiameterRU_ch2 = FormatData(130..133);
			data_NV.BaseL_ch2 = FormatData(133..136);
			data_NV.CutoffThreshold_ch2 = FormatData(136..138);
			data_NV.ParameterU_ch2 = FormatData(141..142);
			data_NV.ParameterZ_ch2 = FormatData(139..141);
			data_NV.ParameterI_ch2 = FormatData(138..139);
			data_NV.ZeroOffset_ch2 = FormatData(142..146);
			data_NV.DelayTime_ch2 = FormatData(148..152);
			data_NV.FlowMax_ch2 = FormatData(152..155);
			data_NV.FlowBoundary_ch2 = FormatData(155..158);
			data_NV.TotalTime_ch2 = FormatData(158..162);
			data_NV.DifferenceTime_ch2 = FormatData(162..166);
			data_NV.FlowVolume_ch2 = FormatData(166..170);
			data_NV.ThermalPowerMW_ch2 = FormatData(170..173);
			data_NV.ThermalPowerGcall_ch2 = FormatData(173..176);
			data_NV.FlowMass_ch2 = FormatData(184..188);
			data_NV.Volume_ch2 = FormatData(188..192);
			data_NV.Mass_ch2 = FormatData(192..196);
			data_NV.ThermalEnergyGJ_ch2 = FormatData(72..76);
			data_NV.Pressure_ch2 = FormatData(210..212);
			data_NV.WeightPulseInput_ch2 = FormatData(223..225);
			data_NV.LengthRU_ch2 = FormatData(225..228);
			data_NV.ThermalPowerGJ_ch2 = FormatData(235..238);
			data_NV.FlowPulseInput_ch2 = FormatData(242..245);
			data_NV.VolumePulseInput_ch2 = FormatData(245..249);
			data_NV.Viscosity_ch2 = FormatData(253..255);
			data_NV.ThermalEnergyGCall_ch2 = FormatData(200..204);

			//OutputConsole_NV(data_NV);
			SetValueDB_NV(data_NV, indexCount);
		}
		catch (Exception ex)
		{
			using (var context = new DataContext())
			{
				GetSqlProcedure(context, dictionary[indexCount][11], "1", DateTime.Now);
			}
			if (errorCount >= limitErrorCom)
			{
				errorCount = 0;
				comm.ClosePort();
				comm.OpenPort(	_baudRate: temp_BaudRate, 
								_dataBits: temp_DataBits,
								_stopBits: temp_StopBits, 
								_parity: temp_Parity, 
								_portName: temp_PortName);
			}
			else
			{
				errorCount++;
				string error = $"Количество накопительных ошибок: {errorCount} из {limitErrorCom}";
				//OutputConsole_Error("\n\nНе получены данные со счетчика!\n\n");
				logWriter.WriteError($"Не получены данные со счетчика!\t\n {error}" + ex);
			}
		}
	}

	// Метод передачи данных RTC-памяти в БД
	private void SetValueDB_RTC(Data_RTC data_RTC, int indexCount)
	{
		try
		{
			using (var context = new DataContext())
			{
				// [indexCount] = -1 от id (ListValueId)
				GetSqlProcedure(context, dictionary[indexCount][0], data_RTC.TimeAndDate, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][1], data_RTC.SerialNumber, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][2], data_RTC.AccessCode, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][3], data_RTC.Configurator, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][4], data_RTC.Interface, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][5], data_RTC.LevelSignalU1Channel1, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][6], data_RTC.LevelSignalU2Channel1, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][7], data_RTC.LevelSignalU3Channel1, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][8], data_RTC.LevelSignalU1Channel2, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][9], data_RTC.LevelSignalU2Channel2, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][10], data_RTC.LevelSignalU3Channel2, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][11], data_RTC.ErrorConection, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][12], data_RTC.ErrorChannel1, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][13], data_RTC.ErrorChannel2, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][14], data_RTC.ErrorSystem, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][15], data_RTC.TemperatureChanel1, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][16], data_RTC.TemperatureChanel2, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][17], data_RTC.Temperature_T3, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][18], data_RTC.Temperature_T4_T5, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][19], data_RTC.Commissioning, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][20], data_RTC.LowDifferenceTemp, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][21], data_RTC.Pressure_P3, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][22], data_RTC.Pressure_P4_P5, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][23], data_RTC.Downtime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][24], data_RTC.RunningTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][25], data_RTC.IrregularFlowTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][26], data_RTC.ExcessFlowTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][27], data_RTC.NoFlowTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][28], data_RTC.NegativeFlowTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][29], data_RTC.DefectTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][30], data_RTC.NoPowerTime, data_RTC.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][31], data_RTC.MaxSensorsPressure, data_RTC.DateTimes);
			}

			if (okResultProcedure > 0)
			{
				Console.WriteLine($"Выполнено процедур: {okResultProcedure}");
				okResultProcedure = 0;
			}
			else
			{
				Console.WriteLine($"Процедуры не выполнены.!!!");
				okResultProcedure = 0;
			}
		}
		catch (Exception ex)
		{
			string error = $"Косяк в отправке выполнения процедур с данными памяти RTC!\n" + ex;
			logWriter.WriteError(error);
		}
	}
	// Метод передачи данных NV-памяти в БД
	private void SetValueDB_NV(Data_NV data_NV, int indexCount)
	{
		try
		{
			using (var context = new DataContext())
			{
				// [indexCount] = -1 от id (ListValueId)
				GetSqlProcedure(context, dictionary[indexCount][32], data_NV.FlowVolume_Sum_Diff, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][33], data_NV.FlowVolume_Sum_Diff_JunTetrad, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][34], data_NV.Volume_Sum_Diff, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][35], data_NV.DiameterRU, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][36], data_NV.BaseL, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][37], data_NV.CutoffThreshold, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][38], data_NV.ParameterU, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][39], data_NV.ParameterZ, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][40], data_NV.ParameterI, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][41], data_NV.ZeroOffset, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][42], data_NV.DelayTime, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][43], data_NV.FlowMax, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][44], data_NV.FlowBoundary, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][45], data_NV.TotalTime, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][46], data_NV.DifferenceTime, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][47], data_NV.FlowVolume, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][48], data_NV.ThermalPowerMW, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][49], data_NV.ThermalPowerGcall, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][50], data_NV.FlowMass, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][51], data_NV.Volume, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][52], data_NV.Mass, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][53], data_NV.ThermalEnergyGJ, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][54], data_NV.Pressure, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][55], data_NV.WeightPulseInput, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][56], data_NV.LengthRU, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][57], data_NV.ThermalPowerGJ, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][58], data_NV.FlowPulseInput, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][59], data_NV.VolumePulseInput, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][60], data_NV.Viscosity, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][61], data_NV.ThermalEnergyGCall, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][62], data_NV.DiameterRU_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][63], data_NV.BaseL_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][64], data_NV.CutoffThreshold_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][65], data_NV.ParameterU_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][66], data_NV.ParameterZ_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][67], data_NV.ParameterI_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][68], data_NV.ZeroOffset_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][69], data_NV.DelayTime_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][70], data_NV.FlowMax_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][71], data_NV.FlowBoundary_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][72], data_NV.TotalTime_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][73], data_NV.DifferenceTime_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][74], data_NV.FlowVolume_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][75], data_NV.ThermalPowerMW_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][76], data_NV.ThermalPowerGcall_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][77], data_NV.FlowMass_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][78], data_NV.Volume_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][79], data_NV.Mass_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][80], data_NV.ThermalEnergyGJ_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][81], data_NV.Pressure_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][82], data_NV.WeightPulseInput_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][83], data_NV.LengthRU_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][84], data_NV.ThermalPowerGJ_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][85], data_NV.FlowPulseInput_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][86], data_NV.VolumePulseInput_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][87], data_NV.Viscosity_ch2, data_NV.DateTimes);
				GetSqlProcedure(context, dictionary[indexCount][88], data_NV.ThermalEnergyGCall_ch2, data_NV.DateTimes);
			}

			if (okResultProcedure > 0)
			{
				Console.WriteLine($"Выполнено процедур: {okResultProcedure}");
				okResultProcedure = 0;
			}
			else
			{
				Console.WriteLine($"Процедуры не выполнены.!!!");
				okResultProcedure = 0;
			}
		}
		catch (Exception ex)
		{
			string error = $"Косяк в отправке выполнения процедур с данными памяти RTC!\n" + ex;
			logWriter.WriteError(error);
		}
	}

	// Вызов процедуры с параметрами 'id', 'value', 'date'.
	private void GetSqlProcedure(DataContext context, int idParam, string? valueParam, DateTime dateParam)
	{
		SqlParameter[] param =
			{
					new ()
					{
						ParameterName = "@p0",
						SqlDbType = System.Data.SqlDbType.Int,
						Value = idParam,
					},
					new ()
					{
						ParameterName = "@p1",
						SqlDbType = System.Data.SqlDbType.VarChar,
						Size = 50,
						Value = valueParam
					},
					new ()
					{
						ParameterName = "@p2",
						SqlDbType = System.Data.SqlDbType.DateTime,
						Value = dateParam
					},
					new ()
					{
						ParameterName = "@p3",
						SqlDbType = System.Data.SqlDbType.Int,
						Direction = System.Data.ParameterDirection.Output
					}
				};
		context.Database.ExecuteSqlRaw("update_cell @p0, @p1, @p2, @p3 output", param);
		if (Convert.ToInt32(param[3].Value) == 1)
			okResultProcedure++;
		//Console.WriteLine($"Результат выполнения процедуры: - {param[3].Value}"); //Если возвращается "1" - значит процедура полностью выполнена.
	}

	// Метод инициализации основных значений и БД.
	private void InitializationDB(int countNumberCounter)
	{
		try
		{
			using (FileStream fs = new FileStream(@"Resources\\db_List_Data.json", FileMode.OpenOrCreate))
			{
				projectObject = JsonSerializer.Deserialize<ProjectObject>(fs);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Этап 1 - Ошибка чтения json-файла.");
			logWriter.WriteError($"{ex}");
		}

		try
		{
			using (var context = new DataContext())
			{
				var checkCreated = context.Database.EnsureCreated();

				if (checkCreated)
				{
					context.Database.ExecuteSqlRaw("-- =============================================\r\n-- Author:\t\t<Радкевич Игорь>\r\n-- Create date: <05.05.2023>\r\n-- Description:\t<Процедура обновления данных в оперативной таблице,\r\n-- и запись этих данных в таблицу истории.>\r\n-- =============================================\r\nCREATE PROCEDURE [dbo].[update_cell] \r\n\r\n\t@cell_id_P int,\r\n\t@cell_value_P varchar(50),\r\n\t@cell_date_P datetime,\r\n\t@cell_out int output\r\n\t\r\nAS\r\n\r\n\tSET NOCOUNT ON;\r\n\tSET DATEFORMAT ymd;\r\n\r\n\tDECLARE @datetime_D datetime\r\n\tDECLARE @value_D VARCHAR(255)\r\n\tDECLARE @hasHistory_D bit\r\n\tDECLARE @idHash_D int\r\n\tDECLARE @changed_D bit\r\n\r\n\t--Выборка конкретной щаписи оп ID.\r\n\tSELECT TOP 1 @datetime_D = DateTimeUpdate, @value_D = Value, @hasHistory_D = Has_History, @idHash_D = Hash, @changed_D = Csd_Changed\r\n\tFROM ListValues\r\n\tWHERE ListValueId = @cell_id_P;\r\n\r\n\t--Обновление значение\r\n\tif @cell_value_P < > @value_D\r\n\t\tBEGIN\r\n\t\t\tUPDATE ListValues\r\n\t\t\tSET Value = @cell_value_P, DateTimeUpdate = @cell_date_P, Csd_Changed = 1\r\n\t\t\tWHERE Hash = @idHash_D;\t\t\t\r\n\t\tEND\r\n\telse\r\n\t\tBEGIN\r\n\t\t\tUPDATE ListValues\r\n\t\t\tSET Value = @cell_value_P, DateTimeUpdate = @cell_date_P, Csd_Changed = 0\r\n\t\t\tWHERE Hash = @idHash_D;\r\n\t\tEND\r\n\t\t\r\n\t--Запись в архивную таблицу\r\n\tif ((@hasHistory_D > 0) AND (@changed_D = 1))\r\n\t\tBEGIN\r\n\t\t\tINSERT INTO History (HashId, Value, DateTime)\r\n\t\t\tVALUES (@idHash_D, @cell_value_P, @cell_date_P)\r\n\t\tEND\r\n\r\n\t--Возврат значения \"1\" - процедура полностью выполнена.\r\n\tSET @cell_out = 1\r\n");
					if (projectObject == null)
						Console.WriteLine("Объект десириализации пуст!");
					else
						context.ProjectObjects.Add(projectObject);
				}
				else
					Console.WriteLine("База данных уже существует!"); //Технический указатель. Требует удаления перед релизом.

				context.SaveChanges();

				for (int i = 1; i <= countNumberCounter; i++)
				{
					dictionary[i] = context.ListValues
						.Where(countId => countId.CounterId == $"СНТ-2 №{i}")
						.Select(id => id.ListValueId).ToList();
				}
			};
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Этап 2 - Ошибка на этапе подключения к БД.");
			logWriter.WriteError($"{ex}");
		}
	}
}
