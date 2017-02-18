using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polypic.Generate.Model
{
    public class ImageRequest
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Steps { get; set; }
        public string Color1 { get; set; }
        public string Color2 { get; set; }
        public string Source { get; set; }
    }
}
