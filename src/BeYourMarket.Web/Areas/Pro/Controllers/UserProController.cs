using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BeYourMarket.Core.Web;
using BeYourMarket.Model.Enum;
using BeYourMarket.Model.Models;
using BeYourMarket.Service;
using BeYourMarket.Web.Controllers;
using BeYourMarket.Web.Models;
using BeYourMarket.Web.Models.Grids;
using BeYourMarket.Web.Utilities;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Repository.Pattern.UnitOfWork;
using Microsoft.Practices.Unity;
using BeYourMarket.Web.Extensions;

namespace BeYourMarket.Web.Areas.Pro.Controllers
{
    public class UserProController : Controller
    {
        #region Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private readonly IListingService _listingService;
        private readonly IListingStatService _ListingStatservice;
        private readonly IListingPictureService _listingPictureservice;
        private readonly IListingReviewService _listingReviewService;
        private readonly IOrderService _orderService;

        private readonly ICustomFieldCategoryService _customFieldCategoryService;
        private readonly ICustomFieldListingService _customFieldListingService;

        private readonly IPictureService _pictureService;
        private readonly IAspNetUserImgFileService _aspNetUserImgFileService;

        private readonly DataCacheService _dataCacheService;
        private readonly SqlDbService _sqlDbService;

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
        public UserProController(
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
                           IListingStatService listingStatservice,
                           DataCacheService dataCacheService,
                           SqlDbService sqlDbService
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

            _dataCacheService = dataCacheService;
            _sqlDbService = sqlDbService;

            _unitOfWorkAsync = unitOfWorkAsync;
        }
        #endregion

        // GET: Pro/UserPro
        public async Task<ActionResult> Index()
        {
            // chzrge l App User de Identity et le logo
            var model = new ProShopModel();
            model = await LoadUserProInfosAndLogo();

            // charge les annonces du pro         
            model.ListingsSearch = await GetCurrentProListingsResult(model.UserPro.Id);

            // test : set le datatokens pour le findview de razor pour trouver le _layout
            //if (!this.ControllerContext.RouteData.DataTokens.ContainsKey("area"))
            //    this.ControllerContext.RouteData.DataTokens.Add("area", "Pro");

            return View("~/Areas/Pro/Views/UserPro/ProShop.cshtml", model);

        }
   
        private async Task<SearchListingModel> GetCurrentProListingsResult(string userId)
        {
            var model = new SearchListingModel();

            model.ListingTypeID = CacheHelper.ListingTypes.Select(x => x.ID).ToList();
            model.ProUserID = userId;

            IEnumerable<Listing> items = null;

            // Prend toutes les annonces du Pro
            // ! pour tests prends tout
            // items = await _listingService.Query(x => x.AspNetUser.Id == model.ProUserID)
            items =  await _listingService.Query(x => x.AspNetUser.Id != null)
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

            return model;
        }

        /// <summary>
        /// Infos detaille du Pro
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> ProIdentity(ProShopModel model)
        {
            model = await LoadUserProInfosAndLogo();

            return View(model);
        }

        [AllowAnonymous]
        /// <summary>
        /// Chargement des infos et affichage dans la vue
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        
        public async Task<ActionResult> ProIdentityUpdate(int? id)
        {
            var model = await LoadUserProInfosAndLogo();
                   
            return View(model);
        }

        /// <summary>
        /// Mise a jour et sauvegarde du model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="form"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ProIdentityUpdate(ProShopModel model, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        {

            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            model.UserPro.Id = user.Id;

            try
            {

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
            }
            catch (Exception ex)
            {

                throw;
            }


            return RedirectToAction("ProIdentity", "UserPro");
            //return View(model);

        }

        // chzrge l App User de Identity et le logo
        public async Task<ProShopModel> LoadUserProInfosAndLogo()
        {
            var userInfos = new ProShopModel();

            var userId = User.Identity.GetUserId();
            var user = UserManager.FindByIdAsync(userId);
            userInfos.UserPro = await user;

            // valeur par defaut , sinon la vue pete en cas de pb d enregistrement
            if (string.IsNullOrEmpty(userInfos.UserPro.Gender))
                userInfos.UserPro.Gender = "M";

            // Charge le logo
            var pictures = _aspNetUserImgFileService.Query(x => x.AspNetUserId == userId).Select();
            var picturesModel = pictures.Select(x =>
                new PictureModel()
                {
                    ID = x.PictureID,
                    Url = ImageHelper.GetUserPrologoImagePath(x.PictureID),
                    Ordering = x.Ordering
                }).OrderBy(x => x.Ordering).ToList();

            // si pas de logo defini, insere au moins 1 image pour l affichage 
            userInfos.Pictures = picturesModel;
            if (userInfos.Pictures.Count == 0)
                userInfos.Pictures.Add(new PictureModel(){
                    ID = 0,
                    Url = ImageHelper.GetUserPrologoImagePath(0),
                    Ordering = 0 
                });

            return userInfos;

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

            try
            {
                 UserManager.Update(existingUser);
                await _unitOfWorkAsync.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
            return new IdentityResult();

        }

        private async Task<List<PictureModel>> checkAndsaveLogo(List<PictureModel> oldPiclogos, IEnumerable<HttpPostedFileBase> NewFileLogos)
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
                                        //.Resize(size)
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

        public async Task<ActionResult> ProListingUpdate(int? id)
        {
            if (CacheHelper.Categories.Count == 0)
            {
                TempData[TempDataKeys.UserMessageAlertState] = "bg-danger";
                TempData[TempDataKeys.UserMessage] = "[[[There are not categories available yet.]]]";
            }

            Listing listing;

            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);

            var model = new ListingUpdateModel()
            {
                Categories = CacheHelper.Categories
            };

            // ASY : a voir fait 2 fois
            model.LocationsRef = CacheHelper.LocationsRef;

            if (id.HasValue)
            {
                // return unauthorized if not authenticated
                if (!User.Identity.IsAuthenticated)
                    return new HttpUnauthorizedResult();

                if (await NotMeListing(id.Value))
                    return new HttpUnauthorizedResult();

                listing = await _listingService.FindAsync(id);

                if (listing == null)
                    return new HttpNotFoundResult();

                // Pictures
                var pictures = await _listingPictureservice.Query(x => x.ListingID == id).SelectAsync();

                var picturesModel = pictures.Select(x =>
                    new PictureModel()
                    {
                        ID = x.PictureID,
                        Url = ImageHelper.GetListingImagePath(x.PictureID),
                        ListingID = x.ListingID,
                        Ordering = x.Ordering
                    }).OrderBy(x => x.Ordering).ToList();

                model.Pictures = picturesModel;
            }
            else
            {
                listing = new Listing()
                {
                    CategoryID = CacheHelper.Categories.Any() ? CacheHelper.Categories.FirstOrDefault().ID : 0,
                    LocationRefID = CacheHelper.LocationsRef.Any() ? CacheHelper.LocationsRef.FirstOrDefault().ID : 0,
                    Created = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day),
                    LastUpdated = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day),
                    Expiration = new DateTime(DateTime.Now.Date.Year + 50, DateTime.Now.Date.Month, DateTime.Now.Date.Day),
                    Enabled = true,
                    Active = true,
                };

                if (User.Identity.IsAuthenticated)
                {
                    listing.ContactEmail = user.Email;
                    listing.ContactName = string.Format("{0} {1}", user.FirstName, user.LastName);
                    listing.ContactPhone = user.PhoneNumber;
                }
            }

            // AS : populate loc ref
            listing.LocationRef = CacheHelper.LocationsRef.Where(m => m.ID == listing.LocationRefID).FirstOrDefault();

            // Populate model with listing
            await PopulateListingUpdateModel(listing, model);

            //return View("~/Views/Listing/ListingUpdate.cshtml", model);


            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ProListingUpdate(Listing listing, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        {
            if (CacheHelper.Categories.Count == 0)
            {
                TempData[TempDataKeys.UserMessageAlertState] = "bg-danger";
                TempData[TempDataKeys.UserMessage] = "[[[There are not categories available yet.]]]";

                return RedirectToAction("Listing", new { id = listing.ID });
            }

            var userIdCurrent = User.Identity.GetUserId();

            // Register account if not login
            if (!User.Identity.IsAuthenticated)
            {
                var accountController = BeYourMarket.Core.ContainerManager.GetConfiguredContainer().Resolve<AccountController>();

                var modelRegister = new RegisterViewModel()
                {
                    Email = listing.ContactEmail,
                    Password = form["Password"],
                    ConfirmPassword = form["ConfirmPassword"],
                };

                // Parse first and last name
                var names = listing.ContactName.Split(' ');
                if (names.Length == 1)
                {
                    modelRegister.FirstName = names[0];
                }
                else if (names.Length == 2)
                {
                    modelRegister.FirstName = names[0];
                    modelRegister.LastName = names[1];
                }
                else if (names.Length > 2)
                {
                    modelRegister.FirstName = names[0];
                    modelRegister.LastName = listing.ContactName.Substring(listing.ContactName.IndexOf(" ") + 1);
                }

                // Register account
                var resultRegister = await accountController.RegisterAccount(modelRegister);

                // Add errors
                AddErrors(resultRegister);

                // Show errors if not succeed
                if (!resultRegister.Succeeded)
                {
                    var model = new ListingUpdateModel()
                    {
                        ListingItem = listing
                    };
                    // Populate model with listing
                    await PopulateListingUpdateModel(listing, model);
                    return View("ListingUpdate", model);
                }

                // update current user id
                var user = await UserManager.FindByNameAsync(listing.ContactEmail);
                userIdCurrent = user.Id;
            }

            bool updateCount = false;

            int nextPictureOrderId = 0;

            // Set default listing type ID
            if (listing.ListingTypeID == 0)
            {
                var listingTypes = CacheHelper.ListingTypes.Where(x => x.CategoryListingTypes.Any(y => y.CategoryID == listing.CategoryID));

                if (listingTypes == null)
                {
                    TempData[TempDataKeys.UserMessageAlertState] = "bg-danger";
                    TempData[TempDataKeys.UserMessage] = "[[[There are not listing types available yet.]]]";

                    return RedirectToAction("Listing", new { id = listing.ID });
                }

                listing.ListingTypeID = listingTypes.FirstOrDefault().ID;
            }

            // ASY : set la lat lng, en insert ou en update depuis la collection form
            listing.Latitude = Double.Parse(form["Latitude"].Replace(',', '.'), CultureInfo.InvariantCulture); ;
            listing.Longitude = Double.Parse(form["Longitude"].Replace(',', '.'), CultureInfo.InvariantCulture);

            if (listing.ID == 0)
            {
                listing.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                listing.IP = Request.GetVisitorIP();
                listing.Expiration = DateTime.MaxValue.AddDays(-1);
                listing.UserID = userIdCurrent;
                listing.Enabled = true;
                listing.Currency = CacheHelper.Settings.Currency;
                listing.Created = DateTime.Now;

                // AS : populate loc ref
                listing.LocationRef = CacheHelper.LocationsRef.Where(m => m.ID == listing.LocationRefID).FirstOrDefault();

                updateCount = true;
                _listingService.Insert(listing);
            }
            else
            {
                if (await NotMeListing(listing.ID))
                    return new HttpUnauthorizedResult();

                var listingExisting = await _listingService.FindAsync(listing.ID);

                listingExisting.Title = listing.Title;
                listingExisting.Description = listing.Description;
                listingExisting.Active = listing.Active;
                listingExisting.Price = listing.Price;

                listingExisting.ContactEmail = listing.ContactEmail;
                listingExisting.ContactName = listing.ContactName;
                listingExisting.ContactPhone = listing.ContactPhone;

                listingExisting.Latitude = listing.Latitude;
                listingExisting.Longitude = listing.Longitude;

                // Gestion des locations    
                listingExisting.Location = listing.Location;
                listingExisting.LocationRefID = listing.LocationRefID;

                listingExisting.ShowPhone = listing.ShowPhone;
                listingExisting.ShowEmail = listing.ShowEmail;

                listingExisting.CategoryID = listing.CategoryID;
                listingExisting.ListingTypeID = listing.ListingTypeID;

                listingExisting.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;

                _listingService.Update(listingExisting);
            }

            // Delete existing fields on item
            var customFieldItemQuery = await _customFieldListingService.Query(x => x.ListingID == listing.ID).SelectAsync();
            var customFieldIds = customFieldItemQuery.Select(x => x.ID).ToList();
            foreach (var customFieldId in customFieldIds)
            {
                await _customFieldListingService.DeleteAsync(customFieldId);
            }

            // Get custom fields
            var customFieldCategoryQuery = await _customFieldCategoryService.Query(x => x.CategoryID == listing.CategoryID).Include(x => x.MetaField.ListingMetas).SelectAsync();
            var customFieldCategories = customFieldCategoryQuery.ToList();

            foreach (var metaCategory in customFieldCategories)
            {
                var field = metaCategory.MetaField;
                var controlType = (BeYourMarket.Model.Enum.Enum_MetaFieldControlType)field.ControlTypeID;

                string controlId = string.Format("customfield_{0}_{1}_{2}", metaCategory.ID, metaCategory.CategoryID, metaCategory.FieldID);

                var formValue = form[controlId];

                if (string.IsNullOrEmpty(formValue))
                    continue;

                formValue = formValue.ToString();

                var itemMeta = new ListingMeta()
                {
                    ListingID = listing.ID,
                    Value = formValue,
                    FieldID = field.ID,
                    ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added
                };

                _customFieldListingService.Insert(itemMeta);
            }

            await _unitOfWorkAsync.SaveChangesAsync();

            if (Request.Files.Count > 0)
            {
                var itemPictureQuery = _listingPictureservice.Queryable().Where(x => x.ListingID == listing.ID);
                if (itemPictureQuery.Count() > 0)
                    nextPictureOrderId = itemPictureQuery.Max(x => x.Ordering);
            }

            if (files != null && files.Count() > 0)
            {
                foreach (HttpPostedFileBase file in files)
                {
                    if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                    {
                        // Picture picture and get id
                        var picture = new Picture();
                        picture.MimeType = "image/jpeg";
                        _pictureService.Insert(picture);
                        await _unitOfWorkAsync.SaveChangesAsync();

                        // Format is automatically detected though can be changed.
                        ISupportedImageFormat format = new JpegFormat { Quality = 90 };
                        Size size = new Size(500, 0);

                        //https://naimhamadi.wordpress.com/2014/06/25/processing-images-in-c-easily-using-imageprocessor/
                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                        {
                            var path = Path.Combine(Server.MapPath("~/images/listing"), string.Format("{0}.{1}", picture.ID.ToString("00000000"), "jpg"));

                            // Load, resize, set the format and quality and save an image.
                            imageFactory.Load(file.InputStream)
                                        .Resize(size)
                                        .Format(format)
                                        .Save(path);
                        }

                        var itemPicture = new ListingPicture();
                        itemPicture.ListingID = listing.ID;
                        itemPicture.PictureID = picture.ID;
                        itemPicture.Ordering = nextPictureOrderId;

                        _listingPictureservice.Insert(itemPicture);

                        nextPictureOrderId++;
                    }
                }
            }

            await _unitOfWorkAsync.SaveChangesAsync();

            // Update statistics count
            if (updateCount)
            {
                _sqlDbService.UpdateCategoryItemCount(listing.CategoryID);
                _dataCacheService.RemoveCachedItem(CacheKeys.Statistics);
            }

            TempData[TempDataKeys.UserMessage] = "[[[Listing is updated!]]]";
            return RedirectToAction("Listing", new { id = listing.ID });
        }

        private async Task<ListingUpdateModel> PopulateListingUpdateModel(Listing listing, ListingUpdateModel model)
        {
            model.ListingItem = listing;

            // Custom fields
            var customFieldCategoryQuery = await _customFieldCategoryService.Query(x => x.CategoryID == listing.CategoryID).Include(x => x.MetaField.ListingMetas).SelectAsync();
            var customFieldCategories = customFieldCategoryQuery.ToList();
            var customFieldModel = new CustomFieldListingModel()
            {
                ListingID = listing.ID,
                MetaCategories = customFieldCategories
            };

            model.CustomFields = customFieldModel;
            model.UserID = listing.UserID;
            model.CategoryID = listing.CategoryID;
            model.ListingTypeID = listing.ListingTypeID;
            model.LocationRefID = listing.LocationRefID;

            // Listing types
            model.ListingTypes = CacheHelper.ListingTypes.Where(x => x.CategoryListingTypes.Any(y => y.CategoryID == model.CategoryID)).ToList();

            // Listing Categories
            //  model.Categories = CacheHelper.Categories;

            // Listing Locations
            //+ model.LocationsRef = CacheHelper.LocationsRef;

            return model;
        }

        public async Task<bool> NotMeListing(int id)
        {
            var userId = User.Identity.GetUserId();
            var item = await _listingService.FindAsync(id);
            return item.UserID != userId;
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
 
        [ChildActionOnly]
        public ActionResult NavigationSide()
        {

            var contentPages = CacheHelper.ContentPages.Where(x => x.Published).OrderBy(x => x.Ordering);

            var model = new NavigationSideModel()
            {
                //       CategoryTree = categoryTree,
                ContentPages = contentPages
            };

            return View("~/Areas/Pro/Views/Shared/_NavigationSide.cshtml", model);
        }


    }
}