import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../auth/authContext.js'


export default function Layout(){
    const { user , logout }= useAuth()
    return(
        <div className="app">
            <header className="topbar">
                <span className="brand"> ERP Inventory &amp; Orders </span>
                <nav>
                    <NavLink to="/" end>Home</NavLink>
                    {user && (
                        <>
                            <NavLink to="/products" end>Products</NavLink>
                            <NavLink to="/customers" end>Customers</NavLink>
                            <NavLink to="/orders" end>Orders</NavLink>
                        </>
                    )}
                </nav>
                <div className="userbox">
                    {user ? (
                        <>
                            <span>{user.fullName} ({user.role})</span>
                            <button onClick={logout}>Sign out</button>
                        </>
                    ) : (
                        <>
                            <NavLink to="/login">Sign in</NavLink>
                            <NavLink to="/register">Register</NavLink>
                        </>
                    )}
                </div>
            </header>
            <main className="content">
                <Outlet />
            </main>
        </div>
    )
}
