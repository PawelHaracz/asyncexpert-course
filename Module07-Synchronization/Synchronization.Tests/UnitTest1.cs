using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Synchronization.Tests
{
    public class UnitTest1
    {
        private const string  Path = @"../../../../../Synchronization/Release/Synchronization";
        [Fact]
        public async Task GivenExampleApp_WhenLocalExclusiveScope_ThenSucceeds()
        {

            var result = await RunProgramAsync(Path, "name false");

            Assert.Equal("Hello world!\n", result);
        }

        [Fact]
        public async Task GivenExampleApp_WhenSingleGlobalExclusiveScope_ThenSucceeds()
        {

            var result = await RunProgramAsync(Path, "name false");

            Assert.Equal("Hello world!\n", result);
        }

        [Fact]
        public async Task GivenExampleApp_WhenDoubleGlobalExclusiveScope_ThenThrows()
        {
            var scopeName = "someScopeName";
            
            var firstRunTask = RunProgramAsync(Path, $"{scopeName} true");
            var exception = await Record.ExceptionAsync(async () =>
                await RunProgramAsync(Path, $"{scopeName} true"));
            await firstRunTask;

            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
            Assert.StartsWith($"Unhandled exception. System.InvalidOperationException: Unable to get a global lock {scopeName}.", 
                exception.Message);

        }

        public static Task<string> RunProgramAsync(string path, string args = "")
        {
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            var process = new Process();
            process.EnableRaisingEvents = true;
            process.StartInfo = new ProcessStartInfo(path, args)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            process.Exited += async (sender, eventArgs) =>
            {
                var senderProcess = sender as Process;
                if (senderProcess is null)
                    return;
                if (senderProcess.ExitCode != 0)
                {
                    var output = await process.StandardError.ReadToEndAsync();
                    tcs.SetException(new Exception(output));
                }
                else
                {
                    var output = await process.StandardOutput.ReadToEndAsync();
                    tcs.SetResult(output);
                }
                process.Dispose();
            };
            process.Start();
            return tcs.Task;
        }
    }
}
