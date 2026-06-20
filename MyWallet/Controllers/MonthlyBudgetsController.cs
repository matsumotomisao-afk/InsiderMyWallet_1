using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWallet.Data;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class MonthlyBudgetsController : Controller
    {
        private readonly MyWalletContext _context;

        public MonthlyBudgetsController(MyWalletContext context)
        {
            _context = context;
        }

        // GET: MonthlyBudgets
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonthlyBudgets.ToListAsync());
        }

        // GET: MonthlyBudgets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monthlyBudget = await _context.MonthlyBudgets
                .FirstOrDefaultAsync(m => m.MonthlyBudgetId == id);
            if (monthlyBudget == null) return NotFound();

            return View(monthlyBudget);
        }

        // GET: MonthlyBudgets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MonthlyBudgets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MonthlyBudgetId,YearNum,MonthNum,BudgetAmount")] MonthlyBudget monthlyBudget)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monthlyBudget);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monthlyBudget);
        }

        // GET: MonthlyBudgets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monthlyBudget = await _context.MonthlyBudgets.FindAsync(id);
            if (monthlyBudget == null) return NotFound();
            return View(monthlyBudget);
        }

        // POST: MonthlyBudgets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MonthlyBudgetId,YearNum,MonthNum,BudgetAmount")] MonthlyBudget monthlyBudget)
        {
            if (id != monthlyBudget.MonthlyBudgetId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monthlyBudget);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonthlyBudgetExists(monthlyBudget.MonthlyBudgetId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(monthlyBudget);
        }

        // GET: MonthlyBudgets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monthlyBudget = await _context.MonthlyBudgets
                .FirstOrDefaultAsync(m => m.MonthlyBudgetId == id);
            if (monthlyBudget == null) return NotFound();

            return View(monthlyBudget);
        }

        // POST: MonthlyBudgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monthlyBudget = await _context.MonthlyBudgets.FindAsync(id);
            if (monthlyBudget != null)
            {
                _context.MonthlyBudgets.Remove(monthlyBudget);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MonthlyBudgetExists(int id)
        {
            return _context.MonthlyBudgets.Any(e => e.MonthlyBudgetId == id);
        }
    }
}
