using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SNT2_WPF.Communication.Logger;

namespace SNT2_WPF.Communication.ComPort
{
    internal class CommunicationManager
    {
		public enum TransmissionType { Text, Hex }

        //property variables
        private string? _baudRate = string.Empty;
        private string? _parity = string.Empty;
        private string? _stopBits = string.Empty;
        private string? _dataBits = string.Empty;
        private string? _portName = string.Empty;
        private int count;
        private List<string> _dataByteList;

        #region Manager Properties
        public List<string> DataByteList
        {
            get { return _dataByteList; }
            set { _dataByteList = value; }
        }
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string? BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }
        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string? Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }
        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string? StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }
        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string? DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }
        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string? PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }
        #endregion

        private SerialPort comPort = new();
		private LogWriter logWriter;
		public CommunicationManager(string baud, string par, string sBits, string dBits, string name)
        {
            _baudRate = baud;
            _parity = par;
            _stopBits = sBits;
            _dataBits = dBits;
            _portName = name;
            _dataByteList = new List<string>();

            //now add an event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
            logWriter = new();
        }
        public CommunicationManager()
        {
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
            _portName = "COM2";
            _dataByteList = new List<string>();

            //add event handler
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
			logWriter = new();
		}

		/// <summary>
		/// Метод проверки открытия COM-порта.
		/// </summary>
		public bool ComPortIsOpen()
        {
            if (comPort.IsOpen && !comPort.BreakState)
                return true;
            else
                return false;
        }

        #region OpenPort
        /// <summary>
        /// Метод открытия COM-порта.
        /// </summary>
        public bool OpenPort(string? _baudRate, string? _dataBits, string? _stopBits, string? _parity, string? _portName)
        {
			try
            {
                if (comPort.IsOpen) comPort.Close();
                comPort.BaudRate = int.Parse(_baudRate is not null ? _baudRate : "9600");
                comPort.DataBits = int.Parse(_dataBits is not null ? _dataBits : "8");
                comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits is not null ? _stopBits : "One");
                comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity is not null ? _parity : "None");
                comPort.PortName = _portName is not null ? _portName : "COM2";

                comPort.Open();

                logWriter.WriteInformation("Открытие COM-порта: " + comPort.PortName);
                Thread.Sleep(5000);

				logWriter.WriteInformation("COM-порт открыт: " + comPort.PortName);
				return true;
            }
            catch (Exception ex)
            {
                string error = "Не удалось открыть COM-Порт - " + comPort.PortName + " |\n\t" + ex;
                logWriter.WriteError(error);
                return false;
            }
        }
        #endregion

        #region ClosePort
        /// <summary>
        /// Метод закрытия COM-порта.
        /// </summary>
        public bool ClosePort()
        {
            try
            {
                if (comPort.IsOpen == true || comPort.BreakState)
                {
                    comPort.BreakState = false;
                    comPort.Close();

					logWriter.WriteInformation("Закрытие COM-порта: " + comPort.PortName);
					logWriter.WriteInformation("COM-порт закрыт: " + comPort.PortName);
				}
                return true;
            }
            catch (Exception ex)
            {
				string error = "Не удалось корректно закрыть COM-Порт - " + comPort.PortName + " |\n\t" + ex;
                logWriter.WriteError(error);
				return false;
            }
        }
        #endregion

        #region HexToByte
        /// <summary>
        /// Метод конвертации строки-hex в массив байтов.
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] HexToByte(string msg)
        {
            try
            {
                msg = msg.Replace(" ", "");
                byte[] comBuffer = new byte[msg.Length / 2];
                for (int i = 0; i < msg.Length; i += 2)
                    comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
                return comBuffer;
            }
            catch (Exception ex)
            {
                string error = "Ошибка конвертации полученной строки в массив байтов! \n";
                logWriter.WriteError(error + ex);
                //протетсировать данноое возвращение!!!!!!!!!!!! Возможно наличие багов.
                throw;
            }
        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// Метод конвертации массива байт в строку-hex.
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        private string ByteToHex(byte[] comByte)
        {
            try
            {
                StringBuilder builder = new StringBuilder(comByte.Length * 3);
                foreach (byte data in comByte)
                    builder.Append(Convert.ToString(data, 16).PadLeft(2, '0'));

                return builder.ToString().ToUpper();
            }
            catch (Exception ex)
            {
                string error = "Ошибка конвертации массива байт в строку-hex! \n";
				logWriter.WriteError(error + ex);

				return string.Empty;
            }
        }
        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// Метод, который будет вызываться, когда в буфере ожидают данные.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!comPort.BreakState)
            {
                try
                {
                    int bytes = comPort.BytesToRead;
                    byte[] comBuffer = new byte[bytes];
                    comPort.Read(comBuffer, 0, bytes);
                    //Console.WriteLine("{0} " + ByteToHex(comBuffer), count);
                    //Console.WriteLine(ByteToHex(comBuffer));
                    count += bytes;
                    foreach (var item in comBuffer)
                    {
                        _dataByteList.Add(Convert.ToString(item, 16).PadLeft(2, '0').ToUpper());
                    }
                    //Console.WriteLine("Размер коллекции: " + _dataByteList.Count);
                }
                catch (Exception ex)
                {
                    string error = "Ошибка чтения с COM-порта.! \n";
                    logWriter.WriteError(error + ex);
                }
            }
            else
            {
                string str = comPort.ReadExisting();
				logWriter.WriteError("Разрыв связи с COM-портом: " + str + "\n");
			}
		}
        #endregion

        #region WriteData
        /// <summary>
        /// Метод отправки данных-hex счетчику.
        /// </summary>
        /// <param name="msg">string hex</param>
        public void WriteData(string msg)
        {
            try
            {
                if (!(comPort.IsOpen == true))
                    comPort.Open();
                byte[] newMsg = HexToByte(msg);
                //Console.WriteLine("\n" + "ОТПРАВКА hex-сообщения счетчику: |" + msg);
                comPort.Write(newMsg, 0, newMsg.Length);
            }
            catch (FormatException ex)
            {
                string error = "Ошибка отправки hex - сообщения счетчику: \n";
                logWriter.WriteError(error + ex);
            }
        }
        #endregion
    }
}
