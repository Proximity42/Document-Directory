import { Routes, Route } from 'react-router';
import { BrowserRouter, NavLink} from 'react-router-dom';
import './App.css';
import MainPage from './components/MainPage';
import Authorization from './components/Authorization';
import AdminPage from './components/AdminPage';
import AccessManagePage from './components/AccessManagePage';
import ProtectedRoute from './components/ProtectedRoute';


function App() {

    return (
        <div>
            <BrowserRouter>
                <nav style={{marginBottom: "30px"}}>
                    <ul style={{display: 'flex', listStyle: 'none', justifyContent: 'space-evenly', marginTop: '0'}}>
                        <li><NavLink to='/'>Главная</NavLink></li>
                        <li><NavLink to='/access'>Управление доступом</NavLink></li>
                        <li><NavLink to='/profile'>Профиль</NavLink></li>
                    </ul>
                </nav>
                <main>
                    <div className="content">
                        <Routes>
                            <Route path="/" element={ <ProtectedRoute><MainPage /></ProtectedRoute> } />
                            <Route path="/login" element={ <Authorization />} />
                            <Route path="/admin" element={ <ProtectedRoute><AdminPage /></ProtectedRoute>} />
                            <Route path="/access" element={ <ProtectedRoute><AccessManagePage/></ProtectedRoute> }/>
                        </Routes>
                    </div>
                </main>
            </BrowserRouter>
        </div>
    );
}

export default App;