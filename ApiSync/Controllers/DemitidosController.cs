using ApiSync.Data;
using ApiSync.Models;
using ApiSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiSync.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class DemitidosController : ControllerBase
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;

        public DemitidosController(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("BuscaDesligados")]
        public List<Funcionario> GetDemitidos()
        {
            DemitidosService demitidoService = new();
            List<Funcionario> funcionarioList = demitidoService.BuscaDemitidos();

            return funcionarioList;
        }
    }
}
