using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlessingSoftware.Controls.Rendering;


namespace Controls.Test {
    [TestClass]
    public class TestDameer {
        [TestMethod]
        public void TestFormatProperty() {
            var k = new DameerTextBox();
            k.Format = "dddd, MMMM dd, yyyy";
            Assert.AreEqual(k.Format , "dddd, MMMM dd, yyyy");
            Assert.AreEqual(k.m_blocks.Count, 4);
        }
    }
}
