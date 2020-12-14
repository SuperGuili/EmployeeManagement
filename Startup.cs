using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
            options.UseSqlServer(_config.GetConnectionString("EmployeeDbConnection")));


            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;

                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

                options.Filters.Add(new AuthorizeFilter(policy));

            }).AddXmlSerializerFormatters()
              .AddRazorRuntimeCompilation();

            services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = "122886919018-42ca5hglmvhkahp5c19t4h8f33vi2lsm.apps.googleusercontent.com";
                options.ClientSecret = "6l_Jh5coGGeNGDqHSDTaLdiM";
            });

            services.ConfigureApplicationCookie(options =>
           {
               options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
           });

            //Claims Policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy",
                policy => policy.RequireClaim("Delete Role", "true"));

                //options.AddPolicy("EditRolePolicy",
                //policy => policy.RequireClaim("Edit Role", "true"));

                //options.AddPolicy("EditRolePolicy",
                //    policy => policy.RequireAssertion(
                //        context => context.User.IsInRole("Admin") &&
                //        context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                //        context.User.IsInRole("Super Admin")
                //        ));

                options.AddPolicy("EditRolePolicy",
                    policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement())
                    );
                //options.InvokeHandlersAfterFailure = false;

                //Roles Policy
                options.AddPolicy("AdminRolePolicy",
                policy => policy.RequireRole("Admin"));
            });

            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

            }).AddEntityFrameworkStores<AppDbContext>();
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });


            //app.UseRouting();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {                  
            //       await context.Response.WriteAsync("Hello World!!!");
            //    });
            //});
        }
    }
}
