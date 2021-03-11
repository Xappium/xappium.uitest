using System;
using System.IO;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests
{
    public class TrxReaderTests
    {
        private static readonly FileInfo TrxFile = new FileInfo(Path.Combine("Resources", "SampleTrx.xml"));

        public TrxReaderTests()
        {
            TestEnvironmentHost.Init();
        }

        [Fact]
        public void DoesNotThrowException()
        {
            var ex = Record.Exception(() => TrxReader.Load(TrxFile));
            Assert.Null(ex);
        }

        [Fact]
        public void ContainsTwoTestDefinitions()
        {
            var trx = TrxReader.Load(TrxFile);
            Assert.Equal(2, trx.TestDefinitions.UnitTest.Count);
        }

        [Fact]
        public void TwoTestsPassed()
        {
            var trx = TrxReader.Load(TrxFile);
            Assert.Equal(2, trx.ResultSummary.Counters.Passed);
        }
    }
}
