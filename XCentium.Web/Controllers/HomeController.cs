using HtmlAgilityPack;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XCentium.Web.Models;

namespace XCentium.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void ExtractData(string url)
        {

        }

        [HttpPost]
        public ActionResult GetExtractedData(string url)
        {
            //@"https?:\/\/[^ ]*\.(?:gif|png|jpg|jpeg)";  //@"(http(s?):)([/|.|\w|\s|-])*\.(?:jpg|gif|png)";   // "<img.+? src =[\"'](.+?)[\"'].*?>";

            // Extract html content from source url
            string htmlContent = ExtractContent(url);

            // Extract url from 'img' tags
            List<string> results = GetImageUrlsFromSource(htmlContent);


            return Json(new
            {
                html = results.Count.ToString()
            }, JsonRequestBehavior.AllowGet);
        }

        private string ExtractContent(string url)
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.GET, DataFormat.Xml);

            IRestResponse htmlData = client.Execute<HtmlData>(request);

            return htmlData.Content;
        }

        private List<string> GetWordsFromSource(string content, string regEx)
        {

            return null;
        }

        private List<string> GetImageUrlsFromSource(string content)
        {
            List<string> results = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            HtmlNodeCollection images = doc.DocumentNode.SelectNodes("//img[@src]");
            foreach(var image in images)
            {
                results.Add(image.Attributes["src"].Value);
            }

            return results;
        }
    }
}