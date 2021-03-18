using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xappium.Logging;

namespace Xappium.Tools
{
    internal static class TrxReader
    {
        public static TestRun Load(FileInfo fileInfo)
        {
            var xml = File.ReadAllText(fileInfo.FullName);
            XmlSerializer serializer = new XmlSerializer(typeof(TestRun), "http://microsoft.com/schemas/VisualStudio/TeamTest/2010");
            using StringReader reader = new StringReader(xml);
            return (TestRun)serializer.Deserialize(reader);
        }

        public static void LogReport(this TestRun testRun)
        {
            if (testRun is null)
                return;

            Logger.WriteLine($"Test Run: {testRun.ResultSummary.Outcome}", LogLevel.Minimal);
            var counters = testRun.ResultSummary.Counters;
            Logger.WriteLine($"Total: {counters.Total}", LogLevel.Minimal);

            if (counters.Total > 0)
            {
                Logger.WriteLine($"Passed: {counters.Passed}", LogLevel.Minimal);

                if (counters.Timeout > 0)
                    Logger.WriteWarning($"Timed Out: {counters.Timeout}");

                if (counters.NotExecuted > 0)
                    Logger.WriteWarning($"Not Executed: {counters.NotExecuted}");

                if (counters.Error > 0)
                    Logger.WriteError($"Errors: {counters.Error}");

                if (counters.Failed > 0)
                    Logger.WriteError($"Failed: {counters.Failed}");

                if (counters.Error > 0 || counters.Failed > 0)
                    Logger.WriteError(testRun.ResultSummary.Output.StdOut);
            }
        }
    }

    [XmlRoot(ElementName="Times", Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public class Times
    {
        [XmlAttribute(AttributeName="creation")]
        public DateTime Creation { get; set; }

        [XmlAttribute(AttributeName="queuing")]
        public DateTime Queuing { get; set; }

        [XmlAttribute(AttributeName="start")]
        public DateTime Start { get; set; }

        [XmlAttribute(AttributeName="finish")]
        public DateTime Finish { get; set; }
    }

    [XmlRoot(ElementName="Deployment")]
    public class Deployment
    {
        [XmlAttribute(AttributeName="runDeploymentRoot")]
        public string RunDeploymentRoot { get; set; }
    }

    [XmlRoot(ElementName="TestSettings")]
    public class TestSettings
    {
        [XmlElement(ElementName="Deployment")]
        public Deployment Deployment { get; set; }

        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName="id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName="UnitTestResult")]
    public class UnitTestResult
    {
        [XmlAttribute(AttributeName="executionId")]
        public string ExecutionId { get; set; }

        [XmlAttribute(AttributeName="testId")]
        public string TestId { get; set; }

        [XmlAttribute(AttributeName="testName")]
        public string TestName { get; set; }

        [XmlAttribute(AttributeName="computerName")]
        public string ComputerName { get; set; }

        [XmlAttribute(AttributeName="duration")]
        public DateTime Duration { get; set; }

        [XmlAttribute(AttributeName="startTime")]
        public DateTime StartTime { get; set; }

        [XmlAttribute(AttributeName="endTime")]
        public DateTime EndTime { get; set; }

        [XmlAttribute(AttributeName="testType")]
        public string TestType { get; set; }

        [XmlAttribute(AttributeName="outcome")]
        public string Outcome { get; set; }

        [XmlAttribute(AttributeName="testListId")]
        public string TestListId { get; set; }

        [XmlAttribute(AttributeName="relativeResultsDirectory")]
        public string RelativeResultsDirectory { get; set; }
    }

    [XmlRoot(ElementName="Results")]
    public class Results
    {
        [XmlElement(ElementName="UnitTestResult")]
        public List<UnitTestResult> UnitTestResult { get; set; }
    }

    [XmlRoot(ElementName="Execution")]
    public class Execution
    {
        [XmlAttribute(AttributeName="id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName="TestMethod")]
    public class TestMethod
    {
        [XmlAttribute(AttributeName="codeBase")]
        public string CodeBase { get; set; }

        [XmlAttribute(AttributeName="adapterTypeName")]
        public string AdapterTypeName { get; set; }

        [XmlAttribute(AttributeName="className")]
        public string ClassName { get; set; }

        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName="UnitTest")]
    public class UnitTest
    {
        [XmlElement(ElementName="Execution")]
        public Execution Execution { get; set; }

        [XmlElement(ElementName="TestMethod")]
        public TestMethod TestMethod { get; set; }

        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName="storage")]
        public string Storage { get; set; }

        [XmlAttribute(AttributeName="id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName="TestDefinitions")]
    public class TestDefinitions
    {
        [XmlElement(ElementName="UnitTest")]
        public List<UnitTest> UnitTest { get; set; }
    }

    [XmlRoot(ElementName="TestEntry")]
    public class TestEntry
    {
        [XmlAttribute(AttributeName="testId")]
        public string TestId { get; set; }

        [XmlAttribute(AttributeName="executionId")]
        public string ExecutionId { get; set; }

        [XmlAttribute(AttributeName="testListId")]
        public string TestListId { get; set; }
    }

    [XmlRoot(ElementName="TestEntries")]
    public class TestEntries
    {
        [XmlElement(ElementName="TestEntry")]
        public List<TestEntry> TestEntry { get; set; }
    }

    [XmlRoot(ElementName="TestList")]
    public class TestList
    {
        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName="id")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName="TestLists")]
    public class TestLists
    {
        [XmlElement(ElementName="TestList")]
        public List<TestList> TestList { get; set; }
    }

    [XmlRoot(ElementName="Counters")]
    public class Counters
    {
        [XmlAttribute(AttributeName="total")]
        public int Total { get; set; }

        [XmlAttribute(AttributeName="executed")]
        public int Executed { get; set; }

        [XmlAttribute(AttributeName="passed")]
        public int Passed { get; set; }

        [XmlAttribute(AttributeName="failed")]
        public int Failed { get; set; }

        [XmlAttribute(AttributeName="error")]
        public int Error { get; set; }

        [XmlAttribute(AttributeName="timeout")]
        public int Timeout { get; set; }

        [XmlAttribute(AttributeName="aborted")]
        public int Aborted { get; set; }

        [XmlAttribute(AttributeName="inconclusive")]
        public int Inconclusive { get; set; }

        [XmlAttribute(AttributeName="passedButRunAborted")]
        public int PassedButRunAborted { get; set; }

        [XmlAttribute(AttributeName="notRunnable")]
        public int NotRunnable { get; set; }

        [XmlAttribute(AttributeName="notExecuted")]
        public int NotExecuted { get; set; }

        [XmlAttribute(AttributeName="disconnected")]
        public int Disconnected { get; set; }

        [XmlAttribute(AttributeName="warning")]
        public int Warning { get; set; }

        [XmlAttribute(AttributeName="completed")]
        public int Completed { get; set; }

        [XmlAttribute(AttributeName="inProgress")]
        public int InProgress { get; set; }

        [XmlAttribute(AttributeName="pending")]
        public int Pending { get; set; }
    }

    [XmlRoot(ElementName="Output")]
    public class Output
    {
        [XmlElement(ElementName="StdOut")]
        public string StdOut { get; set; }
    }

    [XmlRoot(ElementName="ResultSummary")]
    public class ResultSummary
    {
        [XmlElement(ElementName="Counters")]
        public Counters Counters { get; set; }

        [XmlElement(ElementName="Output")]
        public Output Output { get; set; }

        [XmlAttribute(AttributeName="outcome")]
        public string Outcome { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName="TestRun")]
    public class TestRun
    {
        [XmlElement(ElementName="Times")]
        public Times Times { get; set; }

        [XmlElement(ElementName="TestSettings")]
        public TestSettings TestSettings { get; set; }

        [XmlElement(ElementName="Results")]
        public Results Results { get; set; }

        [XmlElement(ElementName="TestDefinitions")]
        public TestDefinitions TestDefinitions { get; set; }

        [XmlElement(ElementName="TestEntries")]
        public TestEntries TestEntries { get; set; }

        [XmlElement(ElementName="TestLists")]
        public TestLists TestLists { get; set; }

        [XmlElement(ElementName="ResultSummary")]
        public ResultSummary ResultSummary { get; set; }

        [XmlAttribute(AttributeName="id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName="name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName="xmlns")]
        public string Xmlns { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
