import { useState } from 'react'
import { Link, useSearchParams} from 'react-router-dom'
import { request } from '../api/http.js'

export default function ConfirmEmail(){
    const [params] = useSearchParams()
    const email = params.get('email')
    const token = params.get('token')
    const [state, setState] = useState({status: 'idle', message: ''})

    if(!email || token){
        return(
            <section>
                <h1>Confirm your email</h1>
                <p className="alert error">This confirmation link is incomplete. </p>
            </section>
        )
    }

    async function handleConfirm(){
        setState({status: 'working', message: ''})
        try{
            await request('/api/Auth/confirm-email', {
                method: 'POST',
                body: {email, token},
                auth: false,
            })
            setState({status: 'done', message: ''})
        } catch(err){
            setState({status: 'error', message: err.message})
        }
    }

    return (
        <section>
            <h1>Confirm your email</h1>
            {state.status === 'done' ? (
                <p className="alert success">
                    Your email is confirmed. You can now <Link to="/login">Sign in</Link>.
                </p>) : (
                    <>
                        <p>Confirm that <strong>{email}</strong> is your address.</p>
                        {state.status === 'error' && (
                            <p className="alert error" role="alert">{state.message}</p>
                        )}
                        <button onClick={handleConfirm} disabled={state.status === 'working'}>
                            {state.status === 'working' ? 'Confirming...' : 'Confirm my email'}
                        </button>
                    </>
                )}
        </section>
    )
}