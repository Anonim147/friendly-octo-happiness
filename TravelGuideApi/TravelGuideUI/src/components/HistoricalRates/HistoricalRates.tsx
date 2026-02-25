import React, { useState, useCallback } from 'react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from 'recharts';
import { apiService } from '../../services/api';
import { ExchangeRateDataPoint } from '../../types';
import './HistoricalRates.css';

const POPULAR_CURRENCIES = ['USD', 'EUR', 'GBP', 'JPY', 'CAD', 'AUD', 'CHF', 'CNY', 'INR', 'BRL'];
const DAY_OPTIONS = [30, 90, 365] as const;
type DayOption = typeof DAY_OPTIONS[number];

const HistoricalRates: React.FC = () => {
  const [fromCurrency, setFromCurrency] = useState('USD');
  const [toCurrency, setToCurrency] = useState('EUR');
  const [days, setDays] = useState<DayOption>(30);
  const [data, setData] = useState<ExchangeRateDataPoint[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [hasSearched, setHasSearched] = useState(false);

  const fetchData = useCallback(async (from: string, to: string, d: DayOption) => {
    if (!from || !to) return;
    setLoading(true);
    setError(null);
    setHasSearched(true);
    try {
      const result = await apiService.getHistoricalRates(from, to, d);
      setData(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch historical rates');
      setData([]);
    } finally {
      setLoading(false);
    }
  }, []);

  const handleSearch = () => {
    fetchData(fromCurrency, toCurrency, days);
  };

  const handleDayToggle = (d: DayOption) => {
    setDays(d);
    if (hasSearched) {
      fetchData(fromCurrency, toCurrency, d);
    }
  };

  const formatDate = (dateStr: string) => {
    const date = new Date(dateStr);
    return date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
  };

  const formatTooltipDate = (label: unknown) => {
    if (typeof label !== 'string') return String(label);
    const date = new Date(label);
    return date.toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
  };

  const minRate = data.length > 0 ? Math.min(...data.map(d => d.rate)) : 0;
  const maxRate = data.length > 0 ? Math.max(...data.map(d => d.rate)) : 1;
  const padding = (maxRate - minRate) * 0.1 || 0.01;

  return (
    <div className="historical-rates">
      <div className="historical-rates-card">
        <h2 className="historical-rates-title">Historical Exchange Rates</h2>
        <p className="historical-rates-subtitle">View exchange rate trends over time</p>

        <div className="historical-rates-controls">
          <div className="currency-selectors">
            <div className="currency-selector-group">
              <label className="currency-label">From</label>
              <select
                className="currency-select"
                value={fromCurrency}
                onChange={e => setFromCurrency(e.target.value)}
              >
                {POPULAR_CURRENCIES.map(c => (
                  <option key={c} value={c}>{c}</option>
                ))}
              </select>
            </div>
            <div className="currency-swap-icon">→</div>
            <div className="currency-selector-group">
              <label className="currency-label">To</label>
              <select
                className="currency-select"
                value={toCurrency}
                onChange={e => setToCurrency(e.target.value)}
              >
                {POPULAR_CURRENCIES.map(c => (
                  <option key={c} value={c}>{c}</option>
                ))}
              </select>
            </div>
          </div>

          <div className="day-toggles">
            {DAY_OPTIONS.map(d => (
              <button
                key={d}
                className={`day-toggle-btn ${days === d ? 'day-toggle-btn-active' : ''}`}
                onClick={() => handleDayToggle(d)}
              >
                {d === 365 ? '1Y' : `${d}D`}
              </button>
            ))}
          </div>

          <button className="search-btn" onClick={handleSearch} disabled={loading}>
            {loading ? 'Loading...' : 'Show Chart'}
          </button>
        </div>

        {error && (
          <div className="historical-rates-error">
            {error}
          </div>
        )}

        {loading && (
          <div className="historical-rates-loading">
            <div className="loading-spinner"></div>
            <p>Fetching exchange rate data...</p>
          </div>
        )}

        {!loading && !error && hasSearched && data.length === 0 && (
          <div className="historical-rates-empty">
            No data available for the selected currency pair and period.
          </div>
        )}

        {!loading && !error && data.length > 0 && (
          <div className="chart-container">
            <div className="chart-header">
              <span className="chart-pair">{fromCurrency} → {toCurrency}</span>
              <span className="chart-period">{days === 365 ? 'Past Year' : `Past ${days} Days`}</span>
            </div>
            <ResponsiveContainer width="100%" height={320}>
              <LineChart data={data} margin={{ top: 10, right: 20, left: 10, bottom: 10 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.1)" />
                <XAxis
                  dataKey="date"
                  tickFormatter={formatDate}
                  tick={{ fill: 'rgba(255,255,255,0.6)', fontSize: 11 }}
                  tickLine={false}
                  axisLine={false}
                  interval={Math.floor(data.length / 6)}
                />
                <YAxis
                  domain={[minRate - padding, maxRate + padding]}
                  tick={{ fill: 'rgba(255,255,255,0.6)', fontSize: 11 }}
                  tickLine={false}
                  axisLine={false}
                  tickCount={5}
                  tickFormatter={(v: number) => v.toFixed(4)}
                />
                <Tooltip
                  contentStyle={{
                    background: 'rgba(30, 30, 60, 0.95)',
                    border: '1px solid rgba(255,255,255,0.15)',
                    borderRadius: '8px',
                    color: 'white',
                  }}
                  labelFormatter={formatTooltipDate}
                  formatter={(value: number | undefined) => [(value ?? 0).toFixed(6), `${fromCurrency}/${toCurrency}`]}
                />
                <Line
                  type="monotone"
                  dataKey="rate"
                  stroke="#7c6fef"
                  strokeWidth={2}
                  dot={false}
                  activeDot={{ r: 4, fill: '#7c6fef' }}
                />
              </LineChart>
            </ResponsiveContainer>
            <div className="chart-stats">
              <div className="chart-stat">
                <span className="chart-stat-label">Min</span>
                <span className="chart-stat-value">{minRate.toFixed(6)}</span>
              </div>
              <div className="chart-stat">
                <span className="chart-stat-label">Max</span>
                <span className="chart-stat-value">{maxRate.toFixed(6)}</span>
              </div>
              <div className="chart-stat">
                <span className="chart-stat-label">Latest</span>
                <span className="chart-stat-value">{data[data.length - 1]?.rate.toFixed(6)}</span>
              </div>
            </div>
          </div>
        )}

        {!hasSearched && (
          <div className="historical-rates-placeholder">
            <div className="placeholder-icon">📈</div>
            <p>Select currencies and click "Show Chart" to view historical exchange rate data.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default HistoricalRates;
