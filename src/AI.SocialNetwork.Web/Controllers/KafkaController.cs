using AI.SocialNetwork.Application.Contracts;
using AI.SocialNetwork.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AI.SocialNetwork.Web.Controllers;

[ApiController]
[Route("api/kafka")]
[Authorize(Roles = "admin")]
public class KafkaController : ControllerBase
{
    private readonly IEventPublisher _publisher;
    private readonly IConfiguration _config;
    private readonly ILogger<KafkaController> _logger;

    public KafkaController(IEventPublisher publisher, IConfiguration config, ILogger<KafkaController> logger)
    {
        _publisher = publisher;
        _config = config;
        _logger = logger;
    }

    // Статус публикатора и список топиков (админ)
    [HttpGet("status")]
    public IActionResult Status()
    {
        var topics = _config.GetSection("Kafka:Topics").Get<string[]>() ?? Array.Empty<string>();
        return Ok(new
        {
            enabled = !string.IsNullOrEmpty(_config["Kafka:BootstrapServers"]),
            bootstrapServers = _config["Kafka:BootstrapServers"],
            publisherStatus = _publisher.Status,
            consumerGroupId = _config["Kafka:ConsumerGroupId"],
            topics
        });
    }

    // Проверка достижимости брокера (addressability probe)
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = _publisher.Status });
    }
}