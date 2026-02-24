import React, { useEffect, useState } from 'react';
import './PetPhotos.css';

const PetPhotos: React.FC = () => {
  const [catUrl, setCatUrl] = useState<string>('');
  const [dogUrl, setDogUrl] = useState<string>('');

  useEffect(() => {
    fetch('https://api.thecatapi.com/v1/images/search')
      .then(res => res.json())
      .then((data: Array<{ url: string }>) => setCatUrl(data[0]?.url ?? ''))
      .catch(() => {});

    fetch('https://dog.ceo/api/breeds/image/random')
      .then(res => res.json())
      .then((data: { message: string }) => setDogUrl(data.message ?? ''))
      .catch(() => {});
  }, []);

  return (
    <div className="pet-photos">
      <div className="pet-card">
        <h3 className="pet-card-title">🐱 Today's Cat</h3>
        {catUrl
          ? <img src={catUrl} alt="A random cat" className="pet-image" />
          : <div className="pet-loading">Loading cat...</div>}
      </div>
      <div className="pet-card">
        <h3 className="pet-card-title">🐶 Today's Dog</h3>
        {dogUrl
          ? <img src={dogUrl} alt="A random dog" className="pet-image" />
          : <div className="pet-loading">Loading dog...</div>}
      </div>
    </div>
  );
};

export default PetPhotos;
