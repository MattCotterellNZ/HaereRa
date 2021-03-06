﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraphQL.Middleware;
using GraphQL.Middleware.Services;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HaereRa.API.DAL;
using HaereRa.API.GraphQL;
using HaereRa.API.Services;
using Microsoft.AspNetCore.Http;

namespace HaereRa.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // For accessing CurrentUser. Must be a Singleton, see https://github.com/aspnet/Hosting/issues/793

            // Add Entity Framework
            services.AddDbContext<HaereRaDbContext>(options => options.UseSqlite("Data Source=data.db"));

            // Add application services
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<IExternalAccountService, ExternalAccountService>();

            // Add GraphQL things
            services.AddScoped<HaereRaQuery>();
            services.AddScoped<HaereRaMutation>();
            services.AddScoped<HaereRaSubscription>();
            services.AddScoped<ISchema, HaereRaSchema>();

            services.AddScoped<IGraphQLService, GraphQLService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Use our GraphQL middleware
            app.UseGraphQL("/GraphQL");

            app.UseGraphiQl("/graphiql");

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
