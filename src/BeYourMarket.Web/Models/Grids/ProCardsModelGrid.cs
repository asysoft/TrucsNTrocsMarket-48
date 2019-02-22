using GridMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models.Grids
{

    public class ProCardsModelGrid : Grid<ProCardItemModel>
    {
        public ProCardsModelGrid(IQueryable<ProCardItemModel> items)
            : base(items)
        {
        }
    }
}