using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskCompletionSourceExercises.Core
{
    public class AsyncTools
    {
        public static Task<string> RunProgramAsync(string path, string args = "")
        {
            var tcs = new TaskCompletionSource<string>();
            
            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(path, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
            process.Exited += (sender, eventArgs) =>
            {
                
                var senderProcess = sender as Process;

                if (senderProcess is null)
                {
                    tcs.SetException(new NullReferenceException());
                    return;
                }
                
                if (senderProcess.ExitCode == 0)
                {
                    tcs.SetResult(senderProcess.StandardOutput.ReadToEnd());
                }
                else
                {
                    tcs.SetException(new Exception(senderProcess.StandardError.ReadToEnd()));
                }

                senderProcess?.Dispose();
            };
            
            process.Start();
            
            return tcs.Task;
        }
    }
}
