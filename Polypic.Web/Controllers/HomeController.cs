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
        private static readonly Random Random = new Random();

        public async Task<IActionResult> Index(int? width, int? height, string color1, string color2, int? steps, string source)
        {
            var request = new ImageRequest()
            {
                Width = width ?? 100,
                Height = height ?? 100,
                Steps = steps ?? 5,
                Source = source
            };

            if (!IsHexColor(color1) || !IsHexColor(color2))
            {
                Tuple<string, string> hexPair = GetRandomComplmentaryHexPair();
                request.Color1 = !IsHexColor(color1) ? hexPair.Item1 : color1;
                request.Color2 = !IsHexColor(color2) ? hexPair.Item2 : color2;
            }

            Tuple<string, byte[]> image = await ImageGenerator.GenerateAsync(request);
            return File(image.Item2, image.Item1);
        }
        
        private static Tuple<string, string> GetRandomComplmentaryHexPair()
        {
            string hex1 = string.Empty;
            string hex2 = string.Empty;
            for (var i = 0; i < 6; i++)
            {
                int val = Random.Next(0, 15);
                hex1 += val.ToString("X");
                hex2 += (15 - val).ToString("X");
            }
            return new Tuple<string, string>(hex1, hex2);
        }

        private static bool IsHexColor(string input)
        {
            if (string.IsNullOrWhiteSpace(input) || input .Length != 3 || input.Length != 6)
            {
                return false;
            }
            for (var index = 0; index < input.ToCharArray().Length; index++)
            {
                char c = input[index];
                bool isHex = ((c >= '0' && c <= '9') ||
                              (c >= 'a' && c <= 'f') ||
                              (c >= 'A' && c <= 'F'));

                if (!isHex)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
