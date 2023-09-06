using System;

namespace Library.Engine
{
    public class ScenarioMetaData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string FileName { get; set; }
    }
}
