using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Raven.Embedded;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.Local.Security.Blazor;
using TcOpen.Inxton.Local.Security.Blazor.Services;
using TcOpen.Inxton.RavenDb;
using TcOpen.Inxton.TcoCore.Blazor.Extensions;
using Vortex.Presentation.Blazor.Services;
using x_template_xHmi.Blazor.Security;
using x_template_xPlcConnector;

namespace x_template_xHmi.Blazor
{
    public class Startup
    {
        private BlazorRoleGroupManager roleGroupManager;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            StartRavenDBEmbeddedServer();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddVortexBlazorServices();


            var userRepo = new RavenDbRepository<UserData>(
                                           new RavenDbRepositorySettings<UserData>
                                           (new string[] { Constants.CONNECTION_STRING_DB }, "UsersBlazor", "", ""));
            var groupRepo = new RavenDbRepository<GroupData>(
                                          new RavenDbRepositorySettings<GroupData>
                                          (new string[] { Constants.CONNECTION_STRING_DB }, "GroupsBlazor", "", ""));

            var roleGroupManager = new BlazorRoleGroupManager(groupRepo);
            Roles.Create(roleGroupManager);
            services.AddVortexBlazorSecurity(userRepo, groupRepo, roleGroupManager);
            services.AddTcoCoreExtensions();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            Entry.Plc.Connector.BuildAndStart();
           
        }


        private static async void StartRavenDBEmbeddedServer()
        {
            // Start embedded RavenDB server

            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine("Starting embedded RavenDB server instance. " +
                "\nYou should not use this instance in production. " +
                "\nUsing embedded RavenDB server you agree to the respective EULA." +
                "\nYou will need to register the licence." +
                "\nThe data are strored in temporary 'bin' folder of your application, " +
                "\nif you want to persist your data safely redirect the DataDirectory into different location.");
            Console.WriteLine("---------------------------------------------------");
            
            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                FrameworkVersion = null,
                DataDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "tmp", "data"),
                AcceptEula = true,
                ServerUrl = "http://127.0.0.1:8080",
              
            });


           // await EmbeddedServer.Instance.RestartServerAsync();
            // Uri url = await EmbeddedServer.Instance.GetServerUriAsync();
            //EmbeddedServer.Instance.OpenStudioInBrowser();
        }
    }

    
}
