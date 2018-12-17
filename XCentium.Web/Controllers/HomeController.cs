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
            string regExPattern = "<img.+?src=[\"'](.+?)[\"'].+?>";
            List<string> results = GetImageUrlsFromSource(htmlContent, regExPattern);


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

        private List<string> GetImageUrlsFromSource(string content, string regEx)
        {
            List<string> results = new List<string>();

            MatchCollection matches = Regex.Matches(content, regEx, RegexOptions.IgnoreCase);

            foreach(var match in matches)
            {
                string imgUrl = match.ToString().Contains("src") ? 
                                    "" 
                                    : "";

                imgUrl = match.ToString();

                results.Add(imgUrl);
            }

            return results;
        }
    }
}