using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polypic.Generate;
using Polypic.Generate.Model;

namespace Polypic.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(int? width, int? height, string color1, string color2, int? steps, string source)
        {
            //Create request object
            var request = new ImageRequest()
            {
                Width = width ?? 100,
                Height = height ?? 100,
                Steps = steps ?? 5,
                Source = source,
                Color1 = color1,
                Color2 = color2
            };

            if (!request.IsEmptyColors() && !request.IsValidColors())
            {
                return RedirectToAction("Error");
            }

            Tuple<string, byte[]> image = await ImageGenerator.GenerateAsync(request);
            return File(image.Item2, image.Item1);
        }

        public IActionResult Error()
        {
            return View();
        }  
    }
}
