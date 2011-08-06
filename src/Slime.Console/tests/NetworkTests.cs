using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Slime.tests {
    [TestFixture]
    public class NetworkTests {
        [Test]
        public void GetNetworkResources_Returns_InitialGETRequestURL() {
            var slimer = new Slimer("http://jimchely.com/", 1024, 768);
            string[] resources = slimer.GetNetworkResources();
            var item = resources.Contains("http://jimchely.com/");
            Assert.IsTrue(item);
        }
    }
}
