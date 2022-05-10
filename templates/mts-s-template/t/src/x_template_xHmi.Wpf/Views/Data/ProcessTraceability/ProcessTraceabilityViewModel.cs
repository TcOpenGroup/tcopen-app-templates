using x_template_xPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcoData;

namespace x_template_xHmi.Wpf.Views.Data.ProcessTraceability
{
    public class ProcessTraceabilityViewModel
    {
        public TcoDataExchangeDisplayViewModel ProcessData
        {
            get
            {
                return new TcoDataExchangeDisplayViewModel() { Model = App.x_template_xPlc.MAIN._technology._processTraceability };
            }           
        }
    }
}
