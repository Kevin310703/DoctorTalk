using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorTalkWebApp.Scraper.Interfaces
{
    internal interface IAtraScraper
    {
        IReadOnlyCollection<IWebElement> GetElements();
        void IterateOverRaceElements();
        void Run();
    }
}
