using Microsoft.Data.SqlClient;
using SNT2_WPF.Communication.IniData;
using SNT2_WPF.Models.DataModel;
using SNT2_WPF.Models.DataConEF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SNT2_WPF.Communication.Logger;
using SNT2_WPF.Models.DBModel;
using System.IO;
using System.Text.Json;

namespace SNT2_WPF.Models.GenerationModel
{
    public class GenerationData
    {
        private readonly IniFile INI = new(@"Resources\Config.ini");
        MainDataModel dataModel;
        static ProjectObject? projectObject;
        static Dictionary<int, List<int>> dictionary;
        string[] counters;


        //Properties
        public int CountCounter { get; set; }

        public GenerationData()
        {
            dataModel = new MainDataModel();
            projectObject = new ProjectObject();
            dictionary = new Dictionary<int, List<int>>();
            counters = new string[4];
            InitializationCountCounter();
            InitializationData();
        }

        private void InitializationCountCounter()
        {
            string tempNumberCounters = INI.ReadINI("SNTConfig", "NumberCounter");
            counters = tempNumberCounters.Replace(" ", "").Split(',');
            CountCounter = counters.Length;
        }

        public void GenerationDataStart()
        {
            Random randomConnect = new Random();
            Random randomPressure = new Random();
            Random randomTemperature = new Random();
            Random randomFlow = new Random();

            using(var context = new DataContext())
            {
                for (int i = 0; i < CountCounter; i++)
                {
                    DateTime dateTime = DateTime.Now;   
                    GetSqlProcedure(context, dictionary[i][0], randomConnect.Next(0,1).ToString(), dateTime);
                    GetSqlProcedure(context, dictionary[i][1], counters[i], dateTime);
                    GetSqlProcedure(context, dictionary[i][2], randomPressure.Next(30, 35).ToString(), dateTime);
                    GetSqlProcedure(context, dictionary[i][3], randomTemperature.Next(49, 73).ToString(), dateTime);
                    GetSqlProcedure(context, dictionary[i][4], randomFlow.Next(1020,1450).ToString(), dateTime);
                    GetSqlProcedure(context, dictionary[i][5], randomPressure.Next(30, 35).ToString(), dateTime);
                    GetSqlProcedure(context, dictionary[i][6], randomTemperature.Next(52, 70).ToString(), dateTime);
                    GetSqlProcedure(context, dictionary[i][7], randomFlow.Next(1220, 1250).ToString(), dateTime);
				}
            }
        }

        // Вызов процедуры с параметрами 'id', 'value', 'date'.
        private static void GetSqlProcedure(DataContext context, int idParam, string valueParam, DateTime dateParam)
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
            //if (Convert.ToInt32(param[3].Value) == 1)
            //    okResultProcedure++;

            //Console.WriteLine($"Результат выполнения процедуры: - {param[3].Value}"); //Если возвращается "1" - значит процедура полностью выполнена.
        }

        // Метод инициализации основных значений .
        private void InitializationData()
        {
            dictionary[0] = new List<int>() { 12, 2, 55, 16, 51, 82, 17, 78};
            dictionary[1] = new List<int>() { 101, 91, 144, 105, 140, 171, 106, 167};
            dictionary[2] = new List<int>() { 190, 180, 233, 194, 229, 260, 195, 156};
            dictionary[3] = new List<int>() { 279, 269, 322, 253, 318, 349, 284, 345};

        }
    }
}
