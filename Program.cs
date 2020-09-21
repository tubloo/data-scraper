using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OfficeOpenXml;


/* TODO
- Mapping of Excel to DB Columns
- Handle Incident ID Primary Key Violation
*/

namespace iDeskDataScraper
{
    class Program
    {
        static void Main(string[] args)
        {

            //TODO Check and Validate Date ArgumentConsole.WriteLine(args[0]);

            //downloadReport();
            exportReportToDB();

        }

        static void downloadReport()
        {
            Console.WriteLine("Downloading Report ...");
            String url = "https://www.who.int/healthinfo/statistics/data/en/";
            //Setup
            EdgeOptions edgeOptions = new Microsoft.Edge.SeleniumTools.EdgeOptions();
            edgeOptions.UseChromium = true;
            //edgeOptions.AddUserProfilePreference()
            //edgeOptions.AddArgument("headless");
            IWebDriver _driver = new Microsoft.Edge.SeleniumTools.EdgeDriver("./files/drivers", edgeOptions);

            //Open Browser
            _driver.Url = url;
            Console.WriteLine(_driver.Title);

            //Download Excel
            IReadOnlyList<IWebElement> listExcel = _driver.FindElement(By.Id("primary")).FindElements(By.TagName("A"));

            Console.WriteLine(listExcel.Count.ToString());

            //Extract Data from Excel and Store in DB
            foreach (IWebElement item in listExcel)
            {
                Console.WriteLine(item.Text);
                item.Click();

                System.Threading.Thread.Sleep(4000);

                //Wait for the alert to be displayed and store it in a variable
                //Alert alert = wait.until(ExpectedConditions.alertIsPresent());

                //Type your message
                //alert.sendKeys("Selenium");

                //Press the OK button
                //alert.dismiss();

                break;
            }

            //Tear Down
            _driver?.Quit();
            _driver?.Dispose();
        }

        static void exportReportToDB()
        {
            Console.WriteLine("Exporting Report Data to DB ...");

            //Read Excel
            FileInfo excelReport = new FileInfo("./files/reports/Book.xlsx");
            using (ExcelPackage excelPackage = new ExcelPackage(excelReport))
            {

                Console.WriteLine(excelReport.Name);
                Console.WriteLine(excelPackage.Workbook.Worksheets.Count);

                ExcelWorksheet firstSheet = excelPackage.Workbook.Worksheets[0];

                Console.WriteLine("Sheet 1 Data");
                Console.WriteLine($"Cell A2 Value   : {firstSheet.Cells["A1"].Text}");
                Console.WriteLine($"Cell A2 Color   : {firstSheet.Cells["A1"].Style.Font.Color.LookupColor()}");
                Console.WriteLine($"Cell B2 Formula : {firstSheet.Cells["A1"].Formula}");
                Console.WriteLine($"Cell B2 Value   : {firstSheet.Cells["A1"].Text}");
                Console.WriteLine($"Cell B2 Border  : {firstSheet.Cells["A1"].Style.Border.Top.Style}");
                Console.WriteLine("");


            }

            saveToDB();

        }

        static void saveToDB()
        {
            
            using (var db = new iDeskDbContext())
            {
                db.Incidents.Add(new Incident { IncidentID = "INC004", Summary = "Test Summary" });
                db.SaveChanges();

                foreach (var incident in db.Incidents)
                {
                    Console.WriteLine(incident.IncidentID, incident.Summary);
                }
                    
            }
        }
    }
}
