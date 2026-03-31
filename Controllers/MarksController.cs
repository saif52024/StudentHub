using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentHub.Data;
using StudentHub.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentHub.Controllers
{
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: Marks
        public async Task<IActionResult> Index()
        {
            var marks = await _context.Marks
                .Include(m => m.Student)
                .ToListAsync();

            return View(marks);
        }

        // ✅ GET: Marks/Create
        public IActionResult Create()
        {
            // Safely check if Students table is accessible
            var students = _context.Students?.ToList() ?? new List<Student>();
            ViewBag.Students = new SelectList(students, "StudentId", "Name");

            return View();
        }

        // ✅ POST: Marks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MarkId,SubjectName,MarksObtained,TotalMarks,StudentId")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mark);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mark added successfully!";
                return RedirectToAction(nameof(Index));
            }

            // In case validation fails, re-populate dropdown
            var students = _context.Students?.ToList() ?? new List<Student>();
            ViewBag.Students = new SelectList(students, "StudentId", "Name", mark.StudentId);
            return View(mark);
        }

        // ✅ GET: Marks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var mark = await _context.Marks.FindAsync(id);
            if (mark == null)
                return NotFound();

            var students = _context.Students?.ToList() ?? new List<Student>();
            ViewBag.Students = new SelectList(students, "StudentId", "Name", mark.StudentId);

            return View(mark);
        }

        // ✅ POST: Marks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MarkId,SubjectName,MarksObtained,TotalMarks,StudentId")] Mark mark)
        {
            if (id != mark.MarkId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Mark updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Marks.Any(e => e.MarkId == id))
                        return NotFound();
                    throw;
                }
            }

            var students = _context.Students?.ToList() ?? new List<Student>();
            ViewBag.Students = new SelectList(students, "StudentId", "Name", mark.StudentId);
            return View(mark);
        }

        // ✅ GET: Marks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var mark = await _context.Marks
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m => m.MarkId == id);

            if (mark == null)
                return NotFound();

            return View(mark);
        }

        // ✅ GET: Marks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var mark = await _context.Marks
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m => m.MarkId == id);

            if (mark == null)
                return NotFound();

            return View(mark);
        }

        // ✅ POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Marks.FindAsync(id);
            if (mark != null)
            {
                _context.Marks.Remove(mark);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mark deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
