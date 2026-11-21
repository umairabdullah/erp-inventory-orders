import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from './authContext'

export default function ProtectedRoute({ roles }) {
    const { user, restoring } = useAuth()
    const location = useLocation()

    if(restoring) return <p>Loading...</p>

    if(!user) return <Navigate to="/login" replace state={{ from: location}} />

    if(roles && !roles.includes(user.role)){
        return <p className="bad">You don't have access to this page.</p>
    }
    return <Outlet />
}

