using BeYourMarket.Model.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;

namespace BeYourMarket.Service
{
    public interface IPubLocationsService : IService<PubLocations>
    {
    }

    public class PubLocationsService : Service<PubLocations>, IPubLocationsService
    {
        public PubLocationsService(IRepositoryAsync<PubLocations> repository)
            : base(repository)
        {
        }

    }

}
