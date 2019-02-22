using BeYourMarket.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class ProManageCardsModel
    {
        // NeedNewCard : indique si le Pro a fini ses cartes valides et s il doit en entrer une nouvelle
        public bool NeedNewCard { get; set; }
        public PrepaidCard CurrentCard { get; set; }
        public List<PrepaidCard> UsedCards { get; set; }

        public UsersAddInfo UserAddInf { get; set; }
        public List<PictureModel> Prologos { get; set; }
        public SearchCardsModel CardsSearch { get; set; }

        public ProManageCardsModel()
        {
            CurrentCard = new PrepaidCard();
            UsedCards = new List<PrepaidCard>();
            UserAddInf = new UsersAddInfo();
            Prologos = new List<PictureModel>();
        }

    }
}