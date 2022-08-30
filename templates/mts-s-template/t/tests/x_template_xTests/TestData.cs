using System;
using TcoInspectors;
using TcOpen.Inxton.Data;

namespace Vortex.Framework.Data.Tests
{
    public class TestData : TcoData.PlainTcoEntity
    {
     
        public DateTime _Created { get; set; }

        public DateTime _Modified { get; set; }

        public PlainTcoAnalogueInspectorData customChecker1 { get; set; } = new PlainTcoAnalogueInspectorData();
        public PlainTcoAnalogueInspectorData customChecker2 { get; set; } = new PlainTcoAnalogueInspectorData();

        public PlainTcoDigitalInspectorData customChecker3 { get; set; } = new PlainTcoDigitalInspectorData();
        public PlainTcoDigitalInspectorData customChecker4 { get; set; } = new PlainTcoDigitalInspectorData();
       
    }

}
