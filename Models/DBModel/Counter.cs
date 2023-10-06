using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DBModel
{
    public class Counter
    {
        [MaxLength(20)]
        public string? CounterId { get; set; }

        [MaxLength(20)]
        public string? NameCounter { get; set; }

        public List<ListValue> ListValues { get; set; }


        public int ProjectObjectId { get; set; }
        public ProjectObject ProjectObject { get; set; }
    }
}
