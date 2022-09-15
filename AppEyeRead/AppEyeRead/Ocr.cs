using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace AppEyeRead
{
    public static class Ocr
    {
        //private const string SubscriptionKey = " "; enter here the subscription key to the Microsoft Azure OCR service


       // private const string UriBase = ""; enter here the uri associated with the Microsoft Azure OCR service

        


        public static async Task<string> GetTextAsync(Stream imageFilePath)
        {
            try
            {
                using (var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(SubscriptionKey))
                { Endpoint = UriBase })
                {
                   
                    var result = await client.RecognizePrintedTextInStreamWithHttpMessagesAsync(true, imageFilePath, OcrLanguages.It);
                    return result.Body.GetText();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static string GetText(this OcrResult result)
        {
            var builder = new StringBuilder();

            foreach (var region in result.Regions)
            {
                foreach (var line in region.Lines)
                {
                    foreach (var word in line.Words)
                    {
                        builder.Append(word.Text);
                        builder.Append(" ");
                    }
                    builder.AppendLine();
                }
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
