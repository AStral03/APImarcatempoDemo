using Anviz.SDK;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APImarcatempoDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UtentiController : ControllerBase
    {
        private readonly AnvizDevice _anvizDevice;

        public UtentiController(AnvizDevice anvizDevice)
        {
            _anvizDevice = anvizDevice;
        }

        // Endpoint per ottenere la lista degli utenti
        [HttpGet("lista")]
        public async Task<IActionResult> GetUtenti()
        {
            try
            {
                // Recupera i dati degli utenti dal dispositivo
                var utenti = await _anvizDevice.GetEmployeesData();

                // Mappa i dati in una forma più chiara e specifica
                var utentiList = utenti.Select(u => new
                {
                    u.Id,
                    u.Password,
                }).ToList();

                // Restituisci la lista degli utenti in formato JSON
                return Ok(utentiList);
            }
            catch (Exception ex)
            {
                // In caso di errore, restituisci un messaggio di errore con status 500
                return StatusCode(500, new
                {
                    messaggio = "Errore durante la lettura degli utenti",
                    errore = ex.Message
                });
            }
        }

    }
}

