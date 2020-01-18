using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{
    public class PubLocations : Repository.Pattern.Ef6.Entity
    {
        public PubLocations()
        {

        }
        //  ID, IdHtml, PubPageName, PubIsFree, PubFileName, PubFileNumber, AspNetUser
        public int ID { get; set; }
        public string IdHtml { get; set; }
        public string PubPageName { get; set; }  // Nom de la vue

        public bool PubIsFree { get; set; }
        public string PubFileName { get; set; }  // Nom du ou des fichiers  Nom+PubFileNumber ( ex test1.jpg )
        public int PubFileNumber { get; set; }  // Nombre de fichiers a boucler

        public virtual AspNetUser AspNetUser { get; set; }

    }
}
