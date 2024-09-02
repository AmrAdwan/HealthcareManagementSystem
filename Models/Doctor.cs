using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareManagementSystem.Models
{
  public class Doctor
  {
    public int DoctorId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Specialization { get; set; }
    public required string PhoneNumber { get; set; }

    [Required]
    public string? Password { get; set; }

    [NotMapped]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? PasswordConfirmation { get; set; }

    [Required]
    public string Rights { get; set; } = "User"; // Default to "User"
    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Location>? Locations { get; set; }
  }
}