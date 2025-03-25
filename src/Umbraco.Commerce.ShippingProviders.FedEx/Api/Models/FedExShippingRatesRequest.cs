using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api.Models
{
    public class FedExShippingRatesRequest
    {
        [JsonPropertyName("accountNumber")]
        public AccountNumber AccountNumber { get; set; }

        [JsonPropertyName("requestedShipment")]
        public RequestedShipment RequestedShipment { get; set; }
    }

    public class AccountNumber
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class RequestedShipment
    {
        [JsonPropertyName("shipper")]
        public Shipper Shipper { get; set; }

        [JsonPropertyName("recipient")]
        public Recipient Recipient { get; set; }

        [JsonPropertyName("pickupType")]
        public string PickupType { get; set; }

        [JsonPropertyName("rateRequestType")]
        public List<string> RateRequestType { get; set; }

        [JsonPropertyName("requestedPackageLineItems")]
        public List<RequestedPackageLineItem> RequestedPackageLineItems { get; set; }

        [JsonPropertyName("preferredCurrency")]
        public string PreferredCurrency { get; set; }

    }

    public class Shipper
    {
        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }

    public class Recipient
    {
        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }

    public class Address
    {
        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; }

        [JsonPropertyName("countryCode")]
        public string CountryCode { get; set; }
    }

    public class RequestedPackageLineItem
    {
        [JsonPropertyName("weight")]
        public Weight Weight { get; set; }
    }

    public class Weight
    {
        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
