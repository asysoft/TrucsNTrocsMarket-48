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
        const int C_CODE_LENGHT = 9;

        IPrepaidCardService _prepaidCardService;
        IUserPrepaidCardService _userPrepaidCardService;
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
            IPrepaidCardService prepaidCardService,
            IUserPrepaidCardService userPrepaidCardService
            )
        {
            _prepaidCardService = prepaidCardService;
            _userPrepaidCardService = userPrepaidCardService;

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
                            CardStatus = 0,   // status  ACtive a la creation
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
        
        /// <summary>
        /// On regarde dans la table de liaison user-cards
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCodeAlreadyUsed(int code)
        {
            bool res = true;
            var item = _userPrepaidCardService.Query(x => x.Code == code.ToString() ).Select();
            if (item.Count() == 0)
                res = false;

            return res;
        }

        /// <summary>
        /// Desactivation du code au niveau table des cartes et aussi table de liaison
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool IsCodeNotActivated(int code)
        {
            bool res = false;
            var item = _prepaidCardService.Query(x => x.Code == code.ToString() && x.IsActif == false).Select();
            var item2 = _userPrepaidCardService.Query(x => x.Code == code.ToString() && x.IsActif == false).Select();

            if ((item.Count() > 0) || (item2.Count() > 0) )
                res = true;

            return res;
        }

        /// <summary>
        /// Verifie que le code utilisé est valide 
        /// pour un nouveau Pro en train de se créeer
        /// </summary>
        /// <param name="CodePro"></param>
        /// <returns></returns>
        public string CheckCodeNewProValid(string CodePro)
        {
            int iCode = 0;
            string resMsgErr = string.Empty;

            CodePro = CodePro.Replace(" ", string.Empty);
            bool bValidInt = int.TryParse(CodePro, out iCode);

            if (CodePro.Length != C_CODE_LENGHT)
                resMsgErr = "[[[Code Secret Invalid : Bad Lenght]]]";
            else if ( !bValidInt)
                resMsgErr = "[[[Code Secret Invalid : Contains invalid caracters]]]";
            else if ( !IsCodeExist(iCode) )
                resMsgErr = "[[[Code Secret Invalid : Does not exist]]]";
            else if (IsCodeAlreadyUsed(iCode))
                resMsgErr = "[[[Code Secret Invalid : Already used]]]";
            else if (IsCodeNotActivated(iCode))
                resMsgErr = "[[[Code Secret Invalid : This code is not Activated]]]";

            return resMsgErr;
        }


    }
}
