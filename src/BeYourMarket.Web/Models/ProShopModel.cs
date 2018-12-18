using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class ProShopModel
    {
        public ApplicationUser UserPro { get; set; }

        public List<PictureModel> Pictures { get; set; }
        public int LogoImgID { get; set; }

        public SearchListingModel ListingsSearch;

        public ProShopModel()
        {
            UserPro = new ApplicationUser();
            Pictures = new List<PictureModel>();
            ListingsSearch = new SearchListingModel();

        }
    }
}