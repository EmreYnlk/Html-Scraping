using ConsoleApp1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nameScrapper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bu kod sadece tek bir site için çalışır");
            scrapper scrapper = new scrapper();
            //scrapper.beginScrapingFromWebsite();

            //htmlDownloader asd = new htmlDownloader();
            htmlDownloader.DownloadHtmlsFromTxt().GetAwaiter().GetResult();
            scrapper.beginScrapingFromTxtFile(1, 316);     //ilk sayfa, son sayfa

            


        }
    }
}
