using System.Linq;
using MbUnit.Framework;

namespace Slime.tests {
    [TestFixture]
    public class SourceTests {
        public Slimer Slimer { get; set; }

        [SetUp]
        public void SetUp() {
        }

        [TearDown]
        public void TearDown() {
            Slimer.Dispose();
        }

        [Test]
        public void Sources_Contains_HTML() {
            Slimer = new Slimer("http://jimchely.com/", 1024, 768, 0);
            var source = Slimer.GetHtml();
            Assert.Contains(source, "html");
        }
    }
}
