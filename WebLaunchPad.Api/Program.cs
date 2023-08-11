using WebLaunchPad.Communication.Devices;
using WebLaunchPad.Communication.Models;
using WebLaunchPad.Communication.Services;

var device = new FakeLaunchpadConsoleDevice();
var service = new LaunchpadDeviceController(device);

for (byte i = 0; i < 8; i++)
{
    for (byte j = 0; j < 8; j++)
    {
        var color = new Color((byte)(31 * i), (byte)(255 - 31 * j), (byte)(31 * j));
        await service.SetColorAsync(i, j, color, CancellationToken.None);

    }
}

await service.FlushAsync(CancellationToken.None);

for (var i = 0; i < 9; i++)
{
    Console.WriteLine();
}


/*
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
*/