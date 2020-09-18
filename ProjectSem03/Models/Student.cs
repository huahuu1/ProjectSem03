using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectSem03.Models
{
    [Table("Student")]
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        //[Required(ErrorMessage = "StudentId is required.")]
        //[StringLength(10, MinimumLength = 3, ErrorMessage = "StudentId must be STUXXXXXXX")]         
        public string StudentId { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Password must be from 5 to 50 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //CONFIRM PASSWORD
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; } //END CONFIRMPASSWORD
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "FirstName must be from 3 to 50 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "LastName must be from 3 to 50 characters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "DateOfBirth is required.")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [MinimumAge(16, ErrorMessage = "Age must be >= 16")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone must be from 10 to 20 characters")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "JoinDate is required.")]
        public DateTime JoinDate { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, MinimumLength = 5, ErrorMessage = "Address must be from 5 to 255 characters")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        public string ProfileImage { get; set; }

        public ICollection<Design> Design { get; set; }
    }
}
