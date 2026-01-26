using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockMe.API;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MockMe.UnitTest
{
    [TestClass]
    public class FileControllerTest
    {
        private static WebApplicationFactory<Startup> _factory;

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _factory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => builder
                    .UseSetting("https_port", "5001")
                    .UseEnvironment("Testing"));
        }

        [TestMethod]
        public async Task Should_ReturnSuccessResponse_SingleFileUpload()
        {
            var client = _factory.CreateClient();

            const string filePath = "test.pdf";
            await File.WriteAllTextAsync(filePath, "test");

            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(new StringContent("101"), "templateId");
            form.Add(fileContent, "templateFile", Path.GetFileName(filePath));
            form.Add(new StringContent("reading"), "Courses");
            form.Add(new StringContent("math"), "Courses");

            var response = await client.PostAsync("api/file/8888/upload", form);
            var json = await response.Content.ReadAsStringAsync();

            Assert.IsNotNull(json);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual("/api/file/8888/upload", response.Headers.Location?.AbsolutePath.ToLower());
        }

        [TestMethod]
        public async Task Should_Return_BadRequest_SingleFileUpload()
        {
            var client = _factory.CreateClient();

            const string filePath = "test.txt";
            await File.WriteAllTextAsync(filePath, "test");

            using var form = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
            form.Add(fileContent, "templateFile", Path.GetFileName(filePath));
            form.Add(new StringContent("101"), "templateId");
            form.Add(new StringContent("Reading"), "Courses");
            form.Add(new StringContent("Math"), "Courses");

            var response = await client.PostAsync("api/file/8888/upload", form);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.IsNull(response.Headers.Location?.AbsolutePath);
            var json = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("\"The file is not supported.\"", json);
        }

        [TestMethod]
        public async Task Should_Return_SuccessResponse_MultiFileUpload()
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

            var response = await client.PostAsync("api/file/8888/uploads", form);
            var json = await response.Content.ReadAsStringAsync();

            Assert.IsNotNull(json);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.AreEqual("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual("/api/file/8888/uploads", response.Headers.Location?.AbsolutePath.ToLower());
        }

        [TestMethod]
        public async Task Should_Download_SingleFile()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("api/file/contoso.pdf");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/pdf", response.Content.Headers.ContentType?.ToString());
            Assert.AreEqual("attachment; filename=contoso.pdf; filename*=UTF-8''contoso.pdf", response.Content.Headers.ContentDisposition?.ToString());
            Assert.AreEqual(134106, response.Content.Headers.ContentLength);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _factory.Dispose();
        }
    }
}
