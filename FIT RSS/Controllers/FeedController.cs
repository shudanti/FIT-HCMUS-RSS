using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Http;
using System.Xml;
using HtmlAgilityPack;

namespace FIT_RSS.Controllers
{
    public class FeedController : ApiController
    {
        private SyndicationFeed getNews()
        {
            String uri = "http://www.fit.hcmus.edu.vn/vn/";
            SyndicationFeed feed = new SyndicationFeed("Tin tức mới nhất từ Khoa CNTT - ĐH Khoa học Tự nhiên",
                "Khoa CNTT - ĐH Khoa học Tự nhiên RSS", new Uri(uri));

            var webget = new HtmlWeb();
            HtmlDocument htmlDoc = webget.Load(uri);

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

            }
            else
            {

                if (htmlDoc.DocumentNode != null)
                {
                    HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='dnn_ctr989_ModuleContent']");

                    if (bodyNode != null)
                    {
                        List<SyndicationItem> items = new List<SyndicationItem>();
                        foreach (HtmlNode node in bodyNode.SelectNodes(".//table"))
                        {
                            String title = node.SelectSingleNode(".//a").Attributes["title"].Value;
                            String link = node.SelectSingleNode(".//a").Attributes["href"].Value;
                            String day = node.SelectSingleNode(".//td[@class='day_month']").InnerText.Trim();
                            String month = node.SelectSingleNode(".//td[@class='day_month'][@style]").InnerText.Trim();
                            String year = node.SelectSingleNode(".//td[@class='post_year']").InnerText.Trim();
                            String date = day + " " + month + " " + year;
                            DateTime d = DateTime.ParseExact(date, @"d M yyyy", null);
                            SyndicationItem item = new SyndicationItem(title, "", new Uri(uri + link), (new Uri(uri + link)).ToString(), d);
                            //item.Title = new TextSyndicationContent(title);
                            //item.Links.Add(new SyndicationLink(new Uri(uri + link)));
                            //item.PublishDate = 
                            items.Add(item);
                        }
                        feed.Items = items;
                    }
                }
            }
            return feed;

        }

        [HttpGet]
        [Route("feed")]
        public HttpResponseMessage GetFeed()
        {
            var rssFormatter = new Rss20FeedFormatter(getNews(), false);
            var output = new StringBuilder();
            using (var writer = XmlWriter.Create(output, new XmlWriterSettings { Indent = true }))
            {
                rssFormatter.WriteTo(writer);
                writer.Flush();
            }

            return new HttpResponseMessage(HttpStatusCode.Accepted)
            {
                Content = new StringContent(output.ToString(), Encoding.UTF8, "application/xml")
            };
        }
    }
}
