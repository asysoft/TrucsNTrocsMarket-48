
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeYourMarket.Model.Models
{

    public class PrepaidCard : Repository.Pattern.Ef6.Entity
    {
        public PrepaidCard()
        {
            UserPrepaidCards = new HashSet<UserPrepaidCard>();
            PrepaidCardsHisto = new HashSet<PrepaidCardHisto>();
        }
        public string Code { get; set; }
        public int NumSerie { get; set; }

        public int NumLot { get; set; }

        public System.DateTime DateGeneration { get; set; }
        public System.DateTime DateFinValidite { get; set; }

        // est utilisable ou non
        public bool IsActif { get; set; }

        //  enum EnumCardStatus : precise l etat de la carte
        public int CardStatus { get; set; }


        // pour creer Many to Many avec des champos customizable en plus dans la table de liaison 
        // On crée 2 One to Many et l entity table de liaison avec les champs
        // https://stackoverflow.com/questions/7050404/create-code-first-many-to-many-with-additional-fields-in-association-table
        public virtual ICollection<UserPrepaidCard> UserPrepaidCards { get; set; }

        public ICollection<PrepaidCardHisto> PrepaidCardsHisto { get; set; }

    }

}