using DomainModel.Enumerations;
using DomainModel.Events;
using System;
using System.Threading.Tasks;

namespace DisplayControl
{
    internal interface ILoggingService
    {
        void Log(LogLevel level, string message)
            => Log(DateTime.Now, level, message);

        Task LogAsync(LogLevel level, string message)
            => LogAsync(DateTime.Now, level, message);

        void Log(DateTime timeStamp, LogLevel level, string message);

        Task LogAsync(DateTime timeStamp, LogLevel level, string message);
    }
}