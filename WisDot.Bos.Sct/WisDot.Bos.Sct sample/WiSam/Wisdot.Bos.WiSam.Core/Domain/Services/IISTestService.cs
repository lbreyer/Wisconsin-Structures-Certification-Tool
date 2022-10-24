using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Web;
using Wisdot.Bos.WiSam.Core.Domain.Models;
using System.Runtime.InteropServices;

namespace Wisdot.Bos.WiSam.Core.Domain.Services
{
    public class IISTestService
    {
        private static Guid FolderDownloads = new Guid("374DE290-123F-4565-9164-39C4925E467B");
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHGetKnownFolderPath(ref Guid id, int flags, IntPtr token, out IntPtr path);


        public static string GetDownloadsPath()
        {
            if (Environment.OSVersion.Version.Major < 6) throw new NotSupportedException();
            IntPtr pathPtr = IntPtr.Zero;
            try
            {
                SHGetKnownFolderPath(ref FolderDownloads, 0, IntPtr.Zero, out pathPtr);
                return Marshal.PtrToStringUni(pathPtr);
            }
            finally
            {
                Marshal.FreeCoTaskMem(pathPtr);
            }
        }

        public async void GetStandardPlan(string spanLength, string substructureSkew, string clearRoadwayWidth, string barrierType,
            string pavingNotch, string abutmentHeight, string pilingType, string fiipsConstructionId, string fiipsDesignId, string fiipsStructureId)
        {
            try
            {
                UriBuilder uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "https";
                uriBuilder.Host = "iisgtwyp.wi.gov";
                string appContext = "/wisp";

                float length = Convert.ToSingle(spanLength);
                float skew = Convert.ToSingle(substructureSkew.Split(' ')[0]);
                float width = Convert.ToSingle(clearRoadwayWidth);
                bool hasPavingNotch = pavingNotch.Equals("true") ? true : false;
                float height = Convert.ToSingle(abutmentHeight);

                // NOTE: Currently set to pull from renamed folder [SKEW]R 
                string planFilePath = String.Format("{0}R/{1}_{2}_{3}_{4}", spanLength, spanLength, skew.ToString(), clearRoadwayWidth, barrierType);
                planFilePath += String.Format("_{0}notch_{1}_{2}.zip", hasPavingNotch ? "yes" : "no", abutmentHeight, pilingType);

                string planRelPath = String.Format("{0}/{1}", appContext, planFilePath);
                uriBuilder.Path = planRelPath;

                string planFileName = Path.GetFileName(planRelPath);

                string contentType = MimeMapping.GetMimeMapping(planRelPath);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uriBuilder.ToString());
                    client.DefaultRequestHeaders.Accept.Clear();

                    // For Basic Authentication http header - "username:password"
                    var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "S_DOT9WISP", CryptorEngine.Decrypt("xfrWHHFehrXK4wWZy/rCCFRq2EU9hJu7", true)));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                    HttpResponseMessage response = await client.GetAsync(uriBuilder.ToString());


                    // Start of Forms-Specific Download Method
                    string path = Path.Combine(GetDownloadsPath(), planFileName);
                    using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                    {
                        response.Content.ReadAsStreamAsync().Result.CopyTo(outputFileStream); 
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

    }
}
