using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class AnnonceurPubModel
    {
        //
        public RegisterViewModel RegisterInfos { get; set; }
        public List<PubsPlaceModel> PubsLocations { get; set; }

        public AnnonceurPubModel()
        {
            RegisterInfos = new RegisterViewModel();
        }
    }
}