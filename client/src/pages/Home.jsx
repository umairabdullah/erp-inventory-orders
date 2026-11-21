import { useEffect, useState } from 'react'
import { checkApiHealth } from '../api/client.js'
import { request } from '../api/http.js'
import { useAuth } from '../auth/authContext.js'


function CatalogSummary(){
    const [count, setCount] = useState(null)
    const [error, setError] = useState('')

    useEffect( () => {
        let ignore = false

        request('/api/Products?pageSize=1')
        .then((page) => {
            if(!ignore) setCount(page.totalCount)
        })
        .catch((err) => {
            if(!ignore) setError(err.message)
        })
        
        return () => {
            ignore = true
        }
    }, [])

    if(error) return <p className="bad">{error}</p>
    if(count === null) return <p>Loading catalog...</p>

    return(
        <p>
            Products in the catalog: <strong>{count}</strong>
        </p>
    )
}

export default function Home(){
    const { user } = useAuth()
    const [ status, setStatus ] = useState('Checking...')
    const [ failed , setFailed ] = useState(false)

    useEffect(() => {
        let ignore = false

        checkApiHealth()
        .then((text) => {
            if(!ignore) setStatus(text)
        })
        .catch((error) => {
            if(!ignore){
                setFailed(true)
                setStatus(error.message)
            }
        })

        return () => {
            ignore = true
        }
    }, [])


  return (
    <section>
      <h1>{user ? `Welcome, ${user.fullName}` : 'Welcome'}</h1>
      <p>
        API status: <strong className={failed ? 'bad' : 'good'}>{status}</strong>
      </p>
      {user && <CatalogSummary />}
    </section>
  )
}