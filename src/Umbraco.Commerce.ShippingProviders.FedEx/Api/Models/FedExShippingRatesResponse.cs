using System.Collections.Generic;

namespace MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api.Models
{
    public class FedExExpressShippingRatesResponse
    {
        public string transactionId { get; set; }
        public Output output { get; set; }
    }


    public class Alert
    {
        public string code { get; set; }
        public string message { get; set; }
        public string alertType { get; set; }
    }

    public class BillingWeight
    {
        public string units { get; set; }
        public decimal value { get; set; }
    }

    public class Name
    {
        public string type { get; set; }
        public string encoding { get; set; }
        public string value { get; set; }
    }

    public class OperationalDetail
    {
        public bool ineligibleForMoneyBackGuarantee { get; set; }
        public string astraDescription { get; set; }
        public string airportId { get; set; }
        public string serviceCode { get; set; }
    }

    public class Output
    {
        public List<Alert> alerts { get; set; }
        public List<RateReplyDetail> rateReplyDetails { get; set; }
        public string quoteDate { get; set; }
        public bool encoded { get; set; }
    }

    public class PackageRateDetail
    {
        public string rateType { get; set; }
        public string ratedWeightMethod { get; set; }
        public decimal baseCharge { get; set; }
        public decimal netFreight { get; set; }
        public decimal totalSurcharges { get; set; }
        public decimal netFedExCharge { get; set; }
        public decimal totalTaxes { get; set; }
        public decimal netCharge { get; set; }
        public decimal totalRebates { get; set; }
        public BillingWeight billingWeight { get; set; }
        public decimal totalFreightDiscounts { get; set; }
        public List<Surcharge> surcharges { get; set; }
        public string currency { get; set; }
    }

    public class RatedPackage
    {
        public int groupNumber { get; set; }
        public decimal effectiveNetDiscount { get; set; }
        public PackageRateDetail packageRateDetail { get; set; }
    }

    public class RatedShipmentDetail
    {
        public string rateType { get; set; }
        public string ratedWeightMethod { get; set; }
        public decimal totalDiscounts { get; set; }
        public decimal totalBaseCharge { get; set; }
        public decimal totalNetCharge { get; set; }
        public decimal totalNetFedExCharge { get; set; }
        public ShipmentRateDetail shipmentRateDetail { get; set; }
        public List<RatedPackage> ratedPackages { get; set; }
        public string currency { get; set; }
    }

    public class RateReplyDetail
    {
        public string serviceType { get; set; }
        public string serviceName { get; set; }
        public string packagingType { get; set; }
        public List<RatedShipmentDetail> ratedShipmentDetails { get; set; }
        public OperationalDetail operationalDetail { get; set; }
        public string signatureOptionType { get; set; }
        public ServiceDescription serviceDescription { get; set; }
    }
    public class ServiceDescription
    {
        public string serviceId { get; set; }
        public string serviceType { get; set; }
        public string code { get; set; }
        public List<Name> names { get; set; }
        public string serviceCategory { get; set; }
        public string description { get; set; }
        public string astraDescription { get; set; }
    }

    public class ShipmentRateDetail
    {
        public string rateZone { get; set; }
        public int dimDivisor { get; set; }
        public decimal fuelSurchargePercent { get; set; }
        public decimal totalSurcharges { get; set; }
        public decimal totalFreightDiscount { get; set; }
        public List<Surcharge> surCharges { get; set; }
        public string pricingCode { get; set; }
        public TotalBillingWeight totalBillingWeight { get; set; }
        public string currency { get; set; }
        public string rateScale { get; set; }
    }

    public class Surcharge
    {
        public string type { get; set; }
        public string description { get; set; }
        public decimal amount { get; set; }
        public string level { get; set; }
    }

    public class TotalBillingWeight
    {
        public string units { get; set; }
        public decimal value { get; set; }
    }


}
