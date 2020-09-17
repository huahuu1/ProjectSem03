using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSem03.Models
{
    [Table("Award")]
    public class Award
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AwardId { get; set; }
        [Required(ErrorMessage = "AwardName is required.....")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "AwardName must be from 3 to 50 characters")]
        public string AwardName { get; set; }
        [Required]
        public int CompetitionID { get; set; }

        public int? PostingID { get; set; } // no need required
        [Required]
        public string StaffId { get; set; }
    }
}
