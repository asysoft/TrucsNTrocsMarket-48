
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
        { }
        public string Code { get; set; }
        public int NumSerie { get; set; }

        public int NumLot { get; set; }

        public System.DateTime DateGeneration { get; set; }
        public System.DateTime DateFinValidite { get; set; }

        public bool IsValid { get; set; }
        public bool IsActif { get; set; }
    }

}