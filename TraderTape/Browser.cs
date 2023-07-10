using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V112.Network;
using OpenQA.Selenium.Edge;
using Cookie = OpenQA.Selenium.Cookie;

namespace TraderTape
{
    public class Browser
    {
        public EdgeDriver? Driver;
        private SavedSession? _session;
        public async Task Initialize()
        {
            var options = new EdgeOptions() { };
            options.AddArgument($"--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.67");

            Driver = new EdgeDriver(options);
            Driver.Url = "https://managemyink.com/login";

            if (File.Exists("browser.json"))
            {
                var js = (IJavaScriptExecutor)Driver!;
                var browserData = JsonConvert.DeserializeObject<SavedSession>(await File.ReadAllTextAsync("browser.json"))!;

                _session = browserData;

                foreach (var localStorage in browserData.LocalStorage)
                {
                    js.ExecuteScript($"localStorage.setItem('{localStorage.Key}', '{localStorage.Value}');");
                    await Task.Delay(100);
                }

                foreach (var cookie in browserData.Cookies)
                {
                    Driver!.Manage().Cookies.AddCookie(new Cookie(cookie.Name, cookie.Value, cookie.Domain, cookie.Path,
                        cookie.Expiry));
                }
            }

            Driver.Navigate().Refresh();

            if (_session == null)
                await Login();
        }

        public async Task Login()
        {
            var emailInput = Driver!.FindElement(By.Name("email"));
            var passInput = Driver.FindElement(By.Name("pass"));
            var submitButton = Driver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Login");

            emailInput.Click();
            emailInput.SendKeys("anthony@kaspr.co");

            await Task.Delay(500);

            passInput.Click();
            passInput.SendKeys("Picturesque1@");

            await Task.Delay(500);

            submitButton!.Click();

            await Task.Delay(2000);

            await SaveInstance();
        }

        // Save the cookies & local storage info from the Selenium browser session.
        public async Task SaveInstance()
        {
            var js = (IJavaScriptExecutor)Driver!;
            var localStorage = JsonConvert.SerializeObject(js.ExecuteScript("var keys = []; for (var key in localStorage) { keys.push(key); } return keys;"));
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
                }).ToArray()
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

            await File.WriteAllTextAsync("browser.json", JsonConvert.SerializeObject(session));

            _session = session;
        }
    }
}
