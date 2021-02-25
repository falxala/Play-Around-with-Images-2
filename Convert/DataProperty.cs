using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlayAroundwithImages2
{
    public class DataProperty
    {
        public struct PasteData
        {
            public IDataObject data { get; set; }
            public string path { get; set; }
            public string url { get; set; }
        }
        public class Outputvalue
        {
            public string Path { get; set; }
            public int Number { get; set; }
            public string Name { get; set; }
        }
        public class ColorMode
        {
            public string Space { get; set; }
            public string[] ICC { get; set; }
            public string Name { get; set; }
        }
    }
}
