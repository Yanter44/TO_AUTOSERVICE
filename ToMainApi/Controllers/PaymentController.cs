using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToMainApi.Interfaces;
using ToMainApi.Models.Dtos.Payments;

namespace ToMainApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("CreditAgentBalance")]
        public async Task<IActionResult> CreditAgentBalance([FromBody] CreditRequest model)
        {
            var result = await _paymentService.Credit(model);
            if (result.Success)
                return Ok();
            return BadRequest();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("DebitAgentBalance")]
        public async Task<IActionResult> DebitAgentBalance([FromBody] DebitRequest model)
        {
            var result = await _paymentService.Debit(model);
            if (result.Success)
                return Ok();
            return BadRequest();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("GetAllTransaction")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var result = await _paymentService.GetAllTransactions();
            if (result.Success)
                return Ok(result.Data);
            return BadRequest();
        }
    }
}
