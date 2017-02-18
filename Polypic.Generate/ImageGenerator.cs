﻿using System;
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
        private static readonly Configuration Configuration = new Configuration();

        public static async Task<Tuple<string, byte[]>> GenerateAsync(ImageRequest request)
        {
            Configuration.AddImageFormat(new JpegFormat());

            Tuple<int, int>[,] verticies = await GenerateVerticiesAsync(request.Width, request.Height, request.Steps);

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
            using (Image image = await GenerateImageAsync(request, verticies))
            using (Image<Color> newImage = await BlendGrainAsync(image))
            using (var ms = new MemoryStream())
            {
                newImage.SaveAsJpeg(ms, 90);
                return new Tuple<string, byte[]>("image/jpeg", ms.ToArray());
            }            
        }

        
        #region Image Draw

        private static async Task<Image> GenerateImageAsync(ImageRequest request, Tuple<int, int>[,] points)
        {
            return await Task.FromResult(GenerateImage(request, points));
        }

        private static Image GenerateImage(ImageRequest request, Tuple<int, int>[,] points)
        {
            Color color1 = Color.FromHex(request.Color1);
            Color color2 = Color.FromHex(request.Color2);

            var image = new Image(request.Width, request.Height, Configuration);

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

        public static async Task<Image<Color>> BlendGrainAsync(Image image)
        {
            return await Task.FromResult(BlendGrain(image));
        }

        public static Image<Color> BlendGrain(Image image)
        {
            using (Stream resourceStream = Assembly.Load(AssemblyName).GetManifestResourceStream("Polypic.Generate.grain.jpg"))
            using(var grainImage = new Image(resourceStream))
            {
                return image.DrawImage(grainImage, 75, new Size(image.Width, image.Height), default(Point));
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
