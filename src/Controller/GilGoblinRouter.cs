using System.Web.Http;

namespace GilGoblin.Controller;

public class GilGoblinController : ApiController
{
    [Route("canary")]
    [HttpGet]
    public static HttpResponseMessage IsAlive() => new(System.Net.HttpStatusCode.OK);

    [Route("price/{id}")]
    [HttpGet]
    public static int GetPrice(int id) => id + 1;
}