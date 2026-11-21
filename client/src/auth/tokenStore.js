const REFRESH_KEY = 'erp.refreshToken'

let accessToken = null
let user = null

const listeners = new Set()

function notify(){
    listeners.forEach((listener) => listener())
}

export const tokenStore = {
    getAccessToken: () => accessToken,

getRefreshToken() {
        try{
            return localStorage.getItem(REFRESH_KEY)
        } catch {
            return null
        }
    },

    getUser: () => user,
    setSession(session){
        accessToken = session.accessToken
        user = session.user
        try{
            localStorage.setItem(REFRESH_KEY, session.refreshToken)
        } catch {
            //nothing to put here.
        }
        notify()
    },

    clear() {
        accessToken = null
        user = null
        try{
            localStorage.removeItem(REFRESH_KEY)
        } catch{
            //nothing to cleanup
        }
        notify()
    }, 

    subscribe(listener){
        listeners.add(listener)
        return () => listeners.delete(listener)
    }, 
}