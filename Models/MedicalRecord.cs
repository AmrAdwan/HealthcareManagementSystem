using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthcareManagementSystem.Models
{
  public class MedicalRecord
  {
    public int MedicalRecordId { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
  }
}