using CloudFlareUtilities;
using HtmlAgilityPack;
using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
namespace ConsoleApp1
{
public class htmlDownloader
    {


    
            public static async Task DownloadHtmlsFromTxt()
            {
                string exePath = AppContext.BaseDirectory;
                string urlListPath = Path.Combine(Directory.GetParent(exePath).Parent.Parent.FullName, "OutputDocuments", "urls.txt");
                string saveFolder = Path.Combine(Directory.GetParent(exePath).Parent.Parent.FullName, "OutputDocuments","downloadedHtml" ,"downloaded_html");

                Directory.CreateDirectory(saveFolder);

                string[] urls = File.ReadAllLines(urlListPath);

                var handler = new ClearanceHandler
                {
                    MaxRetries = 3 // Cloudflare challenge deneme sayısı
                };

                HttpClient client = new HttpClient(handler);

                for (int i = 0; i < urls.Length; i++)
                {
                    string url = urls[i];
                    try
                    {
                        string html = await client.GetStringAsync(url);

                        // HTML’i kaydet
                        string fileName = Path.Combine(saveFolder, $"page_{i + 1}.html");
                        File.WriteAllText(fileName, html);
                        Console.WriteLine("İndirildi: " + url);

                        // HtmlAgilityPack ile parse
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        var titles = doc.DocumentNode.SelectNodes("//div[@class='updates-element-info ml flex flex-col justify-space-between full-width']//h2/a");
                        if (titles != null)
                        {
                            foreach (HtmlNode t in titles)
                            {
                                Console.WriteLine(t.InnerText.Trim());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hata: " + url + " → " + ex.Message);
                    }

                    // Cloudflare rate-limit önlemek için bekleme
                    await Task.Delay(2000);
                }
            }
        }
    }
