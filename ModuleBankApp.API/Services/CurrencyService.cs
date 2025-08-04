namespace ModuleBankApp.API.Services;

using System.Globalization;

public interface ICurrencyService
{
    /// <summary>
    /// Проверяет, является ли код валюты валидным по стандарту ISO 4217
    /// </summary>
    /// <param name="currencyCode">Код валюты (например, USD, EUR)</param>
    /// <returns>True если код валиден, иначе False</returns>
    bool IsValidCurrencyCode(string currencyCode);

    /// <summary>
    /// Возвращает все известные коды валют по ISO 4217
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    IReadOnlyCollection<string> GetAllCurrencyCodes();
}

public class CurrencyService : ICurrencyService
{
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<HashSet<string>> _validCurrencyCodes = new(() =>
    {
        var currencyCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            try
            {
                var region = new RegionInfo(culture.Name);
                currencyCodes.Add(region.ISOCurrencySymbol);
            }
            catch
            {
                // Пропускаем культуры без региональных данных
            }
        }

        return currencyCodes;
    });

    /// <summary>
    /// Проверяет, является ли код валюты валидным по стандарту ISO 4217
    /// </summary>
    /// <param name="currencyCode">Код валюты (например, USD, EUR)</param>
    /// <returns>True если код валиден, иначе False</returns>
    public bool IsValidCurrencyCode(string currencyCode)
    {
        return !string.IsNullOrWhiteSpace(currencyCode) && _validCurrencyCodes.Value.Contains(currencyCode);
    }

    /// <summary>
    /// Возвращает все известные коды валют по ISO 4217
    /// </summary>
    public IReadOnlyCollection<string> GetAllCurrencyCodes()
    {
        return _validCurrencyCodes.Value.ToList().AsReadOnly();
    }
}