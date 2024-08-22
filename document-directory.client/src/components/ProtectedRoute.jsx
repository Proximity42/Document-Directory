import { Navigate } from 'react-router-dom';
import Cookie from 'js-cookie';


const ProtectedRoute = ({ children }) => {
    if (Cookie.get('test') === undefined) {
        return <Navigate to='/login'/>
    }
    return children;
};

export default ProtectedRoute;