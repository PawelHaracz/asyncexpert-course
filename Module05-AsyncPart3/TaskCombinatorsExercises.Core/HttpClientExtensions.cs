using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskCombinatorsExercises.Core
{
    public static class HttpClientExtensions
    {
        /*
         Write cancellable async method with timeout handling, that concurrently tries to get data from
         provided urls (first wins and its response is returned, rest is __cancelled__).
         
         Tips:
         * consider using HttpClient.GetAsync (as it is cancellable)
         * consider using Task.WhenAny
         * you may use urls like for testing https://postman-echo.com/delay/3
         * you should have problem with tasks cancellation -
            - how to merge tokens of operations (timeouts) with the provided token? 
            - Tip: you can link tokens with the help of CancellationTokenSource.CreateLinkedTokenSource(token)
         */
        public static async Task<string> ConcurrentDownloadAsync(this HttpClient httpClient,
            string[] urls, int millisecondsTimeout, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            
            var tasks = urls.Select(u => httpClient.GetAsync(u, token)).ToArray();
            var timeout = Task.Delay(millisecondsTimeout, token);
            var httpTask = Task.WhenAny(tasks);
            var winTask = await Task.WhenAny(timeout, httpTask);
            if (winTask == timeout)
            {
                throw new TaskCanceledException("Timout");
            }
            
            using var result = await await httpTask;
            await using var stream = await result.Content.ReadAsStreamAsync();
            using var stringReader = new StreamReader(stream, Encoding.UTF8);
            var bytes = new byte[stream.Length];
            var memory = new Memory<byte>(bytes);
            _ = await stream.ReadAsync(memory, token);
            
            return Encoding.UTF8.GetString(memory.Span);
        }
    }
}
