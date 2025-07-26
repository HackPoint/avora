using Avora.Agent.Core.Agent;
using Avora.Agent.Core.Query;
using Microsoft.AspNetCore.Mvc;

namespace Avora.Agent.Api;

[ApiController]
[Route("api/[controller]")]
public class AgentController(IAgent agent) : ControllerBase {
    
    [HttpPost("ask")]
    public IActionResult Ask([FromBody] Query query) {
        var response = agent.Execute(query);
        return Ok(response);
    }
}