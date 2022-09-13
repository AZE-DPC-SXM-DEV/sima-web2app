using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2App.Data;
using Web2App.Hubs;
using Web2App.Interfaces;
using Web2App.Middlewares;
using Web2App.Services;

namespace Web2App
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();

            services.AddSession(option=>
            {
                option.Cookie.IsEssential = true;
                option.Cookie.HttpOnly = true;
            });

            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddTransient<ITsContainerService, TsContainerService>();
            services.AddTransient<IQrGenerator, QrGeneratorService>();
            services.AddHttpContextAccessor();

            services.AddLogging();

            //services.AddSignalR();

            services.AddDbContext<AppDbContext>(option =>
            {
                option.UseSqlServer("Data Source=10.180.10.199;Initial Catalog=Scanme;Persist Security Info=true;User ID=mhmFarid;Password=farid@)@)12345;MultipleActiveResultSets=True");
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });

         

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();


         
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapHub<LogHub>("/logHub");
            });
        }
    }
}
