using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace ConsoleApp1
{
    public static class TarotHelper
    {
        public static string ToTitleCase(this string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }

        //https://tarotvnnews.com/y-nghia-la-bai-fool-trong-tarot/

        public static string Parse()
        {
            var url = "https://tarotvnnews.com/giai-y-nghia-78-la-bai-tarot/";
            var cards = new List<KeyValuePair<string, string>>();
            try
            {
                HttpClient hc = new HttpClient();


                HttpResponseMessage response = hc.GetAsync($"{url}").Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNode = pageDocument.DocumentNode.SelectNodes("//table").FirstOrDefault();
                var items = contentNode.SelectNodes("//td");
                foreach (var item in items)
                {
                    var h3Items = item.SelectNodes("h3");
                    if (h3Items == null || h3Items.Count == 0) continue;
                    foreach (var h3 in h3Items)
                    {
                        if (h3.ChildNodes[0] != null && h3.ChildNodes[0].ChildNodes[0] != null)
                        {
                            var atag = h3.ChildNodes[0].ChildNodes[0].SelectNodes("a").FirstOrDefault();
                            if (atag != null)
                            {
                                var href = atag.Attributes["href"].Value;
                                var name = atag.InnerText;
                                name = name.Substring(name.IndexOf(":") + 1).Trim();
                                name = name.ToTitleCase().Replace(" ", "");
                                var value = KeyValuePair.Create(name, href);
                                cards.Add(value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            var str = new StringBuilder();
            foreach (var item in cards)
            {
                str.AppendLine($"{ParseUrls(item.Key, item.Value)},");
            }

            return str.ToString();
        }

        public static string ParseUrls(string name, string url = "https://tarotvnnews.com/y-nghia-la-bai-fool-trong-tarot/")
        {
            try
            {
                HttpClient hc = new HttpClient();


                HttpResponseMessage response = hc.GetAsync($"{url}").Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNode = pageDocument.DocumentNode.SelectNodes("//div[@class='entry']").FirstOrDefault();

                var xuoi = contentNode.SelectNodes("//a[starts-with(@href,'https://tarotvnnews.com/dien-giai-xuoi-cua-la-bai')]").First().Attributes["href"].Value;
                var nguoc = contentNode.SelectNodes("//a[starts-with(@href,'https://tarotvnnews.com/dien-giai-nguoc-cua-la-bai')]").First().Attributes["href"].Value;

                var strBuilder = new StringBuilder();
                strBuilder.AppendLine($"{{");
                strBuilder.AppendLine($"\"Name\": \"{name}\",");
                strBuilder.AppendLine($"\"Xuoi\": {{{ParseContent($"{name}_Xuoi", xuoi)}}},");
                strBuilder.AppendLine($"\"Nguoc\": {{{ParseContent($"{name}_Nguoc", nguoc)}	}}");
                strBuilder.Append("}");

                return strBuilder.ToString();


            }
            catch (Exception ex)
            {
            }

            return string.Empty;
        }

        //https://tarotvnnews.com/dien-giai-xuoi-cua-la-bai-the-fool-trong-tarot/
        public static string ParseContent(string name, string url = "https://tarotvnnews.com/dien-giai-xuoi-cua-la-bai-the-fool-trong-tarot/")
        {
            string congviec, tinhyeu, taichinh, suckhoe, tinhthan;
            try
            {
                HttpClient hc = new HttpClient();


                HttpResponseMessage response = hc.GetAsync($"{url}").Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNode = pageDocument.DocumentNode.SelectNodes("//div[@class='entry']").FirstOrDefault();

                congviec = contentNode.ChildNodes[10].InnerText.Replace("\"", "'").Replace("\n", "");
                tinhyeu = contentNode.ChildNodes[12].InnerText.Replace("\"", "'").Replace("\n", "'");
                taichinh = contentNode.ChildNodes[14].InnerText.Replace("\"", "'").Replace("\n", "'");
                suckhoe = contentNode.ChildNodes[16].InnerText.Replace("\"", "'").Replace("\n", "'");
                tinhthan = contentNode.ChildNodes[18].InnerText.Replace("\"", "'").Replace("\n", "'");

                var strBuilder = new StringBuilder("\n");
                strBuilder.AppendLine($"	\"CongViec \":\"{congviec.Substring(congviec.IndexOf(":") + 1).Trim()}\",");
                strBuilder.AppendLine($"	\"TinhYeu\":\"{tinhyeu.Substring(tinhyeu.IndexOf(":") + 1).Trim()}\",");
                strBuilder.AppendLine($"	\"TaiChinh\":\"{taichinh.Substring(taichinh.IndexOf(":") + 1).Trim()}\",");
                strBuilder.AppendLine($"	\"SucKhoe\":\"{suckhoe.Substring(suckhoe.IndexOf(":") + 1).Trim()}\",");
                strBuilder.AppendLine($"	\"TinhThan\":\"{tinhthan.Substring(tinhthan.IndexOf(":") + 1).Trim()}\"");

                return strBuilder.ToString();

            }
            catch (Exception ex)
            {
            }

            return string.Empty;
        }
    }
}
