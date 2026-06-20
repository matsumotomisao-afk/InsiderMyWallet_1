using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWallet.Data;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class SubjectNamesController : Controller
    {
        private readonly MyWalletContext _context;

        public SubjectNamesController(MyWalletContext context)
        {
            _context = context;
        }

        // GET: SubjectNames
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubjectNames.ToListAsync());
        }

        // GET: SubjectNames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var subjectName = await _context.SubjectNames
                .FirstOrDefaultAsync(m => m.SubjectNameId == id);
            if (subjectName == null) return NotFound();

            return View(subjectName);
        }

        // GET: SubjectNames/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubjectNames/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubjectNameId,CourseName,ImageUrl")] SubjectName subjectName)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subjectName);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subjectName);
        }

        // GET: SubjectNames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subjectName = await _context.SubjectNames.FindAsync(id);
            if (subjectName == null) return NotFound();
            return View(subjectName);
        }

        // POST: SubjectNames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubjectNameId,CourseName,ImageUrl")] SubjectName subjectName)
        {
            if (id != subjectName.SubjectNameId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subjectName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectNameExists(subjectName.SubjectNameId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subjectName);
        }

        // GET: SubjectNames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subjectName = await _context.SubjectNames
                .FirstOrDefaultAsync(m => m.SubjectNameId == id);
            if (subjectName == null) return NotFound();

            return View(subjectName);
        }

        // POST: SubjectNames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subjectName = await _context.SubjectNames.FindAsync(id);
            if (subjectName != null)
            {
                _context.SubjectNames.Remove(subjectName);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectNameExists(int id)
        {
            return _context.SubjectNames.Any(e => e.SubjectNameId == id);
        }
    }
}
