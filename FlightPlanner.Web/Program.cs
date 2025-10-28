using FlightPlanner.Web.Handlers;
using FlightPlanner.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the loading indicator handler
builder.Services.AddTransient<LoadingIndicatorHandler>();

// Configure HttpClient for API with loading indicator handler
builder.Services.AddHttpClient("FlightPlannerApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5062");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<LoadingIndicatorHandler>();

// Register FlightPlannerApiService
builder.Services.AddScoped<IFlightPlannerApiService, FlightPlannerApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();