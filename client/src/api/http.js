
import { tokenStore } from "../auth/tokenStore";

const  API_BASE_URL = import.meta.env.VITE_API_BASE_URL

export class ApiError extends Error {
    constructor(status, problem){
        super(messageFrom(status, problem))
        this.name = 'ApiError'
        this.status = status
        this.errors = problem?.errors ?? null
    }
}

function messageFrom(status, problem){
    if(problem?.errors){
        const message = Object.values(problem.errors).flat()
        if(message.length > 0) return message.join(' ')
    }
return problem?.detail || problem?.title || `The request failed (status ${status}).`
}

async function parseProblem(response){
    const text = await response.text()
    if(!text) return null
    try{
        return JSON.parse(text)
    } catch{
        return null
    }
}

async function safeFetch(url, init) {
    try {
        return await fetch(url, init)
    } catch {
        throw new ApiError(0, {
            detail: 'Cannot reach the server. Please check you connection and try again.'
        })
    }
}

let refreshing = null

export function refreshSession() {
    if(!refreshing){
        refreshing = doRefresh().finally(() => {
            refreshing = null
        })
    }
    return refreshing
}

async function doRefresh() {
    const refreshToken = tokenStore.getRefreshToken()
    if(!refreshToken) throw new ApiError(401, {detail: 'There is no active session'})

    const response = await safeFetch(`${API_BASE_URL}/api/Auth/refresh`, {
        method: 'POST',
        headers: { 'Content-Type' : 'application/json'},
        body: JSON.stringify({ refreshToken}),
    })

    if(!response.ok){
        // Only an invalid token ends the session. A busy or failing server should not sign people out.
        if(response.status === 400 || response.status === 401) tokenStore.clear()
            throw new ApiError(response.status, await parseProblem(response))
    }

    const session = await response.json()
    tokenStore.setSession(session)
    return session
}


export async function request(path, { method = 'GET', body, auth = true } = {}) {
  const send = () => {
    const headers = {}
    if (body !== undefined) headers['Content-Type'] = 'application/json'

    const token = tokenStore.getAccessToken()
    if (auth && token) headers.Authorization = `Bearer ${token}`

    return safeFetch(`${API_BASE_URL}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  }


let response = await send()

if(response.status === 401 && auth && tokenStore.getRefreshToken())
    try {
        await refreshSession()
        response = await send()
    } catch {
        // The session could not be renewed. The original 401 is reported below.
    }


if(!response.ok) throw new ApiError(response.status, await parseProblem(response))
    if(response.status === 204) 
        return null

return response.json()
}