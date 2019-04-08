using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DatingApp.API
{
    public class Startup
    {
        private const string CLIENT_URL = "http://localhost:4200";  // TODO: Obtain this from Configuration

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        readonly string CorsAllowSpecificOriginPolicyName ="_corsAllowSpecificOriginPolicyName";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(dbContext => dbContext.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            services.AddCors(options => {
                options.AddPolicy(CorsAllowSpecificOriginPolicyName, builder => {
                    builder.WithOrigins(CLIENT_URL); // TODO: Obtain this from Configuration
                });
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<IAuthService, AuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // else
            // {
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     app.UseHsts();
            // }

            // app.UseCors(x => x.AllowAnyOrigin()
            //                   .AllowAnyHeader()
            //                   .AllowAnyMethod());
            app.UseCors(this.CorsAllowSpecificOriginPolicyName);
            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
