using System;
using Vortex.Adapters.Connector.Tc3.Adapter;
using Vortex.Connector;

namespace x_template_xPlcConnector
{
    public class Entry
    {
        public static readonly string AmsId = Environment.GetEnvironmentVariable("Tc3Target");

        public static x_template_xPlc.x_template_xPlcTwinController Plc { get; }
            = new x_template_xPlc.x_template_xPlcTwinController(Tc3ConnectorAdapter.Create(AmsId, 851, false));

        private static x_template_xPlc.x_template_xPlcTwinController _plcDesign;
        public static x_template_xPlc.x_template_xPlcTwinController PlcDesign
        {
            get
            {
                if (_plcDesign == null) _plcDesign = new x_template_xPlc.x_template_xPlcTwinController(new ConnectorAdapter(typeof(DummyConnector)));
                return _plcDesign;
            }
        }
    }
}
