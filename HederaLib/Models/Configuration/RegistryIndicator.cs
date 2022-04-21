
namespace ncl.hedera.HederaLib.Models.Configuration
{
    public sealed class RegistryIndicator : CommonIndicatorBase
    {
        public string BaseKey { get; init; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string ValueName { get; init; }
        public string ValueData { get; init; }
        public bool IsRecursive { get; init; }


    }
}
