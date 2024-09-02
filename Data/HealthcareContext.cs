using Microsoft.EntityFrameworkCore;
using HealthcareManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using HealthcareManagementSystem.Models;

namespace HealthcareManagementSystem.Data
{
  public class HealthcareContext : DbContext
  {
    public HealthcareContext(DbContextOptions<HealthcareContext> options) : base(options) { }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Billing> Billings { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
  }
}