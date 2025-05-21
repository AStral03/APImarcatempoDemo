using Anviz.SDK;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("lista")]
        public async Task<IActionResult> GetUtenti()
        {
            try
            {
                var utenti = await _anvizDevice.GetEmployeesData();
                return Ok(utenti);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { messaggio = "Errore durante la lettura degli utenti", errore = ex.Message });
            }
        }
    }
}
