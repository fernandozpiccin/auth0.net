using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth0.NETCore3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AuthenticationApiClient _authClient;

        public ClientsController(IConfiguration configuration)
        {
            Configuration = configuration;
            _authClient = new AuthenticationApiClient(Configuration["Auth0:Domain"]);
        }

        public IConfiguration Configuration { get; }

        [HttpGet]
        public async Task<IEnumerable<Client>> Get()
        {
        

            var mgmntClient = new ManagementApi.ManagementApiClient(Configuration["Auth0:ClientId"], Configuration["Auth0:ClientSecret"], Configuration["Auth0:Domain"]);

            var conns = await mgmntClient.Connections.GetAllAsync(new GetConnectionsRequest(), new ManagementApi.Paging.PaginationInfo());

            foreach (var item in conns)
            {
                if (item.Name.IndexOf("Temp-Int") > -1)
                {
                    await mgmntClient.Connections.DeleteAsync(item.Id);
                }

            }

            return await mgmntClient.Clients.GetAllAsync(new GetClientsRequest(), new ManagementApi.Paging.PaginationInfo());
        }
    }
}
