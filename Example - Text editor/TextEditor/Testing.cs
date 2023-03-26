using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor {
    class FailedAssertion {
        public string Message;
    }


    class TestContext {
        public bool Failed;
        List<FailedAssertion> failedAssertions = new List<FailedAssertion>();
        public List<FailedAssertion> FailedAssertions => failedAssertions;
        public string Name;

        public TestContext(string name) {
            Name = name;
        }

        public void Assert(bool fact, string message) {
            if (fact) return;

            FailedAssertions.Add(
                new FailedAssertion { Message = message }
            );
        }
    }


    internal class Testing {
        public List<TestContext> Results = new List<TestContext>();



        public void PrintResults() {
            int passed = 0, failed = 0;
            foreach(var res in Results) {
                if (!res.Failed) {
                    passed++;
                    continue;
                }

                failed++;

                Console.WriteLine($"\n[FAIL] ---- {res.Name} ---- ");
                foreach(var assertion in res.FailedAssertions) {
                    Console.WriteLine($"\t -> {assertion.Message}");
                }
            }

            Console.WriteLine($"\n\nPassed: {passed} \t\t Failed: {failed}");
        }

        public void Run(string testName, Action<TestContext> testFn) {
            var ctx = new TestContext(testName);
            try {
                testFn(ctx);
            } catch (Exception e) {
                ctx.FailedAssertions.Add(new FailedAssertion {
                    Message = $"Got an exception: ${e}"
                });
            }
        }
    }
}
