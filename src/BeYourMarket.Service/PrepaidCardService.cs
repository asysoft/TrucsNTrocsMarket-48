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

    public interface IPrepaidCardService : IService<PrepaidCard>
    {
    }

    public class PrepaidCardService : Service<PrepaidCard>, IPrepaidCardService
    {
        public PrepaidCardService(IRepositoryAsync<PrepaidCard> repository)
            : base(repository)
        {
        }
    }

}
