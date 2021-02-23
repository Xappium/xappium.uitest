using System;
using System.IO;
using Xappium.BuildSystem;
using Xunit;

namespace Xappium.Cli.Tests
{
    public class CSProjTests
    {
        [Theory]
        [InlineData("SampleAndroidProject.xml", typeof(AndroidProjectFile))]
        [InlineData("SampleiOSProject.xml", typeof(iOSProjectFile))]
        [InlineData("SampleDotNetMultiTargetProject.xml", typeof(DotNetMauiProjectFile))]
        [InlineData("SampleConsoleApp.xml", typeof(DotNetSdkProjectFile))]
        public void FromFileGeneratesCorrectProjectType(string fileName, Type expectedProjectType)
        {
            var filePath = new FileInfo(Path.Combine("Resources", fileName));
            var output = new DirectoryInfo(Path.Combine("test-gen", Path.GetFileNameWithoutExtension(fileName), "bin"));
            CSProjFile proj = null;
            var ex = Record.Exception(() => proj = CSProjFile.Load(filePath, output, "iOS"));

            Assert.Null(ex);
            Assert.NotNull(proj);
            Assert.IsType(expectedProjectType, proj);
        }

        [Theory]
        [InlineData("SampleUwpProject.xml")]
        [InlineData("SampleNetStandardProject.xml")]
        public void ThrowsPlatformNotSupportedException(string fileName)
        {
            var filePath = new FileInfo(Path.Combine("Resources", fileName));
            var output = new DirectoryInfo(Path.Combine("test-gen", Path.GetFileNameWithoutExtension(fileName), "bin"));

            CSProjFile proj = null;
            var ex = Record.Exception(() => proj = CSProjFile.Load(filePath, output, "Foo"));

            Assert.NotNull(ex);
            Assert.Null(proj);

            Assert.IsType<PlatformNotSupportedException>(ex);
        }
    }
}
