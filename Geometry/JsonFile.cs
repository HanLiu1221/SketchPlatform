using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Component
{
    public class JsonFile
    {
        public List<BoxJson> boxes { get; set; }
    }

    public class BoxJson
    {
        public string modelView { get; set; }
        public string box { get; set; }
        public string segment { get; set; }
        public List<GuideJson> guides;
    }

    public class GuideJson
    {
        public PointJson from;
        public PointJson to;
    }

    public class PointJson
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }
}
