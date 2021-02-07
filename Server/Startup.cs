using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStats.Services;
using TwitterStats.Services.Support;
using TwitterStats.Services.Interfaces;
using TwitterStats.Services.Repository;
using TwitterStats.Services.Repository.Interfaces;
using TwitterStats.Services.Queue;
using SocialOpinionAPI.DTO;
using SocialOpinionAPI.DTO.SampledStream;
using SocialOpinionAPI.Models.SampledStream;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.OpenApi.Models;
using System.IO;

namespace TwitterStats
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this._env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            
            //services.AddServerSideBlazor();

            services.AddServerSideBlazor().AddCircuitOptions(o =>
            {
                o.DetailedErrors = true;

                //if (env.IsDevelopment()) //only add details when debugging
                //{
                //    o.DetailedErrors = true;
                //}
            });
            services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pages");

            services.AddMvc();

            services.AddLogging();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SampledStreamDTO, SampledStreamModel>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Annotation, SocialOpinionAPI.Models.SampledStream.Annotation>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.ContextAnnotation, SocialOpinionAPI.Models.SampledStream.ContextAnnotation>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Data, SocialOpinionAPI.Models.SampledStream.Data>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Domain, SocialOpinionAPI.Models.SampledStream.Domain>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Entities, SocialOpinionAPI.Models.SampledStream.Entities>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Entity, SocialOpinionAPI.Models.SampledStream.Entity>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.ReferencedTweet, SocialOpinionAPI.Models.SampledStream.ReferencedTweet>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Stats, SocialOpinionAPI.Models.SampledStream.Stats>();
                cfg.CreateMap<SocialOpinionAPI.DTO.SampledStream.Url, SocialOpinionAPI.Models.SampledStream.Url>();
            });

            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            services.AddTwitterInfrastucture();

            if (_env.IsDevelopment())
            {
                AppSettings s = new AppSettings();
                Configuration.GetSection("TwitterSecrets").Bind(s);
                if ((string.IsNullOrEmpty(s.TwitterApiKey) == true) ||
                    (string.IsNullOrEmpty(s.TwitterSecret) == true) ||
                    (string.IsNullOrEmpty(s.BearerToken) == true))
                {
                    services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
                }
                else
                {
                    services.Configure<AppSettings>(options => Configuration.GetSection("TwitterSecrets").Bind(options));
                }
            }
            else
            {
                services.Configure<AppSettings>(options => Configuration.GetSection("AppSettings").Bind(options));
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Twitter Analytics API", Version = "v1" });
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("Logs/twitterlog.txt")
            .CreateLogger();

            Log.Information(string.Format("Start...{0}", env.EnvironmentName));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("../swagger/v1/swagger.json", "Twitter Analytics API"));



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors(x => x
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");

            });

        }
    }
}
