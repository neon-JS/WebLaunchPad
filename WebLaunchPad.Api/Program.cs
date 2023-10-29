var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IGifConverter, GifConverter>();
builder.Services.AddSingleton<IConcurrencyService>(_ => new ConcurrencyService());

builder.Services.AddSingleton<IDeviceController>(_ =>
{
    var devicePath = builder.Configuration.GetValue<string>("LaunchpadPath");

    if (builder.Environment.IsDevelopment())
    {
        return string.IsNullOrWhiteSpace(devicePath) 
            ? new LaunchpadDeviceController(new FakeLaunchpadConsoleDevice()) 
            : new LaunchpadDeviceController(new SocketDevice(devicePath));
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