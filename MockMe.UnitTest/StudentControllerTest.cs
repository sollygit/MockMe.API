using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockMe.API;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MockMe.UnitTest
{
    [TestClass]
    public class StudentControllerTest
    {
        private static WebApplicationFactory<Startup> _factory;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Console.WriteLine(testContext.TestName);
            _factory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder => builder.UseSetting("https_port", "5001").UseEnvironment("Testing"));
        }

        [TestMethod]
        public async Task ShouldReturnSuccessResponse_SingleFileForm()
        {
            var client = _factory.CreateClient();

            const string filePath = "test.pdf";
            await File.WriteAllTextAsync(filePath, "test");

            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "StudentFile", Path.GetFileName(filePath));
            form.Add(new StringContent("789"), "FormId");
            form.Add(new StringContent("Reading"), "Courses");
            form.Add(new StringContent("Math"), "Courses");

            var response = await client.PostAsync("api/student/123/form", form);
            var json = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual("/api/student/123/form/789", response.Headers.Location?.AbsolutePath.ToLower());
            Assert.IsNotNull(json);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        }

        [TestMethod]
        public async Task ShouldReturnBadRequestIfFileFormatIsNotPdf_SingleFileForm()
        {
            var client = _factory.CreateClient();

            const string filePath = "test.txt";
            await File.WriteAllTextAsync(filePath, "test");

            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "StudentFile", Path.GetFileName(filePath));
            form.Add(new StringContent("789"), "FormId");
            form.Add(new StringContent("Reading"), "Courses");
            form.Add(new StringContent("Math"), "Courses");

            var response = await client.PostAsync("api/student/123/form", form);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.IsNull(response.Headers.Location?.AbsolutePath);
            var json = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("\"The file is not a PDF file.\"", json);
        }

        [TestMethod]
        public async Task ShouldReturnSuccessResponse_MultipleFiles()
        {
            var client = _factory.CreateClient();

            const string testFile1 = "test.pdf";
            await File.WriteAllTextAsync(testFile1, "test1111");
            const string testFile2 = "test2.txt";
            await File.WriteAllTextAsync(testFile2, "test2222222");
            const string testFile3 = "test3.xyz";
            await File.WriteAllTextAsync(testFile3, "test33333333");

            using var form = new MultipartFormDataContent();
            using var fileContent1 = new ByteArrayContent(await File.ReadAllBytesAsync(testFile1));
            using var fileContent2 = new ByteArrayContent(await File.ReadAllBytesAsync(testFile2));
            using var fileContent3 = new ByteArrayContent(await File.ReadAllBytesAsync(testFile3));
            form.Add(fileContent1, "Files", Path.GetFileName(testFile1));
            form.Add(fileContent2, "Files", Path.GetFileName(testFile2));
            form.Add(fileContent3, "Files", Path.GetFileName(testFile3));

            var response = await client.PostAsync("api/student/123/forms", form);
            var json = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.IsNotNull(json);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task ShouldDownloadFile()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("api/student/files/yyyyMMdd-HHmm");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/pdf", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual("attachment; filename=yyyyMMdd-HHmm.pdf; filename*=UTF-8''yyyyMMdd-HHmm.pdf", response.Content.Headers.ContentDisposition?.ToString());
            Assert.AreEqual("134106", response.Content.Headers.ContentLength?.ToString());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory.Dispose();
        }
    }
}
