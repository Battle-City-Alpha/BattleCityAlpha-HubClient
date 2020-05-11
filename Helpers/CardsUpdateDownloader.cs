using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hub_client.Helpers
{
    public static class CardsUpdateDownloader
    {
        public static event Action<int, int> LoadingProgress;
        public static event Action UpdateCompleted;
        public static void DownloadUpdates(string[] updates)
        {
            int i = 0;
            int n = updates.Length;
            using (WebClient wc = new WebClient())
            {
                for (int u = updates.Length - 1; u >= 0; u--)
                {
                    i++;
                    Application.Current.Dispatcher.Invoke(() => LoadingProgress?.Invoke(i, n));
                    UnzipFromStream(wc.OpenRead(GetUpdateFileLink(updates[u])), FormExecution.path);
                }
            }

            Application.Current.Dispatcher.Invoke(() => UpdateCompleted?.Invoke());
            FormExecution.ClientConfig.CardsStuffVersion = FormExecution.GetLastVersion(updates);
            FormExecution.SaveConfig();
        }

        private static Uri GetUpdateFileLink(string updatename)
        {
            return new Uri("http://battlecityalpha.xyz/BCA/UPDATEV2/CardsStuff/zip/" + updatename + ".zip");
        }
        private static void UnzipFromStream(Stream zipStream, string outFolder)
        {
            using (var zipInputStream = new ZipInputStream(zipStream))
            {
                while (zipInputStream.GetNextEntry() is ZipEntry zipEntry)
                {
                    var entryFileName = zipEntry.Name;
                    // To remove the folder from the entry:
                    //var entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here
                    // to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    // 4K is optimum
                    var buffer = new byte[4096];

                    // Manipulate the output filename here as desired.
                    var fullZipToPath = Path.Combine(outFolder, entryFileName);
                    var directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Skip directory entry
                    if (Path.GetFileName(fullZipToPath).Length == 0)
                    {
                        continue;
                    }

                    // Unzip file in buffered chunks. This is just as fast as unpacking
                    // to a buffer the full size of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                    }
                }
            }
        }
    }
}
