using Xunit;
using DataAccessClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xunit_pq_file_storage_project.Models
{
    public class FolderItemTests
    {
        [Fact]
        public void FolderItemSettersAndGettersShouldWork()
        {
            var folderItem = new FolderItem();
            var testFolderName = "testfolder";
            var testFolderIcon = "testicon.png";
            var testFolderPath = "/path/to/file/testfolder";

            var testFiles = new ObservableCollection<FileItem>
        {
            new FileItem
            {
                Name = "test.txt"
            }
        };
            var OtherTestFiles = new ObservableCollection<object>
        {
            new FileItem
            {
                Name = "test2.txt"
            },
            new FolderItem
            {
                Name = "subfolder"
            }
        };

            folderItem.Name = testFolderName;
            folderItem.Icon = testFolderIcon;
            folderItem.Path = testFolderPath;
            folderItem.Files = testFiles;
            folderItem.Items = OtherTestFiles;

            // asserts
            Assert.Equal(testFolderName, folderItem.Name);
            Assert.Equal(testFolderIcon, folderItem.Icon);
            Assert.Equal(testFolderPath, folderItem.Path);
            Assert.Equal(testFiles, folderItem.Files);
            Assert.Equal(OtherTestFiles, folderItem.Items);
        }

        [Fact]
        public void FolderItemConstructorShouldInitPropertiesToDefaultValues()
        {
            var folderItem = new FolderItem();

            // asserts
            Assert.Equal(string.Empty, folderItem.Name);
            Assert.Equal(string.Empty, folderItem.Icon);
            Assert.Equal(string.Empty, folderItem.Path);
            Assert.Empty(folderItem.Files);
            Assert.Empty(folderItem.Items);
        }

        // test for folder items themselves
        [Fact]
        public void FolderItemItemsShouldInformAboutPropertyChangedWhenSettingProperties()
        {
            var folderItem = new FolderItem();
            bool changedProperty = false;

            folderItem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(folderItem.Items))
                {
                    changedProperty = true;
                }
            };

            folderItem.Items = new ObservableCollection<object>
            {
                new FileItem
                {
                    Name = "test.txt"
                },
                new FolderItem
                {
                    Name = "TestFolder"
                }
            };

            // assert
            Assert.True(changedProperty);
        }

        // test for folder item files
        [Fact]
        public void FolderItemFilesShouldInformAboutPropertyChangedWhenSettingProperties()
        {
            var folderItem = new FolderItem();
            bool changedProperty = false;

            folderItem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(folderItem.Files))
                {
                    changedProperty = true;
                }
            };

            folderItem.Files = new ObservableCollection<FileItem>
            {
                new FileItem
                {
                    Name = "test.txt"
                },
                new FileItem
                {
                    Name = "test2.xlsx"
                }
            };

            // assert
            Assert.True(changedProperty);
        }

        // test for folder item name
        [Fact]
        public void FolderItemShouldInformAboutPropertyChangedForName()
        {
            var folderItem = new FolderItem();
            bool changedProperty = false;

            folderItem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(folderItem.Name))
                {
                    changedProperty = true;
                }
            };

            folderItem.Name = "Test folder name";

            // assert
            Assert.True(changedProperty);
        }

        // test for folder item icon
        [Fact]
        public void FolderItemShouldInformAboutPropertyChangedForIcon()
        {
            var folderItem = new FolderItem();
            bool changedProperty = false;

            folderItem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(folderItem.Icon))
                {
                    changedProperty = true;
                }
            };

            folderItem.Icon = "Test-icon.png";

            // assert
            Assert.True(changedProperty);
        }

        // test for folder item path
        [Fact]
        public void FolderItemShouldInformAboutPropertyChangedForPath()
        {
            var folderItem = new FolderItem();
            bool changedProperty = false;

            folderItem.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(folderItem.Path))
                {
                    changedProperty = true;
                }
            };

            folderItem.Path = "/test-path/";

            // assert
            Assert.True(changedProperty);
        }
    }
}