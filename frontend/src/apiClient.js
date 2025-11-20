const gatewayBaseUrl = import.meta.env.VITE_GATEWAY_URL || 'http://localhost:5000'

export async function fetchItems(path, token) {
  const response = await fetch(`${gatewayBaseUrl}${path}`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`)
  }

  return response.json()
}
