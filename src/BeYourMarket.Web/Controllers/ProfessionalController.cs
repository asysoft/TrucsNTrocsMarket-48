using BeYourMarket.Model.Enum;
using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using BeYourMarket.Web.Models;
using BeYourMarket.Web.Models.Grids;
using BeYourMarket.Web.Utilities;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BeYourMarket.Web.Controllers
{
    public class ProfessionalController : Controller
    {
        #region Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly ICategoryService _categoryService;
        private readonly IListingService _listingService;
        private readonly IListingStatService _ListingStatservice;
        private readonly IListingPictureService _listingPictureservice;
        private readonly IListingReviewService _listingReviewService;
        private readonly IOrderService _orderService;
        private readonly ICustomFieldService _customFieldService;
        private readonly ICustomFieldCategoryService _customFieldCategoryService;
        private readonly ICustomFieldListingService _customFieldListingService;

        private readonly IPictureService _pictureService;
        private readonly IAspNetUserImgFileService _aspNetUserImgFileService;

        private readonly IUnitOfWorkAsync _unitOfWorkAsync;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion

        #region Contructors
        public ProfessionalController(
           IUnitOfWorkAsync unitOfWorkAsync,
           IPictureService pictureService,
           IAspNetUserImgFileService AspNetUserImgFileService,
            IListingService listingService,
           IListingPictureService listingPictureservice,
           IOrderService orderService,
           ICustomFieldService customFieldService,
            IListingReviewService listingReviewService,
           ICustomFieldCategoryService customFieldCategoryService,
           ICustomFieldListingService customFieldListingService,
           ISettingDictionaryService settingDictionaryService,
           IListingStatService listingStatservice

            )
        {

            _pictureService = pictureService;
            _aspNetUserImgFileService = AspNetUserImgFileService;
            _listingService = listingService;
            _listingReviewService = listingReviewService;
            _pictureService = pictureService;
            _listingPictureservice = listingPictureservice;
            _orderService = orderService;

            _customFieldCategoryService = customFieldCategoryService;
            _customFieldListingService = customFieldListingService;
            _ListingStatservice = listingStatservice;

            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        // GET: Professional
        //public async Task<ActionResult> Index(string proUserID)
        public async Task<ActionResult> Index(string proUserID)
        {

            //if (!string.IsNullOrEmpty(id))
            //    return RedirectToAction("ContentPage", "Home", new { id = id.ToLowerInvariant() });

            var model = new SearchListingModel();

            model.ListingTypeID = CacheHelper.ListingTypes.Select(x => x.ID).ToList();
            model.ProUserID = proUserID;

            // Recupere les annonces du pro
            await GetCurrentProListingsResult(model);

            return View(model);
        }

        private async Task GetCurrentProListingsResult(SearchListingModel model)
        {
            IEnumerable<Listing> items = null;

            // Prend toutes les annonces du Pro
            // ! pour tests prends tout
            // items = await _listingService.Query(x => x.AspNetUser.Id == model.ProUserID)
            items = await _listingService.Query(x => x.AspNetUser.Id != null)
                    .Include(x => x.ListingPictures)
                    .Include(x => x.Category)
                    .Include(x => x.ListingType)
                    .Include(x => x.AspNetUser)
                    .Include(x => x.ListingReviews)
                    .Include(x => x.LocationRef)
                .SelectAsync();

            model.ListingTypes = CacheHelper.ListingTypes;

            var itemsModelList = new List<ListingItemModel>();

            if (items != null)
            {

                foreach (var item in items.Where(x => x.Active && x.Enabled).OrderByDescending(x => x.Created))
                {
                    itemsModelList.Add(new ListingItemModel()
                    {
                        ListingCurrent = item,
                        UrlPicture = item.ListingPictures.Count == 0 ? ImageHelper.GetListingImagePath(0) : ImageHelper.GetListingImagePath(item.ListingPictures.OrderBy(x => x.Ordering).FirstOrDefault().PictureID)
                    });
                }

            }

            model.Categories = CacheHelper.Categories;
            model.LocationsRef = CacheHelper.LocationsRef;

            model.Listings = itemsModelList;
            model.ListingsPageList = itemsModelList.ToPagedList(model.PageNumber, model.PageSize);
            model.Grid = new ListingModelGrid(model.ListingsPageList.AsQueryable());

        }

        /// <summary>
        /// Point d'entree pour l affichage de la page PRO
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult DisplayProTabsInfos(string id)
        //public PartialViewResult DisplayProTabsInfos(SearchListingModel model, string id)        
        // public ActionResult DisplayProTabsInfos(SearchListingModel model, string id)
        {
            int tabId = Convert.ToInt32(id);

            var userId = User.Identity.GetUserId();
            var user = UserManager.FindById(userId);

            if (user == null) 
                RedirectToAction("Register","Account");

            // ?Construit le model pour gestion des identite et listings et autres des pros
            // Ou utilise ProShopModel
            var modelProUpd = new ProShopModel();
            modelProUpd.UserPro = user;

            // Charge le logo
            var pictures = _aspNetUserImgFileService.Query(x => x.AspNetUserId == userId).Select();
            var picturesModel = pictures.Select(x =>
                new PictureModel()
                {
                    ID = x.PictureID,
                    Url = ImageHelper.GetUserPrologoImagePath(x.PictureID),
                    Ordering = x.Ordering
                }).OrderBy(x => x.Ordering).ToList();

            modelProUpd.Pictures = picturesModel;

            // Pro HOME
            if (tabId == 1)
                return PartialView("_ProHome", modelProUpd);

            // Pro Company Profile
            if (tabId == 2)
            {
               // return PartialView("_ProIdentity", modelProUpd);
                return PartialView("_ProIdentityUpdate", modelProUpd);
            }

            // Pro Add New Listing
            if (tabId == 3)
            {
                //var modelProListing = new ListingUpdateModel();
                //return PartialView("_ProListingUpdate",  modelProListing);
                return PartialView("_ProIdentity", modelProUpd);
            }

            //
            if (tabId == 4)
            {
                 return PartialView("_ProIdentity", modelProUpd);
                //return PartialView("_ProIdentityUpdate", modelProUpd);
            }

            // Par defaut Home
            return PartialView("_ProHome", modelProUpd);
        }


        [HttpPost]
        public PartialViewResult ShowProListings(SearchListingModel model)
        {
            //SearchListingModel model

            // int empid = Convert.ToInt32(id);
            //According to the id to query the database
            //var query = from ee in context.Employees
            //            where ee.EmployeeID == empid
            //            select ee;

            return PartialView("_ProIdentity", new List<int>());
        }

        
        //[AllowAnonymous]
        //public async Task<ActionResult> ProIdentityUpdate(ProShopModel model, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        //{

        //    return View();
        //}

        [HttpPost]
        //public async  Task<ActionResult> ProIdentityUpdate(ProShopModel model)
        //public ActionResult ProIdentityUpdate(ProShopModel model)
        public async Task<ActionResult> ProIdentityUpdate(ProShopModel model, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        {

            var userId = User.Identity.GetUserId ();
            var user = UserManager.FindById(userId);
            model.UserPro.Id = user.Id;

            // met a jour en base lee champs modifiablle du profil (voir pour le pwd)
            var result = UpdateProApplicationUser(model.UserPro);

            // met a jour le logo ,  Charge le logo
            var pictures = _aspNetUserImgFileService.Query(x => x.AspNetUserId == userId).Select();
            var picturesModel = pictures.Select(x =>
                new PictureModel()
                {
                    ID = x.PictureID,
                    Url = ImageHelper.GetUserPrologoImagePath(x.PictureID),
                    Ordering = x.Ordering
                }).OrderBy(x => x.Ordering).ToList();

            model.Pictures = await checkAndsaveLogo(picturesModel, files);

  

            //return RedirectToAction("Index", "Professional");
           // return PartialView("_ProIdentityUpdate", model);
            return View(model);

        }

        public async Task<IdentityResult> UpdateProApplicationUser(ApplicationUser user)
        {
            // Create user if there is no user id
            var existingUser = await UserManager.FindByIdAsync(user.Id);

            if (existingUser != null)
            {
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Gender = user.Gender;           

                existingUser.ProCompany = user.ProCompany;
                existingUser.ProSiret = user.ProSiret;
                existingUser.ProAdress = user.ProAdress;
                existingUser.ProTownZip = user.ProTownZip;
                existingUser.ProPhone = user.ProPhone;
                existingUser.Email = user.Email;
                existingUser.ProSiteWeb = user.ProSiteWeb;

                existingUser.LastAccessDate = DateTime.Now;
                existingUser.LastAccessIP = System.Web.HttpContext.Current.Request.GetVisitorIP();

            }

            await UserManager.UpdateAsync(existingUser);
            await _unitOfWorkAsync.SaveChangesAsync();

            return new IdentityResult ();

        }

        private async Task < List<PictureModel> > checkAndsaveLogo(List<PictureModel> oldPiclogos, IEnumerable<HttpPostedFileBase> NewFileLogos)
        {
            var userId = User.Identity.GetUserId();
            List<PictureModel> logos = new List<PictureModel>();
            Picture picture = null;
            int PictureLogoOrder = 0;

            if (NewFileLogos != null && NewFileLogos.Count() > 0)
            {
                foreach (HttpPostedFileBase file in NewFileLogos)
                {
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        // efface l ancien logo, le fichier et l association en base
                        if (oldPiclogos.Count > 0)
                            await deleteProLogo(oldPiclogos.First().ID);

                        // Picture picture and get id
                         picture = new Picture();

                        picture.MimeType = "image/jpeg";
                        var mimeType = MimeMapping.GetMimeMapping(file.FileName);
                        picture.MimeType = mimeType;

                        _pictureService.Insert(picture);
                         await _unitOfWorkAsync.SaveChangesAsync();

                        // Format is automatically detected though can be changed.
                        ISupportedImageFormat format = new JpegFormat { Quality = 90 };
                        Size size = new Size(200, 200);

                        //https://naimhamadi.wordpress.com/2014/06/25/processing-images-in-c-easily-using-imageprocessor/
                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            var path = Path.Combine(Server.MapPath("~/Images/Profile/Prologos"), string.Format("{0}.{1}", picture.ID.ToString("00000000"), "jpg"));

                            // Load, resize, set the format and quality and save an image.
                            imageFactory.Load(file.InputStream)
                                        .Resize(size)
                                        .Format(format)
                                        .Save(path);
                        }

                        var itemPicture = new AspNetUserImgFile();
                        itemPicture.AspNetUserId = userId;
                        itemPicture.PictureID = picture.ID;
                        itemPicture.Ordering = PictureLogoOrder;

                        _aspNetUserImgFileService.Insert(itemPicture);
                        await _unitOfWorkAsync.SaveChangesAsync();
                    }
                }
            }
            
            // recharge le nouveau logo
            var pictures = _aspNetUserImgFileService.Query(x => x.AspNetUserId == userId).Select();
            var picturesModel = pictures.Select(x =>
                new PictureModel()
                {
                    ID = x.PictureID,
                    Url = ImageHelper.GetUserPrologoImagePath(x.PictureID),
                    Ordering = x.Ordering
                }).OrderBy(x => x.Ordering).ToList();

            return picturesModel;

        }


        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> GetProListingInfos(int id)
        {
            var itemQuery = await _listingService.Query(x => x.ID == id)
                .Include(x => x.Category)
                .Include(x => x.ListingMetas)
                .Include(x => x.ListingMetas.Select(y => y.MetaField))
                .Include(x => x.ListingStats)
                .Include(x => x.ListingType)
                .Include(x => x.LocationRef)
                .SelectAsync();

            var item = itemQuery.FirstOrDefault();

            if (item == null)
                return new HttpNotFoundResult();

            var orders = _orderService.Queryable()
                .Where(x => x.ListingID == id
                    && (x.Status != (int)Enum_OrderStatus.Pending || x.Status != (int)Enum_OrderStatus.Confirmed)
                    && (x.FromDate.HasValue && x.ToDate.HasValue)
                    && (x.FromDate >= DateTime.Now || x.ToDate >= DateTime.Now))
                    .ToList();

            List<DateTime> datesBooked = new List<DateTime>();
            foreach (var order in orders)
            {
                for (DateTime date = order.FromDate.Value; date <= order.ToDate.Value; date = date.Date.AddDays(1))
                {
                    datesBooked.Add(date);
                }
            }

            var pictures = await _listingPictureservice.Query(x => x.ListingID == id).SelectAsync();

            var picturesModel = pictures.Select(x =>
                new PictureModel()
                {
                    ID = x.PictureID,
                    Url = ImageHelper.GetListingImagePath(x.PictureID),
                    ListingID = x.ListingID,
                    Ordering = x.Ordering
                }).OrderBy(x => x.Ordering).ToList();

            var reviews = await _listingReviewService
                .Query(x => x.UserTo == item.UserID)
                .Include(x => x.AspNetUserFrom)
                .SelectAsync();

            var user = await UserManager.FindByIdAsync(item.UserID);

            var itemModel = new ListingItemModel()
            {
                ListingCurrent = item,
                Pictures = picturesModel,
                DatesBooked = datesBooked,
                User = user,
                ListingReviews = reviews.ToList()
            };

            // Update stat count
            var itemStat = item.ListingStats.FirstOrDefault();
            if (itemStat == null)
            {
                _ListingStatservice.Insert(new ListingStat()
                {
                    ListingID = id,
                    CountView = 1,
                    Created = DateTime.Now,
                    ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added
                });
            }
            else
            {
                itemStat.CountView++;
                itemStat.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                _ListingStatservice.Update(itemStat);
            }

            await _unitOfWorkAsync.SaveChangesAsync();

            return View("~/Views/Listing/Listing.cshtml", itemModel);
        }

        public async Task<ActionResult> deleteProLogo(int id)
        {
            try
            {

              //  await _aspNetUserImgFileService.DeleteAsync(id);

                var itemPicture = _aspNetUserImgFileService.Query(x => x.PictureID == id).Select().FirstOrDefault();

                if (itemPicture != null)
                    await _aspNetUserImgFileService.DeleteAsync(itemPicture.AspNetUserId, itemPicture.PictureID);

                await _unitOfWorkAsync.SaveChangesAsync();

                var path = Path.Combine(Server.MapPath("~/Images/Profile/Prologos"), string.Format("{0}.{1}", id.ToString("00000000"), "jpg"));

                System.IO.File.Delete(path);

                var result = new { Success = "true", Message = "" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Success = "false", Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }





    }
}
 