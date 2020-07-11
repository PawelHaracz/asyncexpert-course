using System;
using System.Threading.Tasks;
using TaskCompletionSourceExercises.Core;
using Xunit;

namespace TaskCompletionSourceExercises.Tests
{
    public class AsyncToolsTests
    {
        [Fact]
        public async Task GivenExampleAppRequiringArguments_WhenNoArguments_ThenThrows()
        {
            var path = @"../../../../ExampleApp/Release/ExampleApp";
            var exception = await Record.ExceptionAsync(async () =>
                await AsyncTools.RunProgramAsync(path));

            Assert.NotNull(exception);
            Assert.IsType<Exception>(exception);
            Assert.StartsWith("Unhandled exception. System.Exception: Missing program argument.", exception.Message);
        }

        [Fact]
        public async Task GivenExampleAppRequiringArguments_WhenHaveArguments_ThenSucceeds()
        {
            var path = @"../../../../ExampleApp/Release/ExampleApp";
            
            var result = await AsyncTools.RunProgramAsync(path, "argument");

            Assert.Equal("Hello argument!\nGoodbye.\n", result);
        }
    }
}
