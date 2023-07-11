using HtmlAgilityPack;

using Microsoft.Extensions.Configuration;

using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace LiveVol.UI.Service
{
    public class Browser
    {
        private readonly List<LiveVolData> _data;
        private readonly EdgeDriver _driver;
        private readonly IConfiguration _config;
        private readonly System.Timers.Timer _timer;

        public Browser(List<LiveVolData> data, IConfiguration config, EdgeDriver driver)
        {
            _data = data;
            _config = config;
            _driver = driver;
            _timer = new Timer { Interval = 1000, AutoReset = false };
            _timer.Elapsed += T_Elapsed;
        }
        public async Task Initialize()
        {
            _driver.Url = "https://pro.livevol.com";
            await Login();
            var tabInput = _driver.FindElements(By.CssSelector("span[class*='TabbedView__tabTitle']")).FirstOrDefault(x => x.Text.Contains("Trade Tape"));
            tabInput.Click();
            await Task.Delay(3000);
            _timer.Start();
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var html = new HtmlDocument();
                var gridHtml = _driver.FindElement(By.CssSelector("div[class*='TradeTape__grid'")).GetAttribute("innerHTML");
                html.LoadHtml(gridHtml);

                //foreach (var row in html.DocumentNode.SelectNodes("//div[contains(@data-qa,'tradeTapeRow')]"))
                //{
                //    var data = new LiveVolData()
                //    {
                //        Date = columns[0].Text,
                //        Time = columns[1].Text,
                //        Symbol = columns[2].Text,
                //        Option = columns[3].Text,
                //        Qty = columns[4].Text,
                //        Price = columns[5].Text,
                //        Theo = columns[6].Text,
                //        Exchange = columns[7].Text,
                //        Condition = columns[8].Text,
                //        Market = columns[9].Text,
                //        TradeIV = columns[10].Text,
                //        UnderlyingTradePrice = columns[11].Text
                //    };
                //}

                //GridRows = gridData;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _timer.Start();
            }
        }

        public async Task Login()
        {
            await Task.Delay(8000);
            var emailInput = _driver.FindElement(By.Name("username"));
            var passInput = _driver.FindElement(By.Name("password"));
            var submitButton = _driver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Login");

            emailInput.Click();
            emailInput.SendKeys("tinypieceofland@gmail.com");

            await Task.Delay(500);

            passInput.Click();
            passInput.SendKeys("T@coBell1993");

            await Task.Delay(500);

            submitButton!.Click();
            await Task.Delay(12000);
        }
    }
}
