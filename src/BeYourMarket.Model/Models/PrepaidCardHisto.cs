using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{

    public class PrepaidCardHisto : Repository.Pattern.Ef6.Entity
    {
        public PrepaidCardHisto()
        { }

        public int ID { get; set; }

        public string Code { get; set; }
        //public int NumSerie { get; set; }

        public System.DateTime DateStatus { get; set; }
        public string UsrLogin { get; set; }
        public string IPAccess { get; set; }

        // enum EnumCardStatus de la carte au moment de l'action
        public int LastCardStatus { get; set; }

        public string Msg { get; set; }

        // pour creer Many to Many avec des champos customizable en plus dans la table de liaison 
        // On crée 2 One to Many et l entity table de liaison avec les champs
        // https://stackoverflow.com/questions/7050404/create-code-first-many-to-many-with-additional-fields-in-association-table
        public virtual PrepaidCard PrepaidCardInfo { get; set; }

    }
}
