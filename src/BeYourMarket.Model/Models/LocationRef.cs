using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models
{

    public partial class LocationRef : Repository.Pattern.Ef6.Entity
    {
        public LocationRef()
        {
            this.Listings = new List<Listing>();
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Parent { get; set; }
        public bool Enabled { get; set; }
        public int Ordering { get; set; }

        public virtual ICollection<Listing> Listings { get; set; }
    }

}
