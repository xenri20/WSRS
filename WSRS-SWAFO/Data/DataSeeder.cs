using ClosedXML.Excel;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.Helpers;

namespace WSRS_SWAFO.Data;

/// <summary>
/// Seeds data to database
/// </summary>
public class DataSeeder
{

    /// <summary>
    /// Seeds students and then assigns them a report (administrative and traffic)
    /// </summary>
    /// <param name="filePath">Path to a valid excel file</param>
    /// <param name="context">Application's context</param>
    public static void SeedStudentReports(string filePath, ApplicationDbContext context)
    {
        try
        {
            Console.WriteLine("Starting data seeding...");

            using (var workbook = new XLWorkbook(filePath))
            {
                // Seed students
                var studentWorksheet = workbook.Worksheet("Students");
                if (studentWorksheet != null)
                {
                    Console.WriteLine("Seeding students...");
                    for (int row = 2; row <= studentWorksheet.LastRowUsed()!.RowNumber(); row++)
                    {
                        var student = new Student
                        {
                            StudentNumber = studentWorksheet.Cell(row, 1).GetValue<int>(),
                            FirstName = studentWorksheet.Cell(row, 2).GetValue<string>(),
                            LastName = studentWorksheet.Cell(row, 3).GetValue<string>(),
                            Email = studentWorksheet.Cell(row, 4).GetValue<string>()
                        };

                        if (!context.Students.Any(s => s.StudentNumber == student.StudentNumber))
                        {
                            context.Students.Add(student);
                            Console.WriteLine($"Added student: {student.StudentNumber} - {student.FirstName} {student.LastName}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Students worksheet not found.");
                }

                // Seed reports
                var reportWorksheet = workbook.Worksheet("ReportsEncoded");
                if (reportWorksheet != null)
                {
                    Console.WriteLine("Seeding reports...");
                    for (int row = 2; row <= reportWorksheet.LastRowUsed()!.RowNumber(); row++)
                    {
                        var report = new ReportEncoded
                        {
                            OffenseId = reportWorksheet.Cell(row, 3).GetValue<int>(),
                            CollegeID = reportWorksheet.Cell(row, 4).GetValue<string>(),
                            StudentNumber = reportWorksheet.Cell(row, 5).GetValue<int>(),
                            Course = reportWorksheet.Cell(row, 6).GetValue<string>(),
                            CommissionDate = DateHelper.ParseDate(reportWorksheet.Cell(row, 7).GetValue<string>()),
                            Sanction = reportWorksheet.Cell(row, 9).GetValue<string>(),
                            StatusOfSanction = reportWorksheet.Cell(row, 10).GetValue<string>(),
                            Description = reportWorksheet.Cell(row, 11).GetValue<string>(),
                        };

                        context.ReportsEncoded.Add(report);
                        Console.WriteLine($"Added report: {report.OffenseId} - {report.StudentNumber}");
                    }
                }
                else
                {
                    Console.WriteLine("ReportsEncoded worksheet not found.");
                }

                var trafficReportWorksheet = workbook.Worksheet("TrafficReportsEncoded");
                if (trafficReportWorksheet != null)
                {
                    Console.WriteLine("Seending traffic reports...");
                    for (int row = 2; row <= trafficReportWorksheet.LastRowUsed()!.RowNumber(); row++)
                    {
                        var trafficReport = new TrafficReportsEncoded
                        {
                            OffenseId = trafficReportWorksheet.Cell(row, 3).GetValue<int>(),
                            StudentNumber = trafficReportWorksheet.Cell(row, 4).GetValue<int>(),
                            CollegeID = trafficReportWorksheet.Cell(row, 5).GetValue<string>(),
                            PlateNumber = trafficReportWorksheet.Cell(row, 6).GetValue<string>(),
                            CommissionDate = DateHelper.ParseDate(trafficReportWorksheet.Cell(row, 7).GetValue<string>()),
                            Place = trafficReportWorksheet.Cell(row, 8).GetValue<string>(),
                            Remarks = trafficReportWorksheet.Cell(row, 9).GetValue<string>(),
                            DatePaid = DateHelper.ParseDateNullable(trafficReportWorksheet.Cell(row, 10).GetValue<string>()),
                            ORNumber = trafficReportWorksheet.Cell(row, 11).GetValue<string?>(),
                        };

                        context.TrafficReportsEncoded.Add(trafficReport);
                        Console.WriteLine($"Added report: {trafficReport.OffenseId} - {trafficReport.StudentNumber}");
                    }
                }
                else
                {
                    Console.WriteLine("TrafficReportsEncoded worksheet not found.");
                }

                context.SaveChanges();
                Console.WriteLine("Student reports data seeding completed successfully.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception (logging mechanism not shown here)
            Console.WriteLine($"An error occurred while seeding data: {ex.Message}");
        }
    }

    /// <summary>
    /// Seeds college data to database
    /// </summary>
    /// <param name="context">Application's context</param>
    public static void SeedCollege(ApplicationDbContext context)
    {
        List<string> collegeList = ["CBAA", "CCJE", "CEAT", "CLAC", "COED", "CICS", "COS", "CTHM"];

        Console.WriteLine("Starting data seeding...");

        foreach (var college in collegeList)
        {
            if (!context.College.Any(c => c.CollegeID == college))
            {
                context.College.Add(new College { CollegeID = college });
                Console.WriteLine($"Added {college} successfully");
            }
        }

        context.SaveChanges();
        Console.WriteLine("Seeding colleges data completed successfully.");
    }
}