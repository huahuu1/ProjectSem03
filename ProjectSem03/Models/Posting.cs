using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSem03.Models
{
    [Table("Posting")]
    public class Posting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostingId { get; set; }
        [Required(ErrorMessage = "PostDate is required.")]
        [DataType(DataType.Date)]
        public DateTime PostDate { get; set; }
        public string PostDescription { get; set; }
        public string Mark { get; set; }
        public string Remark { get; set; }
        [ForeignKey("DesignID")]
        public int DesignID { get; set; }
        public bool SoldStatus { get; set; }
        public bool PaidStatus { get; set; }
        [ForeignKey("CompetitionId")]
        public int CompetitionId { get; set; }
        [ForeignKey("StaffId")]
        public string StaffId { get; set; }

        public ICollection<Award> Award { get; set; }
    }
}
