using System.ComponentModel;
using JsonLibraryGenerator.Attributes;
using Newtonsoft.Json;

namespace JsonGeneratorSample
{
    [JsonSwitch]
    public partial class TestModelBase
    {
#if (NEWTONSOFT)
        [JProperty("test")] public virtual string Test { get; set; }
#endif
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class TestModelBase
    {

    }

}
