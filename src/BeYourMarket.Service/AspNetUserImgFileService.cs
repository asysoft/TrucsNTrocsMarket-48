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

    public interface IAspNetUserImgFileService : IService<UserImgFile>
    {
    }

    public class AspNetUserImgFileService : Service<UserImgFile>, IAspNetUserImgFileService
    {
        public AspNetUserImgFileService(IRepositoryAsync<UserImgFile> repository)
            : base(repository)
        {
        }
    }
}
