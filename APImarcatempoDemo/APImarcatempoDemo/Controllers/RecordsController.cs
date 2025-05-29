using Anviz.SDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace APImarcatempoDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecordsController : ControllerBase
    {
        private readonly List<AnvizDevice> _anvizDevices;
        private readonly IMemoryCache _cache;


        public RecordsController(IEnumerable<AnvizDevice> anvizDevices, IMemoryCache cache)
        {
            _anvizDevices = anvizDevices.ToList(); // Converti l'input in una lista
            _cache = cache;
        }


        [HttpGet("lista")]
        public async Task<IActionResult> GetTimbratureUtenti()
        {
            try
            {
                List<object> utentiList = new List<object>();

                foreach (var device in _anvizDevices)
                {
                    var utenti = await device.GetEmployeesData();
                    var records = await device.DownloadRecords(false);

                    var deviceUtentiList = utenti.Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Password,
                        AttendanceRecords = records
                            .Where(r => r.UserCode == u.Id && r.RecordType >= 0 && r.RecordType <= 5)
                            .Select(r => new
                            {
                                r.DateTime,
                                r.RecordType
                            }).ToList()
                    }).Cast<object>().ToList();

                    utentiList.AddRange(deviceUtentiList);
                }

                // Restituisce un oggetto con la proprietà 'data' contenente l'array
                return Ok(new { data = utentiList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    messaggio = "Errore durante la lettura delle timbrature",
                    errore = ex.Message
                });
            }
        }

    }
}