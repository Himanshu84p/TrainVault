using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TrainVault.Models
{
	public class EmployeeModel
	{
		[Key]
		public int EmployeeId { get; set; }

		[DisplayName("Organization")]
		[Required]
		public int OrganizationId { get; set; }
		[Required]
		[MaxLength(50, ErrorMessage = "Max 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Special characters are not allowed.")]
        public string FirstName { get; set; } = null!;
		[MaxLength(50, ErrorMessage = "Max 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Special characters are not allowed.")]
        public string LastName { get; set; } = null!;
		[Required]
		[EmailAddress]

		public string Email { get; set; } = null!;
		[Required]
        [RegularExpression(@"^[7-9][0-9]{9}$", ErrorMessage = "Enter valid mobile number")]
        public string? Phone { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Special characters are not allowed.")]
		[MaxLength(50, ErrorMessage = "Max 50 characters")]

        public string? JobTitle { get; set; }

		public bool? IsDeleted { get; set; }

		public virtual OrganizationModel Organization { get; set; } = null!;
	}
}
