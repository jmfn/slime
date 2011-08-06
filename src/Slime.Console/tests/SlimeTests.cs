using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Slime.tests {
    [TestFixture]
    public class SlimeTests {
        [Test]
        public void foo() {
            var slimer = new Slimer("http://aol.com", 1024, 768);
            //var slimer = new Slimer("http://jimchely.com", 1024, 768);
            slimer.GetRender(true);
        }
    }
}
