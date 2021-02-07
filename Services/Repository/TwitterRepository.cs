using TwitterStats.Services.Repository.Interfaces;
using TwitterStats.Shared.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Serilog;


namespace TwitterStats.Services.Repository
{
    public class TwitterRepository : ITwitterRepository
    {
        private readonly IConfiguration configuration;
        private readonly string ConnectionString;
        public TwitterRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TwitterStatItem>> GetStats()
        {
            try
            {
                //Mock data: data access goes here....
                var list = MockTwitterData.ToList<TwitterStatItem>();
                return await Task.FromResult(list);
               
            }
            catch (Exception ex)
            {
                Log.Error("Exception in GetStats: " + ex.Message);
                throw;
            }
        }

        #region interface methods

        public async Task<TwitterStatItem> GetByIdAsync(int id)
        {
            await Task.FromResult(true);

            throw new NotImplementedException();
        }

        public async Task<int> AddAsync(TwitterStatItem entity)
        {
            await Task.FromResult(true);

            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(TwitterStatItem entity)
        {
            await Task.FromResult(true);

            throw new NotImplementedException();
        }



        public async Task<int> DeleteAsync(int id)
        {
            await Task.FromResult(true);

            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<TwitterStatItem>> GetAllAsync()
        {
            await Task.FromResult(true);

            throw new NotImplementedException();
        }




        #endregion interface methods


        #region mock data

        public static readonly TwitterStatItem[] MockTwitterData = new TwitterStatItem[]
        {
            new TwitterStatItem{Name ="Phoenix",    Value ="AZ"},
            new TwitterStatItem{Name ="Raleigh",    Value ="NC"},
            new TwitterStatItem{Name ="Saint John", Value ="NB (Canada)"},
            new TwitterStatItem{Name ="San Diego",  Value ="CA"}

        };
        #endregion

    }
}
