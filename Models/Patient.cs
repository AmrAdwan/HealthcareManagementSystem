using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthcareManagementSystem.Models
{
  public class Patient
  {
    public int PatientId { get; set; }

    [Required]
    public string? FirstName { get; set; }

    [Required]
    public string? LastName { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string? Gender { get; set; }

    [Required]
    [RegularExpression(@"^.*[a-zA-Z]+.*$", ErrorMessage = "The address must contain at least one letter.")]
    public string? Address { get; set; }

    [Required]
    public string? PhoneNumber { get; set; }

    [Required]
    public string? Password { get; set; }

    [NotMapped]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? PasswordConfirmation { get; set; }

    public ICollection<MedicalRecord>? MedicalRecords { get; set; }
    public ICollection<Appointment>? Appointments { get; set; }
    public ICollection<Billing>? Billings { get; set; }
  }
}
