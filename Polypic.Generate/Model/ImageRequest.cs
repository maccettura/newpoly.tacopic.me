using Polypic.Generate.Utilities;

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


        public bool IsEmptyColors()
        {
            return string.IsNullOrWhiteSpace(Color1) && string.IsNullOrWhiteSpace(Color2);
        }

        public bool IsValidColors()
        {
            return Color1.IsHexColor() && Color2.IsHexColor();
        }
    }
}
