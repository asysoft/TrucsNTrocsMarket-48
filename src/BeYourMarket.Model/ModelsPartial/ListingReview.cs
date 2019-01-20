using BeYourMarket.Model.Enum;
using BeYourMarket.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{
    [MetadataType(typeof(ListingReviewMetaData))]
    public partial class ListingReview
    {
        public string RatingClass
        {
            get
            {
                // ASY : pas de  demi en demi
                return "s" + Math.Round(Rating);
            }
        }
    }

    public class ListingReviewMetaData
    {        
    }
}
