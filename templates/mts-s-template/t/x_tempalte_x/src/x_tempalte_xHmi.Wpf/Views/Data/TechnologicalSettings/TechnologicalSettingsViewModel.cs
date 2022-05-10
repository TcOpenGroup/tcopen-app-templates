using x_template_xPlc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x_template_xHmi.Wpf.Views.Data.TechnologicalSettings
{
    public class TechnologicalSettingsViewModel
    {
        public TechnologicalDataManager TechnologicalData
        {
            get
            {
                return App.x_template_xPlc.MAIN._technology._technologySettings;
            }           
        }
    }
}
