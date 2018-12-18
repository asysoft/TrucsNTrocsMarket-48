using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using BeYourMarket.Web.Areas.Admin.Models;
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

        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        IPrepaidCardService _prepaidCardService;
        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        #endregion

        #region Contructors

        public PrepaidCardController(
           IUnitOfWorkAsync unitOfWorkAsync,
           IPrepaidCardService prepaidCardService
            )
        {

            _prepaidCardService = prepaidCardService;
            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        // GET: Admin/PrepaidCard
        public ActionResult Index()
        {
            GenerateCardParamsModel model = new GenerateCardParamsModel();

            DateTime df = new System.DateTime(2019, 12, 31);
            model.DateFinValidite = df;

            DateTime dn = System.DateTime.UtcNow;
            model.DateGeneration = dn;

            model.NumLot = 1;
            model.LastNumSerie = 1;  // recuperer dans la base !

            model.NbCards = 0;
            model.IsActif = true;

            return View("GenerateNewCards", model);
        }

        //async Task<ActionResult>
        [HttpPost]
        public ActionResult GenerateNewCards(GenerateCardParamsModel model, FormCollection form)
        {
            model.DateFinValidite = new DateTime(2019, 12, 31);
            model.DateGeneration = DateTime.Now;

            model.NumLot = 1;
            model.LastNumSerie = 1;  // recuperer dans la base !

            model.IsActif = true;

            // genere les cartes en bases
            CardsManager cMan = new CardsManager(_unitOfWorkAsync,_prepaidCardService);
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

    }
}