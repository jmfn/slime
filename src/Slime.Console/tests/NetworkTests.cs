using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Slime.tests {
    [TestFixture]
    public class NetworkTests {
        public Slimer Slimer { get; set; }

        [SetUp]
        public void SetUp() {
        }

        [TearDown]
        public void TearDown() {
            Slimer.Dispose();
        }

        [Test]
        public void GetNetworkResources_Returns_InitialGETRequestURL() {
            Slimer = new Slimer("http://jimchely.com/", 1024, 768);
            var resources = Slimer.GetNetworkResources();
            var item = resources.Contains("http://jimchely.com/");
            Assert.IsTrue(item);
        }

        [Test]
        public void TestComscoreHomepage_Fires_Beacon() {
            Slimer = new Slimer("http://www.comscore.com/", 1024, 768);
            var resources = Slimer.GetNetworkResources();
            var item = resources.Select(u => u.StartsWith("http://b.scorecardresearch.com/b?")).Count();
            Assert.GreaterThan(item, 0);
        }
    }
}
