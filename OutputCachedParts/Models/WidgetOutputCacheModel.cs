using System;
using System.Collections.Generic;

namespace CJP.OutputCachedParts.OutputCachedParts.Models {
    public class OutputCachedPartsModel
    {
        public OutputCachedPartsModel() {
            GenerationDateTime = DateTime.UtcNow;
        }
        public string Html { get; set; }
        public DateTime GenerationDateTime { get; set; }
        public IEnumerable<ResourceRequiredModel> RequiredResources { get; set; }
        public IEnumerable<ResourceIncludedModel> IncludedResources { get; set; }
        public IList<string> FootScripts { get; set; }
        public IList<string> HeadScripts { get; set; }
    }
}