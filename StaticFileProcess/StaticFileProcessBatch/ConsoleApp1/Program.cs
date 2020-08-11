using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ConsoleApp1
{
    static class Program
    {
        private static string GetTuViCungHoangDao(DateTime date)
        {
            string dateOfWeek = date.DayOfWeek == 0 ? "chu-nhat" : $"thu-{(int)date.DayOfWeek + 1}";
            string url = $"https://xemtuvi.mobi/tu-vi-ngay-moi/tu-vi-hang-ngay/xem-boi-tu-vi-{dateOfWeek}-ngay-{date.Day}-{date.Month}-{date.Year}-cua-12-cung-hoang-dao.html";
            return url;
        }

        private static string GetTuViConGiap(DateTime date)
        {
            string dateOfWeek = date.DayOfWeek == 0 ? "chu-nhat" : $"thu-{(int)date.DayOfWeek + 1}";
            string url = $"https://xemtuvi.mobi/tu-vi-ngay-moi/tu-vi-hang-ngay-12-con-giap/tu-vi-ngay-{date.Day}-{date.Month}-{date.Year}-{dateOfWeek}-cua-12-con-giap.html";
            return url;
        }

        private static List<string> CrawlXemTuViEcotownlongthanh(string url)
        {
            var result = new List<string>();
            try
            {
                HttpClient hc = new HttpClient();
                HttpResponseMessage response = hc.GetAsync(url).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNode = pageDocument.DocumentNode.SelectNodes("//div[@class='content-post clearfix']").FirstOrDefault();
                if (contentNode != null)
                {
                    int h2Count = 0;
                    int h3Count = 0;
                    var stringBuilder = new StringBuilder();
                    for (int i = 0; i < contentNode.ChildNodes.Count - 1; i++)
                    {
                        var item = contentNode.ChildNodes[i];
                        var style = item.GetAttributeValue("style", "empty");
                        if (style.Contains("text-align:center", StringComparison.OrdinalIgnoreCase) || style.Contains("text-align:right", StringComparison.OrdinalIgnoreCase)) continue;

                        if (item.Name.Equals("h2"))
                        {
                            h2Count++;
                            if (stringBuilder.Length > 0)
                            {
                                result.Add(stringBuilder.ToString().Trim());
                                stringBuilder = new StringBuilder();
                                h3Count = 0;
                            }
                        }
                        if (h2Count < 4)
                            continue;

                        if(item.Name.Equals("h3"))
                        {
                            h3Count++;
                            stringBuilder.Append("<b>" + item.InnerText.Replace('"', '“').Trim() + "</b><br/>");
                        }
                        else if (item.Name.Equals("p") && !string.IsNullOrWhiteSpace(item.InnerText))
                        {
                            if (h3Count == 5)
                            {
                                foreach(var child in item.ChildNodes)
                                {
                                    if (!string.IsNullOrWhiteSpace(child.InnerText))
                                    {
                                        stringBuilder.Append("&emsp;" + child.InnerText.Replace('"', '“').Trim() + "<br/>");
                                    }
                                }
                            }
                            else
                            {
                                stringBuilder.Append("&emsp;" + item.InnerText.Replace('"', '“').Trim() + "<br/><br/>");
                            }
                            if (h3Count == 6 && result.Count == 11)
                                break;
                        }

                    }
                    result.Add(stringBuilder.ToString().Trim());
                }
            }
            catch { }
            return result;
        }

        private static List<string> CrawlXemTuVi(string url)
        {
            var result = new List<string>();
            try
            {
                HttpClient hc = new HttpClient();
                HttpResponseMessage response = hc.GetAsync(url).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNode = pageDocument.DocumentNode.SelectNodes("//div[@class='text-detail news_content fit-content']").FirstOrDefault();
                if (contentNode != null)
                {
                    bool removedUnuseItem = false;
                    var stringBuilder = new StringBuilder();
                    for (int i = 0; i < contentNode.ChildNodes.Count - 1; i++)
                    {
                        var item = contentNode.ChildNodes[i];
                        var style = item.GetAttributeValue("style", "empty");
                        if (style.Contains("text-align:center", StringComparison.OrdinalIgnoreCase) || style.Contains("text-align:right", StringComparison.OrdinalIgnoreCase)) continue;

                        if (item.Name.Equals("h2"))
                        {
                            if (stringBuilder.Length > 0)
                            {
                                result.Add(stringBuilder.ToString().Trim());
                                stringBuilder = new StringBuilder();
                            }
                            if (!removedUnuseItem)
                            {
                                removedUnuseItem = true;
                            }
                        }
                        else if (removedUnuseItem)
                        {
                            if (item.InnerText.Contains("---------------"))
                            {
                                removedUnuseItem = false;
                                continue;
                            }

                            if (item.Name.Equals("p") && !string.IsNullOrWhiteSpace(item.InnerText))
                            {
                                stringBuilder.Append("&emsp;" + item.InnerText.Replace('"', '“').Trim() + "<br/><br/>");
                            }
                            if (item.Name.Equals("ul"))
                            {
                                foreach (var li in item.ChildNodes)
                                {
                                    if (!string.IsNullOrWhiteSpace(li.InnerText))
                                    {
                                        stringBuilder.Append("&emsp;" + li.InnerText.Replace('"', '“').Trim() + "<br/><br/>");
                                    }
                                }

                            }
                        }
                    }
                    result.Add(stringBuilder.ToString().Trim());
                }
            }
            catch { }
            return result;
        }

        private static string buildJson(List<string> congiap, List<string> cunghoangDao, string dateInString)
        {
            if (congiap.Count == 12 && cunghoangDao.Count == 12)
            {
                var strBuilder = new StringBuilder();
                strBuilder.AppendLine($"\"{dateInString}\": {{");

                strBuilder.AppendLine($"    \"Ty\":\"{congiap[0]}\",");
                strBuilder.AppendLine($"	\"Suu\":\"{congiap[1]}\",");
                strBuilder.AppendLine($"	\"Dan\":\"{congiap[2]}\",");
                strBuilder.AppendLine($"	\"Meo\":\"{congiap[3]}\",");
                strBuilder.AppendLine($"	\"Thin\":\"{congiap[4]}\",");
                strBuilder.AppendLine($"	\"Ti\":\"{congiap[5]}\",");
                strBuilder.AppendLine($"	\"Ngo\":\"{congiap[6]}\",");
                strBuilder.AppendLine($"	\"Mui\":\"{congiap[7]}\",");
                strBuilder.AppendLine($"	\"Than\":\"{congiap[8]}\",");
                strBuilder.AppendLine($"	\"Dau\":\"{congiap[9]}\",");
                strBuilder.AppendLine($"	\"Tuat\":\"{congiap[10]}\",");
                strBuilder.AppendLine($"	\"Hoi\":\"{congiap[11]}\",");
                strBuilder.AppendLine($"	\"BachDuong\":\"{cunghoangDao[0]}\",");
                strBuilder.AppendLine($"	\"KimNguu\":\"{cunghoangDao[1]}\",");
                strBuilder.AppendLine($"	\"SongTu\":\"{cunghoangDao[2]}\",");
                strBuilder.AppendLine($"	\"CuGiai\":\"{cunghoangDao[3]}\",");
                strBuilder.AppendLine($"	\"SuTu\":\"{cunghoangDao[4]}\",");
                strBuilder.AppendLine($"	\"XuNu\":\"{cunghoangDao[5]}\",");
                strBuilder.AppendLine($"	\"ThienBinh\":\"{cunghoangDao[6]}\",");
                strBuilder.AppendLine($"	\"BoCap\":\"{cunghoangDao[7]}\",");
                strBuilder.AppendLine($"	\"NhanMa\":\"{cunghoangDao[8]}\",");
                strBuilder.AppendLine($"	\"MaKet\":\"{cunghoangDao[9]}\",");
                strBuilder.AppendLine($"	\"BaoBinh\":\"{cunghoangDao[10]}\",");
                strBuilder.AppendLine($"	\"SongNgu\":\"{cunghoangDao[11]}\"");
                strBuilder.Append("    }");

                return strBuilder.ToString();
            }

            return $"\"{dateInString}\": {{\"Error\":\"Error!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\"}}";
        }
        private static string CrawlTuVi(DateTime date)
        {
            var congiap = CrawlXemTuVi(GetTuViConGiap(date));
            var cunghoangDao = CrawlXemTuVi(GetTuViCungHoangDao(date));
            return buildJson(congiap, cunghoangDao, date.ToString("yyyyMMdd"));
        }

        private static string CrawlTuViBasedMonth(DateTime date)
        {
            var congiap = CrawlXemTuVi($"https://xemtuvi.mobi/tu-vi-hang-thang/tu-vi-thang-{date.Month}-{date.Year}-dong-phuong-cua-12-con-giap.html");
            var cunghoangDao = CrawlXemTuVi($"https://xemtuvi.mobi/tu-vi-hang-thang/tu-vi-thang-{date.Month}-{date.Year}-tay-phuong-cua-12-cung-hoang-dao.html");
            return buildJson(congiap, cunghoangDao, date.ToString("yyyyMM"));
        }

        static void Main(string[] args)
        {
            var rs = TarotHelper.Parse();
           // var rs = CrawlXemTuViEcotownlongthanh(@"https://ecotownlongthanh.vn/tu-vi-thang-5-2020-cua-12-cung-hoang-dao/");
            /*
                        var result = CrawlTuVi(DateTime.Now.AddDays(1));
                        Console.WriteLine(result);
                        result = CrawlTuViBasedMonth(DateTime.Now);*/
            //Console.ReadLine();
            int days = 5, months = 0;
            if (args.Length > 0) int.TryParse(args[0], out days);
            if (args.Length > 1) int.TryParse(args[1], out months);

            var data = Crawl(days, months);

            WriteToFile(data);
        }

        private static void WriteToFile(string data)
        {
            var fileName = $"tuvi_{DateTime.UtcNow.AddHours(7).ToString("yyyyMMddhhmmss")}.json";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.Write(data);
            }
        }

        public static string Crawl(int days, int months)
        {
            //return CrawlBoiSoAiCap();
            StringBuilder stringBuilder = new StringBuilder("{\n");
            var date = DateTime.UtcNow.AddHours(7);
            for (int i = 0; i < months; i++)
            {
                var data = CrawlTuViBasedMonth(date.AddMonths(i));
                stringBuilder.AppendLine($"{data},");
            }
            for (int i = 0; i < days; i++)
            {
                var data = CrawlTuVi(date.AddDays(i));
                stringBuilder.AppendLine($"{data},");
            }
            return stringBuilder.ToString().Trim().Trim(',') + "\n}";
        }

        //https://ecotownlongthanh.vn/tu-vi-thang-5-2020-cua-12-cung-hoang-dao/
        //https://phongthuyso.vn/tu-vi-thang-5-2020-cua-12-con-giap.html


        ////https://phongthuyso.vn/boi-ai-cap.html

        private static string CrawlBoiSoAiCap()
        {
            var strBuilder = new StringBuilder();

             strBuilder.AppendLine($"\"BoiSoAiCap\": {{");
            for (int i = 0; i< 9; i++)
            {
                var rs = CrawlBoiSoAiCap((char) ('A'+i));
                strBuilder.AppendLine($"    \"So{i+1}\":\"{rs}\",");
            }
            strBuilder.Append("    }");

            return strBuilder.ToString();
        }

        private static string CrawlBoiSoAiCap(char name)
        {
            try
            {
                HttpClient hc = new HttpClient();

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("nameguest", ""+name),
                    new KeyValuePair<string, string>("option", "com_boi"),
                    new KeyValuePair<string, string>("view", "aicap"),
                    new KeyValuePair<string, string>("Itemid", "28")
                });
                var url = "https://phongthuyso.vn/boi-ai-cap.html";
                HttpResponseMessage response = hc.PostAsync($"{url}", formContent).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNode = pageDocument.DocumentNode.SelectNodes("//div[@class='content_xemboi']").Skip(1).FirstOrDefault();
                if (contentNode != null)
                {
                    var stringBuilder = new StringBuilder();
                    for (int i = 0; i < contentNode.ChildNodes.Count - 2; i++)
                    {
                        var item = contentNode.ChildNodes[i];
                        if (string.IsNullOrEmpty(item.InnerText.Trim())) continue;
                        if (!item.HasChildNodes || !item.ChildNodes[0].HasChildNodes)
                        {
                            stringBuilder.Append("<br/>&emsp;<b>" + item.InnerText + "</b><br/>");
                        }
                        else
                        {
                            stringBuilder.Append("&emsp;" + item.ChildNodes[0].ChildNodes[0].InnerText + "<br/><br/>");
                        }
                    }
                    return stringBuilder.ToString().Replace("\"", "'").Replace("<br>",string.Empty).Trim();
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }


    }
}
