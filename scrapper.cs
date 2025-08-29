using CloudFlareUtilities;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class scrapper
    {
        string firstWebUrl= "https://demonicscans.org/lastupdates.php?list=numara";
        private readonly IWebDriver _driver;

        public static HtmlDocument ConvertToHtmlDocument(string htmlString)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);
            return htmlDoc;
        }
        public scrapper()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--disable-web-security");
            options.AddArgument("--allow-running-insecure-content");
            _driver = new ChromeDriver(options);
        }
        //316 sayfası var
        public async Task beginScrapingFromTxtFile(int firstHtmlSiteNumber, int lastHtmlSiteNumber)
        {
            string exePath = AppContext.BaseDirectory;
            
            for (int i = firstHtmlSiteNumber ; i < lastHtmlSiteNumber+1; i++)
            {
                string htmlName = "page_" + i.ToString() + ".html";
                string htmlFilePath = Path.Combine(Directory.GetParent(exePath).Parent.Parent.FullName,"OutputDocuments","downloadedHtml","downloaded_html",htmlName);
                HtmlDocument doc = new HtmlDocument();
                try
                {
                    doc.Load(htmlFilePath);
                }
                catch (DirectoryNotFoundException ex)
                {
                    Console.WriteLine("Boş HTML Hatası = " + htmlName);
                    Console.WriteLine("Geçilen sayfa = " + i + "    Yol=    " + htmlFilePath);
                    continue;
                }
                HtmlNode parent = doc.DocumentNode.SelectSingleNode("//div[@id='updates-container']");
                var childs = parent.SelectNodes(".//div[@class='updates-element-info ml flex flex-col justify-space-between full-width']");

                var tempMangaList = new List<string>();
                foreach (var child in childs)
                {

                    var titleNode = child.SelectSingleNode(".//h2/a");
                    if (titleNode != null)
                    {
                        string mangaTitle = titleNode.InnerText.Trim();
                        tempMangaList.Add(mangaTitle);
                    }

                }
                string allMangas = string.Join(Environment.NewLine, tempMangaList);
                textToTxt(allMangas, i);
            }








        }
        public async Task beginScrapingFromWebsite()
        {
            var handler = new ClearanceHandler
            {
                MaxRetries = 3 // Cloudflare challenge deneme sayısı
            };

            using (HttpClient client = new HttpClient(handler))
            {
                for (int i = 1; i < 50; i++)
                {
                    string websiteUrl = firstWebUrl.Replace("numara", i.ToString());
                    try
                    {
                        Console.WriteLine("Şu anki site =   " + websiteUrl);
                        string source = await client.GetStringAsync(websiteUrl);

                        HtmlDocument doc = ConvertToHtmlDocument(source);
                        HtmlNode parent = doc.DocumentNode.SelectSingleNode("//div[@id='updates-container']");

                        if (parent != null)
                        {
                            await webToHTML(parent, i);
                        }
                        else
                        {
                            Console.WriteLine("Parent bulunamadı → " + websiteUrl);
                        }

                        Console.WriteLine("Biten   site =   " + websiteUrl);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Hata: " + websiteUrl + " → " + ex.Message);
                    }

                    
                    await Task.Delay(2000);// Rate limit yememek için
                }
            }
        }


        public async Task webToHTML(HtmlNode parent,int currentPageNumber) {      //burada yapılacak işlem gelen parent içindeki bütün nodları almak
            var childs = parent.SelectNodes(".//div[@class='updates-element-info ml flex flex-col justify-space-between full-width']");
            var tempMangaList = new List<string>();
            foreach (var child in childs)
            {
                
                var titleNode = child.SelectSingleNode(".//h2/a");
                if (titleNode != null)
                {
                    string mangaTitle = titleNode.InnerText.Trim();
                    tempMangaList.Add(mangaTitle);
                }

            }
            string allMangas = string.Join(Environment.NewLine, tempMangaList);
            textToTxt(allMangas, currentPageNumber);
        }


        public static void textToTxt(string text,int currentPageNumber) {
            string exePath = AppContext.BaseDirectory;
            string saveFolder = Path.Combine(Directory.GetParent(exePath).Parent.Parent.FullName, "OutputDocuments", "allmangas");
            Directory.CreateDirectory(saveFolder);  //yoksa olustur
            string filePath = Path.Combine(saveFolder, $"page_{currentPageNumber}.txt");
            File.WriteAllText(filePath, text);

        }







    }
}
