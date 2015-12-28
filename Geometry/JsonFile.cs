using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SketchPlatform
{
    public class JsonFile
    {
        public List<Box> boxes { get; set; }
    }

    public class Box
    {
        public string modelView { get; set; }
        public string box { get; set; }
        public string segment { get; set; }
    }
}
