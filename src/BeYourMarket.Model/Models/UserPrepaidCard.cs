using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{
    public class UserPrepaidCard : Repository.Pattern.Ef6.Entity
    {
        // pour creer Many to Many avec des champos customizable en plus dans la table de liaison 
        // On crée 2 One to Many et l entity table de liaison avec les champs
        // https://stackoverflow.com/questions/7050404/create-code-first-many-to-many-with-additional-fields-in-association-table

        [Key]
        [Column(Order = 0)]
        [ForeignKey("AspNetUser")]
        public string UserID { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("PrepaidCard")]
        public string Code { get; set; }

        // indique si la carte est en cours d utilisation ou non
        public bool IsActif { get; set; }

        public DateTime DateFirstUse { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
        public virtual PrepaidCard PrepaidCard { get; set; }

    }
}
