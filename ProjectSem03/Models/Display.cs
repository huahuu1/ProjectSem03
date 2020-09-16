using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectSem03.Models
{
    public class Display
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DisplayID { get; set; }

        public double Price { get; set; }
        public bool SoldStatus { get; set; }
        public bool PaidStatus { get; set; }
        [Key]
        [Column(Order = 2)]
        public int DesignID { get; set; }
        [Key]
        [Column(Order = 1)]
        public int ExhibitionID { get; set; }
        public string CustomerName { get; set; }
    }
}
