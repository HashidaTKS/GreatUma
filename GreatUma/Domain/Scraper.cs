
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using GreatUma.Utils;
using GreatUma.Infrastructures;
using GreatUma.Models;

namespace GreatUma.Domain
{
    public class Scraper : IDisposable
    {

        private readonly ChromeDriver Chrome;

        bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                Chrome.Quit();
                Chrome.Dispose();
                disposed = true;
            }
        }

        public Scraper()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--allow-running-insecure-content");
            chromeOptions.AddArgument("--ignore-certificate-errors");
            //chromeOptions.AddArguments("headless", "disable-gpu");
            Chrome = new ChromeDriver(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), chromeOptions);
        }

        public List<HorseDatum> GetHorseInfo(RaceData raceData)
        {
            Chrome.Url = raceData.ToHorseInfoPageUrlString();
            Thread.Sleep(500);
            //高速化のためにchromedriverではなくAngleSharpを使っている
            var parser = new HtmlParser();

            var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
            var trCollection = doc.GetElementsByClassName("Shutuba_Table ")?.FirstOrDefault()?.GetElementsByClassName("HorseList");
            if (trCollection == null)
            {
                return null;
            }
            var result = new List<HorseDatum>();
            foreach (var tr in trCollection)
            {
                try
                {
                    var tdCollection = tr.GetElementsByTagName("td");
                    // インデックスではなくtdのクラス名から取得しても良いのだが馬番のクラスが「Umaban1, Umaban2...」と行によって違うので
                    // インデックスアクセスとした。毎回クラス検索するよりインデックスアクセスの方が早いという事情もある。
                    var numberText = tdCollection[1].TextContent;
                    if (!int.TryParse(numberText, out int number))
                    {
                        continue;
                    }
                    var horseName = tdCollection[3].TextContent.Trim();
                    var jockey = tdCollection[6].TextContent.Trim();
                    result.Add(new HorseDatum(number, horseName, jockey));
                }
                catch(Exception ex)
                {
                    //競走除外などの理由で馬数が想定より少なかったようなケース
                    LoggerWrapper.Warn(ex);
                    break;
                }
            }
            return result;
        }


        private IEnumerable<OddsDatum> GetFromWonTable(IHtmlCollection<IElement> trCollection)
        {
            var count = trCollection.Length;
            for (var i = 1; i < count; i++)
            {
                var tdList = trCollection[i].GetElementsByTagName("td");
                if (!int.TryParse(tdList[1].TextContent.Replace("\r", "").Replace("\n", ""), out var horse))
                {
                    continue;
                }
                if (!double.TryParse(tdList.LastOrDefault().TextContent.Replace("\r", "").Replace("\n", ""), out var odds))
                {
                    continue;
                }
                //ここでは馬番号だけが欲しいので、確率その他の情報は無視
                yield return new OddsDatum(new List<HorseDatum> { new HorseDatum(horse, "", "") }, odds, odds);
            }
        }

        private IEnumerable<OddsDatum> GetFromPlaceTable(IHtmlCollection<IElement> trCollection)
        {
            var count = trCollection.Length;
            for (var i = 1; i < count; i++)
            {
                var tdList = trCollection[i].GetElementsByTagName("td");
                if (!int.TryParse(tdList[1].TextContent.Replace("\r", "").Replace("\n", ""), out var horse))
                {
                    continue;
                }
                var oddsString = tdList.LastOrDefault().TextContent.Replace("\r", "").Replace("\n", "");
                if (string.IsNullOrEmpty(oddsString))
                {
                    continue;
                }
                var oddsStringList = oddsString.Split("-");
                if (oddsStringList.Length != 2)
                {
                    continue;
                }
                if (!double.TryParse(oddsStringList[0], out var lowOdds))
                {
                    continue;
                }
                if (!double.TryParse(oddsStringList[1], out var highOdds))
                {
                    continue;
                }
                //ここでは馬番号だけが欲しいので、確率その他の情報は無視
                yield return new OddsDatum(new List<HorseDatum> { new HorseDatum(horse, "", "") }, lowOdds, highOdds);
            }
        }

        private IEnumerable<OddsDatum> GetFromPopularTable(IHtmlCollection<IElement> trCollection, TicketType ticketType)
        {
            var rank = 1;
            if (ticketType == TicketType.Exacta || ticketType == TicketType.Quinella || ticketType == TicketType.Wide)
            {
                rank = 2;
            }
            else if (ticketType == TicketType.Trifecta || ticketType == TicketType.Trio)
            {
                rank = 3;
            }
            var count = trCollection.Length;
            var columnCount = trCollection[0].GetElementsByTagName("th").Length;
            var startIndexOfHorseNumber = columnCount - rank;


            for (var i = 2; i < count; i++)
            {
                var horseData = new List<HorseDatum>(rank);
                double odds;
                try
                {
                    var tdList = trCollection[i].GetElementsByTagName("td");
                    for (var j = 0; j < rank; j++)
                    {
                        var index = startIndexOfHorseNumber + 2 * j;
                        var horse = Convert.ToInt32(tdList[index].TextContent.Replace("\r", "").Replace("\n", ""));
                        //ここでは馬番号だけが欲しいので、確率その他の情報は無視

                        horseData.Add(new HorseDatum(horse, "", ""));
                    }
                    odds = Convert.ToDouble(tdList[startIndexOfHorseNumber - 1].TextContent.Trim('\n', '\r').Split(' ', '\n', '\r')[0]);
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Warn(ex);
                    continue;
                }
                yield return new OddsDatum(horseData, odds, odds);
            }
        }


        public List<OddsDatum> GetOdds(RaceData raceData, TicketType ticketType)
        {
            Chrome.Url = raceData.ToRaseOddsPageUrlString(ticketType);
            Thread.Sleep(500);
            //高速化のためにchromedriverではなくAngleSharpを使っている
            var parser = new HtmlParser();

            if (ticketType == TicketType.Win)
            {
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
                var trCollection = doc.GetElementById("odds_tan_block")?.GetElementsByTagName("tr");
                if (trCollection == null)
                {
                    return null;
                }
                return GetFromWonTable(trCollection).ToList();
            }
            else if (ticketType == TicketType.Place)
            {
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
                var trCollection = doc.GetElementById("odds_fuku_block")?.GetElementsByTagName("tr");
                if (trCollection == null)
                {
                    return null;
                }
                return GetFromPlaceTable(trCollection).ToList();
            }
            else
            {
                var num = 0;
                switch (ticketType)
                {
                    default:
                    case TicketType.Trio:
                        num = raceData.HorseCount * (raceData.HorseCount - 1) * (raceData.HorseCount - 2) / 6;
                        break;
                    case TicketType.Quinella:
                    case TicketType.Wide:
                        num = raceData.HorseCount * (raceData.HorseCount - 1) / 2;
                        break;
                    case TicketType.Trifecta:
                        num = raceData.HorseCount * (raceData.HorseCount - 1) * (raceData.HorseCount - 2);
                        break;
                    case TicketType.Exacta:
                        num = raceData.HorseCount * (raceData.HorseCount - 1);
                        break;
                }
                var results = new List<OddsDatum>(num);
                for (var val = 0; val < num; val += 100)
                {
                    try
                    {
                        var betType = Chrome.FindElement(By.Id("ninki_select"));
                        new SelectElement(betType).SelectByValue(val.ToString());
                        Thread.Sleep(1500);

                        var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
                        var trCollection = doc.GetElementsByClassName("RaceOdds_HorseList_Table").FirstOrDefault()?.GetElementsByTagName("tr");
                        if (trCollection == null)
                        {
                            break;
                        }
                        results.AddRange(GetFromPopularTable(trCollection, ticketType));
                    }
                    catch (Exception ex)
                    {
                        //競走除外などの理由で馬数が想定より少なかったようなケース
                        LoggerWrapper.Warn(ex);
                        break;
                    }
                }
                return results;
            }
        }


        public List<OddsDatum> GetRealTimeOdds(RaceData raceData, TicketType ticketType)
        {
            //netkeibaのオッズは最新のものではないので、最新のデータを提供しているサイトから取得する。
            //現状は地方競馬のリアルタイムオッズに対応できていないので、仕方なくnetkeibaのものを使う。

            if (raceData.HoldingDatum.Region.RagionType == RegionType.Regional)
            {
                return GetOdds(raceData, ticketType);
            }

            Chrome.Url = raceData.ToRealTimeOddsPageUrlString(ticketType);
            Thread.Sleep(500);
            //高速化のためにchromedriverではなくAngleSharpを使っている
            var parser = new HtmlParser();
            var result = new List<OddsDatum>();

            //TODO: あとでリファクタリングする
            if (ticketType == TicketType.Win)
            {
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
                var table = doc.GetElementsByTagName("table").FirstOrDefault();
                if (table == null)
                {
                    return null;
                }
                var trCollection = table.GetElementsByTagName("tr");
                var count = trCollection.Length;
                for (var i = 1; i < count; i++)
                {
                    var tdList = trCollection[i].GetElementsByTagName("td");
                    if (!int.TryParse(tdList[1].TextContent.Replace("\r", "").Replace("\n", ""), out var horse))
                    {
                        continue;
                    }
                    if (!double.TryParse(tdList.LastOrDefault().TextContent.Replace("\r", "").Replace("\n", ""), out var odds))
                    {
                        continue;
                    }
                    var horseName = tdList[2].TextContent.Replace("\r", "").Replace("\n", "");
                    var jockyName = tdList[3].TextContent.Replace("\r", "").Replace("\n", "");
                    //ここでは馬番号だけが欲しいので、確率は無視
                    result.Add(new OddsDatum(new List<HorseDatum> { new HorseDatum(horse, horseName, jockyName) }, odds, odds));
                }
            }
            else if (ticketType == TicketType.Place)
            {
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
                var table = doc.GetElementsByTagName("table").FirstOrDefault();
                if (table == null)
                {
                    return null;
                }
                var trCollection = table.GetElementsByTagName("tr");
                var count = trCollection.Length;
                for (var i = 1; i < count; i++)
                {
                    var tdList = trCollection[i].GetElementsByTagName("td");
                    if (!int.TryParse(tdList[1].TextContent.Replace("\r", "").Replace("\n", ""), out var horse))
                    {
                        continue;
                    }
                    var oddsString = tdList.LastOrDefault()?.TextContent.Replace("\r", "").Replace("\n", "");
                    if (string.IsNullOrEmpty(oddsString))
                    {
                        continue;
                    }
                    var oddsStringList = oddsString.Split("-");
                    if (oddsStringList.Length != 2)
                    {
                        continue;
                    }
                    if (!double.TryParse(oddsStringList[0], out var lowOdds))
                    {
                        continue;
                    }
                    if (!double.TryParse(oddsStringList[1], out var highOdds))
                    {
                        continue;
                    }
                    var horseName = tdList[2].TextContent.Replace("\r", "").Replace("\n", "");
                    var jockyName = tdList[3].TextContent.Replace("\r", "").Replace("\n", "");
                    //ここでは馬番号だけが欲しいので、確率は無視。
                    result.Add(new OddsDatum(new List<HorseDatum> { new HorseDatum(horse, horseName, jockyName) }, lowOdds, highOdds));
                }
            }
            else
            {
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));
                var tableCollection = doc.GetElementsByClassName("ninkijun-table");
                foreach (var table in tableCollection)
                {
                    var trCollection = table.GetElementsByTagName("tr");
                    var count = trCollection.Length;
                    for (var i = 1; i < count; i++)
                    {
                        List<HorseDatum> horseList;
                        var tdList = trCollection[i].GetElementsByTagName("td");
                        var horseListString = tdList[1].TextContent.Replace("\r", "").Replace("\n", "");

                        try
                        {
                            horseList = horseListString.Split('-').Select(_ => new HorseDatum(int.Parse(_), "", "")).ToList();
                        }
                        catch
                        {
                            continue;
                        }
                        var oddsString = tdList.LastOrDefault()?.TextContent.Replace("\r", "").Replace("\n", "");
                        if (string.IsNullOrEmpty(oddsString))
                        {
                            continue;
                        }
                        var oddsStringList = oddsString.Split("-");
                        double lowOdds = 0;
                        double highOdds = 0;
                        if (oddsStringList.Length == 1)
                        {
                            if (!double.TryParse(oddsStringList[0], out lowOdds))
                            {
                                continue;
                            }
                            highOdds = lowOdds;
                        }
                        else if (oddsStringList.Length == 2)
                        {
                            if (!double.TryParse(oddsStringList[0], out lowOdds))
                            {
                                continue;
                            }
                            if (!double.TryParse(oddsStringList[1], out highOdds))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }

                        //ここでは馬番号だけが欲しいので、確率その他の情報は無視。
                        //そもそもこのテーブルから馬名や騎手名は分からない。
                        result.Add(new OddsDatum(horseList, lowOdds, highOdds));
                    }
                }
            }
            return result;
        }

        public RaceResult GetRaceResult(RaceData raceData)
        {
            try
            {
                Chrome.Url = raceData.ToRaceResultPageUrlString();
                Thread.Sleep(500);

                //高速化のためにchromedriverではなくAngleSharpを使っている
                var parser = new HtmlParser();
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));

                var firstPayoutTable = doc.GetElementsByClassName("Payout_Detail_Table").FirstOrDefault();
                var winHorseString = firstPayoutTable.GetElementsByClassName("Tansho").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().TextContent.Trim('\r', '\n');
                var winPayoutString = firstPayoutTable.GetElementsByClassName("Tansho").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n');
                var placeHorseString = firstPayoutTable.GetElementsByClassName("Fukusho").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().TextContent.Trim('\r', '\n');
                var placePayoutString = firstPayoutTable.GetElementsByClassName("Fukusho").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n');
                var quinellaHorseString = firstPayoutTable.GetElementsByClassName("Umaren").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().TextContent.Trim('\r', '\n');
                var quinellaPayoutString = firstPayoutTable.GetElementsByClassName("Umaren").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n');

                var secondPayoutTable = doc.GetElementsByClassName("Payout_Detail_Table").Skip(1).FirstOrDefault();
                var extractHorseString = secondPayoutTable.GetElementsByClassName("Umatan").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().TextContent.Trim('\r', '\n');
                var extractPayoutString = secondPayoutTable.GetElementsByClassName("Umatan").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n');
                var trioHorseString = secondPayoutTable.GetElementsByClassName("Fuku3").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().TextContent.Trim('\r', '\n');
                var trioPayoutString = secondPayoutTable.GetElementsByClassName("Fuku3").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n');
                var trifectaHorseString = secondPayoutTable.GetElementsByClassName("Tan3").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().TextContent.Trim('\r', '\n');
                var trifectaPayoutString = secondPayoutTable.GetElementsByClassName("Tan3").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n');

                var wideHorseStringList = secondPayoutTable.GetElementsByClassName("Wide").FirstOrDefault().GetElementsByClassName("Result").FirstOrDefault().GetElementsByTagName("ul").Select(_ => _.TextContent.Trim('\r', '\n'));
                var wideHorsePayoutString = secondPayoutTable.GetElementsByClassName("Wide").FirstOrDefault().GetElementsByClassName("Payout").FirstOrDefault().TextContent.Trim('\r', '\n').Replace("\r", "").Replace("\n", "").Split('円').Where(_ => !string.IsNullOrEmpty(_));

                return new RaceResult(raceData)
                {
                    WinHorse = WinHorsesStringToList(winHorseString),
                    WinPayout = PayOutStringToDouble(winPayoutString),
                    PlaceHorse = WinHorsesStringToList(placeHorseString),
                    PlacePayout = PayOutStringToDouble(placePayoutString),
                    WideHorseList = wideHorseStringList.Select(WinHorsesStringToList).ToList(),
                    WidePayoutList = wideHorsePayoutString.Select(PayOutStringToDouble).ToList(),
                    QuinellaHorseList = WinHorsesStringToList(quinellaHorseString),
                    QuinellaPayout = PayOutStringToDouble(quinellaPayoutString),
                    ExtractHorseList = WinHorsesStringToList(extractHorseString),
                    ExtractPayout = PayOutStringToDouble(extractPayoutString),
                    TrioHorseList = WinHorsesStringToList(trioHorseString),
                    TrioPayout = PayOutStringToDouble(trioPayoutString),
                    TrifectaHorseList = WinHorsesStringToList(trifectaHorseString),
                    TrifectaPayout = PayOutStringToDouble(trifectaPayoutString),
                };
            }
            catch (Exception ex)
            {
                LoggerWrapper.Warn(ex);
                return null;
            }
        }


        private List<int> WinHorsesStringToList(string winHorsesString)
        {
            //呼び出し元で例外をそのまま受け取りたいので、敢えてTryParseでなくParse
            return winHorsesString.Trim().Split(' ', '\n', '\r').Select(_ => int.Parse(_)).ToList();
        }

        private double PayOutStringToDouble(string payOutString)
        {
            //呼び出し元で例外をそのまま受け取りたいので、敢えてTryParseでなくParse
            return double.Parse(payOutString.Replace(",", "").Replace("円", "").Trim());
        }



        /// <summary>
        /// ある開催日のレース情報を一覧取得する。Numberは使わないが一応。
        /// </summary>
        /// <param name="kaisaiDateYYYYMMDD"></param>
        /// <param name="raceType"></param>
        /// <returns></returns>
        public HoldingInformation GetHoldingInformation(DateTime raceDate, RegionType raceType)
        {
            var url = raceType == RegionType.Regional ?
                $"https://nar.netkeiba.com/top/race_list.html?kaisai_date={raceDate.ToString("yyyyMMdd")}" :
                $"https://race.netkeiba.com/top/race_list.html?kaisai_date={raceDate.ToString("yyyyMMdd")}";

            LoggerWrapper.Debug(url);
            try
            {
                Chrome.Url = url;
                //画面の切り替わり完了待ち
                Thread.Sleep(2000);

                var parser = new HtmlParser();
                var doc = parser.ParseDocument(Chrome.FindElement(By.TagName("body")).GetAttribute("innerHTML"));

                var raceListDataListCollection = doc.GetElementsByClassName("RaceList_Box").FirstOrDefault().GetElementsByClassName("RaceList_DataList");
                var count = raceListDataListCollection.Length;
                var regex = new Regex(@".*?(\d+)回 +(.*?) +(.*)日目.*");

                var raceUrlInfoList = new List<HoldingDatum>();
                for (var i = 0; i < count; i++)
                {
                    //.Textだと非表示のものが取得できない
                    var text = raceListDataListCollection[i].GetElementsByClassName("RaceList_DataTitle").FirstOrDefault().TextContent.Replace("\n", " ").Replace("\r", " ");
                    var match = regex.Match(text);
                    if (!match.Success)
                    {
                        continue;
                    }

                    if (!int.TryParse(match.Groups[1].Value, out var numberOfHeld))
                    {
                        continue;
                    }
                    var regionName = match.Groups[2].Value;
                    if (!int.TryParse(match.Groups[3].Value, out var numberOfDay))
                    {
                        continue;
                    }
                    var startTimeList = new List<DateTime>();
                    var horseCountList = new List<int>();
                    foreach (var item in raceListDataListCollection[i].GetElementsByClassName("RaceList_ItemContent"))
                    {
                        var textForData = item.GetElementsByClassName("RaceData").FirstOrDefault().TextContent.Replace("\r", "").Replace("\n", "");
                        var regexForData = new Regex(@".*?(\d\d):(\d\d).*?(\d+)頭.*?");
                        var matchForData = regexForData.Match(textForData);
                        if (!matchForData.Success)
                        {
                            continue;
                        }

                        if (!int.TryParse(matchForData.Groups[1].Value, out var hours))
                        {
                            continue;
                        }
                        if (!int.TryParse(matchForData.Groups[2].Value, out var minutes))
                        {
                            continue;
                        }
                        if (!int.TryParse(matchForData.Groups[3].Value, out var horseNumber))
                        {
                            continue;
                        }
                        var baseDate = raceDate.Date;
                        startTimeList.Add(baseDate.AddHours(hours).AddMinutes(minutes));
                        horseCountList.Add(horseNumber);
                    }

                    raceUrlInfoList.Add(new HoldingDatum(regionName, numberOfHeld, numberOfDay, raceDate, startTimeList, horseCountList));
                }
                return new HoldingInformation(raceUrlInfoList);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Warn(ex.Message);
                return null;
            }
        }
    }
}

