using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Dapr;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SET_Vinsight.ProxyProcessor
{
    public class LockProcessor
    {
        private readonly ILogger<LockProcessor> _logger;

        public LockProcessor(ILogger<LockProcessor> logger)
        {
            _logger = logger;
        }

        [Function("ProcessLock")]
        public Task ProcessLockAsync(
            [DaprTopicTrigger("servicebus-pubsub", Topic = "locks")]
            byte[] messagePayload) 
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(messagePayload);
                dynamic evt = JsonConvert.DeserializeObject(json);
                string operation = evt.operation ?? "unknown";
                string dataValue = evt.data ?? "<no data>";
                _logger.LogInformation($"Operation: {operation}, Data: {dataValue}");
                _logger.LogInformation($"Timestamp UTC: {DateTime.UtcNow:O}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error on processing event 'locks': {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
