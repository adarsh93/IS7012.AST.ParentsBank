using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS7012.AST.ParentsBank.Models
{
    [CustomValidation(typeof(Transaction), "ValidateTransDate")]
    [CustomValidation(typeof(Transaction), "ValidateAmount")]
    public class Transaction
    {
        public int Id { get; set; }
        public virtual int AccountId { get; set; }  //AccountId: The Id of the account (managed automatically by the framework)
        public virtual Account TransAccount { get; set; }
        public DateTime TransactionDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string Note { get; set; }
        public IEnumerable<SelectListItem> dropdownValue { get; set; }

        public static ValidationResult ValidateAmount(Transaction transaction, ValidationContext context)
        {
            if (transaction.Amount==0)
                return new ValidationResult("A transaction cannot be for a $0.00 amount") ;
            else
                return ValidationResult.Success;
        }

        public String getTranDate()
        {
            return String.Format("{0:MM/dd/yyyy}", TransactionDate);
        }

        public static ValidationResult ValidateTransDate(Transaction transaction, ValidationContext context)
        {
            if (transaction.TransactionDate > DateTime.Today)
                return new ValidationResult("The transaction date cannot be in the future");
            else if (transaction.TransactionDate < DateTime.Today)
                return new ValidationResult("The transaction date cannot be before the current year");
            else
                return ValidationResult.Success;
        }

    }


}