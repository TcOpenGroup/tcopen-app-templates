using System;
using TcOpen.Inxton.Data;

namespace Vortex.Framework.Data.Tests
{
    public class TestData : TcoData.PlainTcoEntity
    {
        public dynamic _recordId { get; set; }
        public DateTime _Created { get; set; }
        public string _EntityId { get; set; }
        public DateTime _Modified { get; set; }

        public CustomCheckerDataChangeable customChecker1 { get; set; } = new CustomCheckerDataChangeable();
        public CustomCheckerDataChangeable customChecker2 { get; set; } = new CustomCheckerDataChangeable();

        public CustomCheckerDataNotChangeable customChecker3 { get; set; } = new CustomCheckerDataNotChangeable();
        public CustomCheckerDataNotChangeable customChecker4 { get; set; } = new CustomCheckerDataNotChangeable();
       
    }

    public class CustomCheckerDataChangeable
    {
        public float Minimum { get; set; }
        public ushort NumberOfAllowedRetries { get; set; }
        public string FailureDescription { get; set; }
        public bool IsByPassed { get; set; }
        public bool IsExcluded { get; set; }
        [Vortex.Connector.EnumeratorDiscriminatorAttribute(typeof(TcoInspectors.eInspectorResult))]
        [RenderIgnoreAttribute(new[] { "Control", "ShadowControl" })]
        public short Result { get; set; }
        [TimeFormatAttribute("ss.fff")]
        public TimeSpan FailedTime { get; set; }
        [TimeFormatAttribute("ss.fff")]
        public TimeSpan PassedTime { get; set; }
        public float Maximum { get; set; }
        [RenderIgnoreAttribute(new[] { "Control", "ShadowControl" })]
        public float Measured { get; set; }
        public string ErrorCode { get; set; }
        [RenderIgnoreAttribute(new[] { "Control", "ShadowControl" })]
        public DateTime TimeStamp { get; set; }
        public ushort RetryAttemptsCount { get; set; }
    }

    public class CustomCheckerDataNotChangeable
    {
        public float Minimum { get; set; }
        public ushort NumberOfAllowedRetries { get; set; }
        public string FailureDescription { get; set; }
        public bool IsByPassed { get; set; }
        public bool IsExcluded { get; set; }
        [Vortex.Connector.EnumeratorDiscriminatorAttribute(typeof(TcoInspectors.eInspectorResult))]
        [RenderIgnoreAttribute(new[] { "Control", "ShadowControl" })]
        public short Result { get; set; }
        [TimeFormatAttribute("ss.fff")]
        public TimeSpan FailedTime { get; set; }
        [TimeFormatAttribute("ss.fff")]
        public TimeSpan PassedTime { get; set; }
        public float Maximum { get; set; }
        [RenderIgnoreAttribute(new[] { "Control", "ShadowControl" })]
        public float Measured { get; set; }
        public string ErrorCode { get; set; }
        [RenderIgnoreAttribute(new[] { "Control", "ShadowControl" })]
        public DateTime TimeStamp { get; set; }
        public ushort RetryAttemptsCount { get; set; }
    }
}
