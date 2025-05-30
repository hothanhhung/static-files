﻿using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Globalization;
using System.IO;
using System.Net;

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
        private static string GetTuViCungHoangDao2(DateTime date)
        {
            string dateOfWeek = date.DayOfWeek == 0 ? "chu-nhat" : $"thu-{(int)date.DayOfWeek + 1}";
            string url = $"https://xemtuvi.mobi/tu-vi-ngay-{date.Day}-{date.Month}-{date.Year}-{dateOfWeek}-tu-vi-hang-ngay-cua-12-cung-hoang-dao.html";
            return url;
        }

        private static string GetTuViCungHoangDao3(DateTime date)
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

        private static string GetTuViConGiap2(DateTime date)
        {
            string dateOfWeek = date.DayOfWeek == 0 ? "chu-nhat" : $"thu-{(int)date.DayOfWeek + 1}";
            string url = $"https://xemtuvi.mobi/tu-vi-ngay-{date.Day}-{date.Month}-{date.Year}-cua-12-con-giap-{dateOfWeek}.html";
            return url;
        }

        private static string GetTuViConGiap3(DateTime date)
        {
            string dateOfWeek = date.DayOfWeek == 0 ? "chu-nhat" : $"thu-{(int)date.DayOfWeek + 1}";
            string url = $"https://xemtuvi.mobi/tu-vi-{dateOfWeek}-ngay-{date.Day}-{date.Month}-{date.Year}-tu-vi-hang-ngay-12-con-giap.html";
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

                        if (item.Name.Equals("h3"))
                        {
                            h3Count++;
                            stringBuilder.Append("<b>" + item.InnerText.Replace('"', '“').Trim() + "</b><br/>");
                        }
                        else if (item.Name.Equals("p") && !string.IsNullOrWhiteSpace(item.InnerText))
                        {
                            if (h3Count == 5)
                            {
                                foreach (var child in item.ChildNodes)
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
            var congiap = CrawlXemTuVi(GetTuViConGiap3(date));
            if (congiap == null || congiap.Count == 0)
            {
                congiap = CrawlXemTuVi(GetTuViConGiap2(date));
            }
            if (congiap == null || congiap.Count == 0)
            {
                congiap = CrawlXemTuVi(GetTuViConGiap(date));
            }


            var cunghoangDao = CrawlXemTuVi(GetTuViCungHoangDao3(date));
            if (cunghoangDao == null || cunghoangDao.Count == 0)
            {
                cunghoangDao = CrawlXemTuVi(GetTuViCungHoangDao2(date));
            }
            if (cunghoangDao == null || cunghoangDao.Count == 0)
            {
                cunghoangDao = CrawlXemTuVi(GetTuViCungHoangDao(date));
            }
            return buildJson(congiap, cunghoangDao, date.ToString("yyyyMMdd"));
        }

        private static string CrawlTuViBasedMonth(DateTime date)
        {
            var congiap = CrawlXemTuVi($"https://xemtuvi.mobi/tu-vi-hang-thang/tu-vi-thang-{date.Month}-{date.Year}-dong-phuong-cua-12-con-giap.html");
            var cunghoangDao = CrawlXemTuVi($"https://xemtuvi.mobi/tu-vi-hang-thang/tu-vi-thang-{date.Month}-{date.Year}-tay-phuong-cua-12-cung-hoang-dao.html");
            if (cunghoangDao.Count == 0)
            {
                cunghoangDao = CrawlXemTuVi($"https://xemtuvi.mobi/tu-vi-hang-thang/tu-vi-thang-{date.Month}-{date.Year}-cua-12-cung-hoang-dao-song-ngu-rat-vo-tu-hanh-phuc.html");
            }
            return buildJson(congiap, cunghoangDao, date.ToString("yyyyMM"));
        }

        static void Main(string[] args)
        {/*
            CrawlLichThucHienQuyen();
            return;*/
            //CrawlGioSocTietKhi();
            // return;
            //var rs = TarotHelper.Parse();
            // var rs = CrawlXemTuViEcotownlongthanh(@"https://ecotownlongthanh.vn/tu-vi-thang-5-2020-cua-12-cung-hoang-dao/");
            /*
                        var result = CrawlTuVi(DateTime.Now.AddDays(1));
                        Console.WriteLine(result);
                        result = CrawlTuViBasedMonth(DateTime.Now);*/
            //Console.ReadLine();
            int days = 15, months = 0;
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
            return crawlTuViFromTVCNews();
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
        // https://tuviso.com/tu-vi-cung-song-ngu-nam-2021.html

        ////https://phongthuyso.vn/boi-ai-cap.html

        private static string CrawlBoiSoAiCap()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendLine($"\"BoiSoAiCap\": {{");
            for (int i = 0; i < 9; i++)
            {
                var rs = CrawlBoiSoAiCap((char)('A' + i));
                strBuilder.AppendLine($"    \"So{i + 1}\":\"{rs}\",");
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
                    return stringBuilder.ToString().Replace("\"", "'").Replace("<br>", string.Empty).Trim();
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }



        private static void CrawlGioSocTietKhi()
        {
            List<DateTime> ngaySoc = new List<DateTime>();
            List<KeyValuePair<DateTime, string>> tietKhi = new List<KeyValuePair<DateTime, string>>();

            for (int i = 1800; i < 2180; i += 20) //2180
            {
                CrawlGioSocTietKhi($"https://www.informatik.uni-leipzig.de/~duc/amlich/DuLieu/Sun-Moon-{i}.html", ngaySoc, tietKhi);
            }

            var ngaySocBuilder = new StringBuilder("{\n");
            foreach (var group in ngaySoc.GroupBy(p => p.Year))
            {
                ngaySocBuilder.Append($"\"{group.Key}\": [");
                for (var i = 0; i < group.Count(); i++)
                {
                    var item = group.ElementAt(i);
                    if (i != 0)
                    {
                        ngaySocBuilder.Append(",");
                    }
                    ngaySocBuilder.Append($"\"{item.Day.ToString("D2")}-{item.Month.ToString("D2")}\"");
                }
                ngaySocBuilder.AppendLine("],");
            }
            ngaySocBuilder.Remove(ngaySocBuilder.Length - 3, 1);
            ngaySocBuilder.AppendLine("}");

            File.WriteAllText(@"ngaysoc.json", ngaySocBuilder.ToString());

            var tietKhiBuilder = new StringBuilder("{\n");
            foreach (var group in tietKhi.GroupBy(p => p.Key.Year))
            {
                tietKhiBuilder.Append($"\"{group.Key}\": [");
                for (var i = 0; i < group.Count(); i++)
                {
                    var item = group.ElementAt(i);
                    if (i != 0)
                    {
                        tietKhiBuilder.Append(",");
                    }
                    tietKhiBuilder.Append($"{{\"{item.Key.Day.ToString("D2")}-{item.Key.Month.ToString("D2")}\":\"{item.Value}\"}}");
                }
                tietKhiBuilder.AppendLine("],");
            }
            tietKhiBuilder.Remove(tietKhiBuilder.Length - 3, 1);
            tietKhiBuilder.AppendLine("}");
            File.WriteAllText(@"tietkhi.json", tietKhiBuilder.ToString());
        }

        private static void CrawlGioSocTietKhi(string url, List<DateTime> ngaySoc, List<KeyValuePair<DateTime, string>> tietKhi)
        {
            var result = new List<string>();
            try
            {
                HttpClient hc = new HttpClient();
                HttpResponseMessage response = hc.GetAsync(url).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var years = pageDocument.DocumentNode.SelectNodes("//table");
                if (years != null)
                {
                    foreach (var yearNode in years)
                    {
                        var trs = yearNode.SelectNodes("tr");
                        if (trs == null || trs.Count < 1) continue;
                        var year = trs[0].InnerText;
                        for (var i = 2; i < trs.Count; i++)
                        {
                            var child = trs[i];
                            var tds = child.SelectNodes("td");
                            if (tds == null || tds.Count < 2) continue;
                            var soc = HttpUtility.HtmlDecode(tds[0].InnerText).Trim();
                            var tietkhi = HttpUtility.HtmlDecode(tds[1].InnerText.Trim());
                            var tietkhiItems = tietkhi.Split('-');
                            if (!string.IsNullOrEmpty(soc))
                            {
                                ngaySoc.Add(ParseToDateTime(soc, year));
                            }
                            if (tietkhiItems.Count() == 2)
                            {
                                tietKhi.Add(new KeyValuePair<DateTime, string>(ParseToDateTime(tietkhiItems[0].Trim(), year), tietkhiItems[1].Trim()));
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private static void CrawlLichThucHienQuyen()
        {
            var firstLink = "https://web.vsd.vn/vi/lich-giao-dich?tab=LICH_THQ&date=13/05/2021";
            var postLink = "https://web.vsd.vn/lich-thq/search";
            var result = new List<string>();
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler firsthandler = new HttpClientHandler();
                firsthandler.CookieContainer = cookies;

                HttpClient firstclient = new HttpClient(firsthandler);
                HttpResponseMessage firstResponse = firstclient.GetAsync(firstLink).Result;


                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;

                HttpClient client = new HttpClient(handler);

                var json = "{\"SearchKey\":\"|||28/03/2021|07/04/2021|VI\",\"CurrentPage\":1,\"RecordOnPage\":10,\"OrderBy\":\"\",\"OrderType\":\"\"}";
                StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(postLink, httpContent).Result;

                //response.
                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var years = pageDocument.DocumentNode.SelectNodes("//table");
                if (years != null)
                {
                    foreach (var yearNode in years)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static DateTime ParseToDateTime(string value, string year)
        {
            DateTime dateTime;
            var str = value.Replace(" ", $"/{year} ");
            DateTime.TryParseExact(str, "dd/MM/yyyy HH:mm", new CultureInfo("vi-VN"), System.Globalization.DateTimeStyles.None, out dateTime);
            return dateTime;
        }


        static string crawlTuViFromTVCNews()
        {
            var error = false;
            var today = DateTime.Now;
            var result = new List<string>();
            var congiap = new List<string>();
            var cunghoangDao = new List<string>();

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpClient hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                HttpResponseMessage response = hc.GetAsync("https://vtcnews.vn/tieu-diem/tu-vi-hom-nay.html").Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNodes = pageDocument.DocumentNode.SelectNodes("//article/h3/a").Select(p => p.Attributes["href"].Value);
                var dayInStr = today.Day.ToString();
                var comparedDayInStr1 = "y-" + dayInStr + "-";
                var comparedDayInStr01 = "y-0" + dayInStr + "-";
                var congiapUrl = contentNodes.FirstOrDefault(p => p.Contains("con-giap") && (p.Contains(comparedDayInStr1) || p.Contains(comparedDayInStr01)));
                var cunghoangDaoUrl = contentNodes.FirstOrDefault(p => p.Contains("hoang-dao") && (p.Contains(comparedDayInStr1) || p.Contains(comparedDayInStr01)));

                if (!string.IsNullOrEmpty(congiapUrl))
                {
                    congiap = crawlTuViFromTVCNews(congiapUrl);
                }
                else
                {
                    var urlTuvi = getFirstPostUrlTuViFromTuViVN("https://tuvi.vn/category/tu-vi-hang-ngay");
                    if (!string.IsNullOrEmpty(urlTuvi) && (urlTuvi.Contains(comparedDayInStr1) || urlTuvi.Contains(comparedDayInStr01)))
                    {
                        congiap = crawlTuViFromTuViVN("https://tuvi.vn" + urlTuvi);
                    }
                    if (congiap.Count != 12)
                    {
                        congiap = Enumerable.Repeat("", 12).ToList();
                        error = true;
                    }
                }

                if (!string.IsNullOrEmpty(cunghoangDaoUrl))
                {
                    cunghoangDao = crawlTuViFromTVCNews(cunghoangDaoUrl);
                }
                else
                {
                    var urlTuvi = getFirstPostUrlTuViFromTuViVN("https://tuvi.vn/category/12-cung-hoang-dao-hang-ngay");
                    if (!string.IsNullOrEmpty(urlTuvi) && (urlTuvi.Contains(comparedDayInStr1) || urlTuvi.Contains(comparedDayInStr01)))
                    {
                        cunghoangDao = crawlTuViFromTuViVN("https://tuvi.vn" + urlTuvi, "h3");
                    }
                    if (cunghoangDao.Count != 12)
                    {
                        cunghoangDao = Enumerable.Repeat("", 12).ToList();
                        error = true;
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }

            return buildJson(congiap, cunghoangDao, today.ToString("yyyyMMdd"));
        }

        static new List<string> crawlTuViFromTVCNews(string url)
        {
            var result = new List<string>();
            try
            {
                HttpClient hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                HttpResponseMessage response = hc.GetAsync("https://vtcnews.vn" + url).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNodes = pageDocument.DocumentNode.SelectNodes("//div[@class='edittor-content box-cont mt15 clearfix ']/p | //div[@class='edittor-content box-cont mt15 clearfix ']/h3");
                var content = new StringBuilder();
                for (int i = 0; i < contentNodes.Count - 1; i++)
                {
                    if (contentNodes[i].FirstChild != null && contentNodes[i].FirstChild.Name == "strong")
                    {
                        if (content.Length > 0)
                        {
                            result.Add(content.ToString());
                            content = new StringBuilder();
                        }
                        continue;
                    }
                    content.Append("&emsp;" + contentNodes[i].InnerText + "<br/><br/>");
                }
                if (content.Length > 0)
                {
                    result.Add(content.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        static string getFirstPostUrlTuViFromTuViVN(string url)
        {
            try
            {
                HttpClient hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                HttpResponseMessage response = hc.GetAsync(url).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNodes = pageDocument.DocumentNode.SelectNodes("//div[@class='post-first-item']/a");
                if (contentNodes != null && contentNodes.Count > 0)
                {
                    return contentNodes[0].GetAttributeValue("href", "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
        static new List<string> crawlTuViFromTuViVN(string url, string startChar = "h2")
        {
            var result = new List<string>();
            try
            {
                HttpClient hc = new HttpClient();
                hc.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
                HttpResponseMessage response = hc.GetAsync(url).Result;

                var pageContents = response.Content.ReadAsStringAsync().Result;
                HtmlDocument pageDocument = new HtmlDocument();
                pageDocument.LoadHtml(pageContents);
                var contentNodes = pageDocument.DocumentNode.SelectNodes("//div[@class='text-p post-content']");
                var content = new StringBuilder();
                if (contentNodes != null && contentNodes.Count > 0)
                {
                    var contentNode = contentNodes[0];
                    var read = false;
                    var tuviContent = new StringBuilder();
                    foreach (var item in contentNode.ChildNodes)
                    {
                        if (item.Name == startChar)
                        {
                            if (read)
                            {
                                result.Add(tuviContent.ToString());
                            }
                            tuviContent = new StringBuilder();
                            read = true;
                            continue;
                        }

                        if (read)
                        {
                            if (item.Name == "p")
                            {
                                tuviContent.Append("<br/><br/>&emsp;").Append(item.InnerText);
                            }
                            else if (item.Name == "ul")
                            {
                                foreach (var it in item.ChildNodes)
                                {
                                    tuviContent.Append("<br/><br/>&emsp;").Append(it.InnerText);
                                }
                            }
                        }
                    }

                    if (tuviContent.Length > 0)
                    {
                        result.Add(tuviContent.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }
    }
}
