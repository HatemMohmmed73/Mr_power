using System;

namespace MR_power.Exceptions
{
    public class MRPowerException : Exception
    {
        public string ErrorCode { get; }
        public object AdditionalData { get; }

        public MRPowerException(string message) : base(message)
        {
        }

        public MRPowerException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public MRPowerException(string message, string errorCode, object additionalData) : base(message)
        {
            ErrorCode = errorCode;
            AdditionalData = additionalData;
        }

        public MRPowerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MRPowerException(string message, string errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
} 