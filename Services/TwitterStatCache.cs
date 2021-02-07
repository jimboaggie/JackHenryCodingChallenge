using SocialOpinionAPI.Models.SampledStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterStats.Shared.DTO;

namespace TwitterStats.Services
{
    public class TwitterStatCache
    {
        public virtual ICollection<SampledStreamModel> TweetList { get; set; }
        public virtual int SampleSize { get; set; } = 0;

        public TwitterStatCache()
        {
            TweetList = new HashSet<SampledStreamModel>();
        }

        public async Task<List<Url>> GetUrlList()
        {
            List<Url> list = new List<Url>();

            try
            {
                foreach (var item in TweetList)
                {
                    if(item.data.entities != null)
                    {
                        if (item.data.entities.urls.Count > 0)
                        {
                            foreach (var url in item.data.entities.urls)
                            {
                                list.Add(url);
                            }
                        }
                    }
     
                }
            }
            catch (Exception)
            {
                throw;
            }


            return await Task.FromResult(list);
        }

        public async Task<List<Entity>> GetDomainList()
        {
            List<Entity> list = new List<Entity>();

            try
            {
                foreach (var item in TweetList)
                {
                    if (item.data.context_annotations.Count > 0)
                    {
                        var anno = item.data.context_annotations;
                        foreach (var c in anno)
                        {
                            var entity = c.entity;
                            if (string.IsNullOrEmpty(entity.description) == true)
                                entity.description = c.domain.description;

                            list.Add(entity);

                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
    

            return await Task.FromResult(list);
        }

        public async Task<Dictionary<string,int>> GetDomainCounts()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();

            try
            {
                var dList = await GetDomainList();
                var results = dList.GroupBy(d => d.name)
                                .OrderBy(group => group.Key)
                                .Select(group => Tuple.Create(group.Key, group.Count()));

                foreach(var item in results)
                {
                    list.Add(item.Item1, item.Item2);
                }
            }
            catch (Exception)
            {
                throw;
            }


            return await Task.FromResult(list);
        }

        public async Task ResetCache(int sampleSize)
        {
            this.SampleSize = await Task.FromResult(sampleSize);
            this.TweetList.Clear();
        }
        public async Task AddTweet(SampledStreamModel data)
        {
            await Task.Run(() => this.TweetList.Add(data));
        }
        public async Task<int> IsCollectionComplete()
        {
            if (TweetList == null)
                return await Task.FromResult(-1);
            else if (SampleSize <= 0)
                return await Task.FromResult(-1);
            else if (TweetList.Count >= SampleSize)
                return await Task.FromResult(0);
            else
               return await Task.FromResult(SampleSize - TweetList.Count);


        }
        public async Task<int> GetCollectionCount()
        {
            if (TweetList == null)
                return await Task.FromResult(-1);
            else
                return await Task.FromResult(TweetList.Count);
        }

        public async Task<ICollection<SampledStreamModel>> GetCollection()
        {
            return await Task.FromResult(TweetList);
        }
    }
}
