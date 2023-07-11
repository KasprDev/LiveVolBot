using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace TraderTape
{
    public class SavedSession
    {
        public Dictionary<string, string> LocalStorage { get; set; }
        public BrowserCookie[] Cookies { get; set; }
        public Dictionary<string, string> SessionStorage { get; set; }
    }
}
