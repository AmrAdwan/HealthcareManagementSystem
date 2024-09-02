using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareManagementSystem.Controllers
{
  public class DoctorsController : Controller
  {
    private readonly HealthcareContext _context;

    public DoctorsController(HealthcareContext context)
    {
      _context = context;
    }

    public async Task<IActionResult> PatientsList(int? doctorId)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");
      if (DoctorLoggedin(doctor_id))
      {
        var patients = await _context.Patients.ToListAsync();
      return View(patients);
      }
      return RedirectToAction("Login");
    }

    public async Task<IActionResult> Index(int? doctorId)
    {
      if (doctorId == null)
      {
        return RedirectToAction("Login");
      }

      else if (DoctorLoggedin(doctorId))
      {
        var doctor = await _context.Doctors
          .Where(d => d.DoctorId == doctorId)
          .ToListAsync();

        return View(doctor);
      }
      return RedirectToAction("Login");
    }

    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(id))
      {
        var doctor = await _context.Doctors
          .Include(d => d.Appointments)
          .Include(d => d.Locations)
          .FirstOrDefaultAsync(m => m.DoctorId == id);
        return View(doctor);
      }
      return RedirectToAction("Login");
    }

    public IActionResult Create()
    {
      return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Specialization,PhoneNumber,Password,PasswordConfirmation")] Doctor doctor)
    {
      if (ModelState.IsValid)
      {
        doctor.Rights = "User";
        _context.Add(doctor);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Login));
      }
      return View(doctor);
    }

    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(id))
      {
        var doctor = await _context.Doctors.FindAsync(id);
        return View(doctor);
      }
      return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("DoctorId,FirstName,LastName,Specialization,PhoneNumber,Password,PasswordConfirmation,Rights")] Doctor doctor)
    {
      if (id != doctor.DoctorId)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          _context.Update(doctor);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!DoctorExists(doctor.DoctorId))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Index), new { DoctorId = doctor.DoctorId });
      }
      Console.WriteLine("notvalid!!!!");
      return View(doctor);
    }



    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(id))
      {
        var doctor = await _context.Doctors
          .FirstOrDefaultAsync(m => m.DoctorId == id);
        return View(doctor);
      }
      return RedirectToAction("Login");
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var doctor = await _context.Doctors.FindAsync(id);
      if (doctor != null)
      {
        _context.Doctors.Remove(doctor);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    // GET: Doctors/AddAppointment/5
    public IActionResult AddAppointment(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      else if (DoctorLoggedin(id))
      {
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName");
        ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name");
        return View(new Appointment { DoctorId = id.Value });
      }
      return RedirectToAction("Login");
    }

    // POST: Doctors/AddAppointment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAppointment([Bind("Date,PatientId,DoctorId,LocationId")] Appointment appointment)
    {
      if (ModelState.IsValid)
      {
        var existingAppointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date);

        if (existingAppointment != null)
        {
          ModelState.AddModelError("", "The doctor already has an appointment at this time.");
          ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", appointment.PatientId);
          ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", appointment.LocationId);
          return View(appointment);
        }

        _context.Add(appointment);
        await _context.SaveChangesAsync();
        // return RedirectToAction(nameof(Details), new { id = appointment.DoctorId });
        return RedirectToAction(nameof(Appointments), new { id = appointment.DoctorId });
      }

      ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", appointment.PatientId);
      ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", appointment.LocationId);
      return View(appointment);
    }

    public async Task<IActionResult> Appointments(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      else if (DoctorLoggedin(id))
      {
        var doctor = await _context.Doctors
          .Include(d => d.Appointments)
          .ThenInclude(a => a.Patient)
          .Include(d => d.Appointments)
          .ThenInclude(a => a.Location)
          .FirstOrDefaultAsync(m => m.DoctorId == id);

        return View(doctor);
      }
      return RedirectToAction("Login");
    }

    // GET: Doctors/EditAppointment/5
    public async Task<IActionResult> EditAppointment(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");

      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(doctor_id))
      {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
          return NotFound();
        }

        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", appointment.PatientId);
        ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", appointment.LocationId);
        return View(appointment);
      }

      return RedirectToAction("Login");
    }

    // POST: Doctors/EditAppointment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAppointment(int id, [Bind("AppointmentId,Date,PatientId,DoctorId,LocationId")] Appointment appointment)
    {
      if (id != appointment.AppointmentId)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          var existingAppointment = await _context.Appointments
              .FirstOrDefaultAsync(a => a.DoctorId == appointment.DoctorId && a.Date == appointment.Date && a.AppointmentId != appointment.AppointmentId);

          if (existingAppointment != null)
          {
            ModelState.AddModelError("", "The doctor already has an appointment at this time.");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", appointment.PatientId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", appointment.LocationId);
            return View(appointment);
          }

          _context.Update(appointment);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!AppointmentExists(appointment.AppointmentId))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Appointments), new { id = appointment.DoctorId });
      }

      ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FirstName", appointment.PatientId);
      ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "Name", appointment.LocationId);
      return View(appointment);
    }

    public async Task<IActionResult> DeleteAppointment(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");

      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(doctor_id))
      {
        var appointment = await _context.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Include(a => a.Location)
            .FirstOrDefaultAsync(a => a.AppointmentId == id);

        if (appointment == null)
        {
          return NotFound();
        }

        return View(appointment);
      }
      return RedirectToAction("Login");
    }

    [HttpPost, ActionName("DeleteAppointment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAppointmentConfirmed(int id)
    {
      var appointment = await _context.Appointments.FindAsync(id);
      _context.Appointments.Remove(appointment);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Appointments), new { id = appointment.DoctorId });
    }

    // Location Management
    public async Task<IActionResult> Locations(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(id))
      {
        var doctor = await _context.Doctors
          .Include(d => d.Locations)
          .FirstOrDefaultAsync(m => m.DoctorId == id);

        return View(doctor);
      }
      return RedirectToAction(("Login"));

    }

    // GET: Doctors/AddLocation/5
    public IActionResult AddLocation(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(id))
      {
        return View(new Location { DoctorId = id.Value });
      }
      return RedirectToAction("Login");

    }

    // POST: Doctors/AddLocation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLocation([Bind("LocationId,Name,DoctorId")] Location location)
    {
      if (ModelState.IsValid)
      {
        var overlappingLocation = await _context.Locations
            .FirstOrDefaultAsync(l => l.DoctorId == location.DoctorId && l.Name == location.Name);

        if (overlappingLocation != null)
        {
          ModelState.AddModelError("", "The doctor already has a location with this name.");
          return View(location);
        }

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Locations), new { id = location.DoctorId });
      }
      return View(location);
    }

    // GET: Doctors/EditLocation/5
    public async Task<IActionResult> EditLocation(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");

      if (id == null)
      {
        return NotFound();
      }

      else if (DoctorLoggedin(doctor_id))
      {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
        {
          return NotFound();
        }
        return View(location);
      }

      return RedirectToAction("Login");

    }

    // POST: Doctors/EditLocation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditLocation(int id, [Bind("LocationId,Name,DoctorId")] Location location)
    {
      if (id != location.LocationId)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
          var overlappingLocation = await _context.Locations
              .FirstOrDefaultAsync(l => l.DoctorId == location.DoctorId && l.Name == location.Name && l.LocationId != location.LocationId);

          if (overlappingLocation != null)
          {
            ModelState.AddModelError("", "The doctor already has a location with this name.");
            return View(location);
          }

          _context.Update(location);
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!LocationExists(location.LocationId))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
        return RedirectToAction(nameof(Locations), new { id = location.DoctorId });
      }
      return View(location);
    }

    public async Task<IActionResult> DeleteLocation(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");

      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(doctor_id))
      {
        var location = await _context.Locations
          .Include(l => l.Doctor)
          .FirstOrDefaultAsync(l => l.LocationId == id);

        if (location == null)
        {
          return NotFound();
        }

        return View(location);
      }
      return RedirectToAction("Login");
    }

    [HttpPost, ActionName("DeleteLocation")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLocationConfirmed(int id)
    {
      var location = await _context.Locations.FindAsync(id);
      _context.Locations.Remove(location);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Locations), new { id = location.DoctorId });
    }

    // GET: Login
    public IActionResult Login()
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");
      if (DoctorLoggedin(doctor_id))
      {
        return RedirectToAction(nameof(Index), new { DoctorId = doctor_id });
      }
      return View();
    }

    // POST: Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string FirstName, string LastName, string Password)
    {
      var doctor = await _context.Doctors
        .FirstOrDefaultAsync(d => d.FirstName == FirstName && d.LastName == LastName && d.Password == Password);

      if (doctor != null)
      {
        HttpContext.Session.SetInt32("DoctorId", doctor.DoctorId);
        HttpContext.Session.SetString("DoctorRights", doctor.Rights);
        return RedirectToAction(nameof(Index), new { doctorId = doctor.DoctorId });
      }

      ModelState.AddModelError(string.Empty, "Invalid login attempt.");
      return View();
    }

    // GET: Patients/Edit/5
    public async Task<IActionResult> EditPatient(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");

      if (id == null)
      {
        return NotFound();
      }

      else if (DoctorLoggedin(doctor_id))
      {
        var patient = await _context.Patients.FindAsync(id);
        if (patient == null)
        {
          return NotFound();
        }
        return View(patient);
      }

      return RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPatient(int id, [Bind("PatientId,FirstName,LastName,DateOfBirth,Gender,Address,PhoneNumber")] Patient patient)
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
      // if (string.IsNullOrWhiteSpace(patient.Password))
      // {
      //   ModelState.AddModelError("Password", "Password is required.");
      //   isValid = false;
      // }
      // else if (existingPatient.Password != patient.Password)
      // {
      //   ModelState.AddModelError("Password", "The password is incorrect.");
      //   isValid = false;
      // }

      if (isValid)
      {
        try
        {
          patient.Password = existingPatient.Password;
          Console.WriteLine("edit patient is done!!");
          _context.Update(patient);
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(PatientsList));
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
      Console.WriteLine("edit patient is failedd!!");
      return View(patient);
    }


    public async Task<IActionResult> DeletePatient(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");

      if (id == null)
      {
        return NotFound();
      }
      else if (DoctorLoggedin(doctor_id))
      {
        var patient = await _context.Patients
          .FirstOrDefaultAsync(m => m.PatientId == id);
        if (patient == null)
        {
          return NotFound();
        }

        return View(patient);
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/Delete/5
    [HttpPost, ActionName("DeletePatient")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePatientConfirmed(int id)
    {
      var patient = await _context.Patients.FindAsync(id);
      if (patient != null)
      {
        _context.Patients.Remove(patient);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(PatientsList));
    }

    // GET: Patients/Create
    public IActionResult CreatePatient()
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");
      if (DoctorLoggedin(doctor_id))
      {
        return View();
      }

      return RedirectToAction("Login");
    }

    // POST: Patients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePatient([Bind("FirstName,LastName,DateOfBirth,Gender,Address,PhoneNumber,Password,PasswordConfirmation")] Patient patient)
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
        return RedirectToAction(nameof(PatientsList));
      }
      return View(patient);
    }

    private bool DoctorLoggedin(int? id)
    {
      var doctor_id = HttpContext.Session.GetInt32("DoctorId");
      if (doctor_id != null && doctor_id == id)

      {
        return true;
      }
      return false;
    }

    private bool DoctorExists(int id)
    {
      return _context.Doctors.Any(e => e.DoctorId == id);
    }

    private bool LocationExists(int id)
    {
      return _context.Locations.Any(e => e.LocationId == id);
    }

    private bool AppointmentExists(int id)
    {
      return _context.Appointments.Any(e => e.AppointmentId == id);
    }

    private bool PatientExists(int id)
    {
      return _context.Patients.Any(e => e.PatientId == id);
    }
  }
}
