using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DBModel
{
    public class ListValue
    {
        public int ListValueId { get; set; }

        [MaxLength(20)]
        public string? CounterId { get; set; }
        public Counter Counter { get; set; }

        public int Hash { get; set; }

        [MaxLength(20)]
        public string? Type { get; set; }

        [MaxLength(20)]
        public string? Value { get; set; }

        public DateTime DateTimeUpdate { get; set; }

        [MaxLength(100)]
        public string? Description { get; set; }

        public bool Csd_Changed { get; set; }

        public bool Has_History { get; set; }
    }
}
