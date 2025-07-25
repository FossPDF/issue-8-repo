
using System.Diagnostics;
using FossPDF.Fluent;
using FossPdfIssue8.Models;
using Microsoft.AspNetCore.Mvc;

namespace FossPdfIssue8.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var myobject = new
        {
            Title = "Oranges",
            Description = "The orange, also called sweet orange to distinguish it from the bitter orange (Citrus × aurantium), is the fruit of a tree."
        };
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Content()
                    .Column(col =>
                    {
                        col.Item().Text($"Title: {myobject.Title}");
                        col.Item().Text($"Description : {myobject.Description}");
                    });
            });
        });

        return new FileContentResult(document.GeneratePdf(), "application/pdf");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return Json(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
