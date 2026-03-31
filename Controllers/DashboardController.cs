using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentHub.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentHub.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 🔐 LOGIN PROTECTION
            if (HttpContext.Session.GetString("User") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // ✅ Total counts
            ViewBag.TotalStudents = await _context.Students.CountAsync();
            ViewBag.TotalDepartments = await _context.Departments.CountAsync();

            // ✅ Average Marks (%)
            if (await _context.Marks.AnyAsync())
            {
                var avgMarks = await _context.Marks
                    .AverageAsync(m => (double)m.MarksObtained / m.TotalMarks * 100);

                ViewBag.AvgMarks = Math.Round(avgMarks, 2);
            }
            else
            {
                ViewBag.AvgMarks = 0;
            }

            // ✅ Attendance Rate (%)
            if (await _context.Attendances.AnyAsync())
            {
                var presentCount = await _context.Attendances.CountAsync(a => a.Status == "Present");
                var totalCount = await _context.Attendances.CountAsync();

                var attendanceRate = (double)presentCount / totalCount * 100;
                ViewBag.AttendanceRate = Math.Round(attendanceRate, 0);
            }
            else
            {
                ViewBag.AttendanceRate = 0;
            }

            // 🔥 TOP STUDENTS (SAFE + REALISTIC)
            var topStudents = await _context.Students
                .Select(s => new
                {
                    s.Name,
                    Avg = s.Marks.Any()
                        ? Math.Round(
                            s.Marks.Average(m => (double)m.MarksObtained / m.TotalMarks * 100),
                            2)
                        : 0
                })
                .OrderByDescending(s => s.Avg)
                .Take(5)
                .ToListAsync();

            ViewBag.TopStudents = topStudents;

            // ✅ Chart Data
            ViewBag.DepartmentNames = await _context.Departments
                .Select(d => d.DepartmentName)
                .ToListAsync();

            ViewBag.DepartmentCounts = await _context.Departments
                .Select(d => d.Students.Count)
                .ToListAsync();

            ViewBag.AttendanceData = new[]
            {
                await _context.Attendances.CountAsync(a => a.Status == "Present"),
                await _context.Attendances.CountAsync(a => a.Status == "Absent")
            };

            return View();
        }
    }
}