using x_template_xPlc;
using x_template_xPlcConnector;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TcoCore;
using TcOpen.Inxton.RavenDb;
using TcOpen.Inxton.RepositoryDataSet;
using x_template_xProductionPlaner.Planer;

namespace x_template_xTests
{
     public class ProductionPlanerTests
    {
        public RepositoryDataSetHandler<ProductionItem> _productionPlanHandler { get; private set; }
        public ProductionPlanController _productionPlaner { get; private set; }

#if DEBUG
        private const int timeOut = 70000;
        private const string SetIdTest = "ProductionPlanerTest";
        private const int lastReqCount = 4;
        private const string lastRecipeName = "default3";

#else
        private const int timeOut = 70000;
#endif
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            x_template_xApp.Get.KillApp();
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var a = x_template_xApp.Get;

            Entry.Plc.Connector.BuildAndStart();

            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");
            var ProcessSettingsRepository = new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings);
            ProcessSettingsRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            ProcessSettingsRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            Entry.Plc.MAIN._technology._processSettings.InitializeRepository(ProcessSettingsRepository);

            ProcessSettingsRepository.Queryable.Where(p => true).Select(p => p._EntityId).ToList().ForEach(p => ProcessSettingsRepository.Delete(p));


            ProcessSettingsRepository.Create("default1", new PlainProcessData()
            {
                EntityHeader = new PlainEntityHeader()
                {
                    NextStation = (short)eStations.CU00x
                },
                CU00x = new PlainCU00xProcessData()
                {
                    Header = new PlainCuHeader()
                    {
                        NextOnPassed = (short)eStations.ST_1
                    },
                    BoltPresenceInspector = new TcoInspectors.PlainTcoDigitalInspector()
                    {
                        _data = new TcoInspectors.PlainTcoDigitalInspectorData()
                        { RequiredStatus = true, ErrorCode = "102030" }
                    },

                    BoltDimensionPresenceInspector = new TcoInspectors.PlainTcoAnalogueInspector()
                    {
                        _data = new TcoInspectors.PlainTcoAnalogueInspectorData()
                        { RequiredMax = 100, ErrorCode = "102040" }
                    }
                }
            });
            ProcessSettingsRepository.Create("default2", new PlainProcessData()
            {
                EntityHeader = new PlainEntityHeader()
                {
                    NextStation = (short)eStations.CU00x
                },
                CU00x = new PlainCU00xProcessData()
                {
                    Header = new PlainCuHeader()
                    {
                        NextOnPassed = (short)eStations.ST_1
                    },
                    BoltPresenceInspector = new TcoInspectors.PlainTcoDigitalInspector()
                    {
                        _data = new TcoInspectors.PlainTcoDigitalInspectorData()
                        { RequiredStatus = true, ErrorCode = "102030" }
                    },

                    BoltDimensionPresenceInspector = new TcoInspectors.PlainTcoAnalogueInspector()
                    {
                        _data = new TcoInspectors.PlainTcoAnalogueInspectorData()
                        { RequiredMax = 100, ErrorCode = "102040" }
                    }
                }
            });

            ProcessSettingsRepository.Create(lastRecipeName, new PlainProcessData()
            {
                EntityHeader = new PlainEntityHeader()
                {
                    NextStation = (short)eStations.CU00x
                },
                CU00x = new PlainCU00xProcessData()
                {
                    Header = new PlainCuHeader()
                    {
                        NextOnPassed = (short)eStations.ST_1
                    },
                    BoltPresenceInspector = new TcoInspectors.PlainTcoDigitalInspector()
                    {
                        _data = new TcoInspectors.PlainTcoDigitalInspectorData()
                        { RequiredStatus = true, ErrorCode = "102030" }
                    },

                    BoltDimensionPresenceInspector = new TcoInspectors.PlainTcoAnalogueInspector()
                    {
                        _data = new TcoInspectors.PlainTcoAnalogueInspectorData()
                        { RequiredMax = 100, ErrorCode = "102040" }
                    }
                }
            });




        }

        [SetUp]
        public void SetUp()
        {

            _productionPlanHandler = RepositoryDataSetHandler<ProductionItem>.CreateSet(new RavenDbRepository<EntitySet<ProductionItem>>
            (new RavenDbRepositorySettings<EntitySet<ProductionItem>>(new string[] { "http://127.0.0.1:8080" }, "ProductionPlan", "", "")));
            _productionPlanHandler.Repository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            _productionPlanHandler.Repository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


            //remove all records
            var records = _productionPlanHandler.Repository.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
            records.ForEach(p => _productionPlanHandler.Repository.Delete(p));
       

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
        public void  verify_planer_is_empty()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));

            ProductionItem tmp;
            _productionPlaner.RefreshItems(out tmp);


            Assert.AreEqual( _productionPlaner.ProductionPlanEmpty, true);
            Assert.AreEqual(_productionPlaner.CurrentProductionSet.Items.Count(), 0);


        }
        [Test]
        [Timeout(timeOut)]
        public void verify_planer_is_completed()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;



            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");
            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));

           _productionPlaner.CurrentProductionSet.Items.Clear();

            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default1", Description = "Test1", RequiredCount = 3, ActualCount = 0, Status = EnumItemStatus.Done });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default2", Description = "Test2", RequiredCount = 5, ActualCount = 0, Status = EnumItemStatus.Done });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = lastRecipeName, Description = "Test3", RequiredCount = lastReqCount, ActualCount = 0, Status = EnumItemStatus.Done });

            _productionPlaner.SaveDataSet(SetIdTest);

            ProductionItem tmp;
            _productionPlaner.RefreshItems(out tmp);


            Assert.AreEqual(_productionPlaner.ProductionPlanCompleted, true);


        }

        [Test]
        [Timeout(timeOut)]
        public void verify_planer_has_all_recipe_in_list()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;



            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");
            var ProcessSettingsRepository = new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings);
            var countRecipe = ProcessSettingsRepository.Queryable.Where(p => true).Select(p => p._EntityId).Count();

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));

            _productionPlaner.RefreshSourceRecipeList();


            Assert.AreEqual(_productionPlaner.RecipeCollection.Count(), countRecipe);


        }
        [Test]
        [Timeout(timeOut)]
        public void verify_planer_in_auto_is_empty()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettingsByPlaner.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataClose.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._loop.Synchron = true;

            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));
            _productionPlaner.CurrentProductionSet.Items.Clear();
            _productionPlaner.SaveDataSet(SetIdTest);

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (!cu._productionPlaner._productionPlanIsEmpty.Synchron) ;

            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


            var producedPieces = TraceabilityRepository.Queryable.Where(p => p.EntityHeader.Recipe == lastRecipeName).Count();

            ProductionItem tmp = null;
            _productionPlaner.RefreshItems(out tmp);
            Assert.AreEqual(true, cu._productionPlaner._productionPlanIsEmpty.Synchron);
       
            Assert.AreEqual(0, producedPieces);


        }

        [Test]
        [Timeout(timeOut)]
        public void run_planer_in_auto_if_all_are_done()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettingsByPlaner.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataClose.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._loop.Synchron = true;

            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));
            _productionPlaner.CurrentProductionSet.Items.Clear();

            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default1", Description = "Test1", RequiredCount = 3, ActualCount = 0, Status = EnumItemStatus.Done });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default2", Description = "Test2", RequiredCount = 5, ActualCount = 0, Status = EnumItemStatus.Done });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = lastRecipeName, Description = "Test3", RequiredCount = lastReqCount, ActualCount = 0, Status = EnumItemStatus.Done });

            _productionPlaner.SaveDataSet(SetIdTest);

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (!cu._productionPlaner._productonPlanCompleted.Synchron) ;

            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


            var producedPieces = TraceabilityRepository.Queryable.Where(p => p.EntityHeader.Recipe == lastRecipeName).Count();

            ProductionItem tmp = null;
            _productionPlaner.RefreshItems(out tmp);
            Assert.AreEqual(true, cu._productionPlaner._productonPlanCompleted.Synchron);
            foreach (var item in _productionPlaner.CurrentProductionSet.Items)
            {
                Assert.AreEqual(item.Status, EnumItemStatus.Done);


            }
            Assert.AreEqual(0, producedPieces);


        }


        [Test]
        [Timeout(timeOut)]
        public void run_planer_in_auto_if_all_are_none()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettingsByPlaner.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataClose.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._loop.Synchron = true;

            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));
            _productionPlaner.CurrentProductionSet.Items.Clear();

            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default1", Description = "Test1", RequiredCount = 3, ActualCount = 0, Status = EnumItemStatus.None });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default2", Description = "Test2", RequiredCount = 5, ActualCount = 0, Status = EnumItemStatus.None });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = lastRecipeName, Description = "Test3", RequiredCount = lastReqCount, ActualCount = 0, Status = EnumItemStatus.None });

            _productionPlaner.SaveDataSet(SetIdTest);

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (!cu._productionPlaner._productonPlanCompleted.Synchron) ;

            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


            var producedPieces = TraceabilityRepository.Queryable.Where(p => p.EntityHeader.Recipe == lastRecipeName).Count();

            ProductionItem tmp = null;
            _productionPlaner.RefreshItems(out tmp);
            Assert.AreEqual(true, cu._productionPlaner._productonPlanCompleted.Synchron);
            foreach (var item in _productionPlaner.CurrentProductionSet.Items)
            {
                Assert.AreEqual(item.Status, EnumItemStatus.None);


            }
            Assert.AreEqual(0, producedPieces);


        }

        [Test]
        [Timeout(timeOut)]
        public void run_planer_in_auto_if_all_are_skiped()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettingsByPlaner.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataClose.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._loop.Synchron = true;

            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));
            _productionPlaner.CurrentProductionSet.Items.Clear();

            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default1", Description = "Test1", RequiredCount = 3, ActualCount = 0, Status = EnumItemStatus.Skiped });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default2", Description = "Test2", RequiredCount = 5, ActualCount = 0, Status = EnumItemStatus.Skiped });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = lastRecipeName, Description = "Test3", RequiredCount = lastReqCount, ActualCount = 0, Status = EnumItemStatus.Skiped });

            _productionPlaner.SaveDataSet(SetIdTest);

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (!cu._productionPlaner._productonPlanCompleted.Synchron) ;

            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


            var producedPieces = TraceabilityRepository.Queryable.Where(p => p.EntityHeader.Recipe == lastRecipeName).Count();

            ProductionItem tmp = null;
            _productionPlaner.RefreshItems(out tmp);
            Assert.AreEqual(true, cu._productionPlaner._productonPlanCompleted.Synchron);
            foreach (var item in _productionPlaner.CurrentProductionSet.Items)
            {
                Assert.AreEqual(item.Status, EnumItemStatus.Skiped);


            }
            Assert.AreEqual(0, producedPieces);


        }



        [Test]
        [Timeout(timeOut)]
        public void run_planer_in_auto_until_compelted_diferent_recipes()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettingsByPlaner.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataClose.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._loop.Synchron = true;

            var ProcessSettningsRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "ProcessSettings", "", "");

            _productionPlaner = new ProductionPlanController(_productionPlanHandler, SetIdTest, new RavenDbRepository<PlainProcessData>(ProcessSettningsRepoSettings));
            _productionPlaner.CurrentProductionSet.Items.Clear();

            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default1", Description = "Test1", RequiredCount = 3, ActualCount = 0, Status = EnumItemStatus.Required });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = "default2", Description = "Test2", RequiredCount = 5, ActualCount = 0, Status = EnumItemStatus.Required });
            _productionPlaner.CurrentProductionSet.Items.Add(new ProductionItem() { Key = lastRecipeName, Description = "Test3", RequiredCount = lastReqCount, ActualCount = 0, Status = EnumItemStatus.Required });

            _productionPlaner.SaveDataSet(SetIdTest);

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (!cu._productionPlaner._productonPlanCompleted.Synchron) ;

            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };


           var producedPieces = TraceabilityRepository.Queryable.Where(p => p.EntityHeader.Recipe == lastRecipeName).Count();

            ProductionItem tmp = null;
            _productionPlaner.RefreshItems(out tmp);
            Assert.AreEqual(true, cu._productionPlaner._productonPlanCompleted.Synchron);
            foreach (var item in _productionPlaner.CurrentProductionSet.Items)
            {
                Assert.AreEqual(item.ActualCount,item.RequiredCount);
                Assert.AreEqual(item.Status, EnumItemStatus.Done);


            }
            Assert.AreEqual(_productionPlaner.CurrentProductionSet.Items.Last().RequiredCount, producedPieces);


        }
    }
}
