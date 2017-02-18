using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Drawing;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Polypic.Generate.Model;


namespace Polypic.Generate
{
    public class ImageGenerator
    {
        private static readonly Random Random = new Random();
        private static readonly AssemblyName AssemblyName = new AssemblyName("Polypic.Generate");

        public static async Task<Tuple<string, byte[]>> GenerateAsync(ImageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Color1))
            {
                //Generate random hex
                request.Color1 = "FF0011";                
            }
            if (string.IsNullOrWhiteSpace(request.Color2))
            {
                //Generate random hex
                request.Color2 = "00DD13";
            }

            Tuple<int, int>[,] verticies = await GenerateVerticiesAsync(request.Width, request.Height, request.Steps);

            using (Image image = await GenerateImageAsync(request.Width, request.Height, Color.FromHex(request.Color1), Color.FromHex(request.Color2), verticies))
            using (Image newImage = await BlendGrainAsync(image))
            using (var ms = new MemoryStream())
            {
                IImageFormat format;
                switch (request.Source)
                {
                    case "gif":
                        format = new GifFormat();
                        break;
                    case "png":
                        format = new PngFormat();
                        break;
                    case "jpg":
                    case "jpeg":
                    default:
                        format = new JpegFormat();
                        break;
                }
                newImage.Save(ms, format);
                return new Tuple<string, byte[]>("image/jpeg", ms.ToArray());
            }                        
        }

        #region Image Draw

        private static async Task<Image> GenerateImageAsync(int width, int height, Color color1, Color color2, Tuple<int, int>[,] points)
        {
            return await Task.FromResult(GenerateImage(width, height, color1, color2, points));
        }

        private static Image GenerateImage(int width, int height, Color color1, Color color2, Tuple<int, int>[,] points)
        {
            var image = new Image(width, height);

            for (var x = 0; x < points.GetLength(0); x++)
            {
                for (var y = 0; y < points.GetLength(1); y++)
                {
                    if (x + 1 < points.GetLength(0) && y + 1 < points.GetLength(1))
                    {
                        var t1 = new[]
                        {
                            new Vector2(points[x, y].Item1, points[x, y].Item2),
                            new Vector2(points[x + 1, y].Item1, points[x + 1, y].Item2),
                            new Vector2(points[x, y + 1].Item1, points[x, y + 1].Item2)
                        };
                        var t2 = new[]
                        {
                            new Vector2(points[x + 1, y].Item1, points[x + 1, y].Item2),
                            new Vector2(points[x + 1, y + 1].Item1, points[x + 1, y + 1].Item2),
                            new Vector2(points[x, y + 1].Item1, points[x, y + 1].Item2)
                        };
                        image.FillPolygon(color1, t1);
                        image.FillPolygon(color2, t2);
                    }
                }
            }

            return image;
        }

        #endregion

        #region Grain Blend

        public static async Task<Image> BlendGrainAsync(Image image)
        {
            return await Task.FromResult(BlendGrain(image));
        }

        public static Image BlendGrain(Image image)
        {
            using (Stream resourceStream = Assembly.Load(AssemblyName).GetManifestResourceStream("Polypic.Generate.grain.jpg"))
            {
                var size = new Size(image.Width, image.Height);

                return (Image)image.DrawImage(new Image(resourceStream), 5, size, default(Point));
            }
        }

        #endregion

        #region Vertex Generation

        private static async Task<Tuple<int, int>[,]> GenerateVerticiesAsync(int width, int height, int steps)
        {
            return await Task.FromResult(GenerateVerticies(width, height, steps));
        }

        private static Tuple<int, int>[,] GenerateVerticies(int width, int height, int steps)
        {
            var map = new Tuple<int, int>[steps + 1, steps + 1];

            int sectionWidth = width / steps;
            int sectionHeight = height / steps;

            for (var x = 0; x < steps + 1; x++)
            {
                for (var y = 0; y < steps + 1; y++)
                {
                    map[x, y] = new Tuple<int, int>(CalculatePoint(x, sectionWidth, steps), CalculatePoint(y, sectionHeight, steps));
                }
            }
            return map;
        }

        private static int CalculatePoint(int index, int dimension, int steps)
        {
            if (index == 0)
            {
                return (index * dimension) + (dimension / -2);
            }
            if (index == steps)
            {
                return (int)Math.Ceiling((double)(index * dimension + dimension / 2));
            }
            return (index * dimension) + (dimension / -2) + Random.Next(0, dimension);
        }

        #endregion

        #region Color Generation

        private static Color GetColor(byte r, byte g, byte b)
        {
            return new Color(r, g, b);
        }

        #endregion
    }
}
