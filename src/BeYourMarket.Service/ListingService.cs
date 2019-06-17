using BeYourMarket.Model.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Service
{
    public interface IListingService : IService<Listing>
    {
        Dictionary<DateTime, int> GetItemsCount(DateTime datetime);

        Dictionary<Category, int> GetCategoryCount();

        string GetNewListingRef();

    }

    public class ListingService : Service<Listing>, IListingService
    {
        public ListingService(IRepositoryAsync<Listing> repository)
            : base(repository)
        {
        }

        public Dictionary<DateTime, int> GetItemsCount(DateTime fromDate)
        {
            var itemsCountDictionary = new Dictionary<DateTime, int>();
            for (DateTime i = fromDate; i <= DateTime.Now.Date; i = i.AddDays(1))
            {
                itemsCountDictionary.Add(i, 0);
            }
            
            var itemsCountQuery = Queryable().Where(x => x.Created >= fromDate).GroupBy(x => System.Data.Entity.DbFunctions.TruncateTime(x.Created)).Select(x => new { i = x.Key.Value, j = x.Count() }).ToDictionary(x => x.i, x => x.j);
            foreach (var item in itemsCountQuery)
            {
                itemsCountDictionary[item.Key] = item.Value;
            }

            return itemsCountDictionary;
        }


        public Dictionary<Category, int> GetCategoryCount()
        {
            return Queryable().GroupBy(x => x.Category).Select(x => new { i = x.Key, j = x.Count() }).ToDictionary(x => x.i, x => x.j);
        }

        public string GetNewListingRef()
        {
            string newListingRef = string.Empty;
            string currYear = DateTime.Now.Year.ToString();
            string currMonth = DateTime.Now.Month.ToString("d2");

            var LastitemsQuery = Queryable().Where(x => !string.IsNullOrEmpty(x.ListingRef) ).OrderByDescending(x => x.ID).First();
            /* 		[ListingRef] =  convert(varchar(4), year(getdate())) 
						+ convert(char(2),getdate(),1)
						+ FORMAT(ID, '100000')  ===> ex :  201905100010  */

            string LastListingRef = LastitemsQuery.ListingRef;
            string lastYearListing = LastListingRef.Substring(0, 4);
            string lastMonthListing = LastListingRef.Substring(4, 2);
            string lastNumberListing = LastListingRef.Substring(6);



            if (lastYearListing != currYear || lastMonthListing != currMonth)
                newListingRef = currYear + currMonth + "100001";
            else
                newListingRef = currYear + currMonth + (int.Parse(lastNumberListing) + 1).ToString() ;

            return newListingRef;

        }

    }
}
