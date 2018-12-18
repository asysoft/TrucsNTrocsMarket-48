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

    public interface IAspNetUserImgFileService : IService<AspNetUserImgFile>
    {
    }

    public class AspNetUserImgFileService : Service<AspNetUserImgFile>, IAspNetUserImgFileService
    {
        public AspNetUserImgFileService(IRepositoryAsync<AspNetUserImgFile> repository)
            : base(repository)
        {
        }
    }
}
