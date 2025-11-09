using BloggingAgent.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add BloggingAgent services
builder.Services.AddBloggingAgentServices(builder.Configuration);

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use custom middleware
app.UseBloggingAgentMiddleware();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{action=Index}/{id?}",
    defaults: new { controller = "Blog" });

app.MapControllerRoute(
    name: "analytics",
    pattern: "analytics/{action=Index}",
    defaults: new { controller = "Analytics" });

app.MapControllerRoute(
    name: "settings",
    pattern: "settings/{action=Index}",
    defaults: new { controller = "Settings" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");

app.Run();
