using BeYourMarket.Model.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeYourMarket.Service
{

    public interface ILocationRefService : IService<LocationRef>
    {

    }

    public class LocationRefService : Service<LocationRef>, ILocationRefService
    {
        public LocationRefService(IRepositoryAsync<LocationRef> repository)
            : base(repository)
        {
        }
    }

}