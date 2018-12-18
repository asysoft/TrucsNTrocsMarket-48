using BeYourMarket.Model.Models;
using System;
using System.Collections.Generic;


namespace TnTPrepaidCard.Lib.Model
{
    public class PrepaidCardManageModel
    {
        public List<PrepaidCard> ActiveCards { get; set; }
        public GenerateCardParamsModel GenerateParams { get; set; }

        public PrepaidCardManageModel()
        {
            //
            ActiveCards = new List<PrepaidCard>();
            GenerateParams = new GenerateCardParamsModel();

        }
    }

    public class GenerateCardParamsModel
    {
        public int NbCards { get; set; }
        public int NumLot { get; set; }
        public int LastNumSerie { get; set; }

        public DateTime DateGeneration { get; set; }
        public DateTime DateFinValidite { get; set; }
        public bool IsActif { get; set; }

        public GenerateCardParamsModel()
        { }

    }

}

/*
        public PrepaidCard()
        { }

        public int NumSerie { get; set; }
        public string Code { get; set; }
        public int NumLot { get; set; }

        public System.DateTime DateGeneration { get; set; }
        public System.DateTime DateFinValidite { get; set; }

        public bool IsValid { get; set; }
        public bool IsActif { get; set; }
 */
