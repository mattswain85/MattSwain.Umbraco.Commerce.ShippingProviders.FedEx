using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Commerce.Common.Logging;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.ShippingProviders;
using MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api;
using MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api.Models;
using Address = MattSwain.Umbraco.Commerce.ShippingProviders.FedEx.Api.Models.Address;

namespace MattSwain.Umbraco.Commerce.ShippingProviders.FedEx
{
    [ShippingProvider("fedex")]
    public class FedExShippingProvider(
        UmbracoCommerceContext ctx,
        IHttpClientFactory httpClientFactory,
        ILogger<FedExShippingProvider> logger, IMemoryCache cache)
        : ShippingProviderBase<FedExSettings>(ctx)
    {
        public override bool SupportsRealtimeRates => true;
        public override async Task<ShippingRatesResult> GetShippingRatesAsync(ShippingProviderContext<FedExSettings> context, CancellationToken cancellationToken = default)
        {
            var package = context.Packages.FirstOrDefault();

            if (package == null || !package.HasWeight)
            {
                logger.Debug("Unable to calculate realtime Fedex rates as the package provided is invalid");
                return ShippingRatesResult.Empty;
            }

            var orderCurrency = await Context.Services.CurrencyService.GetCurrencyAsync(context.Order.CurrencyId);

            var client = FedExExpressClient.Create(httpClientFactory, context.Settings, cache);

            var request = new FedExShippingRatesRequest()
            {
                AccountNumber = new AccountNumber() { Value = context.Settings.AccountNumber },
                RequestedShipment = new RequestedShipment()
                {
                    PickupType = context.Settings.PickupType,
                    RateRequestType = new List<string>() { "PREFERRED" },
                    PreferredCurrency = orderCurrency.Code
                }
            };

            request.RequestedShipment.Recipient = new Recipient()
            {
                Address = new Address()
                {
                    PostalCode = package.ReceiverAddress.ZipCode ?? "",
                    CountryCode = package.ReceiverAddress.CountryIsoCode ?? ""
                }
            };

            request.RequestedShipment.Shipper = new Shipper()
            {
                Address = new Address()
                {
                    PostalCode = package.SenderAddress.ZipCode ?? "",
                    CountryCode = package.SenderAddress.CountryIsoCode ?? ""
                }
            };

            request.RequestedShipment.RequestedPackageLineItems =
            [
                new RequestedPackageLineItem()
                {
                    Weight = new Weight()
                    {
                        Units = "KG",
                        Value = package.Weight.ToString("0")
                    },

                },
            ];

            var token = await client.GetToken(context.Settings.ClientId, context.Settings.ClientSecret);
            if (token == null)
            {
                logger.Debug("Unable to get token");
                return ShippingRatesResult.Empty;
            }

            var resp = await client.GetRatesAsync(request, token.AccessToken);
            if (resp == null)
            {
                logger.Debug("Unable to get rates");
                return ShippingRatesResult.Empty;
            }

            var rates = resp.output.rateReplyDetails;

            // Only return rates where they match the current currency.
            rates = rates.Where(a => a.ratedShipmentDetails.Any(a => a.currency == orderCurrency.Code)).ToList();

            // If product codes set, then only return rates that match the product codes.
            if (!string.IsNullOrEmpty(context.Settings.ProductCodes))
            {
                rates = rates.Where(a => context.Settings.ProductCodes.ToLower().Contains(a.serviceType.ToLower())).ToList();
            }

            var result = new ShippingRatesResult
            {
                Rates = rates.Select(p =>
                {
                    var packageRateDetail = p.ratedShipmentDetails.FirstOrDefault(a => a.currency == orderCurrency.Code);
                    var productPriceTax = packageRateDetail.totalNetCharge - packageRateDetail.totalNetFedExCharge;
                    var productPriceNet = packageRateDetail.totalNetFedExCharge;
                    var productPrice = new Price(productPriceNet, productPriceTax, context.Order.CurrencyId);
                    var productName = p.serviceName;

                    var option = new ShippingOption(p.serviceType, productName);

                    return new ShippingRate(productPrice, option, package.Id);
                }).ToList()
            };

            return result;
        }
    }
}
