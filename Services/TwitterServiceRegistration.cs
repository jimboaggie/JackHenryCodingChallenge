using TwitterStats.Services.Interfaces;
using TwitterStats.Services.Repository;
using TwitterStats.Services.Repository.Interfaces;
using TwitterStats.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using TwitterStats.Services.Queue;

namespace TwitterStats.Services
{
    public static class TwitterServiceRegistration
    {
        public static void AddTwitterInfrastucture(this IServiceCollection services)
        {
            services.AddSingleton<TwitterStatCache>();
            services.AddSingleton<TwitterStreamService>();

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITwitterRepository, TwitterRepository>();


            services.AddHostedService<QueuedHostedService>();
        }
    }
}
