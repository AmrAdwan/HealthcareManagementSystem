using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using Microsoft.AspNetCore.Http;

namespace HealthcareManagementSystem.Controllers
{
  public class PatientsController : Controller
  {
    private readonly HealthcareContext _context;

    public PatientsController(HealthcareContext context)
    {
      _context = context;
    }

    // GET: Patients
    public async Task<IActionResult> Index(int? patientId)
    {
      if (patientId == null)
      {
        return RedirectToAction("Login");
      }

      else if (PatientLoggedin(patientId))
      {
        var patient = await _context.Patients
          .Where(p => p.PatientId == patientId)
          .ToListAsync();

        return View(patient);
      }

      return RedirectToAction("Login");
    }

    // GET: Patients/Details/5
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (PatientLoggedin(id))
      {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Doctor)
            .Include(p => p.Appointments)
                .ThenInclude(a => a.Location)
            .Include(p => p.Billings)
            .Include(p => p.MedicalRecords)
            .FirstOrDefaultAsync(m => m.PatientId == id);

        return View(patient);
      }

      return RedirectToAction("Login");

    }

    // GET: Patients/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Patients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Password,PasswordConfirmation")] Patient patient)
    {
      if (ModelState.IsValid)
      {
        var existingPatient = await _context.Patients
            .FirstOrDefaultAsync(p => p.FirstName == patient.FirstName && p.LastName == patient.LastName);

        if (existingPatient != null)
        {
          ModelState.AddModelError(string.Empty, "A patient with the same first and last name already exists.");
          return View(patient);
        }

        _context.Add(patient);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Login));
      }
      return View(patient);
    }

    // GET: Patients/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (PatientLoggedin(id))
      {
        var patient = await _context.Patients.FindAsync(id);
        return View(patient);
      }

      return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PatientId,FirstName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Password")] Patient patient)
    {
      if (id != patient.PatientId)
      {
        return NotFound();
      }

      var existingPatient = await _context.Patients.AsNoTracking().FirstOrDefaultAsync(p => p.PatientId == id);
      if (existingPatient == null)
      {
        return NotFound();
      }

      bool isValid = true;

      // Validate each field
      if (string.IsNullOrWhiteSpace(patient.FirstName))
      {
        ModelState.AddModelError("FirstName", "First name is required.");
        isValid = false;
      }
      if (string.IsNullOrWhiteSpace(patient.LastName))
      {
        ModelState.AddModelError("LastName", "Last name is required.");
        isValid = false;
      }
      if (patient.DateOfBirth == default(DateTime))
      {
        ModelState.AddModelError("DateOfBirth", "Date of birth is required.");
        isValid = false;
      }
      if (string.IsNullOrWhiteSpace(patient.Gender))
      {
        ModelState.AddModelError("Gender", "Gender is required.");
        isValid = false;
      }
      else if (patient.Gender.ToLower() != "male" && patient.Gender.ToLower() != "female")
      {
        ModelState.AddModelError("Gender", "Gender must be either 'male' or 'female'.");
        isValid = false;
      }
      if (string.IsNullOrWhiteSpace(patient.Address))
      {
        ModelState.AddModelError("Address", "Address is required.");
        isValid = false;
      }
      else if (!System.Text.RegularExpressions.Regex.IsMatch(patient.Address, @"^.*[a-zA-Z]+.*$"))
      {
        ModelState.AddModelError("Address", "The address must contain at least one letter.");
        isValid = false;
      }
      if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
      {
        ModelState.AddModelError("PhoneNumber", "Phone number is required.");
        isValid = false;
      }
      if (string.IsNullOrWhiteSpace(patient.Password))
      {
        ModelState.AddModelError("Password", "Password is required.");
        isValid = false;
      }
      else if (existingPatient.Password != patient.Password)
      {
        ModelState.AddModelError("Password", "The password is incorrect.");
        isValid = false;
      }

      if (isValid)
      {
        try
        {
          patient.Password = existingPatient.Password;

          _context.Update(patient);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index), new { patientId = patient.PatientId });
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!PatientExists(patient.PatientId))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
      }

      return View(patient);
    }



    // GET: Patients/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(id))
      {
        var patient = await _context.Patients
          .FirstOrDefaultAsync(m => m.PatientId == id);

        return View(patient);
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var patient = await _context.Patients.FindAsync(id);
      if (patient != null)
      {
        _context.Patients.Remove(patient);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction("Index", "Home");
    }

    // GET: Patients/AddBilling/5
    public IActionResult AddBilling(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (PatientLoggedin(id))
      {
        return View(new Billing { PatientId = id.Value });
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/AddBilling
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBilling([Bind("Date,Amount,PatientId")] Billing billing)
    {
      if (ModelState.IsValid)
      {
        _context.Add(billing);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Billings), new { id = billing.PatientId });
      }
      return View(billing);
    }

    // GET: Patients/EditBilling/5
    public async Task<IActionResult> EditBilling(int? id)
    {
      var patient_id = HttpContext.Session.GetInt32("PatientId");

      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(patient_id))
      {
        var billing = await _context.Billings.FindAsync(id);
        
        if (billing == null)
        {
          return NotFound();
        }
        return View(billing);
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/EditBilling
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBilling(int id, [Bind("BillingId,Date,Amount,PatientId")] Billing billing)
    {
      if (id != billing.BillingId)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(billing);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!BillingExists(billing.BillingId))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Billings), new { id = billing.PatientId });
      }
      return View(billing);
    }

    // GET: Patients/DeleteBilling/5
    public async Task<IActionResult> DeleteBilling(int? id)
    {
      var patient_id = HttpContext.Session.GetInt32("PatientId");
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(patient_id))
      {
        var billing = await _context.Billings
          .Include(b => b.Patient)
          .FirstOrDefaultAsync(m => m.BillingId == id);
        if (billing == null)
        {
          return NotFound();
        }

        return View(billing);
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/DeleteBilling/5
    [HttpPost, ActionName("DeleteBilling")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBillingConfirmed(int id)
    {
      var billing = await _context.Billings.FindAsync(id);
      _context.Billings.Remove(billing);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Billings), new { id = billing.PatientId });
    }

    // GET: Patients/AddMedicalRecord/5
    public IActionResult AddMedicalRecord(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (PatientLoggedin(id))
      {
        return View(new MedicalRecord { PatientId = id.Value });
      }

      return RedirectToAction("Login");

    }

    // POST: Patients/AddMedicalRecord
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddMedicalRecord([Bind("Date,Description,PatientId")] MedicalRecord medicalRecord)
    {
      if (ModelState.IsValid)
      {
        _context.Add(medicalRecord);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(MedicalRecords), new { id = medicalRecord.PatientId });
      }
      return View(medicalRecord);
    }

    // GET: Patients/EditMedicalRecord/5
    public async Task<IActionResult> EditMedicalRecord(int? id)
    {
      var patient_id = HttpContext.Session.GetInt32("PatientId");
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(patient_id))
      {
        var medicalRecord = await _context.MedicalRecords.FindAsync(id);
        if (medicalRecord == null)
        {
          return NotFound();
        }
        return View(medicalRecord);
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/EditMedicalRecord
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMedicalRecord(int id, [Bind("MedicalRecordId,Date,Description,PatientId")] MedicalRecord medicalRecord)
    {
      if (id != medicalRecord.MedicalRecordId)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(medicalRecord);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!MedicalRecordExists(medicalRecord.MedicalRecordId))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(MedicalRecords), new { id = medicalRecord.PatientId });
      }
      return View(medicalRecord);
    }

    // GET: Patients/DeleteMedicalRecord/5
    public async Task<IActionResult> DeleteMedicalRecord(int? id)
    {
      var patient_id = HttpContext.Session.GetInt32("PatientId");
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(patient_id))
      {
        var medicalRecord = await _context.MedicalRecords
          .Include(m => m.Patient)
          .FirstOrDefaultAsync(m => m.MedicalRecordId == id);
        if (medicalRecord == null)
        {
          return NotFound();
        }

        return View(medicalRecord);
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/DeleteMedicalRecord/5
    [HttpPost, ActionName("DeleteMedicalRecord")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMedicalRecordConfirmed(int id)
    {
      var medicalRecord = await _context.MedicalRecords.FindAsync(id);
      _context.MedicalRecords.Remove(medicalRecord);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(MedicalRecords), new { id = medicalRecord.PatientId });
    }

    public async Task<IActionResult> Appointments(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(id))
      {
        var patient = await _context.Patients
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Location)
                .FirstOrDefaultAsync(m => m.PatientId == id);

        return View(patient);
      }

      return RedirectToAction("Login");
    }

    public async Task<IActionResult> Billings(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(id))
      {
        var patient = await _context.Patients
        .Include(p => p.Billings)
        .FirstOrDefaultAsync(m => m.PatientId == id);

        return View(patient);
      }

      return RedirectToAction("Login");
    }

    public async Task<IActionResult> MedicalRecords(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      else if (PatientLoggedin(id))
      {
        var patient = await _context.Patients
                .Include(p => p.MedicalRecords)
                .FirstOrDefaultAsync(m => m.PatientId == id);

        return View(patient);
      }

      return RedirectToAction("Login");

    }

    // GET: Login
    public async Task<IActionResult> Login()
    {
      var patient_id = HttpContext.Session.GetInt32("PatientId");
      if (PatientLoggedin(patient_id))
      {
        return RedirectToAction(nameof(Index), new { patientId = patient_id });
      }
      return View();
    }

    // POST: Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string FirstName, string LastName, string Password)
    {
      var patient = await _context.Patients
        .FirstOrDefaultAsync(p => p.FirstName == FirstName && p.LastName == LastName && p.Password == Password);

      if (patient != null)
      {
        HttpContext.Session.SetInt32("PatientId", patient.PatientId);
        return RedirectToAction(nameof(Index), new { patientId = patient.PatientId });
      }

      ModelState.AddModelError(string.Empty, "Invalid login attempt.");
      return View();
    }

    private bool PatientLoggedin(int? id)
    {
      var patient_id = HttpContext.Session.GetInt32("PatientId");
      if (patient_id != null && patient_id == id)
      {
        return true;
      }
      return false;
    }
    private bool PatientExists(int id)
    {
      return _context.Patients.Any(e => e.PatientId == id);
    }

    private bool BillingExists(int id)
    {
      return _context.Billings.Any(e => e.BillingId == id);
    }

    private bool MedicalRecordExists(int id)
    {
      return _context.MedicalRecords.Any(e => e.MedicalRecordId == id);
    }
  }
}
