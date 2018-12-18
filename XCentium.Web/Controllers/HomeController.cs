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

        [HttpPost]
        public ActionResult GetExtractedData(string url)
        {
            // Validate url
            if (!IsUrlValid(url))
            {
                return Json(new
                {
                    NbWords = "",
                    imgGallery = ""
                }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Extract html content from source url
                string htmlContent = ExtractContent(url);

                // Extract url from 'img' tags
                List<string> imgResults = GetImageUrlsFromSource(htmlContent);

                // Get list of words from source html/body
                List<string> wordResults = GetWordsFromSource(htmlContent);

                // Get top 8 words from source html/body
                string top8Words = GetTop8WordsFromSource(wordResults);

                // Generate image gallery
                string gallery = GenerateImageGallery(imgResults, url);


                return Json(new
                {
                    NbWords = wordResults.Count,
                    Top8Words = top8Words,
                    imgGallery = gallery,

                }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    NbWords = "Error",
                    imgGallery = ex.Message,

                }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetTop8WordsFromSource(List<string> words)
        {
            string results = "";

            var query = words.GroupBy(word => word)
                              .Select(r =>
                                new {
                                    label = r.Key,
                                    Number = r.Count()
                                }).OrderByDescending(g => g.Number)
                                .Take(8);

            if (null != query)
            {
                foreach (var row in query)
                {
                    results += "<li><label>" + row.label + ": " + row.Number + "</label></li>";
                }
            }

            return "<ul>" + results + "</ul>";
        }

        private static bool IsUrlValid(string url)
        {
            Uri uriResult;
            bool uriOkay = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return uriOkay;
        }

        private string ExtractContent(string url)
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.GET, DataFormat.Xml);

            IRestResponse htmlData = client.Execute<HtmlData>(request);

            return htmlData.Content;
        }

        private List<string> GetWordsFromSource(string content)
        {
            List<string> results = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//body");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    string temp = node.InnerText.Replace("\r", "").Replace("\n", "").Replace("\\", "").Replace("\t", "").Trim();

                    if (!string.IsNullOrEmpty(temp))
                    {
                        var query = temp.Split(' ').ToList()
                                        .Where(w => !string.IsNullOrEmpty(w));

                        results.AddRange(query);
                    }
                }
            }

            return results;
        }

        private List<string> GetImageUrlsFromSource(string content)
        {
            List<string> results = new List<string>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            HtmlNodeCollection images = doc.DocumentNode.SelectNodes("//img[@src]");

            if (null != images)
            {
                foreach (var image in images)
                {
                    results.Add(image.Attributes["src"].Value);
                }
            }

            return results;
        }

        private string GenerateImageGallery(List<string> images, string url)
        {
            if (null == images)
            {
                return "";
            }

            string imgGal = "";
            string domainName = (new Uri(url)).Scheme + "://" + (new Uri(url)).Host;

            foreach (string img in images)
            {
                imgGal += "<li><img alt='' src='" + (img.Contains("http") ? "" : domainName) + img + "'></li> ";
            }
            return imgGal;
        }
    }
}