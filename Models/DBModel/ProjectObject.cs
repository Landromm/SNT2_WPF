using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DBModel
{
    public class ProjectObject
    {

        public int ProjectObjectId { get; set; }

        [MaxLength(50)]
        public string? NameObject { get; set; }

        public List<Counter> Counters { get; set; }
    }
}
