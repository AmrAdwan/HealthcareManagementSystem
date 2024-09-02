using System;
using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.Models
{
  public class Appointment
  {
    public int AppointmentId { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public int LocationId { get; set; }
    public Location? Location { get; set; }
  }
}
