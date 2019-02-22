using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using BeYourMarket.Web.Areas.Admin.Models;
using BeYourMarket.Web.Models;
using BeYourMarket.Web.Models.Grids;
using PagedList;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TnTPrepaidCard.Lib;
using TnTPrepaidCard.Lib.Model;

namespace BeYourMarket.Web.Areas.Admin.Controllers
{
    public class PrepaidCardController : Controller
    {
        // 
        #region Fields

        IPrepaidCardService _prepaidCardService;
        IUserPrepaidCardService _userPrepaidCardService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        #endregion

        #region Contructors

        public PrepaidCardController(
           IUnitOfWorkAsync unitOfWorkAsync,
           IPrepaidCardService prepaidCardService,
           IUserPrepaidCardService userPrepaidCardService
            )
        {
            _prepaidCardService = prepaidCardService;
            _userPrepaidCardService = userPrepaidCardService;

            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        // GET: Admin/PrepaidCard
        public async Task<ActionResult> GenerateNewCards()
        {
            GenerateCardParamsModel model = new GenerateCardParamsModel();

            DateTime df = new System.DateTime(DateTime.Now.Year +1, 12, 31);
            model.DateFinValidite = df;

            DateTime dn = System.DateTime.UtcNow;
            model.DateGeneration = dn;

            var items = await _prepaidCardService.Query(  ).SelectAsync();
            int maxBatchs = items.Select(x => x.NumLot).Max();
            int maxNumSerie = items.Select(x => x.NumSerie).Max();

            model.LastNumLot = maxBatchs;
            model.LastNumSerie = maxNumSerie;

            model.NumLot = maxBatchs + 1;
            model.NbCards = 0;
            model.IsActif = true;

            return View("GenerateNewCards", model);
        }

        [HttpPost]
        public ActionResult DoGenerateNewCards(GenerateCardParamsModel model, FormCollection form)
        {
            model.DateGeneration = DateTime.Now;
            DateTime df = new DateTime(DateTime.Now.Year + 1, 12, 31);
            if (DateTime.TryParse(form["DateFinValidite"], out df))
                model.DateFinValidite = df;

            model.NumLot = model.NumLot ;

            // utilise HiddenFor car DisplayFor bind pas le modele
            //model.LastNumSerie = int.Parse(form["LastNumSerie"]) + 1;
            //model.LastNumLot = int.Parse(form["LastNumLot"]) ;
            model.LastNumSerie = model.LastNumSerie + 1;
            model.LastNumLot = model.LastNumLot;

            model.IsActif = true;

            // genere les cartes en bases
            CardsManager cMan = new CardsManager(_unitOfWorkAsync,_prepaidCardService, _userPrepaidCardService);
            try
            {
                cMan.GenerateCards(model);
            }
            catch(Exception ex)
            {
                Console.Write("Erreur controller method GenerateNewCards : GenerateCards : " + ex.Message);
            }



            return View(model);
        }

        /// <summary>
        /// Affiche la page de recherche
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> ViewCards(SearchCardsModel model)
        {
            // SearchCardsModel : recupere toutes les cards de l utilisateur pour l affichage dans le grid
            model = await GetAllUsersSearchCardsModel(model);

            return View(model);
        }

        /// <summary>
        /// Recherche par critere
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SearchCards(SearchCardsModel model, FormCollection form)
        {
            DateTime dt = DateTime.MinValue;
            DateTime.TryParse(form["Scrit.DateFinValidite"], out dt);
            dt = DateTime.MinValue;
            DateTime.TryParse(form["Scrit.DateFirstUse"], out dt);

            model = await GetSearchCardsByCriteria(model);
            return View("ViewCards", model);
        }

        /// <summary>
        /// Recupere les cartes et donnees associees pour un Pro donnee
        /// Prepare le model pour les afficher dans le gridMvc
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SearchCardsModel> GetAllUsersSearchCardsModel(SearchCardsModel searchCardModel)
        {
            // Prend tout les cartes de tous les Pros
            var itemsCards = await _prepaidCardService.Query()
                                            .Include(x => x.UserPrepaidCards)
                                            .SelectAsync();

            var itemsModelList = new List<ProCardItemModel>();

            if (itemsCards != null)
            {
                foreach (var item in itemsCards.OrderByDescending(x => x.NumSerie))
                {
                    itemsModelList.Add(new ProCardItemModel()
                    {
                        UserAddInf = null, // GetUserAdditionalInfos(userId),
                        CurrentCard = item,
                        DateFirstUse = (item.UserPrepaidCards.Count() == 1 ?
                                            item.UserPrepaidCards.First().DateFirstUse : DateTime.MinValue) ,
                        IsInUse = item.UserPrepaidCards.Count() == 1 && item.UserPrepaidCards.First().IsActif

                    });
                }
            }

            searchCardModel.PageSize = 50; 
            searchCardModel.Cards = itemsModelList;
            searchCardModel.CardsPageList = itemsModelList.ToPagedList(searchCardModel.PageNumber, searchCardModel.PageSize);
            searchCardModel.Grid = new ProCardsModelGrid(searchCardModel.CardsPageList.AsQueryable());

            return searchCardModel;
        }

        /// <summary>
        /// Filtre en fonction des criteres dans la classe SearchCriteria
        /// </summary>
        /// <param name="searchCardModel"></param>
        /// <returns></returns>
        public async Task<SearchCardsModel> GetSearchCardsByCriteria(SearchCardsModel searchCardModel)
        {
            int i;
            IEnumerable<PrepaidCard> itemsCards = new List<PrepaidCard>()  ;
            //------------------------------

            // Filtre du plus susciptible de ramener PEU (  le critere le plus specifique)au plus
            // soit par rapport au details du Pro ( on part de la table de liaison UszerPrepaidCard ou soit
            // a partir

            // Par nom de societe ou Nom du gerant : on part de la table de liaison UserPrepaidCard 
            if ( (searchCardModel.SCrit.ChkProCompany &&  !string.IsNullOrEmpty(searchCardModel.SCrit.ProCompany) )
                || (searchCardModel.SCrit.ChkProOwnerName && !string.IsNullOrEmpty(searchCardModel.SCrit.ProOwnerName) ) )
            {
                var itemsUsrInf = await _userPrepaidCardService.Query()
                                        .Include(x => x.AspNetUser)
                                        .Include(y => y.AspNetUser.UsersAddInfos)
                                        .SelectAsync();

                i = itemsUsrInf.Count();

                // champs de UserAddInfo a filtrer
                if (searchCardModel.SCrit.ChkProCompany)
                    itemsUsrInf = itemsUsrInf.Where(x => x.AspNetUser.UsersAddInfos.First().ProCompany == searchCardModel.SCrit.ProCompany);

                //if (searchCardModel.SCrit.ChkProOwnerName)
                  //  itemsUsrInf = itemsUsrInf.Where(x => x.AspNetUser.UsersAddInfos.First().ProOwn == searchCardModel.SCrit.ProCompany);

                i = itemsUsrInf.Count();

                itemsCards = itemsUsrInf.OrderByDescending(x => x.DateFirstUse).Select(x => x.PrepaidCard).ToList();
                //foreach(var item in itemsUsrInf.OrderByDescending(x => x.DateFirstUse).Select(x => x.PrepaidCard) )
                //{
                //    itemsCards.Add(new PrepaidCard {
                //            Code = item.Code,
                //            NumSerie = item.NumSerie,
                //            NumLot = item.NumLot,
                //            DateFinValidite = item.DateFinValidite,
                //            DateGeneration = item.DateGeneration,
                //            IsActif = item.IsActif
                //    }) ;
                //}

            }
            else
            {
                var items = await _prepaidCardService.Query()
                                .Include(x => x.UserPrepaidCards)
                                .SelectAsync() ;

                itemsCards = items.ToList();
            }
                

            i = itemsCards.Count();

            // cartes actives
            if (itemsCards.Count() > 0 && searchCardModel.SCrit.ChkIsActifCard)
                itemsCards = itemsCards.Where(x => x.IsActif);

            // cartes utilisé
            if (itemsCards.Count() > 0 && searchCardModel.SCrit.ChkIsInUse )
                itemsCards = itemsCards.Where(x => x.UserPrepaidCards != null && x.UserPrepaidCards.Count() > 1 
                                                && x.UserPrepaidCards.First().IsActif == searchCardModel.SCrit.IsInUse)
                                                .Select(x => x.UserPrepaidCards.First().PrepaidCard);

            i = itemsCards.Count();

            // cartes utilisé depuis le
            if (itemsCards.Count() > 0 && searchCardModel.SCrit.ChkDateFirstUse && searchCardModel.SCrit.DateFirstUse != DateTime.MinValue)
                itemsCards = itemsCards.Where(x => x.UserPrepaidCards != null && x.UserPrepaidCards.Count() > 1
                                                && x.UserPrepaidCards.First().DateFirstUse > searchCardModel.SCrit.DateFirstUse )
                                                .Select(x => x.UserPrepaidCards.First().PrepaidCard);

            // cartes valide jusqu au
            if (itemsCards.Count() > 0 && searchCardModel.SCrit.ChkDateFinValidite && searchCardModel.SCrit.DateFinValidite != DateTime.MinValue)
                itemsCards = itemsCards.Where(x => x.UserPrepaidCards != null && x.UserPrepaidCards.Count() > 1
                                                && x.UserPrepaidCards.First().DateFirstUse > searchCardModel.SCrit.DateFirstUse)
                                                .Select(x => x.UserPrepaidCards.First().PrepaidCard);

            i = itemsCards.Count();


            var itemsModelList = new List<ProCardItemModel>();

            if (itemsCards != null && itemsCards.Count() > 0)
            {
                foreach (var item in itemsCards.OrderByDescending(x => x.NumSerie))
                {
                    itemsModelList.Add(new ProCardItemModel()
                    {
                        UserAddInf = null, // GetUserAdditionalInfos(userId),
                        CurrentCard = item,
                        DateFirstUse = (item.UserPrepaidCards.Count() == 1 ?
                                            item.UserPrepaidCards.First().DateFirstUse : DateTime.MinValue),
                        IsInUse = item.UserPrepaidCards.Count() == 1 && item.UserPrepaidCards.First().IsActif

                    });
                }
            }

            searchCardModel.PageSize = 50;
            searchCardModel.Cards = itemsModelList;
            searchCardModel.CardsPageList = itemsModelList.ToPagedList(searchCardModel.PageNumber, searchCardModel.PageSize);
            searchCardModel.Grid = new ProCardsModelGrid(searchCardModel.CardsPageList.AsQueryable());

            return searchCardModel;
        }




    }
}