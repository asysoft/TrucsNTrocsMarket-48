using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{
    public partial class UserCategory : Repository.Pattern.Ef6.Entity
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("AspNetUser")]
        public string AspNetUserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Category")]
        public int CategoryID { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Category Category { get; set; }
    }

}
