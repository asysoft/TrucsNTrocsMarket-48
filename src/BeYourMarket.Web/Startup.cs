using BeYourMarket.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(BeYourMarket.Web.Startup))]
namespace BeYourMarket.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //createRolesandUsers();

        }

        // In this method we will create default User roles and Admin user for login   
        private void createRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Administrator"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);
            }

                //Here we create a Admin super user who will maintain the website                  

                var user = new ApplicationUser();
                user.UserName = "asy";
                user.Email = "asysoft@yahoo.com";

                string userPWD = "TnTtest2018";

                //////////////////////
                user.RegisterDate = DateTime.Now;
                user.LastAccessDate = DateTime.Now;
                user.AcceptEmail = true;
                user.LeadSourceID = 0; //?
                user.EmailConfirmed = true;
                user.PhoneNumberConfirmed = true;
                user.TwoFactorEnabled = true;
                user.LockoutEnabled = false;
                user.AccessFailedCount = 5;
                user.Disabled = false;

                /*
                    [Id][nvarchar](128) NOT NULL,
                   [FirstName] [nvarchar]   (max) NULL,
                   [LastName] [nvarchar] (max) NULL,
                   [RegisterDate] [datetime] NOT NULL,
                   [RegisterIP] [nvarchar] (max) NULL,
                   [LastAccessDate] [datetime]  NOT NULL,
                   [LastAccessIP] [nvarchar] (max) NULL,
                   [DateOfBirth] [datetime] NULL,
                    [AcceptEmail] [bit] NOT NULL,
                    [Gender] [nvarchar] (max) NULL,
                    [LeadSourceID] [int] NOT NULL,
                    [Email] [nvarchar] (256) NULL,
                    [EmailConfirmed]  [bit]  NOT NULL,
                    [PasswordHash] [nvarchar]  (max) NULL,
                    [SecurityStamp] [nvarchar] (max) NULL,
                    [PhoneNumber] [nvarchar]  (max) NULL,
                    [PhoneNumberConfirmed] [bit]  NOT NULL,
                    [TwoFactorEnabled] [bit]  NOT NULL,
                    [LockoutEndDateUtc] [datetime] NULL,
                    [LockoutEnabled] [bit]  NOT NULL,
                    [AccessFailedCount] [int] NOT NULL,
                    [UserName] [nvarchar] (256) NOT NULL,
                     [Disabled] [bit]  NOT NULL,
                     [Rating] [float] NOT NULL,
                */

                //////////////////////

                var chkUser = UserManager.Create(user, userPWD);

            //Add default User to Role Admin   
            if (chkUser.Succeeded)
            {
                var result1 = UserManager.AddToRole(user.Id, "Administrator");

            }
            

            //// creating Creating Manager role    
            //if (!roleManager.RoleExists("Manager"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Manager";
            //    roleManager.Create(role);

            //}

            //// creating Creating Employee role    
            //if (!roleManager.RoleExists("Employee"))
            //{
            //    var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
            //    role.Name = "Employee";
            //    roleManager.Create(role);

            //}
        }

    }
}
