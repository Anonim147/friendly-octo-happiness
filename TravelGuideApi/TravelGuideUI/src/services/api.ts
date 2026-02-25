import { BudgetCalculateRequest, BudgetCalculationResult, Country, CurrencyComparisonRequest, CurrencyComparisonResult, ExchangeRateDataPoint } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || '/api/v1';

class ApiService {
  private async handleResponse<T>(response: Response): Promise<T> {
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || `HTTP error! status: ${response.status}`);
    }
    return response.json();
  }

  async getCountries(): Promise<Country[]> {
    const response = await fetch(`${API_BASE_URL}/comparison/countries`);
    return this.handleResponse<Country[]>(response);
  }

  async compareCurrencies(request: CurrencyComparisonRequest): Promise<CurrencyComparisonResult> {
    const response = await fetch(`${API_BASE_URL}/comparison/currency`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    return this.handleResponse<CurrencyComparisonResult>(response);
  }

  async calculateBudget(request: BudgetCalculateRequest): Promise<BudgetCalculationResult[]> {
    const response = await fetch(`${API_BASE_URL}/budget/calculate`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    });
    return this.handleResponse<BudgetCalculationResult[]>(response);
  }

  async getHistoricalRates(from: string, to: string, days: number): Promise<ExchangeRateDataPoint[]> {
    const response = await fetch(`${API_BASE_URL}/history/rates?from=${from}&to=${to}&days=${days}`);
    return this.handleResponse<ExchangeRateDataPoint[]>(response);
  }
}

export const apiService = new ApiService();
