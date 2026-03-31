using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHub.Data;
using StudentHub.Models;

namespace StudentHub.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Department)
                .ToListAsync();
            return View(students);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();   // ✅ persists data
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.ToList();
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewBag.Departments = _context.Departments.ToList();
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(student);
                await _context.SaveChangesAsync();  // ✅ update persisted
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = _context.Departments.ToList();
            return View(student);
        }
        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null)
                return NotFound();

            return View(student);
        }


        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Department)
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.Department)
                .Include(s => s.Marks)
                .Include(s => s.Attendances)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
                return NotFound();

            // 🧹 Remove related records manually (safe fallback)
            _context.Marks.RemoveRange(student.Marks ?? new List<Mark>());
            _context.Attendances.RemoveRange(student.Attendances ?? new List<Attendance>());
            _context.Students.Remove(student);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Student and related data deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

    }
}
