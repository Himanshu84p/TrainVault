using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TrainVault.CustomValidation;
using TrainVault.DataAccess;

namespace TrainVault.Models
{
    public class TrainingModel
    {
		[Key]
		public int TrainingId { get; set; }
        [Required]
		[FutureDate(ErrorMessage = "The date must be today or in the future")]
        public DateOnly DateOfTraining { get; set; }
        [Required]
        public string Place { get; set; } = null!;
        [Required]
		[MaxLength(100, ErrorMessage = "Max 100 characters")]
        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "Special characters are not allowed.")]
        public string Purpose { get; set; } = null!;
		[Required]
		public int OrganizationId { get; set; }
		[Required]
		public List<int> SelectedEmployeeIds { get; set; } = new List<int>();

		public IEnumerable<SelectListItem> Organizations { get; set; } = Enumerable.Empty<SelectListItem>();
		public IEnumerable<SelectListItem> Employees { get; set; } = Enumerable.Empty<SelectListItem>();
	}
}
