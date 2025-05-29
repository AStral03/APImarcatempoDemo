using APImarcatempoDemo; // Namespace del ScheduledDataDump
using Anviz.SDK;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Configura i servizi
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrazione dei dispositivi Anviz
var deviceIps = new List<string> { "192.168.1.245", "192.168.1.246" }; // Aggiungi gli IP dei tuoi dispositivi, aggiungi "192.168.0.138"

foreach (var ip in deviceIps)
{
    builder.Services.AddSingleton<AnvizDevice>(provider =>
    {
        var manager = new AnvizManager
        {
            ConnectionUser = "admin",
            ConnectionPassword = "12345",
            AuthenticateConnection = true
        };
        return ConnectToDevice(manager, ip, provider.GetRequiredService<ILogger<AnvizDevice>>()).GetAwaiter().GetResult();
    });
}

// Aggiungi il Background Service per il dump dei dati
builder.Services.AddSingleton<ScheduledDataDump>();

var app = builder.Build();

// Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Avvia il dump programmato (puoi farlo all'avvio dell'applicazione)
var scheduledDataDump = app.Services.GetRequiredService<ScheduledDataDump>();
scheduledDataDump.Start();

app.Run();

// Metodo per connettersi al dispositivo
async Task<AnvizDevice> ConnectToDevice(AnvizManager manager, string host, ILogger<AnvizDevice> logger)
{
    try
    {
        var device = await manager.Connect(host);
        logger.LogInformation($"Connessione riuscita al dispositivo con IP: {host}");
        return device;
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Errore di connessione al dispositivo: {ex.Message}");
        throw;
    }
}
