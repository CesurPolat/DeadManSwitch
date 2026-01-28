using Microsoft.AspNetCore.Mvc;
using DeadManSwitch.Services;
using DeadManSwitch.Models;
using Microsoft.AspNetCore.Authorization;

namespace DeadManSwitch.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SwitchController : ControllerBase
    {
        private readonly IMailService _mailService;
        private readonly ISwitchStore _switchStore;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SwitchController> _logger;

        public SwitchController(IMailService mailService, ISwitchStore switchStore, IConfiguration configuration, ILogger<SwitchController> logger)
        {
            _mailService = mailService;
            _switchStore = switchStore;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("ping")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Ping received at {Time}", DateTime.UtcNow);
            _switchStore.UpdatePing();
            
            return Ok(new { message = "Sinyal başarıyla alındı.", timestamp = DateTime.UtcNow });
        }

        [Authorize]
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var state = _switchStore.GetState();
            return Ok(new
            {
                lastPing = state.LastPing == DateTime.MinValue ? "Hiç sinyal alınmadı" : state.LastPing.ToString("yyyy-MM-dd HH:mm:ss"),
                status = state.Status,
                nextCheck = state.NextCheckExpected.ToString("yyyy-MM-dd HH:mm:ss"),
                isAlarm = state.Status == "ALARM"
            });
        }

        [Authorize]
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
