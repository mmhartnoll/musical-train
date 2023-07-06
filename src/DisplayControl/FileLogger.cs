using DomainModel.Enumerations;
using DomainModel.Events;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DisplayControl
{
    internal class FileLogger : ILoggingService
    {
        private readonly SemaphoreSlim semaphoreSlim = new(1);

        private readonly string logFilePath;

        public FileLogger(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public void Log(DateTime timeStamp, LogLevel level, string message)
            => Task.Run(() => LogAsync(timeStamp, level, message)).Wait();

        public async Task LogAsync(DateTime timeStamp, LogLevel level, string message)
        {
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                string formattedMessage = $"\n[{timeStamp}]\n{message}\n";
                await File.AppendAllTextAsync(logFilePath, formattedMessage)
                    .ConfigureAwait(false);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}