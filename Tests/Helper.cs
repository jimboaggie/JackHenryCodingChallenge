using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialOpinionAPI.DTO;
using SocialOpinionAPI.Models;
using SocialOpinionAPI.Models.SampledStream;

namespace TwitterStats.Tests
{
    public class Helper
    {

        public static SampledStreamModel GetMockTwitterData()
        {

            var jsonString = File.ReadAllText(@".\TestData\SampleTwitter.json");
            var model = JsonSerializer.Deserialize<SampledStreamModel>(jsonString);

            return model;
        }


        public static T GetObjectResultContent<T>(ActionResult<T> result)
        {
            return (T)((ObjectResult)result.Result).Value;
        }
    }
}
