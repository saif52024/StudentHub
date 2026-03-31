using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentHub.Data;
using StudentHub.Models;
using System.Threading.Tasks;
using System.Linq;

namespace StudentHub.Controllers
{
    public class AttendancesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            var attendances = await _context.Attendances
                .Include(a => a.Student)
                .ThenInclude(s => s.Department)
                .AsNoTracking()
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            return View(attendances);
        }

        // GET: Attendances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .ThenInclude(s => s.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
                return NotFound();

            return View(attendance);
        }

        // ✅ GET: Attendances/Create
        public IActionResult Create()
        {
            var students = _context.Students?.ToList() ?? new List<Student>();
            ViewBag.Students = new SelectList(students, "StudentId", "Name");
            return View();
        }

        // ✅ POST: Attendances/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AttendanceId,Date,Status,StudentId")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance record added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown on error
            var students = _context.Students?.ToList() ?? new List<Student>();
            ViewBag.Students = new SelectList(students, "StudentId", "Name", attendance.StudentId);
            return View(attendance);
        }


        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
                return NotFound();

            ViewBag.Students = new SelectList(_context.Students, "StudentId", "Name", attendance.StudentId);
            return View(attendance);
        }

        // POST: Attendances/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance attendance)
        {
            if (id != attendance.AttendanceId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Students = new SelectList(_context.Students, "StudentId", "Name", attendance.StudentId);
                return View(attendance);
            }

            try
            {
                var existingAttendance = await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(a => a.AttendanceId == id);
                if (existingAttendance == null)
                    return NotFound();

                // ✅ Attach related Student entity correctly
                var student = await _context.Students.FindAsync(attendance.StudentId);
                if (student != null)
                    _context.Entry(student).State = EntityState.Unchanged;

                _context.Update(attendance);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Attendance record updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(attendance.AttendanceId))
                    return NotFound();
                else
                    throw;
            }
        }

        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .ThenInclude(s => s.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AttendanceId == id);

            if (attendance == null)
                return NotFound();

            return View(attendance);
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Attendance record deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Attendance record not found.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.AttendanceId == id);
        }
    }
}
