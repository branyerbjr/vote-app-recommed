using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using VoteAppBackend.Models;
using Newtonsoft.Json;

namespace VoteAppBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VoteController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;

        public VoteController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpPost]
        public IActionResult SubmitVote([FromBody] Vote vote)
        {
            if (vote == null || string.IsNullOrEmpty(vote.Choice))
            {
                return BadRequest("Invalid vote.");
            }

            // Enviar el voto a Redis
            var db = _redis.GetDatabase();
            var data = JsonConvert.SerializeObject(vote);
            db.ListRightPush("votes", data);

            return Ok("Vote submitted.");
        }
    }
}
