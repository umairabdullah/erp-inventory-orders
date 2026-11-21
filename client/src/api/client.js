const API_BASE_URL = import.meta.env.VITE_API_BASE_URL

export async function checkApiHealth() {
    const response = await fetch(`${API_BASE_URL}/health/live`)

    if(!response.ok){
        throw new Error(`The API answered with status ${response.status}`)
    }
    return response.text()
}