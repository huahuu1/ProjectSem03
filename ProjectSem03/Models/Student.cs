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
        public string StudentId { get; set; }


        [RegularExpression(@"^(?=.*\d)(?=.*[a-zA-Z])(?!.*\s).+$", ErrorMessage = "Password must including numbers and characters and do not contains whitespace")]
        //[Required(ErrorMessage = "Password is required.")]
        [StringLength(49, MinimumLength = 5, ErrorMessage = "Password must be from 5 to 49 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //CONFIRM PASSWORD
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; } //END CONFIRMPASSWORD
        [RegularExpression(@"^([A-Z][a-z]*)$", ErrorMessage = "FirstName do not contains number or special characters or whitespaces and the first character must be capitalized")]
        [Required(ErrorMessage = "FirstName is required.")]
        [StringLength(49, MinimumLength = 1, ErrorMessage = "FirstName must be from 1 to 49 characters")]
        public string FirstName { get; set; }

        [RegularExpression(@"^([A-Z][a-z]*([\s][A-Z][a-z]*)*)$", ErrorMessage = "LastName do not contains number or special characters or invalid whitespaces and the first character must be capitalized")]
        [Required(ErrorMessage = "LastName is required.")]
        [StringLength(49, MinimumLength = 1, ErrorMessage = "LastName must be from 1 to 49 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "DateOfBirth is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [MinimumAge(16, ErrorMessage = "Age must be >= 16")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }

        [DataType(DataType.PhoneNumber)]        
        [RegularExpression(@"^[\d]+$", ErrorMessage = "Phone must be only numberic characters")]
        [Required(ErrorMessage = "Phone is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone must be 10 characters")]
        public string Phone { get; set; }

        [RegularExpression(@"^([\w]{2,})([.][\w]{2,})*[@][\w]{2,}([.][a-zA-Z]{2,3})+$", ErrorMessage = "Invalid Email ( Email must be xx@xx.xxx or xx.xx@xx.xx.xx )")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(49, MinimumLength = 9, ErrorMessage = "Maximum characters of Email is 9-49")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "JoinDate is required.")]
        public DateTime JoinDate { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(254, MinimumLength = 5, ErrorMessage = "Address must be from 5 to 254 characters")]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        public string ProfileImage { get; set; }

        public ICollection<Design> Design { get; set; }
    }
}
