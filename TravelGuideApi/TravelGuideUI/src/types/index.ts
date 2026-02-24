export interface Country {
  code: string;
  name: string;
  currencyCode: string;
  currencyName: string;
  flagUrl: string;
}

export interface CurrencyComparisonRequest {
  homeCountryCode: string;
  destinationCountryCode: string;
}

export interface CurrencyComparisonResult {
  homeCountry: string;
  homeCurrency: string;
  homeCurrencyName: string;
  destinationCountry: string;
  destinationCurrency: string;
  destinationCurrencyName: string;
  exchangeRate: number;
  strengthHint: string;
}

