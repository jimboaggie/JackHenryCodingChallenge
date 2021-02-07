using System;
using System.Collections.Generic;
using System.Text;
using TwitterStats.Services.Repository;

namespace TwitterStats.Services.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        ITwitterRepository TwitterRepos { get; }
      

    }
}
