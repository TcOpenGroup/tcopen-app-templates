using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vortex.Connector;

namespace x_template_xHmi.Blazor
{
    public static class NameOrSymbolConverterBlazor
    {
        public static string Convert(IVortexElement val)
        {
            return val != null ? string.IsNullOrEmpty(val.AttributeName) ? val.GetSymbolTail() : val.AttributeName : "Missing object information";
        }

      
    }
}
