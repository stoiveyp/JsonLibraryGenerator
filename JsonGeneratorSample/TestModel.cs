using System.ComponentModel;
using JsonLibraryGenerator.Attributes;
using Newtonsoft.Json;

namespace JsonGeneratorSample
{
    [JsonSwitch]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class TestModelBase
    {
        [JProperty("test")] public virtual string Test { get; set; }

    }
}
