using x_template_xPlc;
using x_template_xPlcConnector;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TcoCore;
using TcOpen.Inxton.RavenDb;
using TcoRepositoryDataSetHandler.Handler;
using TcoRepositoryDataSetHandler;
using x_template_xStatistic.Statistics;
using Raven.Embedded;
using System.Reflection;

namespace x_template_xTests
{
     public class StatisticTests
    {


#if DEBUG
        private const int timeOut = 70000;
        private const string SetIdTest = "ProductionPlanerTest";
        private const int lastReqCount = 4;
        private const string lastRecipeName = "default3";
        private const string digitalCheckErrorDesription = "BoltPresenceInspector_Error";
        private const string analogueCheckErrorDesription = "BoltDimensionPresenceInspector_Error";
        private RepositoryDataSetHandler<StatisticsDataItem> _statisticsDataHandler;
        private RepositoryDataSetHandler<StatisticsConfig> _statisticsConfigHandler;
        private StatisticsDataController _statisticControler;

#else
        private const int timeOut = 70000;
#endif
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
           
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
           EmbeddedServer.Instance.StartServer(new ServerOptions
            {
                DataDirectory = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.FullName, "tmp", "data"),
                AcceptEula = true,
                ServerUrl = "http://127.0.0.1:8080",
            });



        }

        [SetUp]
        public void SetUp()
        {
            _statisticsDataHandler = RepositoryDataSetHandler<StatisticsDataItem>.CreateSet(new RavenDbRepository<EntitySet<StatisticsDataItem>>(new RavenDbRepositorySettings<EntitySet<StatisticsDataItem>>(new string[] { @"http://localhost:8080" }, "Statistics", "", "")));
            _statisticsConfigHandler = RepositoryDataSetHandler<StatisticsConfig>.CreateSet(new RavenDbRepository<EntitySet<StatisticsConfig>>(new RavenDbRepositorySettings<EntitySet<StatisticsConfig>>(new string[] { @"http://localhost:8080" }, "StatisticsConfig", "", "")));


            _statisticsDataHandler.Repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            _statisticsDataHandler.Repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };

            _statisticsConfigHandler.Repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            _statisticsConfigHandler.Repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


            //remove all records
            var records = _statisticsDataHandler.Repository.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
            records.ForEach(p => _statisticsDataHandler.Repository.Delete(p));

            var recordsConfig = _statisticsConfigHandler.Repository.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
            recordsConfig.ForEach(p => _statisticsConfigHandler.Repository.Delete(p));

        


            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };

            var recordsTraceability = TraceabilityRepository.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
            recordsTraceability.ForEach(p => TraceabilityRepository.Delete(p));

            Entry.Plc.MAIN._technology._cu00x._automatTask._dataLoadProcessSettingsByPlaner.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataLoadProcessSettings.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataCreateNew.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataOpen.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataClose.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataFinalize.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._continueRestore.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._loop.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._inspectionResult.Synchron = true;
            Entry.Plc.MAIN._technology._cu00x._automatTask._inspectionDimensionResult.Synchron = 50;
            Entry.Plc.MAIN._technology._cu00x._recurringFails.AnyRecurringFails.Reached.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._recurringFails.AnyRecurringFails.Counter.Synchron = 0;
            Entry.Plc.MAIN._technology._cu00x._recurringFails.SameRecurringFails.Reached.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._recurringFails.SameRecurringFails.Counter.Synchron = 0;
        }


        [Test]
        [Timeout(timeOut)]
        public void  verify_if_all_counters_initializated_and_cleared()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _statisticControler = new StatisticsDataController("StatisticTest", _statisticsDataHandler, _statisticsConfigHandler);

           


            Assert.AreEqual(_statisticControler.StatisticsData.ErrorCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.RecipeCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.ReworkCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.HourCounter.Count(), 24);
            foreach (var item in _statisticControler.StatisticsData.HourCounter)
            {
                Assert.AreEqual(item.Counter.Passed, 0);
                Assert.AreEqual(item.Counter.Failed, 0);

            }
            Assert.AreEqual(_statisticControler.StatisticsData.ThreeShiftPerDayCounter.Count(), 3);
            foreach (var item in _statisticControler.StatisticsData.ThreeShiftPerDayCounter)
            {
                Assert.AreEqual(item.Counter.Passed, 0);
                Assert.AreEqual(item.Counter.Failed, 0);

            }
            Assert.AreEqual(_statisticControler.StatisticsData.TwoShiftPerDayCounter.Count(), 2);
            foreach (var item in _statisticControler.StatisticsData.TwoShiftPerDayCounter)
            {
                Assert.AreEqual(item.Counter.Passed, 0);
                Assert.AreEqual(item.Counter.Failed, 0);

            }
            Assert.AreEqual(_statisticControler.StatisticsData.CarrierCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.EntityTypeCounter.Count(), 0);

            Assert.AreEqual(_statisticControler.StatisticsData.ProductionTrend.Count(), 8);
            foreach (var item in _statisticControler.StatisticsData.ProductionTrend)
            {
                Assert.AreEqual(item.Trend.Passed, 0);
                Assert.AreEqual(item.Trend.Failed, 0);
             

            }




        }


        [Test]
        [Timeout(timeOut)]
        public void verify_nok_entity_count()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _statisticControler = new StatisticsDataController("StatisticTest", _statisticsDataHandler, _statisticsConfigHandler);

                      
            const string testRecipeName = "ABCD";
            const string testErrorName = "SOME ERROR";
            const string testCarrierName = "5432548457984";

            var isEmpty = nameof(cuData.EntityHeader.IsEmpty).Split('.').ToList().Last();
            var isRework = nameof(cuData.EntityHeader.WasReworked).Split('.').ToList().Last();
            var isMaster = nameof(cuData.EntityHeader.IsMaster).Split('.').ToList().Last();
            var isNormal = "Normal";


            _statisticControler.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = testRecipeName , Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 30, Failures = testErrorName }, Carrier = testCarrierName }
                
            });

            Assert.AreEqual(_statisticControler.StatisticsData.RecipeCounter.Count(), 1);
                   
            var item = _statisticControler.StatisticsData.RecipeCounter.FirstOrDefault(p => p.Id == testRecipeName);
            Assert.AreEqual(item.Id, testRecipeName);
            Assert.AreEqual(item.Counter.Passed, 0);
            Assert.AreEqual(item.Counter.Failed, 1);

            Assert.AreEqual(_statisticControler.StatisticsData.ErrorCounter.Count(), 1);
            var errorItem = _statisticControler.StatisticsData.ErrorCounter.FirstOrDefault(p => p.Id == testErrorName);
            Assert.AreEqual(errorItem.Id, testErrorName);
            Assert.AreEqual(errorItem.Counter, 1);

            Assert.AreEqual(_statisticControler.StatisticsData.CarrierCounter.Count(), 1);
            var carrierItem = _statisticControler.StatisticsData.CarrierCounter.FirstOrDefault(p => p.Id == testCarrierName);
            Assert.AreEqual(carrierItem.Id, testCarrierName);
            Assert.AreEqual(carrierItem.Counter.Failed, 1);
            Assert.AreEqual(carrierItem.Counter.Passed, 0);


            Assert.AreEqual(_statisticControler.StatisticsData.EntityTypeCounter.Count(), 1);
            var entityTypeItem = _statisticControler.StatisticsData.EntityTypeCounter.FirstOrDefault(p => p.Id == isNormal);
            Assert.AreEqual(entityTypeItem.Id, isNormal);
            Assert.AreEqual(entityTypeItem.Counter, 1);
            

            Assert.AreEqual(_statisticControler.StatisticsData.ReworkCounter.Count(), 0);

            Assert.AreEqual(_statisticControler.StatisticsData.HourCounter.Count(), 24);
            var countHourPassed = 0;
            var countHourFailed = 0;
            foreach (var hour in _statisticControler.StatisticsData.HourCounter)
            {
                if (hour.Counter.Passed == 1) countHourPassed++;
                if (hour.Counter.Failed == 1) countHourFailed++;

            }
           
            Assert.AreEqual(1,countHourFailed);
            Assert.AreEqual(0,countHourPassed) ;

        }


        [Test]
        [Timeout(timeOut)]
        public void verify_ok_entity_count()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _statisticControler = new StatisticsDataController("StatisticTest", _statisticsDataHandler, _statisticsConfigHandler);


            const string testRecipeName = "ABCD";
            const string testErrorName = "SOME ERROR";
            const string testCarrierName = "5432548457984";

            var isEmpty = nameof(cuData.EntityHeader.IsEmpty).Split('.').ToList().Last();
            var isRework = nameof(cuData.EntityHeader.WasReworked).Split('.').ToList().Last();
            var isMaster = nameof(cuData.EntityHeader.IsMaster).Split('.').ToList().Last();
            var isNormal = "Normal";


            _statisticControler.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = testRecipeName, Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 20, Failures = testErrorName }, Carrier = testCarrierName }

            });

            Assert.AreEqual(_statisticControler.StatisticsData.RecipeCounter.Count(), 1);

            var item = _statisticControler.StatisticsData.RecipeCounter.FirstOrDefault(p => p.Id == testRecipeName);
            Assert.AreEqual(item.Id, testRecipeName);
            Assert.AreEqual(item.Counter.Passed, 1);
            Assert.AreEqual(item.Counter.Failed, 0);

            Assert.AreEqual(_statisticControler.StatisticsData.ErrorCounter.Count(), 0);
            var errorItem = _statisticControler.StatisticsData.ErrorCounter.FirstOrDefault(p => p.Id == testErrorName);
            Assert.AreEqual(errorItem, null);
   
            Assert.AreEqual(_statisticControler.StatisticsData.CarrierCounter.Count(), 1);
            var carrierItem = _statisticControler.StatisticsData.CarrierCounter.FirstOrDefault(p => p.Id == testCarrierName);
            Assert.AreEqual(carrierItem.Id, testCarrierName);
            Assert.AreEqual(carrierItem.Counter.Failed, 0);
            Assert.AreEqual(carrierItem.Counter.Passed, 1);


            Assert.AreEqual(_statisticControler.StatisticsData.EntityTypeCounter.Count(), 1);
            var entityTypeItem = _statisticControler.StatisticsData.EntityTypeCounter.FirstOrDefault(p => p.Id == isNormal);
            Assert.AreEqual(entityTypeItem.Id, isNormal);
            Assert.AreEqual(entityTypeItem.Counter, 1);


            Assert.AreEqual(_statisticControler.StatisticsData.ReworkCounter.Count(), 0);

            Assert.AreEqual(_statisticControler.StatisticsData.HourCounter.Count(), 24);
            var countHourPassed = 0;
            var countHourFailed = 0;
            foreach (var hour in _statisticControler.StatisticsData.HourCounter)
            {
                if (hour.Counter.Passed == 1) countHourPassed++;
                if (hour.Counter.Failed == 1) countHourFailed++;

            }

            Assert.AreEqual(0, countHourFailed);
            Assert.AreEqual(1, countHourPassed);

        }

        [Test]
        [Timeout(timeOut)]
        public void verify_master_entity_count()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _statisticControler = new StatisticsDataController("StatisticTest", _statisticsDataHandler, _statisticsConfigHandler);


            const string testRecipeName = "ABCD";
            const string testErrorName = "SOME ERROR";
            const string testCarrierName = "5432548457984";

            var isEmpty = nameof(cuData.EntityHeader.IsEmpty).Split('.').ToList().Last();
            var isRework = nameof(cuData.EntityHeader.WasReworked).Split('.').ToList().Last();
            var isMaster = nameof(cuData.EntityHeader.IsMaster).Split('.').ToList().Last();
            var isNormal = "Normal";


            _statisticControler.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = testRecipeName, IsMaster = true, Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 20, Failures = testErrorName }, Carrier = testCarrierName }

            });

            Assert.AreEqual(_statisticControler.StatisticsData.ErrorCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.RecipeCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.ReworkCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.HourCounter.Count(), 24);
            foreach (var item in _statisticControler.StatisticsData.HourCounter)
            {
                Assert.AreEqual(item.Counter.Passed, 0);
                Assert.AreEqual(item.Counter.Failed, 0);

            }
            Assert.AreEqual(_statisticControler.StatisticsData.ThreeShiftPerDayCounter.Count(), 3);
            foreach (var item in _statisticControler.StatisticsData.ThreeShiftPerDayCounter)
            {
                Assert.AreEqual(item.Counter.Passed, 0);
                Assert.AreEqual(item.Counter.Failed, 0);

            }
            Assert.AreEqual(_statisticControler.StatisticsData.TwoShiftPerDayCounter.Count(), 2);
            foreach (var item in _statisticControler.StatisticsData.TwoShiftPerDayCounter)
            {
                Assert.AreEqual(item.Counter.Passed, 0);
                Assert.AreEqual(item.Counter.Failed, 0);

            }
            Assert.AreEqual(_statisticControler.StatisticsData.CarrierCounter.Count(), 0);

            Assert.AreEqual(_statisticControler.StatisticsData.ProductionTrend.Count(), 8);
            foreach (var item in _statisticControler.StatisticsData.ProductionTrend)
            {
                Assert.AreEqual(item.Trend.Passed, 0);
                Assert.AreEqual(item.Trend.Failed, 0);


            }

            Assert.AreEqual(_statisticControler.StatisticsData.EntityTypeCounter.Count(), 1);
            var entityTypeItem = _statisticControler.StatisticsData.EntityTypeCounter.FirstOrDefault(p => p.Id == isMaster);
            Assert.AreEqual(entityTypeItem.Id, isMaster);
            Assert.AreEqual(entityTypeItem.Counter, 1);



        }

        [Test]
        [Timeout(timeOut)]
        public void verify_rework_entity_count()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _statisticControler = new StatisticsDataController("StatisticTest", _statisticsDataHandler, _statisticsConfigHandler);


            const string testRecipeName = "ABCD";
            const string testErrorName = "SOME ERROR";
            const string testCarrierName = "5432548457984";

            var isEmpty = nameof(cuData.EntityHeader.IsEmpty).Split('.').ToList().Last();
            var isRework = nameof(cuData.EntityHeader.WasReworked).Split('.').ToList().Last();
            var isMaster = nameof(cuData.EntityHeader.IsMaster).Split('.').ToList().Last();
            var isNormal = "Normal";


            const string testReworkName = "TestReworkName";
            _statisticControler.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = testRecipeName, WasReworked = true, LastReworkName = testReworkName, Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 20, Failures = testErrorName }, Carrier = testCarrierName }

            });

            Assert.AreEqual(_statisticControler.StatisticsData.ErrorCounter.Count(), 0);
            Assert.AreEqual(_statisticControler.StatisticsData.RecipeCounter.Count(), 1);
           
            Assert.AreEqual(_statisticControler.StatisticsData.HourCounter.Count(), 24);
            Assert.AreEqual(_statisticControler.StatisticsData.HourCounter.Count(), 24);
            var countHourPassed = 0;
            var countHourFailed = 0;
            foreach (var hour in _statisticControler.StatisticsData.HourCounter)
            {
                if (hour.Counter.Passed == 1) countHourPassed++;
                if (hour.Counter.Failed == 1) countHourFailed++;

            }

            Assert.AreEqual(0, countHourFailed);
            Assert.AreEqual(1, countHourPassed);


      
            Assert.AreEqual(_statisticControler.StatisticsData.CarrierCounter.Count(), 1);

            Assert.AreEqual(_statisticControler.StatisticsData.ProductionTrend.Count(), 8);
       
         
            Assert.AreEqual(_statisticControler.StatisticsData.ReworkCounter.Count(), 1);
            var reworkTypeItem = _statisticControler.StatisticsData.ReworkCounter.FirstOrDefault(p => p.Id == testReworkName);
            Assert.AreEqual(reworkTypeItem.Id, isRework);
            Assert.AreEqual(reworkTypeItem.Counter, 1);

            Assert.AreEqual(_statisticControler.StatisticsData.EntityTypeCounter.Count(), 1);
            var entityTypeItem = _statisticControler.StatisticsData.EntityTypeCounter.FirstOrDefault(p => p.Id == isRework);
            Assert.AreEqual(entityTypeItem.Id, isRework);
            Assert.AreEqual(entityTypeItem.Counter, 1);

        }

      }
}
