using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{
    public class UsersAddInfo : Repository.Pattern.Ef6.Entity
    {
        public UsersAddInfo()
        {

        }

        public int ID { get; set; }
        public string UserID { get; set; }

        public string ProCompany { get; set; }
        public string ProSiret { get; set; }
        public string ProAdress { get; set; }
        public string ProTownZip { get; set; }
        public string ProPhone { get; set; }
        public string ProSiteWeb { get; set; }
        public string ProEmail { get; set; }

        public int LocationRefID { get; set; }
        public double? ProLongitude { get; set; }
        public double? ProLatitude { get; set; }

        // Pour les cle etrangere
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual LocationRef LocationRef { get; set; }
    }


}
