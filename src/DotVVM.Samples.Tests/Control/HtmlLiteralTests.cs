﻿using Riganti.Utils.Testing.Selenium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Testing.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotVVM.Samples.Tests.Control
{
    [TestClass]
    public class HtmlLiteralTests : AppSeleniumTest
    {

        [TestMethod]
        public void Control_HtmlLiteral_HtmlLiteral()
        {
            RunInAllBrowsers(browser =>
            {
                browser.NavigateToUrl(SamplesRouteUrls.ControlSamples_HtmlLiteral_HtmlLiteral);

                var column1 = browser.ElementAt("td", 0);
                var column2 = browser.ElementAt("td", 1);

                column1.ElementAt("fieldset", 0).Single("div").CheckIfInnerTextEquals("Hello value");

                column2.ElementAt("fieldset", 0).Single("div").CheckIfInnerTextEquals("Hello value");

                column2.ElementAt("fieldset", 1).FindElements("div").ThrowIfDifferentCountThan(0);
                column2.ElementAt("fieldset", 1).CheckIfInnerText(t => t.Contains("Hello value"));
            });
        }
        
    }
}