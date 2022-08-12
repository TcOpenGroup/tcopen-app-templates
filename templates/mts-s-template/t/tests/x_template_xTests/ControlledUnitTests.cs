using x_template_xPlc;
using x_template_xPlcConnector;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TcoCore;
using TcOpen.Inxton.RavenDb;

namespace x_template_xTests
{
    public class ControlledUnitTests
    {
#if DEBUG
        private const int timeOut = 70000;
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
            //Entry.Plc.MAIN._technology._processSettings.InitializeRemoteDataExchange(ProcessSettingsRepository);


            if (!ProcessSettingsRepository.Queryable.Where(p => p._EntityId == "default").Any())
            {
                ProcessSettingsRepository.Create("default", new PlainProcessData()
                {
                    CU00x = new PlainCU00xProcessData()
                    {
                        BoltPresenceInspector = new TcoInspectors.PlainTcoDigitalInspector()
                        {
                            _data = new TcoInspectors.PlainTcoDigitalInspectorData()
                            { RequiredStatus = true }
                        }
                    }
                });
            }

            var TraceabilityRepoSettings = new RavenDbRepositorySettings<PlainProcessData>(new string[] { @"http://localhost:8080" }, "Traceability", "", "");
            var TraceabilityRepository = new RavenDbRepository<PlainProcessData>(TraceabilityRepoSettings);
            TraceabilityRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; data.qlikId = id; };
            TraceabilityRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            Entry.Plc.MAIN._technology._cu00x._processData.InitializeRepository(TraceabilityRepository);
            //Entry.Plc.MAIN._technology._cu00x._processData.InitializeRemoteDataExchange(TraceabilityRepository);

            //var a= Entry.Plc.MAIN._technology._cu00x._automatTask._task._enabled.Cyclic && Entry.Plc.MAIN._technology._cu00x._automatTask._task._isServiceable.Cyclic;
            //var b = Entry.Plc.MAIN._technology._cu00x._groundTask._task._enabled.Cyclic && Entry.Plc.MAIN._technology._cu00x._groundTask._task._isServiceable.Cyclic;
            //var c = Entry.Plc.MAIN._technology._cu00x._manualTask._enabled.Cyclic && Entry.Plc.MAIN._technology._cu00x._manualTask._isServiceable.Cyclic;



            var TechnologicalSettningsRepoSettings = new RavenDbRepositorySettings<PlainTechnologyData>(new string[] { @"http://localhost:8080" }, "TechnologySettings", "", "");
            var TechnologicalSettingsRepository = new RavenDbRepository<PlainTechnologyData>(TechnologicalSettningsRepoSettings);
            TechnologicalSettingsRepository.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            TechnologicalSettingsRepository.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            Entry.Plc.MAIN._technology._technologySettings.InitializeRepository(TechnologicalSettingsRepository);

            if (!TechnologicalSettingsRepository.Queryable.Where(p => p._EntityId == "default").Any())
            {
                TechnologicalSettingsRepository.Create("default", new PlainTechnologyData()
                {
                    CU00x = new PlainCU00xTechnologicalData()
                    {

                        ArbitrarilyRecurringFailure = 5,
                        SameRecurringFailure = 3
                    }
                });
            }
        }

        [SetUp]
        public void SetUp()
        {
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataLoadProcessSettings.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataCreateNew.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataOpen.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataClose.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._dataFinalize.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._continueRestore.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._loop.Synchron = false;
            Entry.Plc.MAIN._technology._cu00x._automatTask._inspectionResult.Synchron = true;
        }

        [Test]
        [Timeout(timeOut)]
        public void run_manual_mode()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            cu._manualTask.Execute();
            System.Threading.Thread.Sleep(1000);
            Assert.AreEqual(eTaskState.Ready, (eTaskState)cu._groundTask._task._taskState.Synchron);
            Assert.AreEqual(eTaskState.Ready, (eTaskState)cu._automatTask._task._taskState.Synchron);
        }

        [Test]
        [Timeout(timeOut)]
        public void run_ground_mode()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            cu._manualTask.Execute(); // Reset other tasks
            cu._groundTask._task.Execute();


            //            Assert.AreEqual(eTaskState.Ready, (eTaskState)cu._manualTask._taskState.Synchron);
            //            Assert.AreEqual(eTaskState.Ready, (eTaskState)cu._automatTask._task._taskState.Synchron);
            System.Threading.Thread.Sleep(100);
            Assert.AreEqual(eTaskState.Busy, (eTaskState)cu._groundTask._task._taskState.Synchron);
            while ((eTaskState)cu._groundTask._task._taskState.Synchron == eTaskState.Busy) ;
            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;
            Assert.AreEqual(eTaskState.Done, (eTaskState)cu._groundTask._task._taskState.Synchron);

        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_ground_not_done()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;
            cu._manualTask.Execute(); // Reset other tasks

            cu._automatTask._task.Execute();

            System.Threading.Thread.Sleep(250);

            Assert.AreEqual(eTaskState.Ready, (eTaskState)cu._automatTask._task._taskState.Synchron);
        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_ground_done()
        {
            var cu = Entry.Plc.MAIN._technology._cu00x;

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) cu._manualTask.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) cu._groundTask._task.Execute();

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            Assert.AreEqual(eTaskState.Busy, (eTaskState)cu._automatTask._task._taskState.Synchron);
        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_load_process_data()
        {
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;
            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            automat._dataLoadProcessSettings.Synchron = true;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 32766) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.NoAction, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
#if NET5_0_OR_GREATER
            Assert.AreEqual(rec._Created.ToString().Substring(0, 19), cuData.EntityHeader.RecipeCreated.Synchron.AddHours(-1).ToString().Substring(0, 19));
            Assert.AreEqual(rec._Modified.ToString().Substring(0, 19), cuData.EntityHeader.RecipeLastModified.Synchron.AddHours(-1).ToString().Substring(0, 19));
#else
            Assert.AreEqual(rec._Created.ToString().Substring(0, 19), cuData.EntityHeader.RecipeCreated.Synchron.ToString().Substring(0, 19));
            Assert.AreEqual(rec._Modified.ToString().Substring(0, 19), cuData.EntityHeader.RecipeLastModified.Synchron.ToString().Substring(0, 19));
#endif
        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_load_create_new_entity()
        {
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks


            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;



            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) cu._groundTask._task.Execute();


            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();


            while (cu._automatTask._currentStep.ID.Synchron != 32766) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Console.WriteLine(rec._Created.ToString().Substring(0, 19));
            Console.WriteLine(cuData.EntityHeader.RecipeCreated.Synchron.ToString().Substring(0, 19));
#if NET5_0_OR_GREATER
            Assert.AreEqual(rec._Created.ToString().Substring(0, 19), cuData.EntityHeader.RecipeCreated.Synchron.AddHours(-1).ToString().Substring(0, 19));
            Assert.AreEqual(rec._Modified.ToString().Substring(0, 19), cuData.EntityHeader.RecipeLastModified.Synchron.AddHours(-1).ToString().Substring(0, 19));
#else
            Assert.AreEqual(rec._Created.ToString().Substring(0, 19), cuData.EntityHeader.RecipeCreated.Synchron.ToString().Substring(0, 19));
            Assert.AreEqual(rec._Modified.ToString().Substring(0, 19), cuData.EntityHeader.RecipeLastModified.Synchron.ToString().Substring(0, 19));
#endif
        }


        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_load_open_entity()
        {
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Busy) cu._groundTask._task.Execute();

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 32766) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.InProgress, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(cu._cuId.Synchron, cuData.EntityHeader.NextStation.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.InProgress, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(x_template_xPlc.eStations.CU00x, (x_template_xPlc.eStations)cuData.EntityHeader.OpenOn.Synchron);
            Assert.AreEqual(false, cuData.EntityHeader.WasReset.Synchron);
        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_load_open_close_entity()
        {
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            rec.CU00x.BoltPresenceInspector._data.RequiredStatus = false;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataClose.Synchron = true;
            automat._continueRestore.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 10000) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.InProgress, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(rec.CU00x.Header.NextOnPassed, cuData.EntityHeader.NextStation.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.InProgress, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(x_template_xPlc.eStations.NONE, (x_template_xPlc.eStations)cuData.EntityHeader.OpenOn.Synchron);
            Assert.AreEqual(false, cuData.EntityHeader.WasReset.Synchron);
        }


        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_load_open_finalize_entity()
        {
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._dataFinalize.Synchron = true;
            automat._continueRestore.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;



            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) cu._groundTask._task.Execute();

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 10000) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.Passed, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(rec.CU00x.Header.NextOnPassed, cuData.EntityHeader.NextStation.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.Passed, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(x_template_xPlc.eStations.NONE, (x_template_xPlc.eStations)cuData.EntityHeader.OpenOn.Synchron);
            Assert.AreEqual(false, cuData.EntityHeader.WasReset.Synchron);
        }

        [Test]
        [Timeout(timeOut)]
        public void run_automat_mode_load_open_entity_then_ground()
        {
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;



            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) cu._groundTask._task.Execute();

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 32766) ;


            cu._groundTask._task.Execute();

            while (cu._groundTask._currentStep.ID.Synchron != 10000) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Assert.AreEqual(true, cuData.EntityHeader.WasReset.Synchron);
            Assert.AreEqual(TcoInspectors.eOverallResult.Failed, (TcoInspectors.eOverallResult)cuData.EntityHeader.Results.Result.Synchron);
            Assert.AreEqual(rec.CU00x.Header.NextOnFailed, cuData.EntityHeader.NextStation.Synchron);
            Assert.AreEqual(x_template_xPlc.eStations.CU00x, (x_template_xPlc.eStations)cuData.EntityHeader.OpenOn.Synchron);
        }


        [Test]
        [Repeat(5)]
        public void run_automat_mode_load_create_new_entity_many_times()
        {
            return;
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks


            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            System.Threading.Thread.Sleep(250);

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done || !cu._groundTask._groundDone.Synchron) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 32766) ;

            Assert.AreEqual(rec._EntityId, cuData.EntityHeader.Recipe.Synchron);
            Console.WriteLine(rec._Created.ToString().Substring(0, 19));
            Console.WriteLine(cuData.EntityHeader.RecipeCreated.Synchron.ToString().Substring(0, 19));
#if NET5_0_OR_GREATER
            Assert.AreEqual(rec._Created.ToString().Substring(0, 19), cuData.EntityHeader.RecipeCreated.Synchron.AddHours(-1).ToString().Substring(0, 19));
            Assert.AreEqual(rec._Modified.ToString().Substring(0, 19), cuData.EntityHeader.RecipeLastModified.Synchron.AddHours(-1).ToString().Substring(0, 19));
#else
            Assert.AreEqual(rec._Created.ToString().Substring(0, 19), cuData.EntityHeader.RecipeCreated.Synchron.ToString().Substring(0, 19));
            Assert.AreEqual(rec._Modified.ToString().Substring(0, 19), cuData.EntityHeader.RecipeLastModified.Synchron.ToString().Substring(0, 19));
#endif
        }


        [Test]
        [Repeat(3)]
        public void run_automat_mode_monitor_recurring_fails_off_entity_ok()
        {
            var recTech = Entry.Plc.MAIN._technology._technologySettings.GetRepository<PlainTechnologyData>().Read("default");
            //no monitor if are equal zero
            recTech.CU00x.ArbitrarilyRecurringFailure = 0;
            recTech.CU00x.SameRecurringFailure = 0;

            recTech.CopyPlainToCyclic(Entry.Plc.MAIN._technology._technologySettings._data);
            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");

            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._continueRestore.Synchron = true;

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 30000) ;

            Assert.AreEqual(false, cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron);
            Assert.AreEqual(cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron, cu._recurringFails.SameRecurringFails.Reached.Synchron);

        }

        [Test]
        [Repeat(3)]
        public void run_automat_mode_monitor_recurring_fails_off_entity_nok()
        {

            var recTech = Entry.Plc.MAIN._technology._technologySettings.GetRepository<PlainTechnologyData>().Read("default");
            //no monitor if are equal zero
            recTech.CU00x.ArbitrarilyRecurringFailure = 0;
            recTech.CU00x.SameRecurringFailure = 0;

            recTech.CopyPlainToCyclic(Entry.Plc.MAIN._technology._technologySettings._data);


            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");
            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;

            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._inspectionResult.Synchron = false;

            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 30000) ;

            Assert.AreEqual(false, cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron);
            Assert.AreEqual(cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron, cu._recurringFails.SameRecurringFails.Reached.Synchron);

        }

        [Test]
        [Repeat(3)]
        public void run_automat_mode_monitor_recurring_fails_on_entity_ok()
        {
            var recTech = Entry.Plc.MAIN._technology._technologySettings.GetRepository<PlainTechnologyData>().Read("default");
            //no monitor if are equal zero
            recTech.CU00x.ArbitrarilyRecurringFailure = 3;
            recTech.CU00x.SameRecurringFailure = 3;

            recTech.CopyPlainToCyclic(Entry.Plc.MAIN._technology._technologySettings._data);

            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");

            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._continueRestore.Synchron = true;



            cu._manualTask.Execute(); // Reset other tasks

            while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

            cu._groundTask._task.Execute();

            while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

            while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

            while (cu._automatTask._currentStep.ID.Synchron != 30000) ;

            Assert.AreEqual(false, cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron);
            Assert.AreEqual(cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron, cu._recurringFails.SameRecurringFails.Reached.Synchron);

        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void run_automat_mode_monitor_arbitrary_recurring_fails_on(int noOfFails)
        {
            var recTech = Entry.Plc.MAIN._technology._technologySettings.GetRepository<PlainTechnologyData>().Read("default");
            //no monitor if are equal zero
            recTech.CU00x.ArbitrarilyRecurringFailure =(ushort)noOfFails;
            recTech.CU00x.SameRecurringFailure = 0;

            recTech.CopyPlainToCyclic(Entry.Plc.MAIN._technology._technologySettings._data);

            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");

            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._inspectionResult.Synchron = false;

            cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron = false;
            cu._recurringFails.ArbitrarilyRecurringFails.Counter.Synchron = 0;
            cu._recurringFails.SameRecurringFails.Reached.Synchron = false;
            cu._recurringFails.SameRecurringFails.Counter.Synchron = 0;

            foreach (var item in Enumerable.Range(1,noOfFails))
            {
                cu._manualTask.Execute();  //Reset other tasks

                while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

                cu._groundTask._task.Execute();

                while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

                while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

                if (item < noOfFails)
                {
                    while (cu._automatTask._currentStep.ID.Synchron != 30000) ;
                    Assert.AreEqual(false, cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron);
                }
                else
                {
                    while (cu._automatTask._currentStep.ID.Synchron != 22000) ;
                  
                }
                  
            }
            Assert.AreEqual(true, cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron);
        
        }





        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void run_automat_mode_monitor_same_recurring_fails_on(int noOfFails)
        {
            var recTech = Entry.Plc.MAIN._technology._technologySettings.GetRepository<PlainTechnologyData>().Read("default");
            //no monitor if are equal zero
            recTech.CU00x.ArbitrarilyRecurringFailure = 0;
            recTech.CU00x.SameRecurringFailure = (ushort)noOfFails;

            recTech.CopyPlainToCyclic(Entry.Plc.MAIN._technology._technologySettings._data);

            var rec = Entry.Plc.MAIN._technology._processSettings.GetRepository<PlainProcessData>().Read("default");

            var data = Entry.Plc.MAIN._technology._processSettings._data;
            data._EntityId.Synchron = "default";
            var cu = Entry.Plc.MAIN._technology._cu00x;
            var automat = cu._automatTask;
            var cuData = cu._processData._data;


            automat._dataLoadProcessSettings.Synchron = true;
            automat._dataCreateNew.Synchron = true;
            automat._dataOpen.Synchron = true;
            automat._continueRestore.Synchron = true;
            automat._inspectionResult.Synchron = false;

            cu._recurringFails.ArbitrarilyRecurringFails.Reached.Synchron = false;
            cu._recurringFails.ArbitrarilyRecurringFails.Counter.Synchron = 0;
            cu._recurringFails.SameRecurringFails.Reached.Synchron = false;
            cu._recurringFails.SameRecurringFails.Counter.Synchron = 0;

            foreach (var item in Enumerable.Range(1, noOfFails))
            {
                cu._manualTask.Execute();  //Reset other tasks

                while ((eTaskState)cu._manualTask._taskState.Synchron != eTaskState.Busy) ;

                cu._groundTask._task.Execute();

                while ((eTaskState)cu._groundTask._task._taskState.Synchron != eTaskState.Done) ;

                while ((eTaskState)cu._automatTask._task._taskState.Synchron != eTaskState.Busy) cu._automatTask._task.Execute();

                if (item < noOfFails)
                {
                    while (cu._automatTask._currentStep.ID.Synchron != 30000) ;
                    Assert.AreEqual(false, cu._recurringFails.SameRecurringFails.Reached.Synchron);
                }
                else
                {
                    while (cu._automatTask._currentStep.ID.Synchron != 22100) ;

                }

            }
            Assert.AreEqual(true, cu._recurringFails.SameRecurringFails.Reached.Synchron);

        }



    }
}
