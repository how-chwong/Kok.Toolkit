using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Kok.Test.AvaloniaDemo.Services
{
    public class TestConfigService
    {
        private readonly IConfiguration _configuration;

        public TestConfigService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConfig(string key)
        {
            return _configuration[key] ?? string.Empty;
        }
    }

    public class TestConfigService2
    {
        private readonly AppSetting _settings;

        public TestConfigService2(AppSetting settings)
        {
            _settings = settings;
        }

        public string GetUri()
        {
            return _settings.Uri;
        }
    }

    public class AppSetting
    {
        public const string SectionName = "Setting";

        [Required(ErrorMessage = "Elasticsearch Uri 为必填项")]
        [Url(ErrorMessage = "Uri 格式不正确，必须是有效的 URL")]
        public string Uri { get; set; } = "http://localhost:9200";

        [Required(ErrorMessage = "API Key 是必填项")]
        public string ApiKey { get; set; } = string.Empty;
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTestConfigService(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. 绑定配置
            services.AddOptions<AppSetting>()
                .Bind(configuration.GetSection(AppSetting.SectionName));
            //.ValidateDataAnnotations()
            //.ValidateOnStart();

            // 2. 注册客户端 (Singleton)
            services.AddSingleton<TestConfigService2>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<AppSetting>>().Value;

                return new TestConfigService2(settings);
            });

            return services;
        }
    }
}
