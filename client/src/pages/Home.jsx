import { useEffect, useState } from 'react'
import { checkApiHealth } from '../api/client.js'

export default function Home() {
    const [status , setStatus] = useState('Checking...')
    const [failed, setFailed] = useState(false)

    useEffect(() => {
        let ignore = false

        checkApiHealth()
        .then((text) => {
            if(!ignore) setStatus(text)
        })
        .catch((error) => {
            if(!ignore) {
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
            <h1>Welcome</h1>
            <p>
            API status:{' '}
            <strong className={failed ? 'bad' : 'good'}>{status}</strong>
            </p>
        </section>
    )
}