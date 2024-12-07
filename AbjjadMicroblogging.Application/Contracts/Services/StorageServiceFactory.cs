using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Application.Contracts.Services
{
    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public StorageServiceFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public IStorageService CreateStorageService()
        {
            string storageType = _configuration["StorageType"] ?? "Azure"; // Default to Azure

            return storageType.ToLower() switch
            {
                "azure" => _serviceProvider.GetRequiredService<AzureBlobStorageService>(),
                "local" => _serviceProvider.GetRequiredService<LocalFileStorageService>(),
                _ => throw new InvalidOperationException("Invalid storage type specified.")
            };
        }
    }
}