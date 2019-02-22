using BeYourMarket.Web.Models.Grids;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class SearchCardsModel : SortViewModel
    {

        // Champs pour entrer des criteres de recherches
        public SearchCriteria SCrit { get; set; }

        // Champs contenants les resultats qui matchent
        public List<ProCardItemModel> Cards { get; set; }
        public IPagedList<ProCardItemModel> CardsPageList { get; set; }
        public ProCardsModelGrid Grid { get; set; }

        public SearchCardsModel()
        {
            SCrit = new SearchCriteria();
            Cards = new List<ProCardItemModel>();
        }

    }

    /// <summary>
    /// Pour preciser les criteres de recherches
    /// </summary>
    public class SearchCriteria
    {
        public bool ChkProOwnerName { get; set; }
        public string ProOwnerName { get; set; }

        public bool ChkProCompany { get; set; }
        public string ProCompany { get; set; }

        public bool ChkCode { get; set; }
        public string Code { get; set; }

        public bool ChkNumSerie { get; set; }
        public int NumSerie { get; set; }

        public bool ChkNumLot { get; set; }
        public int NumLot { get; set; }

        public bool ChkDateFinValidite { get; set; }
        public DateTime DateFinValidite { get; set; }

        public bool ChkDateFirstUse { get; set; }
        public DateTime DateFirstUse { get; set; }

        public bool ChkIsValidCard { get; set; }
        public bool IsValidCard { get; set; }

        public bool ChkIsActifCard { get; set; }
        public bool IsActifCard { get; set; }

        public bool ChkIsInUse { get; set; }
        public bool IsInUse { get; set; }

        public SearchCriteria()
        {
            ChkProOwnerName = false;
            ProOwnerName = "";

            ChkProCompany = false;
            ProCompany = "";

            ChkCode = false;
            Code = "";

            ChkNumSerie = false;
            NumSerie = 0;

            ChkNumLot = false;
            NumLot = 0;

            ChkDateFirstUse = false;
            DateFirstUse = DateTime.Now;

            ChkDateFinValidite = false;
            DateFinValidite = new System.DateTime(DateTime.Now.Year + 1, 12, 31);

            ChkIsInUse = true;
            IsInUse = true;

            IsActifCard = true;
            ChkIsActifCard = true;

            ChkIsValidCard = true;
            IsValidCard = true;
        }

    }


}