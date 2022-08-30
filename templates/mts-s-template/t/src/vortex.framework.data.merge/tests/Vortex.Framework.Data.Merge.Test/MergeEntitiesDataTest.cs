using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Data.Merge;
using TcOpen.Inxton.RavenDb;
using x_template_xPlc;
using x_template_xPlcConnector;
using x_template_xTests;

namespace Vortex.Framework.Data.Tests
{


    public class MergeEntitiesDataTest
    {
#if DEBUG
        private const int timeOut = 70000;
#else
    private const int timeOut = 70000;
#endif

        private RepositoryBase<TestData> repositorySource;
        private RepositoryBase<TestData> repositoryTarget;
        List<Type> reqTypes = new List<Type>();
        List<string> reqProperties = new List<string>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var a = x_template_xApp.Get;
        }
        [SetUp]
        void SetupRepositories()
        {

            repositoryTarget = new RavenDbRepository<TestData>(new RavenDbRepositorySettings<TestData>(new string[] { @"http://localhost:8080" }, "TargetData", "", ""));

            repositoryTarget.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            repositoryTarget.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };
            
            Entry.Plc.MAIN._technology._processSettings.InitializeRepository(repositoryTarget);
            
          


             repositorySource = new RavenDbRepository<TestData>(new RavenDbRepositorySettings<TestData>(new string[] { @"http://localhost:8080" }, "SourceData", "", ""));

            repositorySource.OnCreate = (id, data) => { data._Created = DateTime.Now; data._Modified = DateTime.Now; };
            repositorySource.OnUpdate = (id, data) => { data._Modified = DateTime.Now; };

            Entry.Plc.MAIN._technology._reworkSettings.InitializeRepository(repositorySource);


            var recordsSource = repositorySource.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
            recordsSource.ForEach(p => repositorySource.Delete(p));

            var recordsTarget = repositoryTarget.Queryable.Where(p => true).Select(p => p._EntityId).ToList();
            recordsTarget.ForEach(p => repositoryTarget.Delete(p));

        }


   

        private void SetupSourceEntity(TestData testDataSource)
        {
            var random = new Random();
            testDataSource.customChecker1.IsByPassed = true;
            testDataSource.customChecker2.IsByPassed = true;
            testDataSource.customChecker3.IsByPassed = true;
            testDataSource.customChecker4.IsByPassed = true;

            testDataSource.customChecker1.IsExcluded = true;
            testDataSource.customChecker2.IsExcluded = true;
            testDataSource.customChecker3.IsExcluded = true;
            testDataSource.customChecker4.IsExcluded = true;

            testDataSource.customChecker1.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataSource.customChecker2.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataSource.customChecker3.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataSource.customChecker4.Result = (short)TcoInspectors.eInspectorResult.NoAction;

            testDataSource.customChecker1.Measured = 0;
            testDataSource.customChecker2.Measured = 0;
            testDataSource.customChecker3.Measured = 0;
            testDataSource.customChecker4.Measured = 0;

            testDataSource.customChecker1.NumberOfAllowedRetries = 1;
            testDataSource.customChecker2.NumberOfAllowedRetries = 2;
            testDataSource.customChecker3.NumberOfAllowedRetries = 3;
            testDataSource.customChecker4.NumberOfAllowedRetries = 4;


            testDataSource.customChecker1.Minimum = (float)random.NextDouble();
            testDataSource.customChecker2.Minimum = (float)random.NextDouble();
            testDataSource.customChecker3.Minimum = (float)random.NextDouble();
            testDataSource.customChecker4.Minimum = (float)random.NextDouble();

            testDataSource.customChecker1.Maximum = (float)random.NextDouble();
            testDataSource.customChecker2.Maximum = (float)random.NextDouble();
            testDataSource.customChecker3.Maximum = (float)random.NextDouble();
            testDataSource.customChecker4.Maximum = (float)random.NextDouble();

        }

        private void SetupEntities(out TestData testDataSource, out TestData testDataTarget)
        {
            testDataSource = new TestData() { _recordId = default(dynamic), _Created = new DateTime(1904522374), _EntityId = "TestValue1447482512", _Modified = new DateTime(1997083921) };
            repositorySource.Create(testDataSource._EntityId, testDataSource);

            testDataTarget = new TestData() { _recordId = default(dynamic), _Created = new DateTime(1904522374), _EntityId = "TestValue1447482513", _Modified = new DateTime(1997083921) };
            repositoryTarget.Create(testDataTarget._EntityId, testDataTarget);
        }

     

        [Test]
        public void CompareIfReqPropertyNotInList()
        {
            // Arrange  
            //SetupRepositories();

            reqTypes.Clear();
            reqTypes.Add(typeof(CustomCheckerDataChangeable));

            reqProperties = PropertyHelper.GetPropertiesNames(new CustomCheckerDataChangeable(), p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);

            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.Measured = (float)random.NextDouble();
            testDataTarget.customChecker2.Measured = (float)random.NextDouble();
            testDataTarget.customChecker3.Measured = (float)random.NextDouble();
            testDataTarget.customChecker4.Measured = (float)random.NextDouble();



            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    , reqTypes
                    , reqProperties
                    , Exclusion
                    , Inclusion
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreNotEqual(testDataSource.customChecker1.Measured.ToString(), mergedTarget.customChecker1.Measured.ToString());
            Assert.AreNotEqual(testDataSource.customChecker2.Measured.ToString(), mergedTarget.customChecker2.Measured.ToString());
            Assert.AreNotEqual(testDataSource.customChecker3.Measured.ToString(), mergedTarget.customChecker3.Measured.ToString());
            Assert.AreNotEqual(testDataSource.customChecker4.Measured.ToString(), mergedTarget.customChecker4.Measured.ToString());


        }

        [Test]
        public void CompareIfReqPropertyIsInList()
        {
            // Arrange  
            SetupRepositories();
            reqTypes.Clear();
            reqTypes.Add(typeof(CustomCheckerDataChangeable));


            reqProperties = PropertyHelper.GetPropertiesNames(new CustomCheckerDataChangeable(), p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);


            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.IsByPassed = !testDataSource.customChecker1.IsByPassed;
            testDataTarget.customChecker2.IsByPassed = !testDataSource.customChecker2.IsByPassed;
            testDataTarget.customChecker3.IsByPassed = !testDataSource.customChecker3.IsByPassed;
            testDataTarget.customChecker4.IsByPassed = !testDataSource.customChecker4.IsByPassed;



            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    , reqTypes
                    , reqProperties
                    , Exclusion
                    , Inclusion
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreEqual(testDataSource.customChecker1.IsByPassed, mergedTarget.customChecker1.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker2.IsByPassed, mergedTarget.customChecker2.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker3.IsByPassed, mergedTarget.customChecker3.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker4.IsByPassed, mergedTarget.customChecker4.IsByPassed);


        }

       
        [Test]
        public void CompareIfReqPropertyIsInListWithExclusion()
        {
            // Arrange  
            SetupRepositories();

            reqTypes.Add(typeof(CustomCheckerDataChangeable));
            reqTypes.Add(typeof(CustomCheckerDataNotChangeable));


            reqProperties = PropertyHelper.GetPropertiesNames(new CustomCheckerDataChangeable(), p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);


            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.IsByPassed = !testDataSource.customChecker1.IsByPassed;
            testDataTarget.customChecker2.IsByPassed = !testDataSource.customChecker2.IsByPassed;
            testDataTarget.customChecker3.IsByPassed = !testDataSource.customChecker3.IsByPassed;
            testDataTarget.customChecker4.IsByPassed = !testDataSource.customChecker4.IsByPassed;

            testDataTarget.customChecker1.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker2.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker3.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataTarget.customChecker4.Result = (short)TcoInspectors.eInspectorResult.NoAction;

            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    , reqTypes
                    , reqProperties
                    , ExclusionTest
                    , Inclusion
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreEqual(testDataSource.customChecker1.IsByPassed, mergedTarget.customChecker1.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker1.NumberOfAllowedRetries, mergedTarget.customChecker1.NumberOfAllowedRetries);

            Assert.AreEqual(testDataSource.customChecker2.IsByPassed, mergedTarget.customChecker2.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker2.NumberOfAllowedRetries, mergedTarget.customChecker2.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker3.IsByPassed, mergedTarget.customChecker3.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker3.NumberOfAllowedRetries, mergedTarget.customChecker3.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker4.IsByPassed, mergedTarget.customChecker4.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker4.NumberOfAllowedRetries, mergedTarget.customChecker4.NumberOfAllowedRetries);


        }

        [Test]
        public void CompareIfReqPropertyIsInListWithInclusion()
        {
            // Arrange  
            SetupRepositories();
            reqTypes.Clear();
            reqTypes.Add(typeof(CustomCheckerDataChangeable));
           

            reqProperties = PropertyHelper.GetPropertiesNames(new CustomCheckerDataChangeable(), p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);


            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.IsByPassed = !testDataSource.customChecker1.IsByPassed;
            testDataTarget.customChecker2.IsByPassed = !testDataSource.customChecker2.IsByPassed;
            testDataTarget.customChecker3.IsByPassed = !testDataSource.customChecker3.IsByPassed;
            testDataTarget.customChecker4.IsByPassed = !testDataSource.customChecker4.IsByPassed;

            testDataTarget.customChecker1.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker2.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker3.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataTarget.customChecker4.Result = (short)TcoInspectors.eInspectorResult.NoAction;

            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    , reqTypes
                    , reqProperties
                    , ExclusionInclusionTest
                    , InclusionTest
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreEqual(testDataSource.customChecker1.IsByPassed, mergedTarget.customChecker1.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker1.NumberOfAllowedRetries, mergedTarget.customChecker1.NumberOfAllowedRetries);

            Assert.AreEqual(testDataSource.customChecker2.IsByPassed, mergedTarget.customChecker2.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker2.NumberOfAllowedRetries, mergedTarget.customChecker2.NumberOfAllowedRetries);

            Assert.AreEqual(testDataSource.customChecker3.IsByPassed, mergedTarget.customChecker3.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker3.NumberOfAllowedRetries, mergedTarget.customChecker3.NumberOfAllowedRetries);

            Assert.AreEqual(testDataSource.customChecker4.IsByPassed, mergedTarget.customChecker4.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker4.NumberOfAllowedRetries, mergedTarget.customChecker4.NumberOfAllowedRetries);


        }
        [Test]
        public void CompareIfReqPropertyIsInListWithInclusionDiffCtor()
        {
            // Arrange  
            SetupRepositories();
            reqTypes.Clear();
            reqTypes.Add(typeof(CustomCheckerDataChangeable));


            reqProperties = PropertyHelper.GetPropertiesNames(new CustomCheckerDataChangeable(), p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum, p => p.Maximum, p => p.NumberOfAllowedRetries);


            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.IsByPassed = !testDataSource.customChecker1.IsByPassed;
            testDataTarget.customChecker2.IsByPassed = !testDataSource.customChecker2.IsByPassed;
            testDataTarget.customChecker3.IsByPassed = !testDataSource.customChecker3.IsByPassed;
            testDataTarget.customChecker4.IsByPassed = !testDataSource.customChecker4.IsByPassed;

            testDataTarget.customChecker1.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker2.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker3.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataTarget.customChecker4.Result = (short)TcoInspectors.eInspectorResult.NoAction;

            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreNotEqual(testDataSource.customChecker1.IsByPassed, mergedTarget.customChecker1.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker1.NumberOfAllowedRetries, mergedTarget.customChecker1.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker2.IsByPassed, mergedTarget.customChecker2.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker2.NumberOfAllowedRetries, mergedTarget.customChecker2.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker3.IsByPassed, mergedTarget.customChecker3.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker3.NumberOfAllowedRetries, mergedTarget.customChecker3.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker4.IsByPassed, mergedTarget.customChecker4.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker4.NumberOfAllowedRetries, mergedTarget.customChecker4.NumberOfAllowedRetries);


        }

        [Test]
        public void CompareWithInclusionAndExclusionFromOutside()
        {
            // Arrange  
            SetupRepositories();

            


            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.IsByPassed = !testDataSource.customChecker1.IsByPassed;
            testDataTarget.customChecker2.IsByPassed = !testDataSource.customChecker2.IsByPassed;
            testDataTarget.customChecker3.IsByPassed = !testDataSource.customChecker3.IsByPassed;
            testDataTarget.customChecker4.IsByPassed = !testDataSource.customChecker4.IsByPassed;

            testDataTarget.customChecker1.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker2.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker3.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataTarget.customChecker4.Result = (short)TcoInspectors.eInspectorResult.NoAction;

            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    , new List<Type>()
                    , new List<string>()
                    , ExclusionTest
                    , Inclusion
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId,ExcludeFromOutsideTest, IncludeFromOutsideTest, ReqPropertyFromOutisde);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreEqual(testDataSource.customChecker1.IsByPassed, mergedTarget.customChecker1.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker1.NumberOfAllowedRetries, mergedTarget.customChecker1.NumberOfAllowedRetries);

            Assert.AreEqual(testDataSource.customChecker2.IsByPassed, mergedTarget.customChecker2.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker2.NumberOfAllowedRetries, mergedTarget.customChecker2.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker3.IsByPassed, mergedTarget.customChecker3.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker3.NumberOfAllowedRetries, mergedTarget.customChecker3.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker4.IsByPassed, mergedTarget.customChecker4.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker4.NumberOfAllowedRetries, mergedTarget.customChecker4.NumberOfAllowedRetries);


        }


        [Test]
        public void CompareWithInclusionAndExclusionFromOutsideDiffCtor()
        {
            // Arrange  
            SetupRepositories();




            TestData testDataSource, testDataTarget;
            SetupEntities(out testDataSource, out testDataTarget);

            SetupSourceEntity(testDataSource);

            var random = new Random();
            testDataTarget.customChecker1.IsByPassed = !testDataSource.customChecker1.IsByPassed;
            testDataTarget.customChecker2.IsByPassed = !testDataSource.customChecker2.IsByPassed;
            testDataTarget.customChecker3.IsByPassed = !testDataSource.customChecker3.IsByPassed;
            testDataTarget.customChecker4.IsByPassed = !testDataSource.customChecker4.IsByPassed;

            testDataTarget.customChecker1.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker2.Result = (short)TcoInspectors.eInspectorResult.Excluded;
            testDataTarget.customChecker3.Result = (short)TcoInspectors.eInspectorResult.NoAction;
            testDataTarget.customChecker4.Result = (short)TcoInspectors.eInspectorResult.NoAction;

            repositorySource.Update(testDataSource._EntityId, testDataSource);

            repositoryTarget.Update(testDataTarget._EntityId, testDataTarget);



            // Act


            var merge = new MergeEntitiesData<TestData>(
                      repositorySource
                    , repositoryTarget
                    );

            merge.Merge(testDataSource._EntityId, testDataTarget._EntityId, ExcludeFromOutsideTest, IncludeFromOutsideTest, ReqPropertyFromOutisde);

            var mergedTarget = repositoryTarget.Read(testDataTarget._EntityId);



            // Assert
            Assert.AreEqual(testDataSource.customChecker1.IsByPassed, mergedTarget.customChecker1.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker1.NumberOfAllowedRetries, mergedTarget.customChecker1.NumberOfAllowedRetries);

            Assert.AreEqual(testDataSource.customChecker2.IsByPassed, mergedTarget.customChecker2.IsByPassed);
            Assert.AreEqual(testDataSource.customChecker2.NumberOfAllowedRetries, mergedTarget.customChecker2.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker3.IsByPassed, mergedTarget.customChecker3.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker3.NumberOfAllowedRetries, mergedTarget.customChecker3.NumberOfAllowedRetries);

            Assert.AreNotEqual(testDataSource.customChecker4.IsByPassed, mergedTarget.customChecker4.IsByPassed);
            Assert.AreNotEqual(testDataSource.customChecker4.NumberOfAllowedRetries, mergedTarget.customChecker4.NumberOfAllowedRetries);


        }

        private IEnumerable<string> ReqPropertyFromOutisde(object obj)
        {
            var retVal = new List<string>();
            switch (obj)
            {

                 case CustomCheckerDataChangeable 
                    c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum,p => p.Maximum,p=>p.NumberOfAllowedRetries);


                case CustomCheckerDataNotChangeable c:
                    return new List<string>();
                default:
                    break;
            }

            return new List<string>();
        }

        private bool ExcludeFromOutsideTest(object obj)
        {
           
            return false;
        }

        private bool IncludeFromOutsideTest(object obj)
        {
            switch (obj)
            {
                case CustomCheckerDataChangeable c:
                    return c is CustomCheckerDataChangeable && c.Result != 0; 


                case CustomCheckerDataNotChangeable c:
                    return c is CustomCheckerDataNotChangeable && c.Result != 0;


                default:
                    break;
            }

            return false;
        }


        private bool ExclusionTest(object obj)
        {
            switch (obj)
            {
                case CustomCheckerDataChangeable c:
                    return c.Result == 0; // not change settings for checkers whitch are not checked yet

                case CustomCheckerDataNotChangeable c:
                    return c.Result == 0; // not change settings for checkers whitch are not checked yet
                default:
                    break;
            }

            return false;
        }

        private bool ExclusionInclusionTest(object obj)
        {
            switch (obj)
            {
              
                case CustomCheckerDataNotChangeable c:
                    return c.Result != 0; // not change settings for checkers whitch are not checked yet
                default:
                    break;
            }

            return false;
        }

        private bool InclusionTest(object obj)
        {
            switch (obj)
            {


                case CustomCheckerDataNotChangeable c:
                    return c is CustomCheckerDataNotChangeable;

             
                default:
                    break;
            }

            return false;
        }
        private bool Inclusion(object obj)
        {
            //switch (obj)
            //{

            //    case PlainstAnalogueCheckerData c:
            //        return c is PlainstAnalogueCheckerData;

            //    case PlainstCu_Header c:
            //        return c is PlainstCu_Header;

            //    case PlainstCu_1_ProcessData c:
            //        return c is PlainstCu_1_ProcessData;
            //    default:
            //        break;
            //}

            return false;
        }

        private bool Exclusion(object obj)
        {
            switch (obj)
            {
            
                default:
                    break;
            }

            return false;
        }

        private IEnumerable<string> PropertiesInclusion(object obj)
        {
            var retVal = new List<string>();
            //switch (obj)
            //{

            //    case PlainstAnalogueCheckerData c:

            //        return DataMerge.PropertyHelper.GetPropertiesNames(c, p => p.IsByPassed, p => p.IsExcluded, p => p.Minimum);


            //    case PlainstCu_1_ProcessData c:
            //        return DataMerge.PropertyHelper.GetPropertiesNames(c, p => p.Check, p => p.LogicCheck, p => p.Header);

            //    default:
            //        break;
            //}

            return new List<string>();
        }
    }

   
}
