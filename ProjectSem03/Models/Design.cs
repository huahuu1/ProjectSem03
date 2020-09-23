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
        [StringLength(30, MinimumLength = 3, ErrorMessage = "DesignName must be from 3 to 30 characters")]
        public string DesignName { get; set; }

        public string Painting { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Description must be from 3 to 255 characters")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "SubmitDate is required.")]
        [DataType(DataType.Date)]
        public DateTime SubmitDate { get; set; }

        [Required(ErrorMessage ="Price is required.")]
        [Range(100, 1000000, ErrorMessage = "Price must be between 100 and 1,000,000$")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [ForeignKey("StudentId")]
        public string StudentId { get; set; }

        [ForeignKey("ExhibitionID")]
        public int? ExhibitionID { get; set; }

        public ICollection<Posting> Posting { get; set; }
    }
}
