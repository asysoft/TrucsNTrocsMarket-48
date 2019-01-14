using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BeYourMarket.Web.Models;
using BeYourMarket.Web.Utilities;
using BeYourMarket.Service;
using BeYourMarket.Service.Models;
using i18n;
using BeYourMarket.Model.Enum;
using BeYourMarket.Model.Models;
using System.Collections.Generic;
using Repository.Pattern.UnitOfWork;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using ImageProcessor;
using System.IO;

namespace BeYourMarket.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Fields

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IAspNetUserCategoriesService _userCategoriesService;
        private readonly IAspNetUserImgFileService _aspNetUserImgFileService;
        private readonly IPictureService _pictureService;

        private IUsersAddInfoService _usersAddInfoService;

        private readonly IUnitOfWorkAsync _unitOfWorkAsync;
        #endregion

        #region Properties
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? System.Web.HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region Constructor
        public AccountController(IUnitOfWorkAsync unitOfWorkAsync, 
                                IEmailTemplateService emailTemplateService, 
                                IAspNetUserCategoriesService UserCategoriesService,
                                IAspNetUserImgFileService AspNetUserImgFileService,
                                IPictureService pictureService,
                                IUsersAddInfoService UsersAddInfoService
                                )
        {
            _unitOfWorkAsync = unitOfWorkAsync;
            _emailTemplateService = emailTemplateService;
            _userCategoriesService = UserCategoriesService;
            _aspNetUserImgFileService = AspNetUserImgFileService;
            _pictureService = pictureService;

            _usersAddInfoService = UsersAddInfoService;
    }
        #endregion

        #region Methods

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Require the user to have a confirmed email before they can log on.
            if (CacheHelper.Settings.EmailConfirmedRequired)
            {
                var user = await UserManager.FindByNameAsync(model.Email);                
                if (user != null)
                {
                    var roleAdministrator = await RoleManager.FindByNameAsync(Enum_UserType.Administrator.ToString());
                    var isAdministrator = user.Roles.Any(x => x.RoleId == roleAdministrator.Id);

                    // Skip email check unless it's an administrator
                    if (!isAdministrator && !await UserManager.IsEmailConfirmedAsync(user.Id))
                    {
                        ModelState.AddModelError("", "[[[You must have a confirmed email to log on.]]]");
                        return View(model);
                    }
                }
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    // ASY : centralie la redirection dans RedirectToLocal
                    // ? dans quel cas Url n est pas null ou est null?
                    //if (string.IsNullOrEmpty(returnUrl))
                    //{
                    //    return RedirectToAction("Index", "Manage");
                    //}
                    //return RedirectToLocal(returnUrl);
                    return RedirectToUserTypeEnv(model.Email);

                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "[[[Invalid login attempt.]]]");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "[[[Invalid code.]]]");
                    return View(model);
            }
        }
                    

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        // pour les pros recuperer logo  
        public async Task<ActionResult> Register(RegisterViewModel model, FormCollection form , IEnumerable<HttpPostedFileBase> files)
        {
            model.ProLatitude = Double.Parse(form["ProLatitude"].Replace(',', '.'), CultureInfo.InvariantCulture); ;
            model.ProLongitude = Double.Parse(form["ProLongitude"].Replace(',', '.'), CultureInfo.InvariantCulture);

            //
            model.ImgFiles = files;
            var result = await RegisterAccount(model);
                
            // Add errors
            AddErrors(result);

            if (result.Succeeded)
            {
                // ASY : redirige vers l env de l user
                //return RedirectToAction("Index", "Manage");
                return RedirectToUserTypeEnv(model.Email);
            }


            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<IdentityResult> RegisterAccount(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                Gender = model.Gender,
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,

                UserType = model.UserType,

                RegisterDate = DateTime.Now,
                RegisterIP = System.Web.HttpContext.Current.Request.GetVisitorIP(),
                LastAccessDate = DateTime.Now,
                LastAccessIP = System.Web.HttpContext.Current.Request.GetVisitorIP()
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Creation des categories du pro, des infos complementaires et affectation au role Pro
                if (model.UserType == Enum_UserType.Professional)
                {
                    // infos compl
                    // Specific aux Pros  
                    UsersAddInfo usrAddInf = new UsersAddInfo();
                    usrAddInf.UserID = user.Id;
                    usrAddInf.ProCompany = model.ProCompany;
                    usrAddInf.ProSiret = model.ProSiret;
                    usrAddInf.ProAdress = model.ProAdress;
                    usrAddInf.ProTownZip = model.ProTownZip;
                    usrAddInf.ProPhone = model.ProPhone;
                    usrAddInf.ProSiteWeb = model.ProSiteWeb;
                    usrAddInf.ProEmail = model.ProEmail;
                    usrAddInf.LocationRefID = model.ProLocationRefID;
                    usrAddInf.ProLatitude = model.ProLatitude;
                    usrAddInf.ProLongitude = model.ProLongitude;

                    _usersAddInfoService.Insert(usrAddInf);

                    //  categories
                    List<string> idsSel = new List<string>();
                    if (model.ProCategoryIDs != null)
                        idsSel = model.ProCategoryIDs.Split(';').ToList();

                    foreach (string catId in idsSel)
                    {
                        _userCategoriesService.Insert(new UserCategory()
                        {
                            AspNetUserId = user.Id,
                            CategoryID = int.Parse(catId),
                            //Created = DateTime.Now,
                            ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added
                        });
                    }

                    // Affecte au role Pro
                    var roleAdded = UserManager.AddToRole(user.Id, Enum_UserType.Professional.ToString());

                    // Sauvegarde le logo
                    int PictureLogoOrder = 0;

                    if (model.ImgFiles != null && model.ImgFiles.Count() > 0)
                    {
                        foreach (HttpPostedFileBase file in model.ImgFiles)
                        {
                            if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                            {
                                // Picture picture and get id
                                var picture = new Picture();

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
                                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: false))
                                {
                                    var path = Path.Combine(Server.MapPath("~/Images/Profile/Prologos"), string.Format("{0}.{1}", picture.ID.ToString("00000000"), "jpg"));

                                    // Load, resize, set the format and quality and save an image.
                                    imageFactory.Load(file.InputStream)
                                                .Resize(size)
                                                .Format(format)
                                                .Save(path);
                                }

                                var itemPicture = new UserImgFile();
                                itemPicture.ObjectState = Repository.Pattern.Infrastructure.ObjectState.Added;
                                itemPicture.AspNetUserId = user.Id;
                                itemPicture.PictureID = picture.ID;
                                itemPicture.Ordering = PictureLogoOrder;

                                _aspNetUserImgFileService.Insert(itemPicture);

                            }
                        }
                    }

                }

                // sauve le logo
                await _unitOfWorkAsync.SaveChangesAsync();

                // connection de l utilisateur
                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                // Send Message
                var roleAdministrator = await RoleManager.FindByNameAsync(BeYourMarket.Model.Enum.Enum_UserType.Administrator.ToString());
                var administrator = roleAdministrator.Users.FirstOrDefault();

                var message = new MessageSendModel()
                {
                    UserFrom = administrator.UserId,
                    UserTo = user.Id,
                    Subject = HttpContext.ParseAndTranslate(string.Format("[[[Welcome to {0}!]]]", CacheHelper.Settings.Name)),
                    Body = HttpContext.ParseAndTranslate(string.Format("[[[Hi, Welcome to {0}! I am happy to assist you if you has any questions.]]]", CacheHelper.Settings.Name))

                };

                await MessageHelper.SendMessage(message);

                // Send an email with this link
                string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var urlHelper = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
                var callbackUrl = urlHelper.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: System.Web.HttpContext.Current.Request.Url.Scheme);

                var emailTemplateQuery = await _emailTemplateService.Query(x => x.Slug.ToLower() == "signup").SelectAsync();
                var emailTemplate = emailTemplateQuery.FirstOrDefault();

                if (emailTemplate != null)
                {
                    dynamic email = new Postal.Email("Email");
                    email.To = user.Email;
                    email.From = CacheHelper.Settings.EmailAddress;
                    email.Subject = emailTemplate.Subject;
                    email.Body = emailTemplate.Body;
                    email.CallbackUrl = callbackUrl;
                    EmailHelper.SendEmail(email);
                }
            }            

            return result;
        }
        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // Check if email confirm required
                if (CacheHelper.Settings.EmailConfirmedRequired && !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                var emailTemplateQuery = await _emailTemplateService.Query(x => x.Slug.ToLower() == "forgotpassword").SelectAsync();
                var emailTemplate = emailTemplateQuery.Single();

                dynamic email = new Postal.Email("Email");
                email.To = user.Email;
                email.From = CacheHelper.Settings.EmailAddress;
                email.Subject = emailTemplate.Subject;
                email.Body = emailTemplate.Body;
                email.CallbackUrl = callbackUrl;
                EmailHelper.SendEmail(email);

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View(new ResetPasswordViewModel() { Code = code });
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");

        }

        /// <summary>
        /// ASY :  redirige dans l environnement de l utilisateur
        /// a voir le cas des login externe
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        private ActionResult RedirectToUserTypeEnv(string userEMail)
        {
            var currentUsr = UserManager.FindByName(userEMail);

            if (currentUsr != null)
            {
                var roleAdministrator = RoleManager.FindByName(Enum_UserType.Administrator.ToString());
                var isAdministrator = currentUsr.Roles.Any(x => x.RoleId == roleAdministrator.Id);
                if (isAdministrator)
                    return RedirectToAction("Index", "Home");

                // Pro
                var rolePro = RoleManager.FindByName(Enum_UserType.Professional.ToString());
                var isPro = currentUsr.Roles.Any(x => x.RoleId == rolePro.Id);
                //if (isPro)
                //    return RedirectToAction("Index", "Professional", new { proUserID = currentUsr.Id } );
                if (isPro)
                    return RedirectToAction("Index", "UserPro", new { area = "Pro" });

                // Autres roles , backoff


            }

            // Sinon Par defaut, les particuliers
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
        #endregion
    }
}