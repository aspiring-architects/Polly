using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
namespace PollyRequest.Policies
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateRetryPolicy { get; }
        public AsyncRetryPolicy<HttpResponseMessage> EqualTimeDelayRetryPolicy { get; }
        public AsyncRetryPolicy<HttpResponseMessage> ExpTimeDelayRetryPolicy { get; }


        public AsyncCircuitBreakerPolicy<HttpResponseMessage> SimpleCircuitBreakerPolicy { get; }

        public AsyncCircuitBreakerPolicy<HttpResponseMessage> AdvancedCircuitBreakerPolicy { get; }


        public ClientPolicy()
        {
            ImmediateRetryPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .RetryAsync(3);

            EqualTimeDelayRetryPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3,delay=>TimeSpan.FromSeconds(5));

            ExpTimeDelayRetryPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, delay => TimeSpan.FromSeconds(Math.Pow(3,delay)));


            /*
             * Polly offers two variations of the policy: the basic circuit breaker that cuts the connection 
             * if a specified number of consecutive failures occur, and the advanced circuit breaker that cuts 
             * the connection when a specified percentage of errors occur over a specified period 
             * and when a minimum number of requests have occurred in that period.
             */

            //if 2 consecutive errors occur, the circuit is cut for 30 seconds
            SimpleCircuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30), OnBreak, OnReset, OnHalfOpen);

            //The circuit will be cut if 30% of requests fail in a 60 second window,
            //with a minimum of 9 requests in the 60 second window, then the circuit should be cut for 30 seconds
            AdvancedCircuitBreakerPolicy = Policy.HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .AdvancedCircuitBreakerAsync(0.30, TimeSpan.FromSeconds(60), 9, TimeSpan.FromSeconds(30));
        }

        private void OnHalfOpen()
        {
            Console.WriteLine("Circuit in test mode, one request will be allowed.");
        }

        private void OnReset()
        {
            Console.WriteLine("Circuit closed, requests flow normally.");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> result, TimeSpan ts)
        {
            Console.WriteLine("Circuit cut, requests will not flow.");
        }
    }
}
