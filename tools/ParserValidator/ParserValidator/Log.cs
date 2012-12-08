using System;

namespace ParserValidator.Source.Common
{
    partial class Log
    {
        public static int SuccessCount;
        public static int WarningCount;
        public static int ErrorCount;

        static partial void Partial_LogLevel(Log.Level level)
        {
            switch (level)
            {
                case Level.Success:
                    ++SuccessCount;
                    break;
                case Level.HighLight:
                    break;
                case Level.Info:
                    break;
                case Level.Error:
                case Level.Exception:
                    ++ErrorCount;
                    break;
                default:
                case Level.Warning:
                    ++WarningCount;
                    break;
            }
        }
    }
}