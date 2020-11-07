using ArduinoAPI.Data;
using ArduinoAPI.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace ArduinoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArduinoController : ControllerBase
    {
        private readonly IMemoryStorage memoryStorage;
        public ArduinoController(IMemoryStorage storage)
        {
            memoryStorage = storage;
        }

        [HttpPost("post")]
        public IActionResult PostInfo([FromForm] CustomerInfo customerInfo)
        {
            var userID = GenID(max: 8);

            memoryStorage.AddItem(userID, customerInfo);

            return RedirectToActionPermanent("Confirm", new { id = userID});
        }

        [HttpGet("status")]
        [Produces("text/html")]
        public ContentResult Confirm(string id)
        {
            if (string.IsNullOrEmpty(id))
                id = GenID(max: 8);

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = (int) HttpStatusCode.OK,
                Content = @$"<html lang='nl'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css' integrity='sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z' crossorigin='anonymous'>
                    <title>Lorum Ipsum</title>
                </head>
                <body>
                    <div class='ml-auto mr-auto w-50 pt-5'>
                        <div class='jumbotron'>
                            <h1 class='display-4'>Uw reis is geboekt!</h1>
                            <p class='lead'>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus malesuada ante non mauris volutpat dignissim id et est. Quisque vitae commodo erat, id mollis nisl.</p>
                            <hr class='my-4'>
                            <p>Uw kaartbewijs is {"#"+id}</p>
                            <a class='btn btn-success btn-lg' href='/' role='button'>Terug naar de home pagina</a>
                          </div>
                    </div>
                </body>
                </html>
                <script src='https://cdnjs.cloudflare.com/ajax/libs/jquery/3.5.0/jquery.js' integrity='sha512-cEgdeh0IWe1pUYypx4mYPjDxGB/tyIORwjxzKrnoxcif2ZxI7fw81pZWV0lGnPWLrfIHGA7qc964MnRjyCYmEQ==' crossorigin='anonymous'></script>
                <script src='https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js' integrity='sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV' crossorigin='anonymous'></script>
                <script src='https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js' integrity='sha384-LtrjvnR4Twt/qOuYxE721u19sVFLVSA4hf/rRt6PrZTmiPltdZcI7q7PXQBYTKyf' crossorigin='anonymous'></script>"
            };
        }

        private string GenID(int min = 0, int max = 16)
        {
            return Guid.NewGuid().ToString().Substring(min, max).Replace("-", "");
        }
    }
}
