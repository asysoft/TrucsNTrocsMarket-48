using BeYourMarket.Model.Models;
using BeYourMarket.Web.Models.Grids;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class SearchListingModel : SortViewModel
    {
        public int CategoryID { get; set; }
        public List<Category> Categories { get; set; }
        public string CategoryIDsSearch { get; set; }

        public List<int> ListingTypeID { get; set; }

        public string SearchText { get; set; }

        public int LocationRefID { get; set; }
        public List<LocationRef> LocationsRef { get; set; }
        public string Location { get; set; }
        public string LocationRefIDsSearch { get; set; }
        
        public bool PhotoOnly { get; set; }

        public double? PriceFrom { get; set; }

        public double? PriceTo { get; set; }

        public List<MetaCategory> MetaCategories { get; set; }

        public List<ListingItemModel> Listings { get; set; }

        public IPagedList<ListingItemModel> ListingsPageList { get; set; }        

        public List<Category> BreadCrumb { get; set; }

        public List<ListingType> ListingTypes { get; set; }

        public ListingModelGrid Grid { get; set; }
    }
}