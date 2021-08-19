using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using RedisWebDay3.Data;

using StackExchange.Redis;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RedisWebDay3.Repository
{
    public interface ICustomerRepository
    {
        Task WritePendingCustomer(Customer c);
        IAsyncEnumerable<Customer> GetPendingCustomer();
    }


    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDistributedCache _redis;
        private readonly IConfiguration _configuration;

        public CustomerRepository(IDistributedCache redis, IConfiguration configuration)
        {
            _redis = redis;
            _configuration = configuration;
        }

        public async Task WritePendingCustomer(Customer c)
        {
            var jsonText = JsonConvert.SerializeObject(c, settings: new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
            await _redis.SetStringAsync(":customer:" + Guid.NewGuid(), jsonText);
        }

        public async IAsyncEnumerable<Customer> GetPendingCustomer()
        {
            ConfigurationOptions options = ConfigurationOptions.Parse(_configuration.GetConnectionString("Redis"));
            ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(options);
            EndPoint endPoint = connection.GetEndPoints().First();
            var pattern = $"instance:customer:*";
            RedisKey[] keys = connection.GetServer(endPoint).Keys(pattern: pattern).ToArray();
            foreach (var key in keys)
            {

                var instanceKey = key.ToString().Replace("instance", "");
                var jsonText = await _redis.GetStringAsync(instanceKey);
                var customer = JsonConvert.DeserializeObject<Customer>(jsonText);
                if (customer != null)
                {
                    await _redis.RemoveAsync(instanceKey);
                    yield return customer;
                }
            }

        }
    }
}
