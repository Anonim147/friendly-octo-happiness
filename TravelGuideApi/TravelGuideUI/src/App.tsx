import React, { useState } from 'react';
import Header from './components/Header/Header';
import TravelComparison from './components/TravelComparison/TravelComparison';
import PetPhotos from './components/PetPhotos/PetPhotos';
import BudgetCalculator from './components/BudgetCalculator/BudgetCalculator';
import HistoricalRates from './components/HistoricalRates/HistoricalRates';
import './App.css';

type View = 'comparison' | 'budget' | 'history';

const App: React.FC = () => {
  const [activeView, setActiveView] = useState<View>('comparison');

  return (
    <div className="app">
      <div className="app-background">
        <div className="bg-shape bg-shape-1"></div>
        <div className="bg-shape bg-shape-2"></div>
        <div className="bg-shape bg-shape-3"></div>
      </div>
      <div className="app-content">
        <Header activeView={activeView} onNavigate={setActiveView} />
        <main className="main-content">
          <PetPhotos />
          {activeView === 'comparison' && <TravelComparison />}
          {activeView === 'budget' && <BudgetCalculator />}
          {activeView === 'history' && <HistoricalRates />}
        </main>
      </div>
    </div>
  );
};

export default App;
