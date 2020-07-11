using System;
using System.Runtime.CompilerServices;

namespace AwaitableExercises.Core
{
    public static class BoolExtensions
    {
        public static BoolAwaiter GetAwaiter(this bool val) => new BoolAwaiter(val);
    }

    public class BoolAwaiter : INotifyCompletion
    {
        private readonly bool _value;

        public BoolAwaiter(bool value)
        {
            _value = value;
        }
        public bool IsCompleted => true;
        public void OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }

        public bool GetResult() => _value;
    }
}
