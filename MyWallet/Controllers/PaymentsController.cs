using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyWallet.Data;
using MyWallet.Models;

namespace MyWallet.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly MyWalletContext _context;

        public PaymentsController(MyWalletContext context)
        {
            _context = context;
        }

        // GET: Payments
        public async Task<IActionResult> Index(int? num)
        {
            var today = DateTime.Today;                //今日の日付
            // -------------------------------------------------------For Generate LinkButton
                           //Paymentテーブルのposte列の月名をDistinctする（重複排除）

   var dstnctPost = _context.Payments
                    .Where(p => p.Posted.Year == today.Year) //今年のデータを抽出する
                    .Select(p => p.Posted.Month)             //月名だけ抽出(Monthのデータ）
                    .Distinct()               //重複を排除
                    .OrderBy(m => m)            //昇順ソート
                    .ToList();                //List化する


                if (dstnctPost != null)
                {

                    ViewBag.DstnctPost = dstnctPost;   //本チュートリアルでは、Listの中に６，７、が入っている。
                }
                //-------------------------------------------------------End For Generate LinkButton


            if (num == null)
            {


                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1); //今年の今月の1日を取得
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1); //今月の末日を取得

                ViewBag.CurrentMonth = today.Month; //現在の月をViewBagに格納

                var payments = _context.Payments
                    .Include(p => p.PaymentTypeNavigation)
                    .Include(p => p.SubjectNameNavigation)
                    .Where(p => p.Posted >= firstDayOfMonth && p.Posted <= lastDayOfMonth);//支払日が今月の1日から末日までのデータに絞り込む

                return View(await payments.ToListAsync());
            }
            else
            {
                var firstDayOfMonth = new DateTime(today.Year, num.Value, 1); //今年の指定された月の1日を取得
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1); //指定された月の末日を取得

                ViewBag.CurrentMonth = num.Value;  //指定された月をViewBagに格納

                var payments = _context.Payments
                .Include(p => p.PaymentTypeNavigation)
                .Include(p => p.SubjectNameNavigation)
                .Where(p => p.Posted >= firstDayOfMonth && p.Posted <= lastDayOfMonth);//支払日が指定された月の1日から末日までのデータに絞り込む
                return View(await payments.ToListAsync());
            }
        }

        // GET: Payments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.PaymentTypeNavigation)
                .Include(p => p.SubjectNameNavigation)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // GET: Payments/Create
        public IActionResult Create()
        {
            var today = DateTime.Today;                //今日の日付
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1); //今年の今月の1日を取得
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1); //今月の末日を取得

            var applicationDbContext = _context.Payments
                .Include(p => p.SubjectNameNavigation)
                .Include(p => p.PaymentTypeNavigation)
                .Where(p => p.Posted >= firstDayOfMonth && p.Posted <= lastDayOfMonth);  //支払日が今月の1日から末日までのデータに絞り込む

            ViewData["PaymentHistory"] = applicationDbContext.ToList();//ToList() を呼び出すとSQLが実行されます取得したデータをViewData[PaymentHistory]にobject化する

            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "TypeName");
            ViewData["SubjectNameId"] = new SelectList(_context.SubjectNames, "SubjectNameId", "CourseName");
            return View();
        }

        // POST: Payments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PaymentId,Posted,ItemName,PaymentName,PaymentTypeId,Amount,SubjectNameId")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "TypeName", payment.PaymentTypeId);
            ViewData["SubjectNameId"] = new SelectList(_context.SubjectNames, "SubjectNameId", "CourseName", payment.SubjectNameId);
            return View(payment);
        }

        // GET: Payments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return NotFound();
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "TypeName", payment.PaymentTypeId);
            ViewData["SubjectNameId"] = new SelectList(_context.SubjectNames, "SubjectNameId", "CourseName", payment.SubjectNameId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PaymentId,Posted,ItemName,PaymentName,PaymentTypeId,Amount,SubjectNameId")] Payment payment)
        {
            if (id != payment.PaymentId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.PaymentId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentTypes, "PaymentTypeId", "TypeName", payment.PaymentTypeId);
            ViewData["SubjectNameId"] = new SelectList(_context.SubjectNames, "SubjectNameId", "CourseName", payment.SubjectNameId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _context.Payments
                .Include(p => p.PaymentTypeNavigation)
                .Include(p => p.SubjectNameNavigation)
                .FirstOrDefaultAsync(m => m.PaymentId == id);
            if (payment == null) return NotFound();

            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.PaymentId == id);
        }
    }
}
