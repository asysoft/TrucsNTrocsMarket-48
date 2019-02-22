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
using Unity;
using BeYourMarket.Web.Extensions;

namespace BeYourMarket.Web.Areas.Pro.Controllers
{
    public class UserProController : Controller
    {
        #region Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private IAspNetUserService _aspNetUserService;
        private IUsersAddInfoService _usersAddInfoService;

        private readonly IListingService _listingService;
        private readonly IListingStatService _ListingStatservice;
        private readonly IListingPictureService _listingPictureservice;
        private readonly IListingReviewService _listingReviewService;
        private readonly IOrderService _orderService;

        private readonly ICustomFieldCategoryService _customFieldCategoryService;
        private readonly ICustomFieldListingService _customFieldListingService;

        private readonly IPictureService _pictureService;
        private readonly IAspNetUserImgFileService _aspNetUserImgFileService;
        private readonly IAspNetUserCategoriesService _aspNetUserCategoriesService;

        private readonly IUserPrepaidCardService _userPrepaidCardService;
        private readonly IPrepaidCardService _prepaidCardService;

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
                            IAspNetUserService AspNetUserService,
                            IUsersAddInfoService UsersAddInfoService,
                            
                           IPictureService pictureService,
                           IAspNetUserImgFileService AspNetUserImgFileService,
                           IAspNetUserCategoriesService AspNetUserCategoriesService,
                           IListingService listingService,
                           IListingPictureService listingPictureservice,
                           IUserPrepaidCardService userPrepaidCardService,
                           IPrepaidCardService prepaidCardService,

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
           
            _aspNetUserService = AspNetUserService;
            _usersAddInfoService = UsersAddInfoService;

            _pictureService = pictureService;
            _aspNetUserImgFileService = AspNetUserImgFileService;
            _aspNetUserCategoriesService = AspNetUserCategoriesService;
            _listingService = listingService;
            _listingReviewService = listingReviewService;
            _pictureService = pictureService;
            _listingPictureservice = listingPictureservice;

            _userPrepaidCardService = userPrepaidCardService;
            _prepaidCardService = prepaidCardService;

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
            var userId = User.Identity.GetUserId();
            return  await ShowProShopByID(userId);
        }

        // GET: Pro/UserPro
        public async Task<ActionResult> ShowProShopByID(string userID)
        {
            // charge l App User de Identity et le logo
            var model = new ProShopModel();
            model = await LoadUserProInfosAndLogoByID(userID);

            // charge les annonces du pro         
            model.ListingsSearch = await GetCurrentProListingsResult(model.UserPro.Id);

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
            items = await _listingService.Query(x => x.AspNetUser.Id == model.ProUserID)
                   //items = await _listingService.Query(x => x.AspNetUser.Id != null)
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
        public async Task<ActionResult> ShowProIdentityByID(string userId)
        {
            var model = await LoadUserProInfosAndLogoByID(userId);
            return View("~/Areas/Pro/Views/UserPro/ProIdentity.cshtml", model);
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
        public ActionResult ProIdentityUpdate(ProShopModel model, FormCollection form, IEnumerable<HttpPostedFileBase> files)
        {
             var userId = User.Identity.GetUserId();
            //var user = await UserManager.FindByIdAsync(userId);
            model.UserPro.Id = userId;
            
            try
            {
                // Met a jour les infos additionel de l user pro
                UsersAddInfo addInf = new UsersAddInfo();
                addInf.ProCompany = form["UserAddInf.ProCompany"];
                addInf.ProSiret = form["UserAddInf.ProSiret"];
                addInf.ProAdress = form["UserAddInf.ProAdress"];
                addInf.ProTownZip = form["UserAddInf.ProTownZip"];
                addInf.ProPhone = form["UserAddInf.ProPhone"];
                addInf.ProSiteWeb = form["UserAddInf.ProSiteWeb"];
                addInf.ProEmail = form["UserAddInf.ProEmail"];

                if ( ! string.IsNullOrEmpty(form["UserAddInf.ProLatitude"]))
                    addInf.ProLatitude = Double.Parse(form["UserAddInf.ProLatitude"].Replace(',', '.'), CultureInfo.InvariantCulture); 
                if (!string.IsNullOrEmpty(form["UserAddInf.ProLongitude"]))
                    addInf.ProLongitude = Double.Parse(form["UserAddInf.ProLongitude"].Replace(',', '.'), CultureInfo.InvariantCulture);
                int locRefID = 0;
                if (int.TryParse(form["UserAddInf.LocationRefID"], out locRefID))
                    addInf.LocationRefID = locRefID;

                // met a jour en base le champs modifiablle du profil (voir pour le pwd)
                UpdateProApplicationUser(model.UserPro, addInf);

                // met a jour les categories
                UpdateUserCategories(userId,model.CategoryIDs);


                // met a jour le logo ,  Charge le logo
                var pictures = _aspNetUserImgFileService.Query(x => x.AspNetUserId == userId).Select();
                var picturesModel = pictures.Select(x =>
                    new PictureModel()
                    {
                        ID = x.PictureID,
                        Url = ImageHelper.GetUserPrologoImagePath(x.PictureID),
                        Ordering = x.Ordering
                    }).OrderBy(x => x.Ordering).ToList();

                model.Pictures = checkAndsaveLogo(picturesModel, files);
            }
            catch (Exception ex)
            {

                throw;
            }


            return RedirectToAction("ProIdentity", "UserPro");
        }

        /// <summary>
        /// charge l App User de Identity et le logo , pour un PRO deja connecté
        /// </summary>
        /// <returns></returns>
        public async Task<ProShopModel> LoadUserProInfosAndLogo()
        {
            var userInfos = new ProShopModel();

            var userId = User.Identity.GetUserId();
            var user = UserManager.FindByIdAsync(userId);
            userInfos.UserPro = await user;

            // valeur par defaut , sinon la vue pete en cas de pb d enregistrement
            if (string.IsNullOrEmpty(userInfos.UserPro.Gender))
                userInfos.UserPro.Gender = "M";

            // donnee additionel du Pro
            userInfos.UserAddInf = GetUserAdditionalInfos(userId);

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
                userInfos.Pictures.Add(new PictureModel() {
                    ID = 0,
                    Url = ImageHelper.GetUserPrologoImagePath(0),
                    Ordering = 0
                });

            // Ajoute les categories de la société ( texte, separe par ","  pour affichages, les ids sont dans table  )
            CategResult categs = getMainCategoriesText(userId); ;
            userInfos.CategoriesText = categs.listNames;
            userInfos.CategoryIDs = categs.listIds;

            return userInfos;

        }

        /// <summary>
        /// charge l App User de Identity et le logo , pour un avoir les infos du listing d'un Pro
        /// ( pas forcemment connecté)
        /// </summary>
        /// <returns></returns>
        public async Task<ProShopModel> LoadUserProInfosAndLogoByID(string userId)
        {
            var userInfos = new ProShopModel();

            var user = UserManager.FindByIdAsync(userId);
            userInfos.UserPro = await user;

            // valeur par defaut , sinon la vue pete en cas de pb d enregistrement
            if (string.IsNullOrEmpty(userInfos.UserPro.Gender))
                userInfos.UserPro.Gender = "M";

            // donnee additionel du Pro
            userInfos.UserAddInf = GetUserAdditionalInfos(userId);

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
                userInfos.Pictures.Add(new PictureModel()
                {
                    ID = 0,
                    Url = ImageHelper.GetUserPrologoImagePath(0),
                    Ordering = 0
                });

            // Ajoute les categories de la société ( texte, separe par ","  pour affichages, les ids sont dans table  )
            CategResult categs = getMainCategoriesText(userId); ;
            userInfos.CategoriesText = categs.listNames;
            userInfos.CategoryIDs = categs.listIds;

            return userInfos;

        }


        /// <summary>
        /// Recupere les infos additionelle concernant le Pro
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private UsersAddInfo GetUserAdditionalInfos(string userId)
        {
            UsersAddInfo addInf = new UsersAddInfo();
            addInf = _usersAddInfoService.Query(x => x.UserID == userId)
                .Include(x => x.LocationRef)
                .Select().FirstOrDefault();

            return addInf;
        }

        /// <summary>
        /// struct pour transmettre le resultat
        /// </summary>
        public struct CategResult
        {
            public string listNames;
            public string listIds;

            public CategResult (string names , string ids)
            {
                listNames = names;
                listIds = ids;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public CategResult getMainCategoriesText(string userId)
        {
            string res = string.Empty;
            string categsId = string.Empty;
            List<string> categsTxt;
            CategResult Procategs = new CategResult(string.Empty, string.Empty) ;

            var userCategs = _aspNetUserCategoriesService.Query(x => x.AspNetUserId == userId)
                 .Include(c => c.Category)
                 .Select()
                 .OrderBy(y => y.CategoryID) ;
           
            if (userCategs != null)
            {
                // concat les noms
                categsTxt = userCategs.Select(x => x.Category.Name).ToList();
                Procategs.listNames = String.Join(", ", categsTxt.ToArray());

                //concat les ids categsId, sauf les id de parent sinon jsTree select tout les enfants (meme ceux decoché)                
                var query = userCategs.Where(x => x.Category.Parent != 0).ToList() ;
                categsTxt = query.Select(x => x.Category.ID.ToString()).ToList();

               // categsTxt = userCategs.Select(x => x.Category.ID.ToString()).ToList();
                Procategs.listIds = String.Join(";", categsTxt.ToArray());
            }
            return Procategs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryIDs"></param>
        private void UpdateUserCategories(string userId, string categoryIDs)
        {
            var vals = _aspNetUserCategoriesService.Query(x => x.AspNetUserId == userId).Select();
            try
            {

                List<string> oldCategs = new List<string>(vals.Select(x => x.CategoryID.ToString()));
                List<string> newCategs = categoryIDs.Split(new char[] { ';' }).OrderBy(x => int.Parse(x)).ToList() ;

                // efface ceux qui ont été enlevé
                foreach (string id in oldCategs)
                    if (!newCategs.Contains(id))
                    {
                        //var toDel = _aspNetUserCategoriesService.Query(x => x.AspNetUserId == userId && x.CategoryID == int.Parse(id)).Select();
                        _aspNetUserCategoriesService.DeleteAsync(userId, int.Parse(id));
                    }

                // ajoute les parents manquants, car jstree ne les mets pas en cas multiselect et qu'on decoche 1 ou plusieurs
                List<string> parsToAdd = new List<string>();
                foreach (string id in newCategs)
                {
                    var el = CacheHelper.Categories.Where(x => x.ID == int.Parse(id)).FirstOrDefault();
                    // ajoute le parents des enfants, attention ne pas traité un parent deja dans la liste
                    if (el.Parent!= 0 && !parsToAdd.Contains(el.Parent.ToString()) && !newCategs.Contains(el.Parent.ToString()) )
                         parsToAdd.Add(el.Parent.ToString());
                }
                if (parsToAdd.Count() > 0)
                    newCategs.InsertRange(0, parsToAdd);

                // cré ceux qui n existe pas
                foreach (string id in newCategs)
                    if (!oldCategs.Contains(id))
                    {
                        _aspNetUserCategoriesService.Insert(new UserCategory()
                        {
                            ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added,
                            AspNetUserId = userId,
                            CategoryID = int.Parse(id)
                        });
                    }

                // enregistre en base
                int res = _unitOfWorkAsync.SaveChanges();
            }
            catch(Exception ex)
            {
                throw;
            }

        }

        public void UpdateProApplicationUser(ApplicationUser user, UsersAddInfo addInf)
        {
            // Create user if there is no user id
            //ApplicationUser existingUser = UserManager.FindById(user.Id);

            var existingUserObj = _aspNetUserService.Query(x => x.Id == user.Id);
            var existingUser = existingUserObj.Select().FirstOrDefault();
            if (existingUser != null)
            {                
                existingUser.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Modified;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Gender = user.Gender;

                existingUser.LastAccessDate = DateTime.Now;
                existingUser.LastAccessIP = System.Web.HttpContext.Current.Request.GetVisitorIP();
            }

            try
            {
                _aspNetUserService.Update(existingUser);
                //UserManager.UpdateAsync(existingUser);
                // SAUVEGARDE
                //_unitOfWorkAsync.SaveChanges();


                var currAddInf = _usersAddInfoService.Query(x => x.UserID == user.Id)
                    .Include(x=> x.LocationRef).Select().FirstOrDefault();
                if (currAddInf == null)  // normallement n arrive pas : on defini toujours des users infos au register
                    _usersAddInfoService.Insert(addInf);
                else
                {                  
                    currAddInf.ProCompany = addInf.ProCompany;
                    currAddInf.ProSiret = addInf.ProSiret;
                    currAddInf.ProAdress = addInf.ProAdress;
                    currAddInf.ProTownZip = addInf.ProTownZip;
                    currAddInf.ProPhone = addInf.ProPhone;
                    currAddInf.ProSiteWeb = addInf.ProSiteWeb;
                    currAddInf.ProEmail = addInf.ProEmail;

                    currAddInf.ProLatitude = addInf.ProLatitude;
                    currAddInf.ProLongitude = addInf.ProLongitude ;

                    currAddInf.LocationRefID = addInf.LocationRefID;

                    _usersAddInfoService.Update(currAddInf);
                }

                // SAUVEGARDE
                _unitOfWorkAsync.SaveChanges();

            }
            catch (Exception ex)
            {

                throw;
            }
    
        }

        private  List<PictureModel> checkAndsaveLogo(List<PictureModel> oldPiclogos, IEnumerable<HttpPostedFileBase> NewFileLogos)
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
                             deleteProLogo(oldPiclogos.First().ID);

                        // Picture picture and get id
                        picture = new Picture();

                        picture.MimeType = "image/jpeg";
                        var mimeType = MimeMapping.GetMimeMapping(file.FileName);
                        picture.MimeType = mimeType;

                        _pictureService.Insert(picture);
                         _unitOfWorkAsync.SaveChangesAsync();

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

                        var itemPicture = new UserImgFile();
                        itemPicture.AspNetUserId = userId;
                        itemPicture.PictureID = picture.ID;
                        itemPicture.Ordering = PictureLogoOrder;

                        _aspNetUserImgFileService.Insert(itemPicture);
                         _unitOfWorkAsync.SaveChangesAsync();
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
        
        public ActionResult deleteProLogo(int id)
        {
            try
            {

                //  await _aspNetUserImgFileService.DeleteAsync(id);

                var itemPicture = _aspNetUserImgFileService.Query(x => x.PictureID == id).Select().FirstOrDefault();

                if (itemPicture != null)
                     _aspNetUserImgFileService.DeleteAsync(itemPicture.AspNetUserId, itemPicture.PictureID);

                 _unitOfWorkAsync.SaveChangesAsync();

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

        public async Task<ActionResult> ProListingView(int id)
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

            // charge les annonces du pro        
            ProListingViewModel modelLst = new ProListingViewModel();
            modelLst.ListingItem = itemModel;
            modelLst.ListingsSearch = await GetCurrentProListingsResult(itemModel.User.Id);

            modelLst.ProInfos = await LoadUserProInfosAndLogoByID(itemModel.User.Id);

            return View(modelLst);
            //return View("~/Views/Listing/Listing.cshtml", itemModel);
        }

        public async Task<ActionResult> ProListingView_old(int? id)
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

                // Pictures TODOOOOOOOOOOOOOOOOOOOOOOOOOO
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
                    listing.OwnerUserType = Enum_UserType.Professional;
                }
            }

            // charge les donnée du Pro
            ProShopModel modelPro = await LoadUserProInfosAndLogo();

            // AS : populate loc ref et Latitude et longitude avec celles du Pro
            listing.Latitude = modelPro.UserAddInf.ProLatitude;
            listing.Longitude = modelPro.UserAddInf.ProLongitude;
            listing.LocationRefID = modelPro.UserAddInf.LocationRefID;
            listing.LocationRef = CacheHelper.LocationsRef.Where(m => m.ID == listing.LocationRefID).FirstOrDefault();

            listing.UserID = User.Identity.GetUserId();

            // Populate model with listing
            await PopulateListingUpdateModel(listing, model);

            modelPro.currListing = model;

            return View();
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

                // Pictures TODOOOOOOOOOOOOOOOOOOOOOOOOOO
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
                    listing.OwnerUserType = Enum_UserType.Professional;
                }
            }

            // charge les donnée du Pro
            ProShopModel modelPro = await LoadUserProInfosAndLogo();

            // AS : populate loc ref et Latitude et longitude avec celles du Pro
            listing.Latitude = modelPro.UserAddInf.ProLatitude;
            listing.Longitude = modelPro.UserAddInf.ProLongitude;
            listing.LocationRefID = modelPro.UserAddInf.LocationRefID;
            listing.LocationRef = CacheHelper.LocationsRef.Where(m => m.ID == listing.LocationRefID).FirstOrDefault();

            listing.UserID = User.Identity.GetUserId();

            // Populate model with listing
            await PopulateListingUpdateModel(listing, model);

            modelPro.currListing = model;

            return View(modelPro);
        }

        [HttpPost]
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
            //listing.Latitude = Double.Parse(form["Latitude"].Replace(',', '.'), CultureInfo.InvariantCulture); ;
            //listing.Longitude = Double.Parse(form["Longitude"].Replace(',', '.'), CultureInfo.InvariantCulture);

            if (listing.ID == 0)
            {
                listing.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                listing.IP = Request.GetVisitorIP();
                listing.Expiration = DateTime.MaxValue.AddDays(-1);
                listing.UserID = userIdCurrent;
                listing.Enabled = true;
                listing.Currency = CacheHelper.Settings.Currency;
                listing.Created = DateTime.Now;

                // AS : populate loc ref et Latitude et longitude avec celles du Pro
                //listing.Latitude = modelPro.UserAddInf.ProLatitude;
                //listing.Longitude = modelPro.UserAddInf.ProLongitude;
                //listing.LocationRefID = modelPro.UserAddInf.LocationRefID;
                listing.LocationRef = CacheHelper.LocationsRef.Where(m => m.ID == listing.LocationRefID).FirstOrDefault();

                listing.OwnerUserType = Enum_UserType.Professional;

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
                listingExisting.OwnerUserType = listing.OwnerUserType;

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
            return RedirectToAction("Index", new { area= "Pro" });
            //return RedirectToAction("Listing", new { id = listing.ID });
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

        public async Task<ActionResult> ProManageCards()
        {
            // charge l App User de Identity et le logo
            var userId = User.Identity.GetUserId();
            ProShopModel proModel =  await LoadUserProInfosAndLogoByID(userId);

            //
            ProManageCardsModel cardManModel = new ProManageCardsModel();
            cardManModel.UserAddInf = proModel.UserAddInf;
            cardManModel.Prologos = proModel.Pictures;

            // SearchCardsModel : recupere toutes les cards de l tuilisateur pour l affichage dans le grid
            cardManModel.CardsSearch = await GetUserSearchCardsModel(userId);

            // NeedNewCard : indique si le Pro a fini ses cartes valides et s il doit en entrer une nouvelle
            var nbCardsInUse = cardManModel.CardsSearch.Cards.Select(x => x.CurrentCard.UserPrepaidCards.Select(y => y.IsActif)).Count();
            cardManModel.NeedNewCard = (nbCardsInUse == 0);

            //
            return View(cardManModel);
        }

        /// <summary>
        /// Recupere les cartes et donnees associees pour un Pro donnee
        /// Prepare le model pour les afficher dans le gridMvc
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SearchCardsModel> GetUserSearchCardsModel(string userId)
        {
            SearchCardsModel searchCardModel = new SearchCardsModel();

            // Prend tout les cartes utilisées par le Pro
            var itemsUserCards = await _userPrepaidCardService.Query(x => x.UserID == userId)
                                            .Include(x => x.PrepaidCard)                            
                                            .SelectAsync();

            var itemsModelList = new List<ProCardItemModel>();

            if (itemsUserCards != null)
            {
                foreach (var item in itemsUserCards.OrderByDescending(x => x.DateFirstUse))
                {
                    itemsModelList.Add(new ProCardItemModel()
                    {
                        UserAddInf = GetUserAdditionalInfos(userId),
                        CurrentCard = item.PrepaidCard,
                        DateFirstUse = item.DateFirstUse
                    });
                }
            }

            searchCardModel.Cards = itemsModelList;
            searchCardModel.CardsPageList = itemsModelList.ToPagedList(searchCardModel.PageNumber, searchCardModel.PageSize);
            searchCardModel.Grid = new ProCardsModelGrid(searchCardModel.CardsPageList.AsQueryable());

            return searchCardModel;
        }


    }
}