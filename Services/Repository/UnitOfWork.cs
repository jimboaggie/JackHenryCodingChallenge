using TwitterStats.Services.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterStats.Services.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ITwitterRepository TwitterRepos { get; }

        public UnitOfWork(ITwitterRepository twitterRepository)
        {
            TwitterRepos = twitterRepository; 
        }
    

    }
}
