using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using System.Globalization;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Currency;

public static class CurrencyProfileUtils
{
    public static List<CurrencyProfile> GetListOfAvailableCurrencyData()
    {
        List<CurrencyProfile> currencyProfiles = GetRegionInfos()
            .Select(ri => new CurrencyProfile
            {
                Code = ri!.ISOCurrencySymbol,
                Symbol = ri.CurrencySymbol,
                Name = ri.CurrencyEnglishName
            }).ToList();

        return currencyProfiles;
    }

    public static CurrencyProfile GetCurrencyData(string isoCode)
    {
        RegionInfo regionInfo = GetRegionInfos()
            .FirstOrDefault(ri => ri.ISOCurrencySymbol == isoCode);

        if (regionInfo == null)
        {
            throw new InvalidCurrencyIsoCodeException(isoCode);
        }

        return new CurrencyProfile
        {
            Code = regionInfo.ISOCurrencySymbol,
            Symbol = regionInfo.CurrencySymbol,
            Name = regionInfo.CurrencyEnglishName
        };
    }

    public static bool TryGetCurrencyData(string isoCode, out CurrencyProfile currencyProfile)
    {
        RegionInfo regionInfo = GetRegionInfos()
            .FirstOrDefault(ri => ri.ISOCurrencySymbol == isoCode);

        if (regionInfo == null)
        {
            currencyProfile = null;
            return false;
        }

        currencyProfile = new CurrencyProfile
        {
            Code = regionInfo.ISOCurrencySymbol,
            Symbol = regionInfo.CurrencySymbol,
            Name = regionInfo.CurrencyEnglishName
        };
        return true;
    }

    private static IEnumerable<RegionInfo> GetRegionInfos()
    {
        return CultureInfo.GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Select(c =>
            {
                try
                {
                    return new RegionInfo(c.Name);
                }
                catch
                {
                    return null;
                }
            })
            .Where(ri => ri != null);
    }
}