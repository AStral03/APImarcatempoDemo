using Anviz.SDK;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi i servizi al contenitore
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registra AnvizDevice come singleton
builder.Services.AddSingleton<AnvizDevice>(provider =>
{
    // Crea il manager e connetti al dispositivo
    var manager = new AnvizManager
    {
        ConnectionUser = "admin", // Cambia con il tuo username
        ConnectionPassword = "12345", // Cambia con la tua password
        AuthenticateConnection = true
    };

    // Connessione asincrona al dispositivo
    return ConnectToDevice(manager, "192.168.1.246").GetAwaiter().GetResult();
});

// Metodo per connettersi al dispositivo
async Task<AnvizDevice> ConnectToDevice(AnvizManager manager, string host)
{
    try
    {
        // Connetti al dispositivo
        var device = await manager.Connect(host);

        // Se la connessione ha avuto successo, restituisci il dispositivo
        Console.WriteLine($"Connessione riuscita al dispositivo con IP: {host}");
        return device;
    }
    catch (Exception ex)
    {
        // In caso di errore, mostra il messaggio di errore
        Console.WriteLine($"Errore di connessione al dispositivo: {ex.Message}");
        throw;
    }
}

var app = builder.Build();

// Configurazione della pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

