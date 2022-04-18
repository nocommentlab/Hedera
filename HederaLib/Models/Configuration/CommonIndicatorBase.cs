using ncl.hedera.HederaLib.Models.TheHive;


namespace ncl.hedera.HederaLib.Models.Configuration
{
    public class CommonIndicatorBase
    {
        public string Type { get; init; }
        public string Guid { get; init; }
        public Observable Observable { get; set; }
    }
}
