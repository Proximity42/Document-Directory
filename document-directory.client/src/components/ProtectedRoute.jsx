import { Navigate } from 'react-router-dom';
import Cookie from 'js-cookie';
import { isJwtExpired } from 'jwt-check-expiration';

const ProtectedRoute = ({ children }) => {
    if (Cookie.get('test') === undefined || isJwtExpired(Cookie.get('test'))) {
        return <Navigate to='/login'/>
    }
  
    return children;
};

export default ProtectedRoute;