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

export interface BudgetCalculateRequest {
  homeCurrency: string;
  dailyBudget: number;
  tripDays: number;
  destinationCountries: string[];
}

export interface ExchangeRateDataPoint {
  date: string;
  rate: number;
}

export interface BudgetCalculationResult {
  countryCode: string;
  countryName: string;
  destinationCurrency: string;
  exchangeRate: number;
  dailyBudgetHome: number;
  tripDays: number;
  totalBudgetHome: number;
  totalBudgetLocal: number;
  dailyLocalAmount: number;
  dataFreshness: string;
  rateTimestamp: string;
}
