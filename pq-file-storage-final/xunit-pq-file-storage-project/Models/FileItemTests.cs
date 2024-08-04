using Xunit;
using DataAccessClassLibrary.Models;

namespace xunit_pq_file_storage_project.Models
{
    public class FileItemTests
    {
        [Fact]
        public void FileItemSettersAndGettersShouldWork()
        {
            var fileItem = new FileItem();
            var testFileName = "test.txt";
            var testIcon = "testicon.png";
            var testPath = "/path/to/file/test.txt";

            fileItem.Name = testFileName;
            fileItem.Icon = testIcon;
            fileItem.Path = testPath;

            // asserts
            Assert.Equal(testFileName, fileItem.Name);
            Assert.Equal(testIcon, fileItem.Icon);
            Assert.Equal(testPath, fileItem.Path);
        }

        [Fact]
        public void FileItemConstructorShouldInitializePropertiesToNull()
        {
            var fileItem = new FileItem();

            Assert.Null(fileItem.Name);
            Assert.Null(fileItem.Icon);
            Assert.Null(fileItem.Path);
        }

        [Fact]
        public void FileItemConstructorShouldInitProperties()
        {
            var testFileName = "test.txt";
            var testIcon = "testicon.png";
            var testPath = "/path/to/file/test.txt";

            var fileItem = new FileItem
            {
                Name = testFileName,
                Icon = testIcon,
                Path = testPath
            };

            Assert.Equal(testFileName, fileItem.Name);
            Assert.Equal(testIcon, fileItem.Icon);
            Assert.Equal(testPath, fileItem.Path);
        }
    }
}