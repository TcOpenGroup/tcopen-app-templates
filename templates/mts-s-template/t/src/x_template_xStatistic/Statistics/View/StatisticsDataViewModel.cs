


using System;
using TcOpen.Inxton.Input;
using x_template_xPlc;

namespace x_template_xStatistic.Statistics.View
{
    public class StatisticsDataViewModel
    {
        public StatisticsDataViewModel()
        {


            ClearCounterCommand = new RelayCommand(a => ClearCounter());
            ClearErrorCounterCommand = new RelayCommand(a => ClearErrorCounter());
            ClearRecipeCounterCommand = new RelayCommand(a => ClearRecipeCounter());
            ClearHourCounterCommand = new RelayCommand(a => ClearShiftAndHourCounter());
            ClearProductionTypeCounterCommand = new RelayCommand(a => ClearProductionTypeCounter());
            ClearCarrierCounterCommand = new RelayCommand(a => ClearCarrierCounter());
            ClearReworkCounterCommand = new RelayCommand(a => ClearReworkCounter());
            ClearTrendCounterCommand = new RelayCommand(a => ClearTrendCounter());
            SaveSettingsCommand = new RelayCommand(a => SaveSettings());
            AddNewTestCommand = new RelayCommand(a => AddTest());
      
        }

      

        public StatisticsDataViewModel(StatisticsDataController statController)
        {


            ClearCounterCommand = new RelayCommand (a => ClearCounter());
            ClearErrorCounterCommand = new RelayCommand(a => ClearErrorCounter());
            ClearRecipeCounterCommand = new RelayCommand(a => ClearRecipeCounter());
            ClearHourCounterCommand = new RelayCommand(a => ClearShiftAndHourCounter());
            ClearProductionTypeCounterCommand = new RelayCommand(a => ClearProductionTypeCounter());
            ClearCarrierCounterCommand = new RelayCommand(a => ClearCarrierCounter());
            ClearReworkCounterCommand = new RelayCommand(a => ClearReworkCounter());
            ClearTrendCounterCommand = new RelayCommand(a => ClearTrendCounter());
            SaveSettingsCommand = new RelayCommand(a => SaveSettings());
            AddNewTestCommand = new RelayCommand(a => AddTest());
            Controller = statController;



        }
        private void AddTest()
        {

            Controller.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = "ABCD", Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 30, Failures = "CHYBA 5 ,CHYBA 6" }, Carrier = "5432548457984" }
            });

            Controller.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = "XY", Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 20, Failures = "" }, IsMaster = false, Carrier = "5432548457456" }
            }); ;
            Controller.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = "XYZ", Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 20, Failures = "" }, IsMaster = false }
            }); ;
            Controller.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = "XXXY", Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 20, Failures = "" }, IsEmpty = true }
            }); ;

            Controller.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = "XXXX", Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 30, Failures = "" }, Carrier = "0123548457984" }
            }); ;
            Controller.Count(new PlainProcessData()
            {
                _Modified = DateTime.Now,
                EntityHeader = new PlainEntityHeader() { Recipe = "XXXX", Results = new TcoInspectors.PlainTcoComprehensiveResult() { Result = 30, Failures = "" }, Carrier = "0123548457984", WasReworked = true, LastReworkName = "Rwork OP20" }
            }); ;
        }


        private void ClearProductionTypeCounter()
        {
            Controller.ClearProductionTypeCounterValues();
        }
        private void ClearCarrierCounter()
        {
            Controller.ClearCarrierCounterValues();
        }

        private void ClearReworkCounter()
        {
            Controller.ClearReworkCounter();
        }

        private void ClearTrendCounter()
        {
            Controller.ClearTrendCounter();
        }

        private void SaveSettings()
        {
            Controller.SaveConfigSet(Controller.SetId);
        }


        private void ClearShiftAndHourCounter()
        {
            Controller.ClearShiftCounterValues();
        }

        private void ClearRecipeCounter()
        {
            Controller.ClearRecipeCounterValues();
        }

        private void ClearErrorCounter()
        {
            Controller.ClearErrorCounterValues();
        }

        private void ClearCounter()
        {
            Controller.ClearCounterValues();
        }

      


        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        public StatisticsDataController Controller { get; set; }

        public RelayCommand ClearErrorCounterCommand { get; private set; }
        public RelayCommand ClearRecipeCounterCommand { get; private set; }
        public RelayCommand ClearCounterCommand { get; private set; }
        public RelayCommand ClearTrendCounterCommand { get; private set; }
        public RelayCommand ClearProductionTypeCounterCommand { get; private set; }
        public RelayCommand ClearCarrierCounterCommand { get; private set; }
        public RelayCommand ClearReworkCounterCommand { get; private set; }
        public RelayCommand ClearHourCounterCommand { get; private set; }
        public RelayCommand SaveSettingsCommand { get; private set; }
        public RelayCommand AddNewTestCommand { get; private set; }
        public RelayCommand ConfigCommand { get; private set; }

    }
}

