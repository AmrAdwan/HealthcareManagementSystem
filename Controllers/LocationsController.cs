using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthcareManagementSystem.Data;
using HealthcareManagementSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HealthcareManagementSystem.Controllers
{
  public class LocationsController : Controller
  {
    private readonly HealthcareContext _context;

    public LocationsController(HealthcareContext context)
    {
      _context = context;
    }

    // GET: Locations
    public async Task<IActionResult> Index()
    {
      var locations = _context.Locations.Include(l => l.Doctor);
      return View(await locations.ToListAsync());
    }

    // GET: Locations/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var location = await _context.Locations
          .Include(l => l.Doctor)
          .FirstOrDefaultAsync(m => m.LocationId == id);
      if (location == null)
      {
        return NotFound();
      }
      return View(location);
    }

    // POST: Locations/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("LocationId,Name,DoctorId")] Location location)
    {
      if (id != location.LocationId)
      {
        return NotFound();
      }

      if (ModelState.IsValid)
      {
        try
        {
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
        return RedirectToAction(nameof(Index));
      }
      return View(location);
    }

    // GET: Locations/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var location = await _context.Locations
          .Include(l => l.Doctor)
          .FirstOrDefaultAsync(m => m.LocationId == id);
      if (location == null)
      {
        return NotFound();
      }

      return View(location);
    }

    // POST: Locations/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var location = await _context.Locations.FindAsync(id);
      _context.Locations.Remove(location);
      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool LocationExists(int id)
    {
      return _context.Locations.Any(e => e.LocationId == id);
    }
  }
}
