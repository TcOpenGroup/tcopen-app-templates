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
using x_template_xDataMerge.Rework;
using x_template_xHmi.Blazor.Security;
using x_template_xPlc;
using x_template_xPlcConnector;

namespace x_template_xHmi.Blazor
{
    public class Startup
    {
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

            var roleGroupManager = new RoleGroupManager(groupRepo);
            Roles.Create(roleGroupManager);
            services.AddVortexBlazorSecurity(userRepo,roleGroupManager);
            services.AddTcoCoreExtensions();

            SetUpRepositoriesUsingRavenDb();
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

        private void SetUpRepositoriesUsingRavenDb()
        {
            var ProcessDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "ProcessSettings", "", "");
            IntializeProcessDataRepositoryWithDataExchange(Entry.Plc.MAIN._technology._processSettings, new RavenDbRepository<PlainProcessData>(ProcessDataRepoSettings));

            var TechnologicalDataRepoSettings = new RavenDbRepositorySettings<PlainTechnologyData>(new string[] { Constants.CONNECTION_STRING_DB }, "TechnologySettings", "", "");
            IntializeTechnologyDataRepositoryWithDataExchange(Entry.Plc.MAIN._technology._technologySettings, new RavenDbRepository<PlainTechnologyData>(TechnologicalDataRepoSettings));

            var ReworklDataRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "ReworkSettings", "", "");
            IntializeProcessDataRepositoryWithDataExchange(Entry.Plc.MAIN._technology._reworkSettings, new RavenDbRepository<PlainProcessData>(ReworklDataRepoSettings));

            var Traceability = new RavenDbRepositorySettings<PlainProcessData>(new string[] { Constants.CONNECTION_STRING_DB }, "Traceability", "", "");
            IntializeProcessDataRepositoryWithDataExchange(Entry.Plc.MAIN._technology._processTraceability, new RavenDbRepository<PlainProcessData>(Traceability));
            IntializeProcessDataRepositoryWithDataExchange(Entry.Plc.MAIN._technology._cu00x._processData, new RavenDbRepository<PlainProcessData>(Traceability));

            Rework = new ReworkModel(new RavenDbRepository<PlainProcessData>(ReworklDataRepoSettings), new RavenDbRepository<PlainProcessData>(Traceability));
        }

        public static ReworkModel Rework { get; private set; }

        private static void IntializeProcessDataRepositoryWithDataExchange(ProcessDataManager processData, IRepository<PlainProcessData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            processData.InitializeRepository(repository);
            processData.InitializeRemoteDataExchange(repository);
        }

        private static void IntializeTechnologyDataRepositoryWithDataExchange(TechnologicalDataManager manager, IRepository<PlainTechnologyData> repository)
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            manager.InitializeRepository(repository);
            manager.InitializeRemoteDataExchange(repository);
        }
    }

    
}
