using Umbraco.Commerce.Core.ShippingProviders;

namespace MattSwain.Umbraco.Commerce.ShippingProviders.FedEx
{
    public class FedExSettings
    {
        [ShippingProviderSetting(SortOrder = 200)]
        public string AccountNumber { get; set; }

        [ShippingProviderSetting(SortOrder = 210)]
        public string ClientId { get; set; }

        [ShippingProviderSetting(SortOrder = 220)]
        public string ClientSecret { get; set; }

        [ShippingProviderSetting(SortOrder = 230)]
        public string PickupType { get; set; }

        [ShippingProviderSetting(SortOrder = 240)]
        public string ProductCodes { get; set; }

        [ShippingProviderSetting(SortOrder = 10000)]
        public bool TestMode { get; set; }
    }
}
