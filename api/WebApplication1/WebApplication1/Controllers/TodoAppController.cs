using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AmazonDynamoDBClient _dynamoDbClient;
        private readonly DynamoDBContext _context;

        public TodoAppController(IConfiguration configuration)
        {
            _configuration = configuration;
            var awsOptions = configuration.GetAWSOptions();
            _dynamoDbClient = new AmazonDynamoDBClient(awsOptions);
            _context = new DynamoDBContext(_dynamoDbClient);
        }

        [HttpGet]
        [Route("GetNotes")]
        public async Task<JsonResult> GetNotes()
        {
            var request = new ScanRequest
            {
                TableName = "Notes"
            };
            var response = await _dynamoDbClient.ScanAsync(request);
            var items = response.Items;
            return new JsonResult(items);
        }

        [HttpPost]
        [Route("AddNotes")]
        public async Task<JsonResult> AddNotes([FromForm] string newNotes)
        {
            var table = Table.LoadTable(_dynamoDbClient, "Notes");
            var item = new Document
            {
                ["Id"] = Guid.NewGuid().ToString(),
                ["Note"] = newNotes
            };
            await table.PutItemAsync(item);
            return new JsonResult("Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteNotes")]
        public async Task<JsonResult> DeleteNotes([FromForm] string id)
        {
            var table = Table.LoadTable(_dynamoDbClient, "Notes");
            await table.DeleteItemAsync(id);
            return new JsonResult("Deleted Successfully");
        }
    }
}
