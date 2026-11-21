import { useCallback, useEffect, useMemo, useState, useSyncExternalStore } from "react"
import { refreshSession, request } from "../api/http"
import { AuthContext } from "./authContext"
import { tokenStore } from "./tokenStore"

export function AuthProvider({ children }) {
    // useSyncExternalStore reads data kept outside React and re-renders when it changes.
    const user = useSyncExternalStore(tokenStore.subscribe, tokenStore.getUser)
    const [restoring, setRestoring] = useState(() => Boolean(tokenStore.getRefreshToken()))

    useEffect(() => {
        if(!tokenStore.getRefreshToken()) return
        // StrictMode runs this effect twice in development. Both calls share one request.

        refreshSession()
        .catch(() => {})
        .finally(() => setRestoring(false))
    }, [])
    
    const login = useCallback(async (email, password) => {
    const session = await request('/api/Auth/login', {
        method: 'POST',
        body: { email, password },
        auth: false,
    })
    tokenStore.setSession(session)
    }, [])

    const logout = useCallback(async () => {
        const refreshToken  = tokenStore.getRefreshToken()
        tokenStore.clear()

        if(refreshToken){
            try {
                await request('/api/Auth/logout', {
                    method: 'POST',
                    body: { refreshToken },
                    auth: false,
                })
            } catch {
                // The local session is already gone. The server token expires on its own.
            }
        }
    }, [])

    const value = useMemo(
        () => ({ user, restoring, login, logout}),
        [user, restoring, login, logout]
    )

    return <AuthContext.Provider value = {value}>{children}</AuthContext.Provider>
}