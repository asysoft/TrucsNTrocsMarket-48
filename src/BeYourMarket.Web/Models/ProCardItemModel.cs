using BeYourMarket.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class ProCardItemModel 
    {
        public PrepaidCard CurrentCard { get; set; }

        public UsersAddInfo UserAddInf { get; set; }
        public DateTime DateFirstUse { get; set; }
        public bool IsInUse { get; set; }

       // public List<PictureModel> Pictures { get; set; }

        // Errors History on cards : a voir ?
        public List<string> CardHistory { get; set; }

        public ProCardItemModel()
        {
            CurrentCard = new PrepaidCard();
            CardHistory = new List<string>();
            UserAddInf = new UsersAddInfo();
 
        }

    }
}