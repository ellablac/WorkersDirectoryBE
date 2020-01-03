using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using WorkersDirectoryBE.Models;

namespace WorkersDirectoryBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
        
    {
        private readonly IDocumentClient _documentClient;
        readonly String databaseId;
        readonly String collectionId;
        public IConfiguration Configuration { get; }

        public WorkersController(IDocumentClient documentClient, IConfiguration configuration)
        {
            _documentClient = documentClient;
            Configuration = configuration;

            databaseId = Configuration["DatabaseId"];
            collectionId = "TeamRoles";

            //BuildCollection().Wait();
        }

        private async Task BuildCollection()
        {
            await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = databaseId });
            await _documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(databaseId),
                new DocumentCollection { Id = collectionId });
        }
        // GET: api/Workers
        [HttpGet]
        public IQueryable<Worker> Get()
        {          
            return _documentClient.CreateDocumentQuery<Worker>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
        }
       
        [HttpGet("{id}")]
        public IQueryable<Worker> Get(string id)
        {
            return _documentClient.CreateDocumentQuery<Worker>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).Where((i) => i.id == id);
        }      

        [HttpPost]
        //public async Task<IActionResult> Post([FromBody] Worker item)
        public async Task<System.Net.HttpStatusCode> Post([FromBody] Worker item)
        {
            var response = await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId), item);
            return response.StatusCode;
            //return Ok();
        }

        //*************************************************************************************
        // This is where we swap the roles. This is envoked by a Put request to the endpoint
        // api/workers. The request body must contain JSON of a worker object with a new role.
        //*************************************************************************************
        [HttpPut]
        public async Task<IQueryable<Worker>> Put([FromBody] Worker worker1NewRole)
        {
            Worker worker1OldRole = (_documentClient.CreateDocumentQuery<Worker>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).Where((i) => i.id == worker1NewRole.id)).AsEnumerable().First();
            Worker worker2 = (_documentClient.CreateDocumentQuery<Worker>(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId)).Where((i) => i.role == worker1NewRole.role)).AsEnumerable().First();
            worker2.role = worker1OldRole.role;
            await Put(worker1NewRole.id, worker1NewRole);
            await Put(worker2.id, worker2);
            return Get();     
            //return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Worker item)
        {
            await _documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, id),
                item);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(databaseId, collectionId, id));
            return Ok();
        }


    }
}
