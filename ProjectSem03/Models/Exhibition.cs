using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSem03.Models
{
    [Table("Exhibition")]
    public class Exhibition
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExhibitionId { get; set; }
        [Required(ErrorMessage = "ExhibitionName is required.....")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "ExhibitionName must be from 3 to 50 characters")]
        public string ExhibitionName { get; set; }
        [Required(ErrorMessage = "Place is required.....")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Place must be from 10 to 100 characters")]
        public string Place { get; set; }
        [Required(ErrorMessage = "StartDate is required.....")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "EndDate is required.....")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required]
        public string StaffId { get; set; }
    }
}
