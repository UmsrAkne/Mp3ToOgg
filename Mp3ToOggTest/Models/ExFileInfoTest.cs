using System.IO;
using Mp3ToOgg.Models;
using NUnit.Framework;

namespace Mp3ToOggTest.Models
{
    [TestFixture]
    public class ExFileInfoTest
    {
        [Test]
        public void SetExtensionTest()
        {
            var f = new ExFileInfo(new FileInfo(@"temp\name.mp3"));
            Assert.That(f.Name, Is.EqualTo("name.mp3"));
            Assert.That(f.FileInfo.FullName, Is.EqualTo(new FileInfo(@"temp\name.mp3").FullName));

            f.SetExtension(".wav");
            Assert.That(f.Name, Is.EqualTo("name.wav"));
            Assert.That(f.FileInfo.FullName, Is.EqualTo(new FileInfo(@"temp\name.wav").FullName));
        }
    }
}