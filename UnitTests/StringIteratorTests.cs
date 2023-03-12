//using Xunit;
//using MinimalAF.Rendering;
//using MinimalAF;

//namespace UnitTests
//{
//    public class StringIteratorTests
//    {
//        [Fact]
//        public void StringIteraterCount() {
//            string text = "a b c d";

//            var iter = new StringIterator(text, " ");

//            Assert.Equal(4, iter.Count);
//            Assert.Equal(4, iter.Count);
//            Assert.Equal(4, iter.Count);

//            Assert.Equal(1, new StringIterator("asdsfasdf", " ").Count);
//            Assert.Equal(0, new StringIterator("", " ").Count);
//            Assert.Equal(4, new StringIterator("///", "/").Count);
//        }


//        [Fact]
//        public void StringIteraterIteration() {
//            string text = "a b c d";

//            var iter = new StringIterator(text, " ");
//            Assert.True(iter.GetNext().ToString() == "a");
//            Assert.True(iter.GetNext().ToString() == "b");
//            Assert.True(iter.GetNext().ToString() == "c");
//            Assert.True(iter.GetNext().ToString() == "d");

//            Assert.False(iter.MoveNext());
//        }


//        [Fact]
//        public void StringIteratorForeach() {
//            string text = "a b c d";

//            string[] arr = new string[] { "a", "b", "c", "d" };
//            int i = 0;
//            foreach (var s in new StringIterator(text, " ")) {
//                Assert.Equal(arr[i], s.ToString());
//                i++;
//            }
//            Assert.Equal(arr.Length, i);
//        }


//        [Fact]
//        public void StringIteratorForeachNewlines() {
//            string text = @"a
//b
//c
//d";

//            string[] arr = new string[] { "a\r", "b\r", "c\r", "d" };
//            int i = 0;
//            foreach (var s in new StringIterator(text, "\n")) {
//                Assert.Equal(arr[i], s.ToString());
//                i++;
//            }
//            Assert.Equal(arr.Length, i);
//        }


//        [Fact]
//        public void StringIteratorForeachDontSkipEmpty() {
//            string text = "/6//7/";

//            string[] arr = new string[] { "", "6", "", "7", "" };
//            int i = 0;
//            foreach (var s in new StringIterator(text, "/", false)) {
//                Assert.Equal(s.ToString(), arr[i]);
//                i++;
//            }
//            Assert.Equal(arr.Length, i);
//        }
//    }
//}