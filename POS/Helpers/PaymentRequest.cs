using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Net.Http;


namespace POS.Helpers
{
    /// <summary>
    /// Class PaymentRequest: Gửi request thanh toán đến MoMo
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// Khởi tạo một đối tượng PaymentRequest
        /// </summary>
        public PaymentRequest()
        {
        }

        /// <summary>
        /// Hàm tạo chuỗi hash từ chuỗi dữ liệu
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="postJsonString"></param>
        /// <returns></returns>
        public static async Task<string> sendPaymentRequest(string endpoint, string postJsonString)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMilliseconds(15000);
                    var content = new StringContent(postJsonString, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(endpoint, content);

                    response.EnsureSuccessStatusCode();

                    string jsonresponse = await response.Content.ReadAsStringAsync();

                    //todo parse it
                    return jsonresponse;
                    //return new MomoResponse(mtid, jsonresponse);
                }
            }
            catch (HttpRequestException e)
            {
                return e.Message;
            }
        }
    }
}
