using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProjectSem03.Models
{
    [Table("Staff")]
    public class Staff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required(ErrorMessage = "StaffId is required.....")]
        [RegularExpression(@"^STA+[0-9]{2,6}$")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "AccId must be from 3 to 9 characters")]
        public string StaffId { get; set; }
        [Required(ErrorMessage = "StaffName is required.....")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "StaffName must be from 3 to 50 characters")]
        public string StaffName { get; set; }
        [Required(ErrorMessage = "Password is required.....")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z])(?!.*\s).+$", ErrorMessage = "Password must including numbers and characters and do not contains whitespace")]
        [StringLength(49, MinimumLength = 5, ErrorMessage = "Password must be from 5 to 49 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "JoinDate is required.....")]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }
        public string ProfileImage { get; set; }
        [Required(ErrorMessage = "Email is required.....")]
        [RegularExpression(@"^[a-zA-z]+[0-9]*@[a-zA-z]+.com*$")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone is required.....")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone must be from 10 to 20 characters")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address is required.....")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Address must be from 5 to 255 characters")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        [Required(ErrorMessage = "Role is required.....")]
        [Range(0, 2, ErrorMessage = "Role must be from 0 to 2")]
        public int Role { get; set; }

        public ICollection<Posting> Posting { get; set; }
        public ICollection<Award> Award { get; set; }
        public ICollection<Exhibition> Exhibition { get; set; }
        public ICollection<Competition> Competition { get; set; }
    }
}
