using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace x_template_xStatistic.Statistics
{
    public class StatisticsDataItem 
    {
        public StatisticsDataItem()
        {
            counter = new CounterItem();
            errorCounter = new ObservableCollection<KeyValueSimple>();
            reworkCounter = new ObservableCollection<KeyValueSimple>();
            productionTrend = new ObservableCollection<KeyValueTrend>();

            entityTypeCounter = new ObservableCollection<KeyValueSimple>();
            recipeCounter = new ObservableCollection<KeyValueCounter>();
            threeShiftPerDayCounter = new ObservableCollection<KeyValueCounter>();
            twoShiftPerDayCounter = new ObservableCollection<KeyValueCounter>();
            carrierCounter = new ObservableCollection<KeyValueCounter>();
            hourCounter = new ObservableCollection<KeyValueCounter>();
            productionStack = new List<StackSample>();



        }

        /// <summary>
        /// Simple counter value
        /// </summary>
        public CounterItem Counter
        {
            get
            {
                return counter;
            }

            set
            {
                counter = value;
            }
        }

        /// <summary>
        /// A list polpulated by errors (occurrence of errors)
        /// </summary>
        public IList<KeyValueSimple> ErrorCounter
        {
            get
            {
                return errorCounter;
            }
            set
            {
                if (errorCounter == value)
                {
                    return;
                }

                errorCounter = value;
            }
        }

        public IList<KeyValueSimple> ReworkCounter
        {
            get
            {
                return reworkCounter;
            }
            set
            {
                if (reworkCounter == value)
                {
                    return;
                }

                reworkCounter = value;
            }
        }

        public IList<KeyValueSimple> EntityTypeCounter
        {
            get
            {
                return entityTypeCounter;
            }
            set
            {
                if (entityTypeCounter == value)
                {
                    return;
                }

                entityTypeCounter = value;
            }
        }


        public IList<KeyValueTrend> ProductionTrend
        {
            get
            {
                return productionTrend;
            }
            set
            {
                if (productionTrend == value)
                {
                    return;
                }

                productionTrend = value;
            }
        }

        public IList<KeyValueCounter> RecipeCounter
        {
            get
            {
                return recipeCounter;
            }
            set
            {
                if (recipeCounter == value)
                {
                    return;
                }

                recipeCounter = value;
            }
        }

        public IList<KeyValueCounter> CarrierCounter
        {
            get
            {
                return carrierCounter;
            }
            set
            {
                if (carrierCounter == value)
                {
                    return;
                }

                carrierCounter = value;
            }
        }

        public IList<KeyValueCounter> ThreeShiftPerDayCounter
        {
            get
            {
                return threeShiftPerDayCounter;
            }
            set
            {
                if (threeShiftPerDayCounter == value)
                {
                    return;
                }

                threeShiftPerDayCounter = value;
            }
        }

        public IList<KeyValueCounter> TwoShiftPerDayCounter
        {
            get
            {
                return twoShiftPerDayCounter;
            }
            set
            {
                if (twoShiftPerDayCounter == value)
                {
                    return;
                }

                twoShiftPerDayCounter = value;
            }
        }

        public IList<KeyValueCounter> HourCounter
        {
            get
            {
                return hourCounter;
            }
            set
            {
                if (hourCounter == value)
                {
                    return;
                }

                hourCounter = value;
            }
        }

        public List<StackSample> ProductionStack
        {
            get
            {
                return productionStack;
            }
            set
            {
                if (productionStack == value)
                {
                    return;
                }

                productionStack = value;
            }
        }



        private IList<KeyValueSimple> errorCounter;
        private IList<KeyValueCounter> recipeCounter;
        private IList<KeyValueCounter> threeShiftPerDayCounter;
        private IList<KeyValueCounter> twoShiftPerDayCounter;

        private IList<KeyValueCounter> hourCounter;
        private IList<KeyValueSimple> reworkCounter;
        private IList<KeyValueCounter> carrierCounter;
        private IList<KeyValueSimple> entityTypeCounter;
        private List<StackSample> productionStack;
        private IList<KeyValueTrend> productionTrend;
     
        private CounterItem counter;


     
    }

    public class CounterItem 
    {
        public long Passed { get; set; }
        public long Failed { get; set; }

    }
    public class TrendItem
    {
        public double Passed { get; set; }
        public double Failed { get; set; }

        public double RealTarget { get; set; }
        public double CalculatedTarget { get; set; }
        public double PassFailedRatio { get; set; }

    }

    public class KeyValueSimple
    {
        public string Id { get; set; }
        public string AttributeName { get; set; }
        public long Counter { get; set; }
    }

    public class StackSample
    {
        public DateTime _Modified { get; set; }
        public CounterItem Counter { get; set; }
       
    }

    public class KeyValueCounter
    {
        private string attributeName;

        public string Id { get; set; }
        public string AttributeName { 
            get {
                if (attributeName == null)
                {
                    string empty = string.Empty;
                    attributeName = empty;
                }
                return attributeName;}
            set => attributeName = value; }
        public CounterItem Counter { get; set; }


    }

    public class KeyValueTrend
    {
        public string Id { get; set; }
        public string AttributeName { get; set; }
        public TrendItem Trend { get; set; }
    }
}
