using Xunit;
using MinimalAF.Rendering;
using MinimalAF;

namespace UnitTests
{
    public class StringIteratorTests
    {
        [Fact]
        public void StringIteraterCount() {
            string text = "a b c d";

            var iter = new StringIterator(text, " ");

            Assert.Equal(4, iter.Count);
            Assert.Equal(4, iter.Count);
            Assert.Equal(4, iter.Count);

            Assert.Equal(1, new StringIterator("asdsfasdf", " ").Count);
            Assert.Equal(0, new StringIterator("", " ").Count);
        }


        [Fact]
        public void StringIteraterIteration() {
            string text = "a b c d";

            var iter = new StringIterator(text, " ");
            Assert.True(iter.GetNext().ToString() == "a");
            Assert.True(iter.GetNext().ToString() == "b");
            Assert.True(iter.GetNext().ToString() == "c");
            Assert.True(iter.GetNext().ToString() == "d");

            Assert.True(iter.MoveNext());
        }


        [Fact]
        public void StringIteratorForeach() {
            string text = "a b c d";

            string[] arr = new string[] { "a", "b", "c", "d" };
            int i = 0;
            foreach (var s in new StringIterator(text, " ")) {
                Assert.Equal(s.ToString(), arr[i]);
                i++;
            }
        }
    }
}