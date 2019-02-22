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

    public interface IUserPrepaidCardService : IService<UserPrepaidCard>
    {
    }

    public class UserPrepaidCardService : Service<UserPrepaidCard>, IUserPrepaidCardService
    {
        public UserPrepaidCardService(IRepositoryAsync<UserPrepaidCard> repository)
            : base(repository)
        {
        }
    }
}
