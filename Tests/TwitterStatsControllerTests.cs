using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TwitterStats;
using TwitterStats.Controllers;
using TwitterStats.Services;
using TwitterStats.Services.Interfaces;
using TwitterStats.Shared.DTO;
using Microsoft.AspNetCore.Mvc;
using TwitterStats.Services.Support;
using SocialOpinionAPI.Models.SampledStream;
using TwitterStats.Services.Queue;

namespace TwitterStats.Tests
{
    public class TwitterStatsControllerTests
    {
        private SampledStreamModel model;
        private List<SampledStreamModel> tweets = new List<SampledStreamModel>();
        private TwitterStatCache sut = new TwitterStatCache();

        public TwitterStatsControllerTests()
        {
            model = Helper.GetMockTwitterData();
            tweets.Add(model);

        }


        [Fact(DisplayName = "Get 4 Domains from Twitter Feed")]
        public async Task Get4DomainsFromSampleTwitterFeed()
        {
            int expected = 4;

            var mockOptions = new OptionsWrapper<AppSettings>(new AppSettings
            {
                TwitterApiKey = "SOMEKEYVALUE",
                TwitterSecret = "SOMESECRET",
                BearerToken = "SOMETOKEN"

            });

            TwitterStatCache cache = new TwitterStatCache();
            cache.SampleSize = 1;
            await cache.AddTweet(model);

            var mockQueue = new Mock<IBackgroundTaskQueue>();

            TwitterStreamService service = new TwitterStreamService(mockOptions, cache, mockQueue.Object);

            var controller = new TwitterStatsController(mockOptions,service,cache);
            var actionResult = controller.GetDomainListFromCollection();
            var resultObject = Helper.GetObjectResultContent<List<Entity>>(actionResult.Result);
            Assert.Equal(expected, resultObject.Count);


        }


    }
}
