using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{

    public class PubsPlaceModel
    {
        /*
         *       ,[IdHtml]
      ,[PubPageName]
      ,[PubIsFree]
      ,[PubFileName]
      ,[PubFileNumber]
      ,[AspNetUser_Id]
         * */

      public string IdHtml { get; set; }
        public string PubPageName { get; set; }
        public string PubIsFree { get; set; }
        public string PubFileName { get; set; }
        public string PubFileNumber { get; set; }
        // ,[AspNetUser_Id]
    }
}