using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS7012.AST.ParentsBank.Models
{
    //[CustomValidation(typeof(Account), "ValidateEmail")]
    public class Account
    {
        public int Id { get; set; }
        [EmailAddress]
        public string OwnerEmail { get; set; }
        //[CustomValidation(typeof(Account), "ValidateEmail")]
        [EmailAddress]
        //[System.ComponentModel.DataAnnotations.Compare(nameof(OwnerEmail), ErrorMessage = "Passwords don't match.")]        
        public string RecipientEmail { get; set; }
        [Required]
        public string RecipientName { get; set; }
        [ReadOnly(true)]
        public DateTime OpenDate { get; set; }
        [Range(0, 100)]
        //public decimal InterestRate { get { return InterestRate; } set { this.InterestRate = ((decimal)value / 100); } }
        public decimal InterestRate { get; set; }
        //[Required]
        //public string AccountName { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishList> WishListItems { get; set; }

        //public static ValidationResult ValidateEmail(Account account, ValidationContext context)
        //{
        //    if (account == null)
        //    {
        //        return ValidationResult.Success;
        //    }
        //    else if (account.OwnerEmail.ToLower() == account.RecipientEmail.ToLower())
        //    {
        //        return new ValidationResult("Owner and recipient cannot have same email address");
        //    }
        //    else
        //        return ValidationResult.Success;
        //}


        public decimal CurrentBalance() {

            var bal = Transactions.Sum(trxn => trxn.Amount);


            return bal;
        }

        public String getOpenDate()
        {
            return String.Format("{0:MM/dd/yyyy}", OpenDate);
        }

        public decimal YearToDateInterestEarned()
        {
            // define the starting balance
            // $0 will be fine for your project
            decimal startingBalance = 0;
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1); //calculates the start of the year -- 01/01/YYYY
            DateTime endDate = DateTime.Now; //current day!
            List<Calculators.InterestTransaction> txns = new List<Calculators.InterestTransaction>();

            //add the transactions to a seperate list...
            foreach (var item in Transactions)
            {
                txns.Add(new Calculators.InterestTransaction(item.TransactionDate, item.Amount));
            }

            // calculate interest
            Calculators.InterestCalculator calc = new Calculators.InterestCalculator();
            //decimal percentInterestRate = this.InterestRate/100;
            decimal interestPlusBalance = calc.calculateYTD_Interest(startDate,
                                                                     endDate,
                                                                     this.InterestRate,
                                                                     startingBalance,
                                                                     txns);

            // return the calculated amount rounded to 2 decimal places
            return Math.Round(interestPlusBalance, 2);
        }
        

    }
    }



