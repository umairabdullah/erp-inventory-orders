import { useState } from 'react'
import { Link } from 'react-router-dom'
import { request } from '../api/http.js'

export default function Register(){
    const [ fullName , setFullName] = useState('')
    const [ email, setEmail ] = useState('')
    const [ password , setPassword ] = useState('')
    const [ error , setError ] = useState('')
    const [ done , setDone ] = useState(false)
    const [ submitting , setSubmitting ] = useState(false)

    async function handleSubmit(event){
        event.preventDefault()
        setError('')
        setSubmitting(true)
    

    try{
        await request('/Api/Auth/register', {
            method: 'POST',
            body: {fullName, email, password},
            auth: false,
        })
        setDone(true)
    }catch(err){
        setError(
            err.status === 429 ? 'Too many attempts. Please wait a minute and try again.' : err.message,
        )
    } finally {
        setSubmitting(false)
    }
}

if(done){
    return(
        <section>
            <h1>Check your inbox</h1>
            <p>
                we sent a confirmation link to <strong>{email}</strong>. Confirm your email, then {' '}
                <Link to="/login">sign in</Link>
            </p>
        </section>
    )
}

return(
    <section>
        <h1>Create an account</h1>
        <form className="form" onSubmit={handleSubmit}>
            <label>
                Full Name
                <input value={fullName} onChange={(e) => setFullName(e.target.value)} required />
            </label>
             <label>
                Email
                <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    autoComplete="username"
                    required
                />
        </label>
         <label>
          Password
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            autoComplete="new-password"
            required
          />
          <span className="hint">At least 8 characters, with upper case, lower case, and a digit.</span>
        </label>
        {error && <p className="alert error" role="alert">{error}</p>}

        <button type="submit" disabled={submitting}>
            {submitting ? 'Creating account...' : 'Create account'}
        </button>
        </form>
    </section>
)
}