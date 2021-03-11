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

        [Fact]
        public void HandlesIOSProjectFile()
        {
            var filePath = new FileInfo(Path.Combine("Resources", "SampleiOSProject.xml"));
            var output = new DirectoryInfo(Path.Combine("test-gen", "SampleiOSProject", "bin"));
            CSProjFile proj = null;
            var ex = Record.Exception(() => proj = CSProjFile.Load(filePath, output, "iOS"));

#if WINDOWS_NT
            Assert.NotNull(ex);
            Assert.IsType<PlatformNotSupportedException>(ex);
#else
            Assert.Null(ex);
            Assert.NotNull(proj);
            Assert.IsType<iOSProjectFile>(proj);
#endif
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
