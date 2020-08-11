using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pipelines
{
    public class PipeLineCounter
    {
        private const byte NewLineCharacterByte = (byte) '\n';
        public async Task<int> CountLines(Uri uri)
        {
            var countOfLines = 0;
            using var client = new HttpClient();
            await using var stream = await client.GetStreamAsync(uri);
            
            var pipeReader = PipeReader.Create(stream);
            while (true)
            {
                var result = await pipeReader.ReadAsync();
                var buffer = result.Buffer;
                SequencePosition? position;
                do
                {
                    position = buffer.PositionOf(NewLineCharacterByte);
                    if (position != null)
                    {
                        countOfLines++;
                        var next = buffer.GetPosition(1, position.Value);
                        buffer = buffer.Slice(next);
                    }
                } while (position != null);
                
                pipeReader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted) break;
            }
            await pipeReader.CompleteAsync();
            
            return countOfLines;
        }
    }
}
