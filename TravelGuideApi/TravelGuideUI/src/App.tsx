import React, { useState } from 'react';
import Header from './components/Header/Header';
import TravelComparison from './components/TravelComparison/TravelComparison';
import PetPhotos from './components/PetPhotos/PetPhotos';
import BudgetCalculator from './components/BudgetCalculator/BudgetCalculator';
import './App.css';

type View = 'comparison' | 'budget';

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
          {activeView === 'comparison' ? <TravelComparison /> : <BudgetCalculator />}
        </main>
      </div>
    </div>
  );
};

export default App;
