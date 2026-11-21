import { useState } from 'react'
import { Link, Navigate, useLocation } from 'react-router-dom'
import { request } from '../api/http.js'
import { useAuth } from '../auth/authContext.js'

export default function Login() {
    const { user, login } = useAuth()
    const location = useLocation()
    const redirectTo = location.state?.from?.pathname || '/'

    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState('')
    const [info, setInfo] = useState('')
    const [needsConfirmation, setNeedsConfirmation] = useState(false)
    const [submitting, setSubmitting] = useState(false)

    if(user) return <Navigate to={redirectTo} replace />
    
    async function handleSubmit(event){
        event.preventDefault()
        setError('')
        setInfo('')
        setNeedsConfirmation(false)
        setSubmitting(true)

        try{
            await login(email, password)
        } catch(err){
            if(err.status === 403) setNeedsConfirmation(true)
                setError(
            err.status === 429 ? 'Too many attempts. Please wait a minute and try again.' : err.message)
        }
        finally{
            setSubmitting(false)
        }
    }

    async function handleResend(){
        setError('')
        try{
            await request('/Api/Auth/resend-confirmation', {
                method: 'POST',
                body: {email},
                auth: false,
            })
            setNeedsConfirmation(false)
            setInfo('If this account exists and is not confirmed yet, a new link has been sent.')
        } catch(err){
            setError(err.message)
        }
    }

    return(
        <section>
            <h1>Sign in</h1>
            <form className='form' onSubmit={handleSubmit}>
                <label>
                    Email
                    <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    autoComplete='username'
                    required
                    />
                </label>
                 <label>
                    Password
                    <input
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    autoComplete='current-password'
                    required
                    />
                </label>
                {error && <p className="alert error" role="alert"> {error} </p>}
                {info && <p className="alert success"> {info} </p>}

                {needsConfirmation && email && (
                    <button type="button" onClick={handleResend}>Resend confirmation email.</button>
                )}

                <button type="submit" disabled={submitting}>
                    {submitting ? 'Signing in...' : 'Sign in'}
                </button>
            </form>

            <p className="hint">
                No account yet? <Link to="/register">Create one</Link>
            </p>
        </section>
    )
}