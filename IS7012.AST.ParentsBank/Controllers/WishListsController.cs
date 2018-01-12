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
    public class WishListsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WishLists
        public ActionResult Index()
        {
            List<WishList> listForOwner = new List<WishList>();
            List<WishList> listForChild = new List<WishList>();
            int aFlag = 0;
            foreach (Account a in db.Accounts)
            {
                if (a.OwnerEmail == User.Identity.Name)
                {
                    aFlag = 1;
                    foreach (WishList w in db.WishLists)
                    {
                        if (w.AccountId == a.Id)
                            listForOwner.Add(w);

                    }
                }
                else if (a.RecipientEmail == User.Identity.Name)
                {
                    aFlag = -1;
                    foreach (WishList w in db.WishLists)
                    {
                        if (w.AccountId == a.Id)
                            listForChild.Add(w);
                    }
                }

            }
            if (aFlag == 1)
                return View(listForOwner);
            else if (aFlag == -1)
                return View(listForChild);
            else
                return RedirectToAction("Create");
        }

        // GET: WishLists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList wishList = db.WishLists.Find(id);
            if (wishList is null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            else if ((wishList.Account.RecipientEmail == User.Identity.Name) || (wishList.Account.OwnerEmail == User.Identity.Name))
                return View(wishList);
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            if (wishList == null)
            {
                return HttpNotFound();
            }
            //return View(wishList);
        }

        // GET: WishLists/Create
        public ActionResult Create()
        {
            List<Account> childList = new List<Account>();
            //List<Transaction> childTranList = new List<Transaction>();
            //List<Transaction> childTranList2 = new List<Transaction>();
            String loggedInUser = User.Identity.Name;
            //int a = 0;
            foreach (Account acc in db.Accounts)
            {
                if (acc.OwnerEmail == loggedInUser)
                {
                    childList.Add(acc);
                    ViewBag.AccountId = new SelectList(childList, "Id", "RecipientEmail");
                    //ViewBag.Account = new SelectList(childList, "Id", "RecipientEmail");
                }
                else if (acc.RecipientEmail == loggedInUser)
                {
                    childList.Add(acc);
                    ViewBag.AccountId = new SelectList(childList, "Id", "RecipientEmail");
                    //ViewBag.Account = new SelectList(childList, "Id", "RecipientEmail");
                }
                //childList.Add(acc);

            }
            //ViewBag.AccountId = new SelectList(childList, "Id", "RecipientEmail");
            //ViewBag.Account = new SelectList(childList, "Id", "RecipientEmail");
            return View();
        }

        // POST: WishLists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishList wishList)
        {
            if (ModelState.IsValid)
            {
                db.WishLists.Add(wishList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            // GET ONLY TEAMS BELONGING TO THE CURRENT USER FOR THE DROPDOWN
            //string currentlyLoggedInUsername = User.Identity.Name;
            //var acc = db.Accounts
            //    .Where(x => x.OwnerEmail == currentlyLoggedInUsername
            //    || x.RecipientEmail == currentlyLoggedInUsername).ToList();

            //ViewBag.AccountId = new SelectList(acc, "Id", "Name", wishList.AccountId);

            return View(wishList);
        }

        // GET: WishLists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList wishList = db.WishLists.Find(id);
            List<WishList> listWish = new List<WishList>();
            listWish.Add(wishList);
            if (wishList == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(listWish, "AccountId", "AccountId", wishList.AccountId);
            return View(wishList);
        }

        // POST: WishLists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishList wishList)
        {
            if (ModelState.IsValid)
            {

                db.Entry(wishList).State = EntityState.Modified;
                //wishList.AccountId = db.WishLists.Find(wishList.Id).Account.Id;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.WishLists, "AccountId", "AccountId", wishList.AccountId);
            return View(wishList);
        }

        // GET: WishLists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList wishList = db.WishLists.Find(id);
            if (wishList == null)
            {
                return HttpNotFound();
            }
            return View(wishList);
        }

        // POST: WishLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WishList wishList = db.WishLists.Find(id);
            db.WishLists.Remove(wishList);
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

        public ActionResult Search(String description, String price)
        {
            bool searchPerformed = false;
            //List<WishList> wishListItems = new List<WishList>();
            var wishListItems = db.WishLists
              .Include(w => w.Account)
              .Where(x => x.Account.OwnerEmail == User.Identity.Name || x.Account.RecipientEmail == User.Identity.Name);

            if (String.IsNullOrWhiteSpace(description) && String.IsNullOrWhiteSpace(price))
            {
                return View(wishListItems.ToList());
            }
            else
            {

                if (!String.IsNullOrWhiteSpace(description))
                {
                    wishListItems = wishListItems.Where(x => x.Description.Contains(description));
                    searchPerformed = true;
                }

                if (!String.IsNullOrWhiteSpace(price))
                {
                    decimal tprice = Decimal.Parse(price);
                    //String.F
                    // wishListItems = wishListItems.Where(s => s.Cost==price);
                    wishListItems = wishListItems.Where(x => x.Cost == tprice);
                    searchPerformed = true;

                }


                if (searchPerformed)
                {
                    // return search results
                    return View(wishListItems.ToList());
                }
                else
                {
                    // return empty list
                    return View(new List<WishList>());
                }
                //return View(wishListItems.ToList());
            }


            //return View();
        }
    }
}
