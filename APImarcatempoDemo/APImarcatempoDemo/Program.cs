using Anviz.SDK;
using System.Net.Sockets;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura AnvizDevice come singleton
builder.Services.AddSingleton<AnvizDevice>(provider =>
{
    var socket = new TcpClient("192.168.1.246", 5010); // IP/porta del tuo dispositivo
    var device = new AnvizDevice(socket);
    device.SetConnectionPassword("1", "12345"); // Sostituisci se necessario
    return device;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
