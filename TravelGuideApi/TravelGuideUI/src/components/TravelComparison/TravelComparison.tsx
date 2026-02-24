import React, { useState, useEffect, useCallback } from 'react';
import { Country, CurrencyComparisonResult } from '../../types';
import { apiService } from '../../services/api';
import CountrySelector from './CountrySelector';
import LoadingSpinner from '../common/LoadingSpinner';
import ErrorMessage from '../common/ErrorMessage';
import './TravelComparison.css';

const TravelComparison: React.FC = () => {
  const [countries, setCountries] = useState<Country[]>([]);
  const [homeCountryCode, setHomeCountryCode] = useState<string>('');
  const [destinationCountryCode, setDestinationCountryCode] = useState<string>('');
  const [comparisonResult, setComparisonResult] = useState<CurrencyComparisonResult | null>(null);
  
  const [loadingCountries, setLoadingCountries] = useState(true);
  const [loadingComparison, setLoadingComparison] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCountries = useCallback(async () => {
    try {
      setLoadingCountries(true);
      setError(null);
      const data = await apiService.getCountries();
      setCountries(data);
    } catch (err) {
      setError('Failed to load countries. Please try again.');
      console.error('Error fetching countries:', err);
    } finally {
      setLoadingCountries(false);
    }
  }, []);

  useEffect(() => {
    fetchCountries();
  }, [fetchCountries]);

  const handleExplore = async () => {
    if (!homeCountryCode || !destinationCountryCode) {
      return;
    }

    try {
      setLoadingComparison(true);
      setError(null);
      const result = await apiService.compareCurrencies({
        homeCountryCode,
        destinationCountryCode,
      });
      setComparisonResult(result);
    } catch (err) {
      setError('Failed to compare currencies. Please try again.');
      console.error('Error comparing currencies:', err);
    } finally {
      setLoadingComparison(false);
    }
  };

  const handleSwapCountries = () => {
    const temp = homeCountryCode;
    setHomeCountryCode(destinationCountryCode);
    setDestinationCountryCode(temp);
    setComparisonResult(null);
  };

  const canExplore = homeCountryCode && destinationCountryCode && homeCountryCode !== destinationCountryCode;

  const getStrengthIcon = (hint: string) => {
    if (hint.toLowerCase().includes('stronger')) {
      return (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
          <polyline points="18 15 12 9 6 15" />
        </svg>
      );
    } else if (hint.toLowerCase().includes('weaker')) {
      return (
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
          <polyline points="6 9 12 15 18 9" />
        </svg>
      );
    }
    return (
      <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
        <line x1="5" y1="12" x2="19" y2="12" />
      </svg>
    );
  };

  const getStrengthClass = (hint: string) => {
    if (hint.toLowerCase().includes('stronger')) return 'strength-up';
    if (hint.toLowerCase().includes('weaker')) return 'strength-down';
    return 'strength-neutral';
  };

  return (
    <div className="travel-comparison-card">
      <div className="card-header">
        <div className="card-icon">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <circle cx="12" cy="12" r="10" />
            <path d="M2 12h20" />
            <path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z" />
          </svg>
        </div>
        <div className="card-title-group">
          <h2 className="card-title">Travel Comparison</h2>
          <p className="card-subtitle">Compare currency strength between your home and destination</p>
        </div>
      </div>

      {loadingCountries ? (
        <div className="card-loading">
          <LoadingSpinner size="medium" text="Loading countries..." />
        </div>
      ) : error && !countries.length ? (
        <ErrorMessage message={error} onRetry={fetchCountries} />
      ) : (
        <>
          <div className="selectors-container">
            <CountrySelector
              label="Home Country"
              icon="home"
              countries={countries}
              value={homeCountryCode}
              onChange={(code) => {
                setHomeCountryCode(code);
                setComparisonResult(null);
              }}
            />
            
            <button 
              className="swap-button"
              onClick={handleSwapCountries}
              disabled={!homeCountryCode && !destinationCountryCode}
              title="Swap countries"
            >
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                <polyline points="17 1 21 5 17 9" />
                <path d="M3 11V9a4 4 0 0 1 4-4h14" />
                <polyline points="7 23 3 19 7 15" />
                <path d="M21 13v2a4 4 0 0 1-4 4H3" />
              </svg>
            </button>

            <CountrySelector
              label="Destination"
              icon="destination"
              countries={countries}
              value={destinationCountryCode}
              onChange={(code) => {
                setDestinationCountryCode(code);
                setComparisonResult(null);
              }}
            />
          </div>

          <button
            className={`explore-button ${canExplore ? 'active' : ''}`}
            onClick={handleExplore}
            disabled={!canExplore || loadingComparison}
          >
            {loadingComparison ? (
              <>
                <span className="button-spinner"></span>
                Comparing...
              </>
            ) : (
              <>
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                  <circle cx="11" cy="11" r="8" />
                  <line x1="21" y1="21" x2="16.65" y2="16.65" />
                </svg>
                Explore
              </>
            )}
          </button>

          {error && countries.length > 0 && (
            <ErrorMessage message={error} />
          )}

          {comparisonResult && (
            <div className="comparison-result">
              <div className="result-header">
                <h3>Currency Comparison</h3>
                <span className={`strength-badge ${getStrengthClass(comparisonResult.strengthHint)}`}>
                  {getStrengthIcon(comparisonResult.strengthHint)}
                  {comparisonResult.strengthHint}
                </span>
              </div>

              <div className="exchange-display">
                <div className="currency-block home">
                  <span className="currency-code">{comparisonResult.homeCurrency}</span>
                  <span className="currency-value">1</span>
                  <span className="currency-country">{comparisonResult.homeCountry}</span>
                </div>
                
                <div className="exchange-arrow">
                  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <line x1="5" y1="12" x2="19" y2="12" />
                    <polyline points="12 5 19 12 12 19" />
                  </svg>
                </div>
                
                <div className="currency-block destination">
                  <span className="currency-code">{comparisonResult.destinationCurrency}</span>
                  <span className="currency-value">{comparisonResult.exchangeRate.toFixed(4)}</span>
                  <span className="currency-country">{comparisonResult.destinationCountry}</span>
                </div>
              </div>
            </div>
          )}
        </>
      )}
    </div>
  );
};

export default TravelComparison;
