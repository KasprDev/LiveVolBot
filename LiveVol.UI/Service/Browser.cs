using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using TraderTape;

namespace LiveVol.UI.Service
{
    public class Browser
    {
        public EdgeDriver? Driver;
        private SavedSession? _session;
        private System.Timers.Timer _timer;
        public List<LiveVolData> GridRows = new List<LiveVolData>();

        public async Task Initialize()
        {
            var options = new EdgeOptions() { };
            options.AddArgument($"--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.67");

            Driver = new EdgeDriver(options);
            Driver.Url = "https://pro.livevol.com";

            //if (File.Exists("browser.json"))
            //{
            //    var js = (IJavaScriptExecutor)Driver!;
            //    var browserData = JsonConvert.DeserializeObject<SavedSession>(await File.ReadAllTextAsync("browser.json"))!;

            //    _session = browserData;

            //    foreach (var localStorage in browserData.LocalStorage)
            //    {
            //        js.ExecuteScript($"localStorage.setItem('{localStorage.Key}', '{localStorage.Value}');");
            //    }

            //    foreach (var sessionStorage in browserData.SessionStorage)
            //    {
            //        js.ExecuteScript($"sessionStorage.setItem('{sessionStorage.Key}', '{sessionStorage.Value}');");
            //    }

            //    foreach (var cookie in browserData.Cookies)
            //    {
            //        Driver!.Manage().Cookies.AddCookie(new Cookie(cookie.Name, cookie.Value, cookie.Domain, cookie.Path,
            //            cookie.Expiry));
            //    }

            //    Driver.Url = $"https://pro.livevol.com";
            //}

            //if (_session == null)
                await Login();

            var tabInput = Driver!.FindElements(By.CssSelector("span[class*='TabbedView__tabTitle']")).FirstOrDefault(x => x.Text.Contains("Trade Tape"));
            tabInput.Click();

            await Task.Delay(3000);

            _timer = new System.Timers.Timer() { Interval = TimeSpan.FromSeconds(1).TotalMilliseconds, AutoReset = false };
            _timer.Elapsed += T_Elapsed;
            _timer.Start();
        }

        private void T_Elapsed(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var grid = Driver!.FindElement(By.CssSelector("div[class*='TradeTape__grid'"))
                    .FindElements(By.CssSelector("div[data-qa='tradeTapeRow'"));

                List<LiveVolData> gridData = new List<LiveVolData> { };

                foreach (var el in grid)
                {
                    var columns = el.FindElements(By.CssSelector("div[class*='TradeTape__standardCell'"));

                    var data = new LiveVolData()
                    {
                        Date = columns[0].Text,
                        Time = columns[1].Text,
                        Symbol = columns[2].Text,
                        Option = columns[3].Text,
                        Qty = columns[4].Text,
                        Price = columns[5].Text,
                        Theo = columns[6].Text,
                        Exchange = columns[7].Text,
                        Condition = columns[8].Text,
                        Market = columns[9].Text,
                        TradeIV = columns[10].Text,
                        UnderlyingTradePrice = columns[11].Text
                    };

                    gridData.Add(data);
                }

                GridRows = gridData;
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
            await Task.Delay(10000);
            var emailInput = Driver!.FindElement(By.Name("username"));
            var passInput = Driver.FindElement(By.Name("password"));
            var submitButton = Driver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Login");

            emailInput.Click();
            emailInput.SendKeys("tinypieceofland@gmail.com");

            await Task.Delay(500);

            passInput.Click();
            passInput.SendKeys("T@coBell1993");

            await Task.Delay(500);

            submitButton!.Click();

            await Task.Delay(12000);

            await SaveInstance();
        }

        // Save the cookies & local storage info from the Selenium browser session.
        public async Task SaveInstance()
        {
            var js = (IJavaScriptExecutor)Driver!;
            var localStorage = JsonConvert.SerializeObject(js.ExecuteScript("var keys = []; for (var key in localStorage) { keys.push(key); } return keys;"));
            var sessionStorage = JsonConvert.SerializeObject(js.ExecuteScript("var keys = []; for (var key in sessionStorage) { keys.push(key); } return keys;"));
            var cookies = Driver!.Manage().Cookies.AllCookies.ToArray();

            var session = new SavedSession()
            {
                LocalStorage = new Dictionary<string, string>() { },
                Cookies = cookies.Select(x => new BrowserCookie()
                {
                    Name = x.Name,
                    Value = x.Value,
                    Domain = x.Domain,
                    Path = x.Path,
                    Expiry = x.Expiry,
                }).ToArray(),
                SessionStorage = new Dictionary<string, string>() {}
            };

            // List of keys returned in localStorage that need to be ignored.
            var ignore = new []
            {
                "length",
                "clear",
                "getItem",
                "key",
                "removeItem",
                "setItem"
            };

            foreach (var key in JsonConvert.DeserializeObject<string[]>(localStorage)!)
            {
                if (ignore.Contains(key)) { continue; }

                var val = js.ExecuteScript($"return localStorage.getItem('{key}');").ToString()!;
                session.LocalStorage.Add(key, val);
            }

            foreach (var key in JsonConvert.DeserializeObject<string[]>(sessionStorage)!)
            {
                if (ignore.Contains(key)) { continue; }

                var val = js.ExecuteScript($"return sessionStorage.getItem('{key}');").ToString()!;
                session.SessionStorage.Add(key, val);
            }

            await File.WriteAllTextAsync("browser.json", JsonConvert.SerializeObject(session));

            _session = session;
        }
    }
}
