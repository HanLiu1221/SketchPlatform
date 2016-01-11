using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Component
{
//    public class JsonFile
//    {
//        public List<BoxJson> boxes { get; set; }
//    }

//    public class BoxJson
//    {
//        public string modelView { get; set; }
//        public string box { get; set; }
//        public string segment { get; set; }
//        public List<GuideJson> guides;
//    }

//    public class GuideJson
//    {
//        public PointJson from;
//        public PointJson to;
//    }

//    public class PointJson
//    {
//        public string x { get; set; }
//        public string y { get; set; }
//        public string z { get; set; }
//    }

    public class JsonFile
    {
        public string modelView { get; set; }
        public List<SequenceJson> sequence;
    }

    public class SequenceJson
    {
        public string box { get; set; }
        public string segment { get; set; }
        public List<string> hasGuides { get; set; }
        public List<GuideSequenceJson> guide_sequence;
        public List<GuideJson> guides;
        public List<PointJson> face_to_draw;
        public List<PointJson> face_to_highlight;
        public List<GuideJson> arrows { get; set; }
        public List<string> previous_guides { get; set; }
    }

    public class GuideSequenceJson
    {
        public string type { get; set; }
        public List<string> guide_indexes { get; set; }
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
