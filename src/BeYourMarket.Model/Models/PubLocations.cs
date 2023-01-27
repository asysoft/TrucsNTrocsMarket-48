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

        public int ID { get; set; }
        public string PageName { get; set; }  // Nom de la vue

        public string TopFileName { get; set; }  // Nom du ou des fichiers  Nom+PubFileNumber ( ex test1.jpg )
        public bool TopIsFree { get; set; }
        public int TopFileNum { get; set; }  // Nombre de fichiers a boucler
        public virtual AspNetUser TopAspNetUser { get; set; }

        public string LeftFileName { get; set; }  // Nom du ou des fichiers  Nom+PubFileNumber ( ex test1.jpg )
        public bool LeftIsFree { get; set; }
        public int LeftFileNum { get; set; }  // Nombre de fichiers a boucler
        public virtual AspNetUser LeftAspNetUser { get; set; }

        public string RightFileName { get; set; }  // Nom du ou des fichiers  Nom+PubFileNumber ( ex test1.jpg )                
        public bool RightIsFree { get; set; }
        public int RightFileNum { get; set; }  // Nombre de fichiers a boucler        
        public virtual AspNetUser RightAspNetUser { get; set; }

    }
}
