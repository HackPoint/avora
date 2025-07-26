using Avora.Agent.Api.Models;
using Avora.Agent.Core.Agent;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Avora.Agent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentController(IAgent agent, IQueryFactory queryFactory) : ControllerBase {
    [HttpPost("ask")]
    public IActionResult Ask([FromBody] QueryDto dto) {
        if (string.IsNullOrWhiteSpace(dto.OriginalText)) 
            return BadRequest("OriginalText is required.");

        var query = queryFactory.Create(dto.OriginalText);
        
        return Ok(new {
            query.Id,
            query.NormalizedText,
            Tokens = query.Tokens.Select(t => t.Value).ToArray(),
            query.Embedding
        });
    }
    
    
}