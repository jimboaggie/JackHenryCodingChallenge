using TwitterStats.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TwitterStats.Services.Repository.Interfaces
{
    public interface ITwitterRepository : IGenericRepository<TwitterStatItem>
    {
        Task<List<TwitterStatItem>> GetStats();

    }
}
