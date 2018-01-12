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
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            var aName = User.Identity.Name;
            //String aName = a.Name;
            //int aId = new int();
            List<Account> OwneraccList = new List<Account>();
            List<Account> ChildList = new List<Account>();
            foreach (Account acc in db.Accounts)
            {
                if (acc.OwnerEmail == aName)
                {
                    OwneraccList.Add(acc);
                    // aId = acc.Id;
                    //break;
                    // return View(db.Accounts.ToList());
                }
                if (acc.RecipientEmail==aName)
                {
                    ChildList.Add(acc);
                }
            }
            //Account temp = db.Accounts.Find(aId);
            //Account reqAcc = temp;
            //List<Account> emptyAcc = new List<Account>();
            //if (OwneraccList.Count == 0)
            //    return View();
            if (OwneraccList.Count > 0)
            {
                return View(OwneraccList);
            }
            else if(ChildList.Count>0)
            {
                return RedirectToAction("Details", db.Accounts.Find(ChildList.First().Id));
            }
            else
                return RedirectToAction("Create");
            //if (reqAcc is null)
            //    return View(emptyAcc);
            //else if (reqAcc.RecipientEmail == aName)
            //    return RedirectToAction("Details", aId);
            
            //return View(db.Accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            //ViewBag.OpenDate = new DateTime().Date;
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,OwnerEmail,RecipientEmail,RecipientName,OpenDate,InterestRate")] Account account)
        {
            
            foreach (Account acc in db.Accounts)
            {
                if (acc.RecipientEmail == account.RecipientEmail)
                    ModelState.AddModelError("RecipientEmail","Recipient already has an account.");
                if(acc.OwnerEmail==account.RecipientEmail)
                    ModelState.AddModelError("RecipientEmail", "This account already exists as an Owner.");
            }

                if (ModelState.IsValid)
            {
                //foreach(Account acc in db.Accounts)
                //{
                //    if (acc.RecipientEmail == account.RecipientEmail)
                //        ModelState.AddModelError("RecipientEmail","Recipient already has an account.");
                //}
                //Console.Out.WriteLine("In Create in Home Controller");

                account.OwnerEmail = User.Identity.Name;
                account.OpenDate = DateTime.Now;
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Console.Out.WriteLine("In Create in Home Controller");
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerEmail,RecipientEmail,RecipientName,OpenDate,InterestRate")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.OwnerEmail = User.Identity.Name;
                account.OpenDate = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            if(account.CurrentBalance()>0||account.CurrentBalance()<0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //String ownerEmail = account.OwnerEmail;
            String childEmail = account.RecipientEmail;
            int accId = account.Id;
            foreach(WishList w in db.WishLists)
            {
                if (accId == w.AccountId)
                {
                    db.WishLists.Remove(w);
                   // db.SaveChanges();
                }
            }
            foreach(Transaction t in db.Transactions)
            {
                if(childEmail==t.TransAccount.RecipientEmail)
                {
                    db.Transactions.Remove(t);
                    //db.SaveChanges();
                }
            }
            db.Accounts.Remove(account);
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
