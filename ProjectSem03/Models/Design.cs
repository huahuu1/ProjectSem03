using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSem03.Models
{
    [Table("Design")]
    public class Design
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DesignId { get; set; }
        [Required(ErrorMessage = "DesignName is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "DesignName must be from 3 to 100 characters")]
        public string DesignName { get; set; }
        //[Required(ErrorMessage = "Painting is required.")]
        //[DataType(DataType.Upload)]
        public string Painting { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Description must be from 3 to 255 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Required(ErrorMessage = "SubmitDate is required.")]
        [DataType(DataType.Date)]
        public DateTime SubmitDate { get; set; } //date <= competitions end date
        public double Price { get; set; }
        //[Required]
        [ForeignKey("StudentId")]
        public string StudentId { get; set; }
        [ForeignKey("ExhibitionID")]
        public int? ExhibitionID { get; set; }

        public ICollection<Posting> Posting { get; set; }
    }
}
