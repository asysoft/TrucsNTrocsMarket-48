using BeYourMarket.Model.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BeYourMarket.Web.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "[[[Code]]]")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "[[[Remember this browser?]]]")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "[[[Email]]]")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "[[[Email]]]")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "[[[Password]]]")]
        public string Password { get; set; }

        [Display(Name = "[[[Remember me?]]]")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
            //ImgFiles = new List<PictureModel>();
            ImgFiles = new List<HttpPostedFileBase>();
        }

        [Required]
        [EmailAddress]
        [Display(Name = "[[[Email]]]")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "[[[The {0} must be at least {2} characters long.]]]", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "[[[Password]]]")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "[[[Confirm password]]]")]
        [Compare("Password", ErrorMessage = "[[[The password and confirmation password do not match.]]]")]
        public string ConfirmPassword { get; set; }
        
        [Display(Name = "[[[First Name]]]")]
        public string FirstName { get; set; }
        
        [Display(Name = "[[[Last Name]]]")]
        public string LastName { get; set; }

        [Display(Name = "[[[Phone Number]]]")]
        public string PhoneNumber { get; set; }

        // Permet de savoir le type de compte a a creation ( par defaut : normal, sinon Pro)
        public Enum_UserType UserType { get; set; }

        [Display(Name = "[[[Gender]]]")]
        public string Gender { get; set; }

        [Display(Name = "[[[Company Name]]]")]
        public string ProCompany { get; set; }

        [Display(Name = "[[[Siret]]]")]
        public string ProSiret { get; set; }

        [Display(Name = "[[[Adress]]]")]
        public string ProAdress { get; set; }

        [Display(Name = "[[[Town Or Zip]]]")]
        public string ProTownZip { get; set; }

        [Display(Name = "[[[Company Phone]]]")]
        public string ProPhone { get; set; }

        // liste des categ du pro separe  par un; 
        public string ProCategoryIDs { get; set; }

        [Display(Name = "[[[Company Web Site]]]")]
        public string ProSiteWeb { get; set; }

        [Display(Name = "[[[Company Email]]]")]
        public string ProEmail { get; set; }

        [Display(Name = "[[[Location ID]]]")]
        public int ProLocationRefID { get; set; }

        [Display(Name = "[[[Latitude]]]")]
        public double? ProLatitude { get; set; }

        [Display(Name = "[[[Longitude]]]")]
        public double? ProLongitude { get; set; }
        
        [Display(Name = "[[[Pro Card Number]]]")]
        public string ProCardNumber { get; set; }

        // pour les Pro Uniquement, logo en indice 0 et autres ( pubs ? best of products ?...)
        public IEnumerable<HttpPostedFileBase> ImgFiles { get; set; }

    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "[[[Email]]]")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "[[[The {0} must be at least {2} characters long.]]]", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "[[[Password]]]")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "[[[Confirm password]]]")]
        [Compare("Password", ErrorMessage = "[[[The password and confirmation password do not match.]]]")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "[[[Email]]]")]
        public string Email { get; set; }
    }
}
