using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IS7012.AST.ParentsBank.Models
{
    public class WishList
    {
        public int Id { get; set; }
        public virtual int AccountId { get; set; }
        public virtual Account Account { get; set; }
        public DateTime DateAdded { get; set; }
        [Required]
        public int Cost { get; set; }
        [Required]
        public string Description { get; set; }
        [Url]
        public string Link { get; set; }
        public bool Purchased { get; set; } //Purchased: boolean indicator that the item has been purchased or not
        public String getAddedDate()
        {
            return String.Format("{0:MM/dd/yyyy}", DateAdded);
        }

    }
}