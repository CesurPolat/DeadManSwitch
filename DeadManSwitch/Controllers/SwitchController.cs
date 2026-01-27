using Microsoft.AspNetCore.Mvc;
using DeadManSwitch.Services;

namespace DeadManSwitch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwitchController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SwitchController> _logger;

        public SwitchController(IMailService mailService, IConfiguration configuration, ILogger<SwitchController> logger)
        {
            _mailService = mailService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Ping received at {Time}", DateTime.UtcNow);
            
            return Ok(new { message = "Sinyal başarıyla alındı.", timestamp = DateTime.UtcNow });
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail()
        {
            try
            {
                var senderEmail = _configuration["SmtpSettings:SenderEmail"] ?? "";
                await _mailService.SendEmailAsync(senderEmail, "Dead Man Switch Test", "Bu bir test e-postasıdır. Mail servisi çalışıyor.");
                
                return Ok(new { message = "Test e-postası gönderildi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "E-posta gönderilemedi.", details = ex.Message });
            }
        }
    }
}
