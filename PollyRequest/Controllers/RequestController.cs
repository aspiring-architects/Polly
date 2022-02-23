using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PollyRequest.Policies;

namespace PollyRequest.Properties
{
   
    [Route("[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> ReceiveGreetings()
        {
            ////Approach 1: Default
            //HttpClient client = new HttpClient();
            //HttpResponseMessage response = await client.GetAsync("http://localhost:5089/Hello");

            //Approach 2: Immedicate Retry
            //HttpClient client = new HttpClient();
            //ClientPolicy policy = new ClientPolicy();
            //HttpResponseMessage response = await policy.ImmediateRetryPolicy.ExecuteAsync(() => client.GetAsync("http://localhost:5089/Hello"));

            //Approach 3: Equal Time Delay Retry Policy
            //HttpClient client = new HttpClient();
            //ClientPolicy policy = new ClientPolicy();
            //HttpResponseMessage response = await policy.EqualTimeDelayRetryPolicy.ExecuteAsync(() => client.GetAsync("http://localhost:5089/Hello"));

            //Approach 4: Exp Time Delay Retry Policy
            //HttpClient client = new HttpClient();
            //ClientPolicy policy = new ClientPolicy();
            //HttpResponseMessage response = await policy.ExpTimeDelayRetryPolicy.ExecuteAsync(() => client.GetAsync("http://localhost:5089/Hello"));

            //Approach 5: Simple Circuit Breaker Policy
            //HttpClient client = new HttpClient();
            //ClientPolicy policy = new ClientPolicy();
            //HttpResponseMessage response = await policy.SimpleCircuitBreakerPolicy.ExecuteAsync(() => client.GetAsync("http://localhost:5089/Hello"));

            //Approach 6: Advanced Circuit Breaker Policy
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await ClientPolicy.AdvancedCircuitBreakerPolicy.ExecuteAsync(() => client.GetAsync("http://localhost:5089/Hello"));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Received Success Greetings " + DateTime.Now);
                string res = await response.Content.ReadAsStringAsync();
                return Ok(res);
            }
            else
            {
                Console.WriteLine("Received Error " + DateTime.Now);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
