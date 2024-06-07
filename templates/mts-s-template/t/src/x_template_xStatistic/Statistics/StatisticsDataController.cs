using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TcOpen.Inxton.RepositoryDataSet;
using x_template_xPlc;

namespace x_template_xStatistic.Statistics
{
    public class StatisticsDataController :INotifyPropertyChanged
    {
        private const string ThirdShift = "03";
        private const string FirstShift = "01";
        private const string SecondShift = "02";
        private const string Format = "00";
        private const string UndefinedError = "UNDEFINED";

        public StatisticsDataController(string setId,
                                       RepositoryDataSetHandler<StatisticsDataItem> dataHandler,
                                       RepositoryDataSetHandler<StatisticsConfig> configHandler)
        {

            DataHandler = dataHandler;
            ConfigHandler = configHandler;


            this.setId = setId;
            LoadConfigSet(setId);
            LoadDataSet(setId);

            PrepareConfigColections();

            PrepareCollections();

            UpdateAttributeDescription();

            this.DataHandler.Update(this.setId, this.CurrentDataSet);
            this.ConfigHandler.Update(this.setId, this.CurrentConfigSet);
        }

        private void PrepareConfigColections()
        {
            if (Config.ThreeShiftPerDayCounter.Count() == 0)
            {
                foreach (var item in Enumerable.Range(1, 3))
                {
                    Config.ThreeShiftPerDayCounter.Add(new KeyValueCounter() { Id = item.ToString(Format), Counter = new CounterItem() });
                }
            }
            if (Config.TwoShiftPerDayCounter.Count() == 0)
            {
                foreach (var item in Enumerable.Range(1, 2))
                {
                    Config.TwoShiftPerDayCounter.Add(new KeyValueCounter() { Id = item.ToString(Format), Counter = new CounterItem() });
                }
            }

            if (Config.SetShiftAndHours.Count() == 0)
            {
                foreach (var item in Enumerable.Range(00, 24))
                {
                    var threeShift = ThirdShift;
                    if ((item >= 6) && (item < 14))
                        threeShift = FirstShift;
                    if ((item >= 14) && (item < 22))
                        threeShift = SecondShift;
                    var twoShift = SecondShift;
                    if ((item >= 6) && (item < 18))
                        twoShift = FirstShift;


                    Config.SetShiftAndHours.Add(new AssignHourToShift() { Id = item.ToString(Format), ThreeShiftDayId = threeShift, TwoShiftDayId = twoShift });
                }
            }
            if (Config.ProductionTrend.Count() != 8)
            {
                foreach (var item in Enumerable.Range(1, 8))
                {
                    Config.ProductionTrend.Add(new KeyValueSimple() { Id = item.ToString(Format) });
                }
            }
        }

        private void PrepareCollections()
        {
           
            if ( StatisticsData.HourCounter.Count() != 24)
            {
                StatisticsData.HourCounter.Clear();
                foreach (var item in Enumerable.Range(00, 24))
                {
                   StatisticsData.HourCounter.Add(new KeyValueCounter() { Id = item.ToString(Format), Counter = new CounterItem() });
                }
            }
            if (StatisticsData.ThreeShiftPerDayCounter.Count() != 3)
            {
                StatisticsData.ThreeShiftPerDayCounter.Clear();
                foreach (var item in Enumerable.Range(1, 3))
                {
                   StatisticsData.ThreeShiftPerDayCounter.Add(new KeyValueCounter() { Id = item.ToString(Format), Counter = new CounterItem() });
                }
            }
            if (StatisticsData.TwoShiftPerDayCounter.Count() != 2)
            {
                StatisticsData.TwoShiftPerDayCounter.Clear();
                foreach (var item in Enumerable.Range(1, 2))
                {
                   StatisticsData.TwoShiftPerDayCounter.Add(new KeyValueCounter() { Id = item.ToString(Format), Counter = new CounterItem() });
                }
            }

            

            if (StatisticsData.ProductionTrend.Count() != 8)
            {
                foreach (var item in Enumerable.Range(1, 8))
                {
                   StatisticsData.ProductionTrend.Add(new KeyValueTrend() { Id = item.ToString(Format), Trend = new TrendItem() });
                }
            }
        }



        private void UpdateAttributeDescription()
        {
            foreach (var item in Config.ThreeShiftPerDayCounter)
            {
               StatisticsData.ThreeShiftPerDayCounter.FirstOrDefault(p => p.Id == item.Id).AttributeName = item.AttributeName;
            }

            foreach (var item in Config.TwoShiftPerDayCounter)
            {
               StatisticsData.TwoShiftPerDayCounter.FirstOrDefault(p => p.Id == item.Id).AttributeName = item.AttributeName;
            }


            foreach (var item in Config.SetShiftAndHours)
            {
               StatisticsData.HourCounter.FirstOrDefault(p => p.Id == item.Id).AttributeName = item.AttributeName;
            }

            foreach (var item in Config.ProductionTrend)
            { 
               StatisticsData.ProductionTrend.FirstOrDefault(p => p.Id == item.Id).AttributeName = item.AttributeName;
            }
           
        }


        //clear counters methods
        internal void ClearTrendCounter()
        {
            StatisticsData.ProductionStack.Clear();
            StatisticsData.ProductionTrend.Clear();
            PrepareCollections();
            UpdateAttributeDescription();

           
            SaveDataSet(SetId);

        }

        internal void ClearReworkCounter()
        {
            StatisticsData.ReworkCounter.Clear();

          
            SaveDataSet(SetId);

        }

        internal void ClearCarrierCounterValues()
        {
            StatisticsData.CarrierCounter.Clear();

            
            SaveDataSet(SetId);

        }

        internal void ClearProductionTypeCounterValues()
        {
            StatisticsData.EntityTypeCounter.Clear();

         
            SaveDataSet(SetId);

        }

        internal void ClearShiftCounterValues()
        {

            StatisticsData.HourCounter.Clear();
            StatisticsData.ThreeShiftPerDayCounter.Clear();
            StatisticsData.TwoShiftPerDayCounter.Clear();
            PrepareCollections();
            UpdateAttributeDescription();

           
            SaveDataSet(SetId);

        }

        internal void ClearRecipeCounterValues()
        {
            StatisticsData.RecipeCounter.Clear();
        
           
            SaveDataSet(SetId);
        }

        internal void ClearErrorCounterValues()
        {
            StatisticsData.ErrorCounter.Clear();
          
            SaveDataSet(SetId);

        }

        internal void ClearCounterValues()
        {
            StatisticsData.Counter.Failed = 0;
            StatisticsData.Counter.Passed = 0;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatisticsData"));
            SaveDataSet(SetId);

        }

        

        public void Count(PlainProcessData data)
        {
            
            CurrentDataSet = this.DataHandler.Read(setId);
            var header = data.EntityHeader;
            var isFailed = header.Results.Result == 30;
            var isNormalProduction = !header.IsEmpty && !header.IsMaster ;

            if (isNormalProduction && (eStations)header.OpenOn != eStations.NONE)
            {


                var res = isFailed ? StatisticsData.Counter.Failed++ : StatisticsData.Counter.Passed++;
                StatisticsData.ProductionStack.Add(new StackSample()
                { _Modified = data._Modified, Counter = isFailed ? new CounterItem() { Failed = 1 } : new CounterItem() { Passed = 1 } });
                //clear old samples
                StatisticsData.ProductionStack.RemoveAll(x => (DateTime.Now - x._Modified).TotalDays > 1);
               
                // error type count
                if (isFailed)
                {
                    
                    if (Config.SplitMultipleErrors)
                    {
                        foreach (var item in data.EntityHeader.Results.Failures.Split(';'))
                        {
                            if (item != string.Empty)
                            {
                                CountErrorType( item);

                            }
                        }
                    }
                    else
                    {
                        CountErrorType(data.EntityHeader.Results.Failures);
                    }

                }

                // rework counter
                if (header.WasReworked)
                {
                    var anyRework = StatisticsData.ReworkCounter.Any(p => p.Id == data.EntityHeader.LastReworkName);

                    if (anyRework)
                    {
                        var count = StatisticsData.ReworkCounter.FirstOrDefault(p => p.Id == data.EntityHeader.LastReworkName).Counter;
                        count++;
                        StatisticsData.ReworkCounter.FirstOrDefault(c => c.Id == data.EntityHeader.LastReworkName).Counter = count;
                    }
                    else
                        StatisticsData.ReworkCounter.Add(new KeyValueSimple() { Id = data.EntityHeader.LastReworkName, Counter = 1 });
                }


                // Trend counter
                var countLastHourOk = StatisticsData.ProductionStack.Where(x => (DateTime.Now - x._Modified).Hours < 1).Count(y => y.Counter.Passed == 1);


                foreach (var item in Enumerable.Range(1, 8))
                {
                    var countOk = StatisticsData.ProductionStack.Where(x => (DateTime.Now - x._Modified).Hours < item).Where(y => y.Counter.Passed == 1).Count();
                    var countNok = StatisticsData.ProductionStack.Where(x => (DateTime.Now - x._Modified).Hours < item).Where(y => y.Counter.Failed == 1).Count();

                    var anyTrend = StatisticsData.ProductionTrend.Any(p => p.Id == item.ToString(Format));

                    if (anyTrend)
                    {
                        if (Config.ProductionTrendTarget!=0 && (countOk!=0 || countNok!=0))
                        {
                           StatisticsData.ProductionTrend.Where(c => c.Id == item.ToString(Format)).FirstOrDefault().Trend = Config.ProductionTrendTarget != 0
                                ? new TrendItem()
                                {
                                    Passed = countOk,
                                    Failed = countNok,
                                    CalculatedTarget = Config.ProductionTrendTarget * item,
                                    RealTarget = 100 * countOk / (Config.ProductionTrendTarget * item),
                                    PassFailedRatio = 100 * countNok / (countOk + countNok)
                                }
                                : new TrendItem() { Passed = countOk, Failed = countNok, CalculatedTarget = Config.ProductionTrendTarget * item, PassFailedRatio = 100 * (countOk + countNok) / countNok };
                        }
                    }
                }


                //recipes  counter
                var anyRecipe =StatisticsData.RecipeCounter.Any(p => p.Id == data.EntityHeader.Recipe);


                if (anyRecipe)
                {
                    var count =StatisticsData.RecipeCounter.FirstOrDefault(p => p.Id == data.EntityHeader.Recipe).Counter;
                    var resinc = isFailed ? count.Failed++ : count.Passed++;
                    var update =StatisticsData.RecipeCounter.Where(c => c.Id == data.EntityHeader.Recipe).Select(c => { c.Counter = count; return c; });
                }
                else
                {
                   StatisticsData.RecipeCounter.Add(new KeyValueCounter()
                    { Id = data.EntityHeader.Recipe, Counter = isFailed ? new CounterItem() { Failed = 1 } : new CounterItem() { Passed = 1 } });
                }
                //hour counter
                var modified =StatisticsData.HourCounter.Any(p => p.Id == data._Modified.ToString("HH"));

                if (modified)
                {
                    var count =StatisticsData.HourCounter.FirstOrDefault(p => p.Id == data._Modified.ToString("HH")).Counter;
                    var resinc = isFailed ? count.Failed++ : count.Passed++;
                    var update =StatisticsData.HourCounter.Where(c => c.Id == data._Modified.ToString("HH")).Select(c => { c.Counter = count; return c; });
                }
                else
                {

                   StatisticsData.HourCounter.Add(new KeyValueCounter()
                    { Id = data._Modified.ToString("HH"), Counter = isFailed ? new CounterItem() { Failed = 1 } : new CounterItem() { Passed = 1 } });
                }

                //three shift conters
                foreach (var item in StatisticsData.ThreeShiftPerDayCounter)
                {
                    var threeShiftCounter = new CounterItem();


                    var reqHours = Config.SetShiftAndHours.Where(c => c.ThreeShiftDayId == item.Id);
                    foreach (var hour in reqHours)
                    {
                        threeShiftCounter.Passed += StatisticsData.HourCounter.FirstOrDefault(c => c.Id == hour.Id).Counter.Passed;
                        threeShiftCounter.Failed += StatisticsData.HourCounter.FirstOrDefault(c => c.Id == hour.Id).Counter.Failed;
                    }

                    item.Counter  = threeShiftCounter;

                   
                }

                //two shift conters
                foreach (var item in StatisticsData.TwoShiftPerDayCounter)
                {
                    var twoShiftCounter = new CounterItem();


                    var reqHours = Config.SetShiftAndHours.Where(c => c.TwoShiftDayId == item.Id);
                    foreach (var hour in reqHours)
                    {
                        twoShiftCounter.Passed += StatisticsData.HourCounter.FirstOrDefault(c => c.Id == hour.Id).Counter.Passed;
                        twoShiftCounter.Failed += StatisticsData.HourCounter.FirstOrDefault(c => c.Id == hour.Id).Counter.Failed;
                    }

                    item.Counter = twoShiftCounter;
                  
                }




                //carrier  counter
                var anycarrier =StatisticsData.CarrierCounter.Any(p => p.Id == data.EntityHeader.Carrier);

                if (!string.IsNullOrEmpty(data.EntityHeader.Carrier))
                {
                    if (anycarrier)
                    {
                        var count =StatisticsData.CarrierCounter.FirstOrDefault(p => p.Id == data.EntityHeader.Carrier).Counter;
                        var inc = isFailed ? count.Failed++ : count.Passed++;

                        var update =StatisticsData.CarrierCounter.Where(c => c.Id == data.EntityHeader.Carrier).Select(c => { c.Counter = count; return c; });
                    }
                    else
                    {
                       StatisticsData.CarrierCounter.Add(new KeyValueCounter()
                        { Id = data.EntityHeader.Carrier, Counter = isFailed ? new CounterItem() { Failed = 1 } : new CounterItem() { Passed = 1 } });
                    }
                }

            }
            // entity type counter
            var isEmpty = nameof(data.EntityHeader.IsEmpty).Split('.').ToList().Last();
            var isRework = nameof(data.EntityHeader.WasReworked).Split('.').ToList().Last();
            var isMaster = nameof(data.EntityHeader.IsMaster).Split('.').ToList().Last();
            var isNormal = "Normal";

            var idKey = isNormal;
            if (header.IsEmpty)
            {
                idKey = isEmpty;
            }
            else if (header.IsMaster)
            {
                idKey = isMaster;
            }
            else if (header.WasReworked)
            {
                idKey = isRework;
            }

            var anyEntity = StatisticsData.EntityTypeCounter.Any(p => p.Id == idKey);

            if (anyEntity)
            {
                var count = StatisticsData.EntityTypeCounter.FirstOrDefault(p => p.Id == idKey).Counter;
                count++;
                StatisticsData.EntityTypeCounter.Where(c => c.Id == idKey).FirstOrDefault().Counter = count;
            }
            else
               StatisticsData.EntityTypeCounter.Add(new KeyValueSimple() { Id = idKey, Counter = 1 });

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatisticsData"));
          
            this.DataHandler.Update(setId, this.CurrentDataSet);
        }

        private void CountErrorType( string item)
        {
            var itm = item;
            if (itm == string.Empty)
            {
                itm = UndefinedError;
            }
            var anyFailure = StatisticsData.ErrorCounter.Any(p => p.Id == itm);

            if (anyFailure)
            {
                var count = StatisticsData.ErrorCounter.FirstOrDefault(p => p.Id == itm).Counter;
            
                count++;

                StatisticsData.ErrorCounter.FirstOrDefault(c => c.Id == itm).Counter = count;

            }
            else
                StatisticsData.ErrorCounter.Add(new KeyValueSimple() { Id = itm, Counter = 1 });
        }

        public void NotifyUpdate()
        {
            this.CurrentDataSet = this.DataHandler.Read(setId);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("StatisticsData"));
        }




        /// <summary>
        /// Gets datahandler of this 
        /// </summary>
        protected RepositoryDataSetHandler<StatisticsDataItem> DataHandler { get;  }
        public RepositoryDataSetHandler<StatisticsConfig> ConfigHandler { get; }
        public string SetId { get =>setId; }
        public StatisticsConfig Config { get => CurrentConfigSet.Item; }
        public StatisticsDataItem StatisticsData { get => CurrentDataSet.Item; }


        private string setId;
     
        public void LoadConfigSet(string setId)
        {
     
            var result = ConfigHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setId);

            if (result == null)
            {
                ConfigHandler.Create(setId, CurrentConfigSet);
            }
            else
                CurrentConfigSet = this.ConfigHandler.Read(setId);
          
        }


        public void LoadDataSet(string setId)
        {
  
            var result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setId);

            if (result == null)
            {
                DataHandler.Create(setId, CurrentDataSet);
            }
            else
                CurrentDataSet = DataHandler.Read(setId);

           
        }


        public void SaveDataSet(string setId)
        {
            EntitySet<StatisticsDataItem> result;
            try
            {
                result = DataHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setId);

            }
            catch (Exception)
            {

                throw;
            }


            if (result == null)
            {
                DataHandler.Create(setId, CurrentDataSet);
            }
            DataHandler.Update(setId, CurrentDataSet);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentDataSet"));

        }

        public void SaveConfigSet(string setId)
        {
            EntitySet<StatisticsConfig> result;
            try
            {
                result = ConfigHandler.Repository.Queryable.FirstOrDefault(p => p._EntityId == setId);

            }
            catch (Exception)
            {

                throw;
            }


            if (result == null)
            {
                this.ConfigHandler.Create(setId, CurrentConfigSet);
            }
            this.ConfigHandler.Update(setId, CurrentConfigSet);

           
        }

        /// <summary>
        /// Gets current statistic data set.
        /// </summary>
        public EntitySet<StatisticsDataItem> CurrentDataSet { get; set; } = new EntitySet<StatisticsDataItem>();

        /// <summary>
        /// Gets current statistic config data set.
        /// </summary>
        public EntitySet<StatisticsConfig> CurrentConfigSet { get; set; } = new EntitySet<StatisticsConfig>();
      
        
        public event PropertyChangedEventHandler PropertyChanged;

      








    }
    public static class ExtensionMethods
    {
        /// <summary>
        /// Allows an IEnumerable to iterate through its collection and perform an action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}
