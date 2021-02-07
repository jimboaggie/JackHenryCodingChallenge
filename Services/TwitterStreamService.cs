using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using TwitterStats.Services.Support;
using TwitterStats.Services.Queue;
using SocialOpinionAPI.Services.SampledStream;
using SocialOpinionAPI.Models.SampledStream;
using SocialOpinionAPI.Core;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TwitterStats.Services
{
    public class TwitterStreamService 
    {
        private readonly AppSettings appSettings;
        private TwitterStatCache cache;
        private IBackgroundTaskQueue queue;
        private OAuthInfo oAuthInfo;



        public TwitterStreamService(IOptions<AppSettings> appSettings, TwitterStatCache cache , IBackgroundTaskQueue queue)
        {
            this.appSettings = appSettings.Value;
            this.cache = cache;
            this.queue = queue;
            oAuthInfo = new OAuthInfo { ConsumerKey = appSettings.Value.TwitterApiKey, ConsumerSecret = appSettings.Value.TwitterSecret };

     
        }

        public async Task StartStreaming(int sampleSize)
        {

            Log.Information($"TwitterStreamService.StartStreaming {DateTime.Now:hh:mm:ss}");
            await cache.ResetCache(sampleSize);

            try
            {
                SampledStreamService streamService = new SampledStreamService(oAuthInfo, this.appSettings.BearerToken);
                streamService.DataReceivedEvent += StreamService_DataReceivedEvent;
                streamService.StartStream("https://api.twitter.com/2/tweets/sample/stream?expansions=author_id,attachments.media_keys&tweet.fields=public_metrics,attachments,context_annotations,entities&media.fields=duration_ms,height,media_key,preview_image_url,type,url,width,public_metrics", sampleSize, 1);

            }
            catch (Exception)
            {

                throw;
            }


        }

        private  void StreamService_DataReceivedEvent(object sender, EventArgs e)
        {

            try
            {

                SampledStreamService.DataReceivedEventArgs eventArgs = e as SampledStreamService.DataReceivedEventArgs;
                SampledStreamModel model = eventArgs.StreamDataResponse;
                Task.Run(() => cache.AddTweet(model));

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
