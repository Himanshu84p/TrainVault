using System.ComponentModel.DataAnnotations;
using TrainVault.DataAccess;

namespace TrainVault.Models
{
	public class OrganizationModel
	{
        [Key]
        public int OrganizationId { get; set; }

        [Required(ErrorMessage = "Organization Name is required.")]
        [MinLength(3, ErrorMessage = "Organization Name must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Organization Name must be less than 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Organization Name can only contain letters and spaces.")]
        public string OrganizationName { get; set; } = null!;

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(70, ErrorMessage = "Address must be less than 255 characters.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(30, ErrorMessage = "City must be less than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City can only contain letters and spaces.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [MaxLength(30, ErrorMessage = "State must be less than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "State can only contain letters and spaces.")]
        public string? State { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(30, ErrorMessage = "Country must be less than 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Country can only contain letters and spaces.")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [MaxLength(10, ErrorMessage = "Phone number must be 10 digits.")]
        [RegularExpression(@"^[7-9][0-9]{9}$", ErrorMessage = "Enter valid phone number")]
        public string? Phone { get; set; }

        public bool IsDeleted { get; set; }

		public virtual ICollection<EmployeeModel> Employees { get; set; } = new List<EmployeeModel>();
	}
}
