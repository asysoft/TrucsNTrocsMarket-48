using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace BeYourMarket.Web.Utilities
{
    public class ImageHelper
    {
        public static string fileFormat = "00000000";

        public static string ImageVersion(string filePath)
        {
            DateTime lastWriteTime = File.GetLastWriteTime(HostingEnvironment.MapPath(filePath));

            // display version in dex format
            return string.Format("{0}?v={1:x}", VirtualPathUtility.ToAbsolute(filePath), lastWriteTime.Ticks);
        }

        public static bool HasImage(int id)
        {
            var filePath = string.Format("~/images/listing/{0}.jpg", id.ToString(fileFormat));

            return File.Exists(HostingEnvironment.MapPath(filePath));
        }

        public static string GetListingImagePath(int id)
        {
            var filePath = string.Format("~/images/listing/{0}.jpg", id.ToString(fileFormat));
            if (File.Exists(HostingEnvironment.MapPath(filePath)))
            {
                return ImageVersion(filePath);
            }
            else
            {
                return "http://placehold.it/500x300";
            }
        }

        public static string GetUserPrologoImagePath(int id)
        {
            var filePath = string.Format("~/images/Profile/ProLogos/{0}.jpg", id.ToString(fileFormat));
            if (File.Exists(HostingEnvironment.MapPath(filePath)))
            {
                return ImageVersion(filePath);
            }
            else
            {
                return "http://placehold.it/500x300";
            }
        }

        public static string GetUserProfileImagePath(string name)
        {
            var filePath = string.Format("~/images/profile/{0}.jpg", name);
            if (File.Exists(HostingEnvironment.MapPath(filePath)))
            {
                return ImageVersion(filePath);
            }
            else
            {
               // return ImageVersion(string.Format("~/images/profile/{0}.jpg", "Avatar_Defaut") ); 
                return "http://www.gravatar.com/avatar/?d=mm";
            }
        }

        public static string GetCommunityImagePath(string name, string format = "jpg", bool returnEmptyIfNotFound = false)
        {
            var filePath = string.Format("~/images/community/{0}.{1}", name, format);
            if (File.Exists(HostingEnvironment.MapPath(filePath)))
            {
                return ImageVersion(filePath);
            }
            else if (returnEmptyIfNotFound)
            {
                return string.Empty;
            }
            else
            {
                return "http://placehold.it/500x300";
            }
        }


        public static string GetCommunityImagePubsPath(string name, string format = "jpg", bool returnEmptyIfNotFound = false)
        {
            var filePath = string.Format("~/images/community/pubs/{0}.{1}", name, format);
            if (File.Exists(HostingEnvironment.MapPath(filePath)))
            {
                return ImageVersion(filePath);
            }
            else if (returnEmptyIfNotFound)
            {
                return string.Empty;
            }
            else
            {
                return "http://placehold.it/500x300";
            }
        }

        /// <summary>
        /// ecupere le nombre max de pub pour une section (gauche, droite haut)
        /// Soit en fixe soien comptant le nb de fichier commencant par pubName ?
        /// </summary>
        /// <param name="pubName"></param>
        /// <returns></returns>
        public static int GetMaxNbforPub(string pubName)
        {
            int nb = 3;
            switch (pubName)
            {
                case "pub_Index_Haut_800x200_Img":
                case "pub_Index_Gauche_250x350_Img":
                case "pub_Index_Droite_250x350_Img":
                    nb = 1;
                    break;

                case "pub_Listings_Haut_800x200_Img":
                case "pub_Listings_Gauche_250x350_Img":
                case "pub_Listings_Droite_250x350_Img":
                    nb = 1;
                    break;

                case "pub_Register_Haut_800x200_Img1":
                case "pub_Register_Gauche_250x350_Img":
                case "pub_Register_Droite_250x350_Img":
                    nb = 1;
                    break;
            }

            return nb;
        }

    }
}