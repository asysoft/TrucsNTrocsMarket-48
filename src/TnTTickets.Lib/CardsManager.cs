using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TnTPrepaidCard.Lib.Model;

namespace TnTPrepaidCard.Lib
{
    public class CardsManager
    {
        const int C_MIN_CODE = 100000001;
        const int C_MAX_CODE = 999999999;

        IPrepaidCardService _prepaidCardService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        //

        /// <summary>
        /// 
        /// </summary>
        public CardsManager()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWorkAsync"></param>
        /// <param name="prepaidCardService"></param>
        public CardsManager(
            IUnitOfWorkAsync unitOfWorkAsync,
           IPrepaidCardService prepaidCardService)
        {
            _prepaidCardService = prepaidCardService;
            _unitOfWorkAsync = unitOfWorkAsync;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardsParams"></param>
        /// <returns></returns>
        public void GenerateCards(GenerateCardParamsModel cardsParams)
        {
            List<PrepaidCard> resCards = new List<PrepaidCard>();

            int numLot = cardsParams.NumLot;
            int numSerie = cardsParams.LastNumSerie ;

            bool generated = false;
            int code = 0;
            int nbGenerated = 0;

            try
            {

                while (nbGenerated < cardsParams.NbCards)
                {
                    Random random = new Random();
                    code = random.Next(C_MIN_CODE, C_MAX_CODE);

                    generated = resCards.Select(x => int.Parse(x.Code)).Contains(code);
                    if (  ( ! IsCodeExist(code) )  && ( !generated) )
                    { 
                        resCards.Add(new PrepaidCard
                            {   
                            Code = code.ToString() ,
                            DateFinValidite = cardsParams.DateFinValidite,
                            DateGeneration = DateTime.Now,
                            IsActif = cardsParams.IsActif,
                            IsValid = true,
                            NumLot = cardsParams.NumLot,
                            NumSerie = numSerie
                        });

                        nbGenerated++;
                        numSerie = cardsParams.LastNumSerie + nbGenerated;
                    }

                }

                // Si on a nos nb code on ecrit en base 
                if (resCards.Count() == cardsParams.NbCards )
                {
                    foreach(PrepaidCard newcard in resCards)
                    {
                        newcard.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                        _prepaidCardService.Insert(newcard); 
                        
                        // sauvegarde
                        _unitOfWorkAsync.SaveChanges();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.Write("Erreur GenerateCards : " + ex.Message);
                Console.Write("Erreur - n Generer \\ nb  Souhaite : " + nbGenerated + " \\ " + cardsParams.NbCards );
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCodeExist(int code)
        {
            bool res = true;
            var item = _prepaidCardService.Query(x => x.Code == code.ToString() ).Select();
            if (item.Count() == 0)
                res = false;

            return res;
        }

    }
}
