import React, { useState, useCallback } from 'react';
import { BudgetCalculationResult } from '../../types';
import { apiService } from '../../services/api';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './BudgetCalculator.css';

const HOME_CURRENCIES = ['USD', 'EUR', 'GBP', 'JPY', 'PLN', 'AUD', 'CAD'];

const DESTINATION_COUNTRIES = [
  { code: 'TH', name: 'Thailand' },
  { code: 'JP', name: 'Japan' },
  { code: 'VN', name: 'Vietnam' },
  { code: 'ID', name: 'Indonesia' },
  { code: 'PL', name: 'Poland' },
  { code: 'DE', name: 'Germany' },
  { code: 'FR', name: 'France' },
  { code: 'US', name: 'USA' },
  { code: 'GB', name: 'United Kingdom' },
];

const COUNTRY_FLAGS: Record<string, string> = {
  TH: '🇹🇭',
  JP: '🇯🇵',
  VN: '🇻🇳',
  ID: '🇮🇩',
  PL: '🇵🇱',
  DE: '🇩🇪',
  FR: '🇫🇷',
  US: '🇺🇸',
  GB: '🇬🇧',
};

const BudgetCalculator: React.FC = () => {
  const [homeCurrency, setHomeCurrency] = useState<string>('USD');
  const [dailyBudget, setDailyBudget] = useState<string>('');
  const [tripDays, setTripDays] = useState<string>('');
  const [selectedCountries, setSelectedCountries] = useState<string[]>([]);
  const [results, setResults] = useState<BudgetCalculationResult[] | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const toggleCountry = useCallback((code: string) => {
    setSelectedCountries(prev =>
      prev.includes(code) ? prev.filter(c => c !== code) : [...prev, code]
    );
    setResults(null);
  }, []);

  const canCalculate =
    homeCurrency &&
    dailyBudget !== '' &&
    Number(dailyBudget) > 0 &&
    tripDays !== '' &&
    Number(tripDays) > 0 &&
    selectedCountries.length > 0;

  const handleCalculate = useCallback(async () => {
    if (!canCalculate) return;

    try {
      setLoading(true);
      setError(null);
      const data = await apiService.calculateBudget({
        homeCurrency,
        dailyBudget: Number(dailyBudget),
        tripDays: Number(tripDays),
        destinationCountries: selectedCountries,
      });
      setResults(data);
    } catch (err) {
      setError('Failed to calculate budget. Please try again.');
      console.error('Error calculating budget:', err);
    } finally {
      setLoading(false);
    }
  }, [canCalculate, homeCurrency, dailyBudget, tripDays, selectedCountries]);

  return (
    <div className="budget-calculator-card">
      <div className="card-header">
        <div className="card-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <rect x="2" y="7" width="20" height="14" rx="2" ry="2" />
            <path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16" />
          </svg>
        </div>
        <div className="card-title-group">
          <h2 className="card-title">Trip Budget Calculator</h2>
          <p className="card-subtitle">Calculate your travel budget across multiple destinations</p>
        </div>
      </div>

      <div className="form-section">
        <div className="form-row">
          <div className="form-group">
            <label className="form-label" htmlFor="homeCurrency">Home Currency</label>
            <select
              id="homeCurrency"
              className="form-select"
              value={homeCurrency}
              onChange={e => {
                setHomeCurrency(e.target.value);
                setResults(null);
              }}
            >
              {HOME_CURRENCIES.map(c => (
                <option key={c} value={c}>{c}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="dailyBudget">Daily Budget</label>
            <input
              id="dailyBudget"
              className="form-input"
              type="number"
              min="1"
              step="any"
              placeholder="e.g. 100"
              value={dailyBudget}
              onChange={e => {
                setDailyBudget(e.target.value);
                setResults(null);
              }}
            />
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="tripDays">Trip Days</label>
            <input
              id="tripDays"
              className="form-input"
              type="number"
              min="1"
              step="1"
              placeholder="e.g. 14"
              value={tripDays}
              onChange={e => {
                setTripDays(e.target.value);
                setResults(null);
              }}
            />
          </div>
        </div>

        <div className="form-group destinations-group">
          <label className="form-label">Destinations</label>
          <div className="country-checkboxes">
            {DESTINATION_COUNTRIES.map(({ code, name }) => (
              <label
                key={code}
                className={`country-checkbox-label ${selectedCountries.includes(code) ? 'selected' : ''}`}
              >
                <input
                  type="checkbox"
                  className="country-checkbox-input"
                  checked={selectedCountries.includes(code)}
                  onChange={() => toggleCountry(code)}
                />
                <span className="country-flag">{COUNTRY_FLAGS[code]}</span>
                <span className="country-name">{name}</span>
              </label>
            ))}
          </div>
        </div>

        <button
          className={`calculate-button ${canCalculate ? 'active' : ''}`}
          onClick={handleCalculate}
          disabled={!canCalculate || loading}
        >
          {loading ? (
            <>
              <span className="button-spinner"></span>
              Calculating...
            </>
          ) : (
            <>
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <rect x="4" y="2" width="16" height="20" rx="2" />
                <line x1="8" y1="6" x2="16" y2="6" />
                <line x1="8" y1="10" x2="16" y2="10" />
                <line x1="8" y1="14" x2="12" y2="14" />
              </svg>
              Calculate Budget
            </>
          )}
        </button>

        {error && <ErrorMessage message={error} />}
      </div>

      {results && results.length > 0 && (
        <div className="results-section">
          <div className="results-header">
            <h3>Budget Breakdown</h3>
            <span className="results-meta">{results.length} destination{results.length > 1 ? 's' : ''}</span>
          </div>
          <div className="result-cards">
            {results.map(result => (
              <div key={result.countryCode} className="result-card">
                <div className="result-card-header">
                  <span className="result-flag">{COUNTRY_FLAGS[result.countryCode] ?? '🌍'}</span>
                  <div className="result-country-info">
                    <span className="result-country-name">{result.countryName}</span>
                    <span className="result-currency-code">{result.destinationCurrency}</span>
                  </div>
                  <span className={`freshness-badge ${result.dataFreshness.toLowerCase() === 'live' ? 'badge-live' : 'badge-stale'}`}>
                    {result.dataFreshness}
                  </span>
                </div>

                <div className="result-amounts">
                  <div className="result-total">
                    <span className="total-label">Total Budget</span>
                    <span className="total-local">
                      {result.totalBudgetLocal.toLocaleString(undefined, { maximumFractionDigits: 2 })} {result.destinationCurrency}
                    </span>
                    <span className="total-home">
                      {result.totalBudgetHome.toLocaleString(undefined, { maximumFractionDigits: 2 })} {homeCurrency}
                    </span>
                  </div>

                  <div className="result-detail-grid">
                    <div className="result-detail">
                      <span className="detail-label">Daily</span>
                      <span className="detail-value">
                        {result.dailyLocalAmount.toLocaleString(undefined, { maximumFractionDigits: 2 })} {result.destinationCurrency}
                      </span>
                    </div>
                    <div className="result-detail">
                      <span className="detail-label">Rate</span>
                      <span className="detail-value">
                        1 {homeCurrency} = {result.exchangeRate.toFixed(4)} {result.destinationCurrency}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {loading && (
        <div className="card-loading">
          <LoadingSpinner size="medium" text="Calculating budgets..." />
        </div>
      )}
    </div>
  );
};

export default BudgetCalculator;
