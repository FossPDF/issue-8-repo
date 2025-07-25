using FossPDF.Drawing;
using FossPDF.FontSubset;

var builder = WebApplication.CreateSlimBuilder(new WebApplicationOptions
{
    WebRootPath = null,
    ContentRootPath = null,
});

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

FontManager.RegisterSubsetCallback((fontManager, subsets) =>
{
    var newFonts = new List<byte[]>(subsets.Count());
    var tasks = subsets.Select(fontToBeSubset => Task.Run(() =>
    {
        var subset = fontToBeSubset.Glyphs;
        var builder = new FontSubsetBuilder();
        builder.AddGlyphs(subset);
        builder.SetFont(fontToBeSubset.ShaperFont);

        var bytes = builder.Build();
        newFonts.Add(bytes);
        return Task.CompletedTask;
    }));
    Task.WaitAll(tasks.ToArray());

    fontManager.ClearCacheReadyForSubsets();

    foreach (var newFont in newFonts)
    {
        using var ms = new MemoryStream(newFont);
        fontManager.RegisterFont(ms);
    }

    newFonts.Clear();
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
