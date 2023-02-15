using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Authen.Data;
using Authen.Models;
using Microsoft.AspNetCore.Authorization;

namespace Authen.Controllers
{

    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [Authorize(Policy = "CanDoEveryThing")]
        [Authorize(Policy = "CanAddAndView")]
        [Authorize(Policy = "ViewOnly")]
        public async Task<IActionResult> Index()
        {
              return View(await _context.Students.ToListAsync());
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [Authorize(Policy = "CanDoEveryThing")]
        [Authorize(Policy = "CanAddAndView")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDoEveryThing")]
        [Authorize(Policy = "CanAddAndView")]
        public async Task<IActionResult> Create([Bind("StudentId,Name,DateOfBirth,Email,Address")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        [Authorize(Policy = "CanDoEveryThing")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDoEveryThing")]
        public async Task<IActionResult> Edit(long id, [Bind("StudentId,Name,DateOfBirth,Email,Address")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
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
            return View(student);
        }

        [Authorize(Policy = "CanDoEveryThing")]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Students == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanDoEveryThing")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Students == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Students'  is null.");
            }
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(long id)
        {
          return _context.Students.Any(e => e.StudentId == id);
        }
    }
}
