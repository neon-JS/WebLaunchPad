var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IGifConverter, GifConverter>();
builder.Services.AddSingleton<IConcurrencyService>(_ => new ConcurrencyService());

builder.Services.AddSingleton<IDeviceController>(_ =>
{
    var devicePath = builder.Configuration.GetValue<string>("LaunchpadPath");

    if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(devicePath))
    {
        return new LaunchpadDeviceController(new FakeLaunchpadConsoleDevice());
    }

    if (string.IsNullOrWhiteSpace(devicePath))
    {
        throw new ApplicationException("Device path not defined");
    }

    return new LaunchpadDeviceController(new FileHandleDevice(devicePath));
});

var app = builder.Build();

app.UsePathBase(new PathString("/api"));
app.UseMiddleware<TaskCancellationMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();