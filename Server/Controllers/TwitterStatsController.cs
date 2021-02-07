using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialOpinionAPI.Models.SampledStream;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStats.Services;
using TwitterStats.Services.Support;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterStats.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TwitterStatsController : ControllerBase
    {


        private readonly AppSettings appSettings;
        private readonly TwitterStreamService twitterStreamService;
        private readonly TwitterStatCache cache;


        public TwitterStatsController(IOptions<AppSettings> appSettings, TwitterStreamService twitterStreamService, TwitterStatCache cache)
        {
            this.appSettings = appSettings.Value;
            this.twitterStreamService = twitterStreamService;
            this.cache = cache;
        }

        /// <summary>
        /// Starts a Twitter Stream Sampling
        /// </summary>
        /// <param name="sampleSize">configures the stream to stop at the provided sample size</param>
        /// <returns>Twitter Stream Started</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("StartStreaming")]
        [HttpGet]
        public async Task<ActionResult<string>> StartStreaming([FromQuery] int sampleSize)
        {
            await Task.Run(() => twitterStreamService.StartStreaming(sampleSize)).ConfigureAwait(false);
            return Ok("Twitter Stream Started");
        }

        /// <summary>
        /// Get the Status of the Twitter Stream Collection Process
        /// </summary>
        /// <returns>a string indicating the state of the collection process</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("GetSampleCollectionStatus")]
        [HttpGet]
        public async Task<ActionResult<string>> GetSampleCollectionStatus()
        {
            int isCompleteResult = await cache.IsCollectionComplete();
            if (isCompleteResult < 0)
            {
                return Ok("Tweet Sample collection not initialized");
            }
            else if (isCompleteResult == 0)
            {
                return Ok("Tweet Sample collection completed");
            }
            else
            {
                return Ok(string.Format("Tweet Sample collection in progress: {0} items remaining", isCompleteResult));
            }

        }
        /// <summary>
        /// Gets the sample Twitter data collected.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(List<SampledStreamModel>))]
        [Route("GetSampleCollection")]
        [HttpGet]
        public async Task<ActionResult<List<SampledStreamModel>>> GetSampleCollection()
        {
            List<SampledStreamModel> list = new List<SampledStreamModel>();

            try
            {
                var result = await cache.GetCollection();
                if(result == null)
                    return NotFound("Null Data");
                else
                    return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of url objects from the sample collection
        /// </summary>
        /// <returns>A list of Twitter Url Entities</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(List<Url>))]
        [Route("GetUrlListFromCollection")]
        [HttpGet]
        public async Task<ActionResult<List<Url>>> GetUrlListFromCollection()
        {
            List<Url> list = new List<Url>();

            try
            {
                var result = await cache.GetUrlList();
                if (result == null)
                    return NotFound("Null Data");
                else
                    return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of domain entites from the sample collection
        /// </summary>
        /// <returns>A list of Twitter Domain Entities</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(List<Entity>))]
        [Route("GetDomainListFromCollection")]
        [HttpGet]
        public async Task<ActionResult<List<Entity>>> GetDomainListFromCollection()
        {
            List<Entity> list = new List<Entity>();

            try
            {
                var result = await cache.GetDomainList();
                if (result == null)
                    return NotFound("Null Data");
                else
                    return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets a list of domains and the count of their occurances from a Twitter sample collection
        /// </summary>
        /// <returns>A list of Twitter Domain Entry Counts</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(typeof(Dictionary<string, int>))]
        [Route("GetDomainCounts")]
        [HttpGet]
        public async Task<ActionResult<Dictionary<string,int>>> GetDomainCounts()
        {
            Dictionary<string, int> list = new Dictionary<string, int>();

            try
            {
                var result = await cache.GetDomainCounts();
                if (result == null)
                    return NotFound("Null Data");
                else
                    return Ok(result.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
