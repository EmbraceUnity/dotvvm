﻿using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using DotVVM.Testing.Abstractions;
using Riganti.Utils.Testing.Selenium.Core;
using Riganti.Utils.Testing.Selenium.DotVVM;

namespace DotVVM.Samples.Tests.New
{
    public class FileUploadTests : AppSeleniumTest
    {
        public FileUploadTests(ITestOutputHelper output) : base(output)
        {
        }
        [Fact]
        [Timeout(120000)]
        public void Control_FileUpload_FileUpload()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_FileUpload_FileUpload);
                browser.Wait(1000);

                // get existing files
                var existingFiles = browser.FindElements("li").Select(e => e.GetText()).ToList();
                browser.Wait(1000);

                // generate a sample file to upload
                var tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, string.Join(",", Enumerable.Range(1, 100000)));

                // write the full path to the dialog

                ElementWrapperExtensions.UploadFile((ElementWrapper)browser.First(".dotvvm-upload-button a"), tempFile);

                // wait for the file to be uploaded

                browser.WaitFor(() => browser.First(".dotvvm-upload-files").GetText() == "1 files", 60000,
                    "File was not uploaded in 1 min interval.");

                //TODO: TestContext.WriteLine("The file was uploaded.");

                // submit
                browser.Click("input[type=button]");

                // verify the file is there present
                browser.WaitFor(
                    () =>
                        browser.First("ul").FindElements("li").FirstOrDefault(t => !existingFiles.Contains(t.GetText())) !=
                        null, 60000, "File was not uploaded correctly.");

                // delete the file
                var firstLi =
                    browser.First("ul").FindElements("li").FirstOrDefault(t => !existingFiles.Contains(t.GetText()));
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_FileUpload_FileUpload + "?delete=" + firstLi.GetText());

                // delete the temp file
                File.Delete(tempFile);
            });
        }

        [Fact()]
        [Timeout(120000)]
        [SampleReference(nameof(SamplesRouteUrls.ControlSamples_FileUpload_IsAllowedOrNot))]
        public void Control_FileUpload_IsAllowedOrNot_IsFileAllowed()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_FileUpload_IsAllowedOrNot);
                browser.Wait(1000);

                var isFileTypeAllowed = browser.Single("span.isFileTypeAllowed");
                var isMaxSizeExceeded = browser.Single("span.isMaxSizeExceeded");

                var textFile = CreateTempFile("txt", 1);
                ElementWrapperExtensions.UploadFile((ElementWrapper)browser.First(".dotvvm-upload-button a"), textFile);

                browser.WaitFor(() => browser.First(".dotvvm-upload-files").GetText() == "1 files", 60000,
                    "File was not uploaded in 1 min interval.");

                isFileTypeAllowed.CheckIfTextEquals("true");
                isMaxSizeExceeded.CheckIfTextEquals("false");

                File.Delete(textFile);
            });
        }

        [Fact]
        [Timeout(120000)]
        public void Control_FileUpload_IsFileNotAllowed()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_FileUpload_IsAllowedOrNot);
                browser.Wait(1000);

                var isFileTypeAllowed = browser.Single("span.isFileTypeAllowed");
                var isMaxSizeExceeded = browser.Single("span.isMaxSizeExceeded");

                var mdFile = CreateTempFile("md", 1);
                ElementWrapperExtensions.UploadFile((ElementWrapper)browser.First(".dotvvm-upload-button a"), mdFile);

                browser.WaitFor(() => browser.First(".dotvvm-upload-files").GetText() == "1 files", 60000,
                    "File was not uploaded in 1 min interval.");

                isFileTypeAllowed.CheckIfTextEquals("false");
                isMaxSizeExceeded.CheckIfTextEquals("false");

                File.Delete(mdFile);
            });
        }

        [Fact]
        [Timeout(120000)]
        [SampleReference(nameof(SamplesRouteUrls.ControlSamples_FileUpload_IsAllowedOrNot))]
        public void Control_FileUpload_IsAllowedOrNot_FileTooLarge()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_FileUpload_IsAllowedOrNot);
                browser.Wait(1000);

                var isFileTypeAllowed = browser.Single("span.isFileTypeAllowed");
                var isMaxSizeExceeded = browser.Single("span.isMaxSizeExceeded");

                var largeFile = CreateTempFile("txt", 2);
                ElementWrapperExtensions.UploadFile((ElementWrapper)browser.First(".dotvvm-upload-button a"), largeFile);

                browser.WaitFor(() => browser.First(".dotvvm-upload-files").GetText() == "1 files", 60000,
                    "File was not uploaded in 1 min interval.");

                isFileTypeAllowed.CheckIfTextEquals("true");
                isMaxSizeExceeded.CheckIfTextEquals("true");

                File.Delete(largeFile);
            });
        }

        [Fact]
        [Timeout(120000)]
        public void Control_FileUpload_FileSize()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_FileUpload_FileSize);
                browser.Wait(1000);

                var fileSize = browser.Single("span.fileSize");

                var file = CreateTempFile("txt", 2);
                ElementWrapperExtensions.UploadFile((ElementWrapper)browser.First(".dotvvm-upload-button a"), file);

                browser.WaitFor(() => browser.First(".dotvvm-upload-files").GetText() == "1 files", 60000,
                    "File was not uploaded in 1 min interval.");

                fileSize.CheckIfTextEquals("2 MB");

                File.Delete(file);
            });
        }

        private string CreateTempFile(string extension, long size)
        {
            var tempFile = Path.GetTempFileName();
            tempFile = Path.ChangeExtension(tempFile, extension);

            using (var fs = new FileStream(tempFile, FileMode.CreateNew))
            {
                fs.SetLength(size * 1024 * 1024);
            }

            return tempFile;
        }

        //TODO: FileUpload with UploadCompleted command

        // TODO: RenderSettings.Mode="Server"

    }
}