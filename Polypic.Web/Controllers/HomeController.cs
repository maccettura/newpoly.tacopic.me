using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using Microsoft.AspNetCore.Mvc;
using Polypic.Generate;
using Polypic.Generate.Model;

namespace Polypic.Web.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(int? width, int? height, string color1, string color2, int? steps, string source)
        {
            var model = new ImageRequest()
            {
                Width = width ?? 100,
                Height = height ?? 100,
                Steps = steps ?? 5,
                Color1 = string.IsNullOrWhiteSpace(color1) ? "FF0022" : color1,
                Color2 = string.IsNullOrWhiteSpace(color2) ? "003377" : color2,
                Source = source
            };

            Tuple<string, byte[]> image = await ImageGenerator.GenerateAsync(model);
            return File(image.Item2, image.Item1);
        }
    }
}
