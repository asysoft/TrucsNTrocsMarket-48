using BeYourMarket.Model.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Service
{
    public interface IUsersAddInfoService : IService<UsersAddInfo>
    {
    }

    public class UsersAddInfoService : Service<UsersAddInfo>, IUsersAddInfoService
    {
        public UsersAddInfoService(IRepositoryAsync<UsersAddInfo> repository)
            : base(repository)
        {
        }
    }
}
