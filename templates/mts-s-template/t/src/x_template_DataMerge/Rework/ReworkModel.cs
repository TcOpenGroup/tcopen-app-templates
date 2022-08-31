
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcOpen.Inxton.Data;
using TcOpen.Inxton.Data.Merge;
using x_template_xPlc;

namespace x_template_DataMerge.Rework
{
    public class ReworkModel
    {
        private MergeEntitiesData<PlainProcessData> merger;


        public ReworkModel(IRepository<PlainProcessData> repositorySource, IRepository<PlainProcessData> repositoryTarget )
        {
            merger = new MergeEntitiesData<PlainProcessData>(
                    repositorySource
                  , repositoryTarget

                  );

        }

        public bool ReworkEntity(string sourceId, string targetId)
        {
            var retVal = false;

            try
            {
                merger.Merge(sourceId, targetId, Exclude, Include, ReqProperty);

                merger.Target.EntityHeader.WasReset = false;
                merger.Target.EntityHeader.Results.Failures = string.Empty;
                merger.Target.EntityHeader.Results.Result =(short) TcoInspectors.eOverallResult.InProgress;
                merger.Target.EntityHeader.NextStation = merger.Source.EntityHeader.NextStation;
                merger.Target.EntityHeader.WasReworked = true;
                merger.Target.EntityHeader.ReworkCount += 1;
                merger.Target.EntityHeader.LastReworkName = merger.Source._EntityId;


                merger.TargetRepository.Update(targetId, merger.Target);
                retVal = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return retVal;
        }



        public async Task<bool> ReworkEntityAsync(string sourceId, string targetId)
        {


            var retVal = false;
            var progress = 0;

            try
            {
                var target = merger.TargetRepository.Read(targetId);
                var source = merger.SourceRepository.Read(sourceId);


                if (source.EntityHeader.NextStation > target.EntityHeader.LastStation)
                {
                    MessageBox.Show("Nevhodny rework. Nieje mozne reworkovat kus na danu stanicu!", "Pozor!");
                    return false;
                }
                else if (target.EntityHeader.Results.Result == (short)TcoInspectors.eOverallResult.Passed)
                {
                    MessageBox.Show("Neje mozne revorkovat OK kus!", "Pozor!");
                    return false;
                }
                else if (target.EntityHeader.Results.Result == (short)TcoInspectors.eOverallResult.InProgress)
                {
                    MessageBox.Show("Neje mozne revorkovat ktory je v procese a nema priznak NOK kus!", "Pozor!");
                    return false;
                }


                merger.Merge(sourceId, targetId, Exclude, Include, ReqProperty);


                merger.Target.EntityHeader.WasReset = false;
                merger.Target.EntityHeader.Results.Failures = string.Empty;
                merger.Target.EntityHeader.Results.Result = (short)TcoInspectors.eOverallResult.InProgress;
                merger.Target.EntityHeader.NextStation = merger.Source.EntityHeader.NextStation;
                merger.Target.EntityHeader.WasReworked = true;
                merger.Target.EntityHeader.ReworkCount += 1;
                merger.Target.EntityHeader.LastReworkName = merger.Source._EntityId;

                merger.TargetRepository.Update(targetId, merger.Target);
                progress++;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            MessageBox.Show("Reorkovanie kusu prebehlo uspesne", "Info!");
            retVal = true;


            return retVal;


        }

        public IQueryable<PlainProcessData> Reworks
        {
            get { return merger.SourceRepository.Queryable; }
        }

        public IQueryable<PlainProcessData> Entities
        {
            get { return merger.TargetRepository.Queryable; }
        }


        private IEnumerable<string> ReqProperty(object obj)
        {
            var retVal = new List<string>();
            switch (obj)
            {
                // here you define  properties witch are relevant for reqired types to change by rework 
                case TcoInspectors.PlainTcoDigitalInspectorData c:
                    return PropertyHelper.GetPropertiesNames(c,  p => p.RetryAttemptsCount ,p =>p.IsByPassed,p => p.IsExcluded);
                case TcoInspectors.PlainTcoAnalogueInspectorData c:
                    return PropertyHelper.GetPropertiesNames(c,  p => p.RetryAttemptsCount, p => p.IsByPassed, p => p.IsExcluded);
                case TcoInspectors.PlainTcoDataInspectorData c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.RetryAttemptsCount, p => p.IsByPassed, p => p.IsExcluded);
                case PlainCuHeader c:
                    return PropertyHelper.GetPropertiesNames(c, p => p.NextOnPassed, p => p.NextOnFailed);

                default:
                    break;
            }

            return new List<string>();
        }

        private bool Exclude(object obj)
        {
            // some special conditions to exluce
            return false;
        }

        private bool Include(object obj)
        {
            switch (obj)
            {
                // here is definitions of  all types and condition witch are relevat to change by rework (source)
                case TcoInspectors.PlainTcoInspectorData c:
                    return  c.Result != (short)TcoInspectors.eOverallResult.NoAction;
                case PlainCuHeader c:
                    return c is PlainCuHeader;
                        //&& (c.NextOnPassed != 0
                        //  || c.NextOnPassed != (short)enumStations.NONE
                        //  || c.NextOnFailed != 0
                        //  || c.NextOnFailed != (short)enumStations.NONE);
                default:
                    break;
            }

            return false;
        }
    }
}
