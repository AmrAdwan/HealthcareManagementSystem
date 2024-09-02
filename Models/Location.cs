namespace HealthcareManagementSystem.Models
{
  public class Location
  {
    public int LocationId { get; set; }
    public string? Name { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; } 
  }
}
