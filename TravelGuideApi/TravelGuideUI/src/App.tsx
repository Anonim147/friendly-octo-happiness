import React from 'react';
import Header from './components/Header/Header';
import TravelComparison from './components/TravelComparison/TravelComparison';
import './App.css';

const App: React.FC = () => {
  return (
    <div className="app">
      <div className="app-background">
        <div className="bg-shape bg-shape-1"></div>
        <div className="bg-shape bg-shape-2"></div>
        <div className="bg-shape bg-shape-3"></div>
      </div>
      <div className="app-content">
        <Header />
        <main className="main-content">
          <TravelComparison />
        </main>
      </div>
    </div>
  );
};

export default App;
