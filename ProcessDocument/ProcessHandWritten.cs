using ProcessDocument.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ProcessDocument
{

    public class ProcessHandWritten
    {
        private static string _document;
        private static List<string> _apiKeys;
        private static string _uri;

        public ProcessHandWritten(string document, AppConfig config)
        {
            _document = document;
            _apiKeys = new List<string>();
            _apiKeys.Add(config.AzureSettings.Key1);
            _apiKeys.Add(config.AzureSettings.Key2);
            _uri = config.AzureSettings.Endpoint;
        }

        /// <summary>
        /// Gets the handwritten text from the specified image file by using the Computer Vision REST API.
        /// </summary>
        /// <param name="imageFilePath">The image file with handwritten text.</param>
        private static async void ReadHandwrittenText(string imageFilePath)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKeys);

            // Request parameter. Set "handwriting" to false for printed text.
            string requestParameters = "handwriting=true";

            // Assemble the URI for the REST API Call.
            string uri = _uri + "?" + requestParameters;

            HttpResponseMessage response = null;

            // This operation requrires two REST API calls. One to submit the image for processing,
            // the other to retrieve the text found in the image. This value stores the REST API
            // location to call to retrieve the text.
            string operationLocation = null;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(imageFilePath);
            ByteArrayContent content = new ByteArrayContent(byteData);

            // This example uses content type "application/octet-stream".
            // You can also use "application/json" and specify an image URL.
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            // The first REST call starts the async process to analyze the written text in the image.
            response = await client.PostAsync(uri, content);

            // The response contains the URI to retrieve the result of the process.
            if (response.IsSuccessStatusCode)
                operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
            else
            {
                // Display the JSON error data.
                Console.WriteLine("\nError:\n");
                Console.WriteLine(JsonPrettyPrint(await response.Content.ReadAsStringAsync()));
                return;
            }

            // The second REST call retrieves the text written in the image.
            //
            // Note: The response may not be immediately available. Handwriting recognition is an
            // async operation that can take a variable amount of time depending on the length
            // of the handwritten text. You may need to wait or retry this operation.
            //
            // This example checks once per second for ten seconds.
            string contentString;
            int i = 0;
            do
            {
                System.Threading.Thread.Sleep(1000);
                response = await client.GetAsync(operationLocation);
                contentString = await response.Content.ReadAsStringAsync();
                ++i;
            }
            while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

            if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
            {
                Console.WriteLine("\nTimeout error.\n");
                return;
            }

            // Display the JSON response.
            Console.WriteLine("\nResponse:\n");
            Console.WriteLine(JsonPrettyPrint(contentString));
        }
        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        

    }
}
