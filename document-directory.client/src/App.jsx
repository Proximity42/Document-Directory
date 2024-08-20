import { useEffect, useState } from 'react';
import { Routes, Route } from 'react-router';
import { BrowserRouter, NavLink} from 'react-router-dom';
import './App.css';
import MainPageComponent from './components/MainPageComponent';
import AuthorizationComponent from './components/AuthorizationComponent';
import PageAdminComponent from './components/PageAdminComponent';


function App() {

    return (
        <div>
            <BrowserRouter>
                <nav style={{marginBottom: "30px"}}>
                    <ul style={{display: 'flex', listStyle: 'none', justifyContent: 'space-evenly', marginTop: '0'}}>
                        <li><NavLink to='/'>Главная</NavLink></li>
                        <li><NavLink to='/profile'>Профиль</NavLink></li>
                    </ul>
                </nav>
                <main>
                    <div className="content">
                        <Routes>
                            <Route path="/" element={ <MainPageComponent /> } />
                            <Route path="/login" element={ <AuthorizationComponent />} />
                            <Route path="/admin" element={ <PageAdminComponent />} />
                            
                        </Routes>
                    </div>
                </main>
            </BrowserRouter>
        </div>
    );
    
    //async function populateWeatherData() {
    //    const response = await fetch('weatherforecast');
    //    const data = await response.json();
    //    setForecasts(data);
    //}
}

export default App;