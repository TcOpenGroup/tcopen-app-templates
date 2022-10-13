using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vortex.Presentation;
using System.Globalization;
using TcOpen.Inxton.Input;
using x_template_xDataMerge.Rework;
using x_template_xPlc;
using x_template_xPlcConnector;
using x_template_xHmi.Blazor;

namespace x_template_xPlc
{ 
    public class OfflineReworkDataViewModel
    {
        public OfflineReworkDataViewModel()
        {
        }

        public ProcessDataManager SourceData
        {
            get
            {
                return Entry.Plc.MAIN._technology._reworkSettings;
            }
        }

        public ProcessDataManager TargetData
        {
            get
            {
                return Entry.Plc.MAIN._technology._cu00x._processData;
            }
        }

        public ReworkModel ReworkModel
        {
            get
            {
                return Startup.Rework;
            }
        }

        public async Task MergeDataAsync(string source, string target)
        {
            await ReworkModel.ReworkEntityAsync(source, target);
        }
    }
}
