using BeYourMarket.Model.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;


namespace BeYourMarket.Service
{
    public interface ICategoryListingTypeService : IService<CategoryListingType>
    {
        
    }

    public class CategoryListingTypeService : Service<CategoryListingType>, ICategoryListingTypeService
    {
        public CategoryListingTypeService(IRepositoryAsync<CategoryListingType> repository)
            : base(repository)
        {            
        }
    }
}
