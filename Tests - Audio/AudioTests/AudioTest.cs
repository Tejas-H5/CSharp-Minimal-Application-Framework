using System;

namespace AudioEngineTests.AudioTests
{
    public abstract class AudioTest
    {
        public void RunTest()
        {
            string testName = GetType().Name;
            Console.WriteLine("\nNow running [" + testName + "]");
            Test();
            Console.WriteLine("[" + testName + "] has completed.\n");
        }

        public abstract void Test();
    }
}
