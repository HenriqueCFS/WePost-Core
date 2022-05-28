using core.Data.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace core.Controllers
{
    public class GetValuesPayload
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "IsAdmin")]
        [Route("getvalue")] 
        public IActionResult GetValue([FromBody] GetValuesPayload model)
        {
            return Ok(model.Name);
        }
    }
}
