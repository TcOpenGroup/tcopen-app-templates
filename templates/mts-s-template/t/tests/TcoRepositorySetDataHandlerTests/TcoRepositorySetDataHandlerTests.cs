using NUnit.Framework;
using Raven.Embedded;
using System.IO;
using System.Reflection;
using TcOpen.Inxton.RavenDb;
using TcoRepositorySetDataHandler;
using TcoRepositorySetDataHandler.Handler;

namespace TcoRepositorySetDataHandlerTests
{
    public class TcoRepositorySetDataHandlerTests
    {
        public RepositorySetDataHandler<ProductionItem> _productionPlanHandler { get; private set; }

        [SetUp]
        public void Setup()
        {
             _productionPlanHandler = RepositorySetDataHandler<ProductionItem>.CreateSet(new RavenDbRepository<EntitySet<ProductionItem>>
                (new RavenDbRepositorySettings<EntitySet<ProductionItem>>(new string[] { "http://127.0.0.1:8080" }, "ProductionPlanTest", "", "")));



        }


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

            EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "tmp", "data"),
                AcceptEula = true,
                ServerUrl = "http://127.0.0.1:8080",
            });
        }

        [Test]
        public void create_test_data()
        {
            var setId = "testId";
            

            //todo
            EntitySet<ProductionItem> entitySet =  new EntitySet<ProductionItem>();
           //_productionPlanHandler.Repository.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
           // recordsSource.ForEach(p => repositorySource.Delete(p));



            _productionPlanHandler.Create(setId, entitySet);



            _productionPlanHandler.Repository.Queryable.GetEnumerator().Current.Items.ForEach(p => _productionPlanHandler.Repository.Delete(p));
            Assert.Pass();
        }
    }

    public class ProductionItem : IItemsCollection
    {
        private string key;

        /// <summary>
        /// Gets or sets the key of this instruction item (list of process set).
        /// </summary>
        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                if (key == value)
                {
                    return;
                }

                key = value;

            }
        }

        private int reqCount;

        /// <summary>
        /// Gets or sets required Ccounter value 
        /// </summary>
        public int RequiredCount
        {
            get => reqCount;
            set
            {
                if (reqCount == value)
                {
                    return;
                }

                reqCount = value;

            }
        }

        private int actualCount;
        /// <summary>
        /// Gets or sets actual counter value.
        /// </summary>
        public int ActualCount
        {
            get => actualCount;
            set
            {
                if (actualCount == value)
                {
                    return;
                }

                actualCount = value;
            }
        }

        private string description;


        /// <summary>
        /// gets or sets additional information. 
        /// </summary>
        public string Description
        {
            get => description;
            set
            {
                if (description == value)
                {
                    return;
                }

                description = value;

            }
        }

    








    }
}