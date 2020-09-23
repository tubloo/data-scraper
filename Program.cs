using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Edge.SeleniumTools;
using OpenQA.Selenium;
using OfficeOpenXml;


/* TODO
- Mapping of Excel to DB Columns
- Handle Incident ID Primary Key Violation
- Store Last Sync Date
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

                ExcelWorksheet firstSheet = excelPackage.Workbook.Worksheets[0];

                Console.WriteLine("\nExcel ...");
                Console.WriteLine("Total Rows - " + firstSheet.Dimension.Rows.ToString());

                using (var db = new iDeskDbContext())
                {


                    for (int i = 2; i <= firstSheet.Dimension.Rows; i++)
                    {
                        Console.WriteLine(firstSheet.Cells[i, 1].Text);

                        Incident incident = new Incident();

                        /*
                        Iterate through class properties
                        Assign Propert Value by Property Attribute Excel Position 
                        */

                        Type type = typeof(Incident);
                        PropertyInfo[] properties = type.GetProperties();
                        foreach (PropertyInfo property in properties)
                        {
                            property.SetValue(incident, firstSheet.Cells[i, property.GetCustomAttribute<ExcelColPos>().Colpos].Text);
                        }


                        db.Incidents.Add(incident);

                        try
                        {
                            db.SaveChanges();
                        }
                        catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
                        {
                            db.Incidents.Update(incident);
                            db.SaveChanges();
                        }
                    }

                    ControlParam control = new ControlParam();
                    control.Key = "Last Executed On";
                    control.Value = System.DateTime.Now.ToString();
                    db.ControlParams.Update(control);
                    db.SaveChanges();

                }
            }

            PrintDB();

        }

        static void PrintDB()
        {

            Console.WriteLine("\nDatabase ..");

            using (var db = new iDeskDbContext())
            {


                foreach (var incident in db.Incidents)
                {
                    Console.WriteLine(incident.IncidentID, incident.Summary);
                }

            }
        }
    }
}
