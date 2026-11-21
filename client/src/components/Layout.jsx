import { NavLink, Outlet } from 'react-router-dom'

export default function Layout(){
    return(
        <div className="app">
            <header className="topbar">
                <span className="brand"> ERP Inventory &amp; Orders </span>
                <nav>
                    <NavLink to="/" end>Home</NavLink>
                    <NavLink to="/products" end>Products</NavLink>
                    <NavLink to="/customers" end>Customers</NavLink>
                    <NavLink to="/orders" end>Orders</NavLink>
                    <NavLink to="/login" end>Login</NavLink>
                </nav>
            </header>
            <main className="content">
                <Outlet />
            </main>
        </div>
    )
}
