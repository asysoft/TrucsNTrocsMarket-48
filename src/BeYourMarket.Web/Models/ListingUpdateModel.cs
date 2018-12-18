using BeYourMarket.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Web.Models
{
    public class ListingUpdateModel
    {
        public ListingUpdateModel()
        {            
            Categories = new List<Category>();
            Users = new List<ApplicationUser>();
            Pictures = new List<PictureModel>();
            CustomFields = new CustomFieldListingModel();

            // ASY : zones et villes dans combo
            LocationsRef = new List<LocationRef>();
        }

        public int CategoryID { get; set; }

        // ASY : zones et ville dans combo
        public int LocationRefID { get; set; }

        public int ListingTypeID { get; set; }

        public string UserID { get; set; }

        public Listing ListingItem { get; set; }

        public List<Category> Categories { get; set; }

        public List<LocationRef> LocationsRef { get; set; }

        public List<ListingType> ListingTypes { get; set; }

        public List<ApplicationUser> Users { get; set; }

        public List<PictureModel> Pictures { get; set; }

        public CustomFieldListingModel CustomFields { get; set; }

        // pour les Pro Uniquement, logo en indice 0 et autres ( pubs ? best of products ?...)
        //public List<PictureModel> ImgLogoFiles { get; set; }

    }
}
