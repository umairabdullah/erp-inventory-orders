import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Home from './pages/Home.jsx'
import Login from './pages/Login.jsx'
import Products from './pages/Products.jsx'
import Customers from './pages/Customers.jsx'
import Orders from './pages/Orders.jsx'
import NotFound from './pages/NotFound.jsx'


export default function App(){
  return(
    <Routes>
        <Route element={<Layout />}>
          <Route index element={<Home />} />
          <Route path="login" element={<Login />} />
          <Route path="products" element={<Products />} />
          <Route path="customers" element={<Customers />} />
          <Route path="orders" element={<Orders />} />
          <Route path="*" element={<NotFound />} />
        </Route>
    </Routes>
  )
}
