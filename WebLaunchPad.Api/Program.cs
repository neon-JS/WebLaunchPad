var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IImageConverter, ImageConverter>();
builder.Services.AddSingleton<IConcurrencyService>(_ => new ConcurrencyService());
builder.Services.AddSingleton<IDeviceController>(_ =>
{
    var device = new FakeLaunchpadConsoleDevice();
    return new LaunchpadDeviceController(device);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
}

// app.UseHttpsRedirection();

app.UsePathBase(new PathString("/api"));
app.UseMiddleware<TaskCancellationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();


for (var i = 0; i < 9; i++)
{
    Console.WriteLine();
}