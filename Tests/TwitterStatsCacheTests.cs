using Xunit;
using TwitterStats;
using TwitterStats.Controllers;
using TwitterStats.Services;
using TwitterStats.Services.Interfaces;
using TwitterStats.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using SocialOpinionAPI.Models.SampledStream;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterStats.Tests
{

    public class TwitterStatsCacheTests
    {
        private SampledStreamModel model;
        private List<SampledStreamModel> tweets = new List<SampledStreamModel>();
        private TwitterStatCache sut = new TwitterStatCache();

        public TwitterStatsCacheTests()
        {
            model = Helper.GetMockTwitterData();
            tweets.Add(model);

        }


        [Fact(DisplayName = "Get four domains")]
        public async Task GetFourDomainsFromSampleFeed()
        {
            int expected = 4;

            await sut.AddTweet(model);
            var results = await sut.GetDomainList();

            Assert.IsType<List<Entity>>(results);

            Assert.Equal(expected, results.Count);

        }

        [Fact(DisplayName = "Get one url")]
        public async Task GetOneUrlFromSampleFeed()
        {
            int expected = 1;

            await sut.AddTweet(model);
            var results = await sut.GetUrlList();

            Assert.IsType<List<Url>>(results);

            Assert.Equal(expected, results.Count);

        }
    }
}
