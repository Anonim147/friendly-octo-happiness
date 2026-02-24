import React from 'react';
import { Country } from '../../types';
import './CountrySelector.css';

interface CountrySelectorProps {
  label: string;
  icon: 'home' | 'destination';
  countries: Country[];
  value: string;
  onChange: (code: string) => void;
  disabled?: boolean;
}

const CountrySelector: React.FC<CountrySelectorProps> = ({
  label,
  icon,
  countries,
  value,
  onChange,
  disabled = false,
}) => {
  const selectedCountry = countries.find(c => c.code === value);

  return (
    <div className="country-selector">
      <label className="selector-label">
        <span className="label-icon">
          {icon === 'home' ? (
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
              <polyline points="9 22 9 12 15 12 15 22" />
            </svg>
          ) : (
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="12" cy="10" r="3" />
              <path d="M12 21.7C17.3 17 20 13 20 10a8 8 0 1 0-16 0c0 3 2.7 6.9 8 11.7z" />
            </svg>
          )}
        </span>
        {label}
      </label>
      <div className="selector-wrapper">
        {selectedCountry?.flagUrl && (
          <img 
            src={selectedCountry.flagUrl} 
            alt={`${selectedCountry.name} flag`}
            className="selected-flag"
          />
        )}
        <select
          className="selector-input"
          value={value}
          onChange={(e) => onChange(e.target.value)}
          disabled={disabled}
        >
          <option value="">Select a country</option>
          {countries.map((country) => (
            <option key={country.code} value={country.code}>
              {country.name} ({country.currencyCode})
            </option>
          ))}
        </select>
        <span className="selector-arrow">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
            <polyline points="6 9 12 15 18 9" />
          </svg>
        </span>
      </div>
      {selectedCountry && (
        <div className="currency-info">
          <span className="currency-badge">{selectedCountry.currencyCode}</span>
          <span className="currency-name">{selectedCountry.currencyName}</span>
        </div>
      )}
    </div>
  );
};

export default CountrySelector;
