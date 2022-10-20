using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace x_template_xStatistic.Statistics
{
    public class StatisticsConfig 
    {
        private double productionTrendTarget;
        private List<AssignHourToShift> setShiftAndHours;
        private List<KeyValueCounter> threeShiftPerDay;
        private List<KeyValueCounter> twoShiftPerDay;
        private List<KeyValueSimple> productionTrend;
        private bool splitMultipleErrors;

        public StatisticsConfig()
        {
            setShiftAndHours = new List<AssignHourToShift>();
            threeShiftPerDay = new List<KeyValueCounter>();
            twoShiftPerDay = new List<KeyValueCounter>();
            productionTrend = new List<KeyValueSimple>();

        }

      
        public double ProductionTrendTarget
        {
            get => productionTrendTarget; set
            {
                productionTrendTarget = value;
                
            }
        }
        public bool SplitMultipleErrors
        {
            get => splitMultipleErrors; set
            {
                splitMultipleErrors = value;
                
            }
        }



        public List<AssignHourToShift> SetShiftAndHours
        {
            get
            {
                return setShiftAndHours;
            }
            set
            {
                if (setShiftAndHours == value)
                {
                    return;
                }

                setShiftAndHours = value;
            }
        }


        public List<KeyValueCounter> ThreeShiftPerDayCounter
        {
            get
            {
                return threeShiftPerDay;
            }
            set
            {
                if (threeShiftPerDay == value)
                {
                    return;
                }

                threeShiftPerDay = value;
            }
        }

        public List<KeyValueCounter> TwoShiftPerDayCounter
        {
            get
            {
                return twoShiftPerDay;
            }
            set
            {
                if (twoShiftPerDay == value)
                {
                    return;
                }

                twoShiftPerDay = value;
            }
        }

        public List<KeyValueSimple> ProductionTrend
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


        public event PropertyChangedEventHandler PropertyChanged;

    }
    public class AssignHourToShift
    {
        private string attributeName;

        public string Id { get; set; }
        public string AttributeName
        {
            get
            {
                if (attributeName == null)
                {
                    string empty = string.Empty;
                    attributeName = empty;
                }
                return attributeName;
            }
            set => attributeName = value;
        }
        public string TwoShiftDayId { get; set; }

        public string ThreeShiftDayId { get; set; }

    }
}