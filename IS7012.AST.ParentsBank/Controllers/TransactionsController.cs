using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IS7012.AST.ParentsBank.Models;

namespace IS7012.AST.ParentsBank.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.TransAccount);
            String loggedInUser = User.Identity.Name;
            List<Transaction> ownList = new List<Transaction>();
            List<Transaction> childList = new List<Transaction>();
            int identityFlag=0;
            foreach (Transaction t in transactions)
            {
                if (t.TransAccount.OwnerEmail == loggedInUser)
                {
                    ownList.Add(t);
                    identityFlag = 1;
                }
                else if (t.TransAccount.RecipientEmail ==loggedInUser)
                {
                    childList.Add(t);
                    identityFlag = -1;
                }
            }
            if (identityFlag == 1)
            {
                return View(ownList);
            }
            else if(identityFlag==-1)
            {
                return View(childList);
            }

                //Account ownAcc = new Account();
                //foreach ( Account acc in db.Accounts)
                //{
                //    if(loggedInUser==acc.OwnerEmail)
                //    {
                //        ownAcc = acc;
                //        //return View(acc.Transactions);
                //    }
                //    else
                //        return View(transactions.ToList());
                //}

                //if(loggedInUser==transactions)
             return RedirectToAction("Create");
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Transaction transaction = db.Transactions.Find(id);
            if(transaction is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            else if ((transaction.TransAccount.RecipientEmail == User.Identity.Name)||(transaction.TransAccount.OwnerEmail == User.Identity.Name))
                return View(transaction);
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            //return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            //has to be changed, only the particular parent's children should be displayed in dropdown
            List<Account> childList = new List<Account>();
            String loggedInUser = User.Identity.Name;
            foreach (Account acc in db.Accounts)
            {
                if (acc.OwnerEmail == loggedInUser)
                    childList.Add(acc);

            }
           // ViewBag.AccountId = new SelectList(db.Accounts, "Id", "RecipientEmail");
          
                ViewBag.AccountId = new SelectList(childList, "Id", "RecipientEmail");
        
        
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //public IEnumerable<SelectListItem> getallValues()
        //{
        //    var list = new Account();
        //    IEnumerable<SelectListItem> ddlItems = from s in db.Accounts select new SelectListItem {    Text = s.RecipientEmail, Value = s.RecipientEmail.ToString() };
        //    return ddlItems;
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            Account acc = db.Accounts.Find(transaction.AccountId);
            //decimal sum = 0;
            //foreach (Transaction t in acc.Transactions)
            //{
            //    sum = sum + t.Amount;
            //}
            if (transaction.Amount < 0)
            {
                if (Math.Abs(transaction.Amount) > acc.CurrentBalance())
                {
                    ModelState.AddModelError("Amount", "A debit cannot be for more that the current account balance");
                }

            }

            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<Account> childList = new List<Account>();
            String loggedInUser = User.Identity.Name;
            foreach (Account acct in db.Accounts)
            {
                if (acct.OwnerEmail == loggedInUser)
                    childList.Add(acct);

            }
            // ViewBag.AccountId = new SelectList(db.Accounts, "Id", "RecipientEmail");

            ViewBag.AccountId = new SelectList(childList, "Id", "RecipientEmail");

            return View(transaction);


        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            List<Account> listOfAcc = new List<Account>();
            foreach(Account acc in db.Accounts)
            {
                if (acc.OwnerEmail == User.Identity.Name)
                    listOfAcc.Add(acc);
                
            }
            //ViewBag.AccountId = new SelectList(db.Accounts, "Id", "OwnerEmail", transaction.AccountId);
            ViewBag.AccountId = new SelectList(listOfAcc, "Id", "RecipientEmail", transaction.AccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "OwnerEmail", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
