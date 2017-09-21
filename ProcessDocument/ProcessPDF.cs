using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using System.Text;
using ProcessDocument.Config;
using PdfSharpCore.Pdf.IO;

namespace ProcessDocument
{
    public class ProcessPDF
    {
        private static string _document;
        private static List<string> _apiKeys;
        private static string _uri;

        public ProcessPDF(string document, AppConfig config)
        {
            _document = document;
            _apiKeys = new List<string>();
            _apiKeys.Add(config.AzureSettings.Key1);
            _apiKeys.Add(config.AzureSettings.Key2);
            _uri = config.AzureSettings.Endpoint;
        }

        public string PDFCore(string filename)
        {
            string result = "";

            //var doc = new pdf

            return result;
        }

        /// <summary>
        /// Gets the text visible in the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        public async Task<string> MakeOCRRequestAsync(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKeys);

            // Request parameters.
            string requestParameters = "language=unk&detectOrientation=true";

            // Assemble the URI for the REST API Call.
            string uri = _uri + "/ocr?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = imageFilePath.GetImageAsByteArray();

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                Console.WriteLine("Making web call...");
                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Received response...");
                // Display the JSON response.
                Console.WriteLine("\nResponse:\n");
                var result = contentString.JsonPrettyPrint();
                Console.WriteLine(result);
                return result;
            }
        }


    }
}
