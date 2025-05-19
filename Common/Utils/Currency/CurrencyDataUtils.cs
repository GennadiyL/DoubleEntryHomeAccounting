using GLSoft.DoubleEntryHomeAccounting.Common.Exceptions;
using System.Globalization;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Utils.Currency;

public static class CurrencyDataUtils
{
    public static List<CurrencyData> GetListOfAvailableCurrencyData()
    {
        List<CurrencyData> currencyData = GetRegionInfos()
            .Select(ri => new CurrencyData
            {
                Code = ri!.ISOCurrencySymbol,
                Symbol = ri.CurrencySymbol,
                Name = ri.CurrencyEnglishName
            }).ToList();
        
        return currencyData;
    }

    public static CurrencyData GetCurrencyData(string isoCode)
    {
        RegionInfo regionInfo = GetRegionInfos()
            .FirstOrDefault(ri => ri.ISOCurrencySymbol == isoCode);
        
        if (regionInfo == null)
        {
            throw new InvalidCurrencyIsoCodeException(isoCode);
        }

        return new CurrencyData
        {
            Code = regionInfo.ISOCurrencySymbol,
            Symbol = regionInfo.CurrencySymbol,
            Name = regionInfo.CurrencyEnglishName
        };
    }

    public static bool TryGetCurrencyData(string isoCode, out CurrencyData currencyData)
    {
        RegionInfo regionInfo = GetRegionInfos()
            .FirstOrDefault(ri => ri.ISOCurrencySymbol == isoCode);
        
        if (regionInfo == null)
        {
            currencyData = null;
            return false;
        }

        currencyData = new CurrencyData
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