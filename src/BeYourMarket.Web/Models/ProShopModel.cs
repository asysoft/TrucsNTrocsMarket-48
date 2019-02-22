using BeYourMarket.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class ProShopModel
    {
        public ApplicationUser UserPro { get; set; }
        public UsersAddInfo UserAddInf { get; set; }

        public List<PictureModel> Pictures { get; set; }
        public int LogoImgID { get; set; }
        public string CategoriesText { get; set; }
        public string CategoryIDs { get; set; }

        public SearchListingModel ListingsSearch;
        public ListingUpdateModel currListing;

        public ProShopModel()
        {
            UserPro = new ApplicationUser();
            Pictures = new List<PictureModel>();
            UserAddInf = new UsersAddInfo();

            ListingsSearch = new SearchListingModel();
            currListing = new ListingUpdateModel();

        }
    }

    /// <summary>
    /// outes les infos du listing et du Pro associé
    /// </summary>
    public class ProListingViewModel
    {
        public SearchListingModel ListingsSearch;
        public ListingItemModel ListingItem;
        public ProShopModel ProInfos;
        public ProListingViewModel()
        {
            ListingsSearch = new SearchListingModel();
            ListingItem = new ListingItemModel();
            ProInfos = new ProShopModel();
        }
    }


}