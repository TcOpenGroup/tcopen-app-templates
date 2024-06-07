using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Data.MongoDb;
using TcOpen.Inxton.Local.Security;
using TcOpen.Inxton.RavenDb;
using TcOpen.Inxton.Security;
using x_template_xPlc;

namespace x_template_xPlcConnector
{
    public class RepositoryEntry

    {
        /// <summary>
        /// Initializes <see cref="ProcessDataManager"/>s or Initializes <see cref="TechnologicalDataManager"/>s repository for data exchange between PLC and storage (database).
        /// </summary>
        /// <typeparam name="T1">Type of manager</typeparam>
        /// <typeparam name="T2">Type of reposistory</typeparam>
        /// <param name="manager">Data manager</param>
        /// <param name="repository">Repository</param>
        /// <param name="withDataExchange">Define if data are  exchanged between PLC and storage (database)</param>
        public static void InitializeRepository<T1, T2>(T1 manager, IRepository<T2> repository, bool withDataExchange) where T1 : TcoData.TcoDataExchange
                                                                                                                      where T2 : PlainTcoEntityBase
        {
            repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            manager.InitializeRepository(repository);
            if (withDataExchange)
            {
                manager.InitializeRemoteDataExchange(repository);
            }

        }



        public static IAuthenticationService CreateSecurityManageUsingRavenDb(bool withDefaultGroups, bool withDefaultRoles)
        {

            var users = new RavenDbRepository<UserData>(new RavenDbRepositorySettings<UserData>(new string[] { Entry.Settings.GetConnectionString() }, "Users", "", ""));
            var groups = new RavenDbRepository<GroupData>(new RavenDbRepositorySettings<GroupData>(new string[] { Entry.Settings.GetConnectionString() }, "Groups", "", ""));

            var roleGroupManager = new RoleGroupManager(groups);
          
            var manager = SecurityManager.Create(users, roleGroupManager);

            if (withDefaultGroups)
            {
                DefaultGroups.Create();
            }
            if (withDefaultRoles)
            {
                DefaultRoles.Create();
            }


            return manager;

        }

        public static IAuthenticationService CreateSecurityManageUsingMongoDb(bool withDefaultGroups,bool withDefaultUsers)
        {

            var users = new MongoDbRepository<UserData>(new MongoDbRepositorySettings<UserData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Users"));   
            var groups = new MongoDbRepository<GroupData>(new MongoDbRepositorySettings<GroupData>(Entry.Settings.GetConnectionString(), Entry.Settings.DbName, "Groups"));
            var roleGroupManager = new RoleGroupManager(groups);
            
            var manager = SecurityManager.Create(users, roleGroupManager);
          
            if (withDefaultGroups)
            {
                // create only once 
                var con = groups.Count;
                if (groups.Count <= 1)
                {
                    DefaultGroups.Create();
                    DefaultRoles.Create();

                }
            }

            if (withDefaultUsers)
            {
                if (users.Count<=1)
                {
                    DefaultUsers.Create();
        
                }
            }

            return manager;

        }

        private List<UserData> GetAllUsers(IRepository<UserData> repo) => repo.GetRecords("*", Convert.ToInt32(repo.Count + 1), 0).ToList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mongoDbRepositorySettings"></param>
        public static void InitializeIndexProcessDataRepositoryMongoDb(MongoDbRepositorySettings<PlainProcessData> mongoDbRepositorySettings) 
        {

            var indexes = mongoDbRepositorySettings.Collection.Indexes.List().ToList();
            var name = "_EntityId";
            if (!indexes.Exists(i => i.GetElement("name").ToString().Contains(name)))
            {
                var indexOptions = new CreateIndexOptions();
                indexOptions.Name = name;
                var indexKey = Builders<PlainProcessData>.IndexKeys.Descending(p => p._EntityId);
                mongoDbRepositorySettings.Collection.Indexes.CreateOne(new CreateIndexModel<PlainProcessData>(indexKey, indexOptions));

            }

            name = "_Created";
            if (!indexes.Exists(i => i.GetElement("name").ToString().Contains(name)))
            {
                var indexOptions = new CreateIndexOptions();
                indexOptions.Name = name;
                var indexKey = Builders<PlainProcessData>.IndexKeys.Descending(p => p._Created);
                mongoDbRepositorySettings.Collection.Indexes.CreateOne(new CreateIndexModel<PlainProcessData>(indexKey, indexOptions));

            }
            name = "_Modified";
            if (!indexes.Exists(i => i.GetElement("name").ToString().Contains(name)))
            {
                var indexOptions = new CreateIndexOptions();
                indexOptions.Name = name;
                var indexKey = Builders<PlainProcessData>.IndexKeys.Descending(p => p._Modified);
                mongoDbRepositorySettings.Collection.Indexes.CreateOne(new CreateIndexModel<PlainProcessData>(indexKey, indexOptions));

            }
        }
    }
}
