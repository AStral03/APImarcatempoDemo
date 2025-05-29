using APImarcatempoDemo.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class ScheduledDataDump : IDisposable
{
    private Timer? _timer;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // Esecuzione ogni 2 minuti
    private readonly string _connectionString = "Server=localhost;Database=marcatempo;User=root;Password="; // Modifica con la tua stringa di connessione MySQL

    public ScheduledDataDump()
    {
        // Costruttore vuoto
    }

    public void Start()
    {
        // Avvia il primo dump subito e poi ripeti ogni 2 minuti
        DumpDataAsync().Wait();
        ScheduleNextRun();
    }

    private void ScheduleNextRun()
    {
        // Se il timer esiste già, lo dispose
        _timer?.Dispose();

        // Riprogramma l'esecuzione del dump a intervalli regolari
        _timer = new Timer(async _ =>
        {
            await DumpDataAsync();

            // Riprogramma per eseguire tra 2 minuti
            ScheduleNextRun();
        }, null, _interval, _interval); // Intervallo di 2 minuti
    }

    private async Task DumpDataAsync()
    {
        try
        {
            Console.WriteLine($"Inizio dump dati alle {DateTime.Now}");

            // Recupera e inserisci i dati nel database
            await RecuperaETimbricaIEInserisciInMySql();

            Console.WriteLine($"Dump dati completato alle {DateTime.Now}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante il dump dei dati: {ex.Message}");
        }
    }

    private async Task RecuperaETimbricaIEInserisciInMySql()
    {
        // Implementa la logica per recuperare i dati dalla tua API
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://localhost:7246/records/lista"); // Modifica con il tuo endpoint API

                // Deserializza il JSON in un oggetto ApiResponse
                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(response);

                if (apiResponse != null)
                {
                    Console.WriteLine($"Recuperati {apiResponse.Data.Count} utenti.");

                    // Inserisci i dati nel database
                    await InsertDataIntoMySql(apiResponse.Data);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante la chiamata all'API o deserializzazione: {ex.Message}");
        }
    }

    private async Task InsertDataIntoMySql(List<UserAttendance> data)
    {
        // Inserisce i dati nel database
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var user in data)
                {
                    foreach (var record in user.AttendanceRecords)
                    {
                        var commandText = @"
                            INSERT INTO Timbrature (UserId, UserName, CheckTime, RecordType)
                            VALUES (@UserId, @UserName, @CheckTime, @RecordType)";

                        var command = new MySqlCommand(commandText, connection, transaction);
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        command.Parameters.AddWithValue("@UserName", user.Name);
                        command.Parameters.AddWithValue("@CheckTime", record.DateTime);
                        command.Parameters.AddWithValue("@RecordType", record.RecordType);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                await transaction.CommitAsync();
                Console.WriteLine("Dati inseriti correttamente.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Errore durante l'inserimento nel database: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
