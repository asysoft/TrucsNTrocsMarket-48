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
    public interface IMessageService : IService<Message>
    {
    }

    public class MessageService : Service<Message>, IMessageService
    {
        public MessageService(IRepositoryAsync<Message> repository)
            : base(repository)
        {
        }
    }
}
