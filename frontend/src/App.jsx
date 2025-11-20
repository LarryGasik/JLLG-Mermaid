import React, { useState } from 'react'
import { Auth0Provider, useAuth0 } from '@auth0/auth0-react'
import { fetchItems } from './apiClient'

const services = [
  { path: '/svc-a/items', label: 'Service A Items' },
  { path: '/svc-b/items', label: 'Service B Items' },
  { path: '/svc-c/items', label: 'Service C Items' }
]

function Items() {
  const { isAuthenticated, loginWithRedirect, getIdTokenClaims, logout } = useAuth0()
  const [result, setResult] = useState('')
  const [error, setError] = useState('')

  const loadItems = async (path) => {
    setError('')
    setResult('Loading...')
    try {
      const claims = await getIdTokenClaims()
      const token = claims.__raw
      const data = await fetchItems(path, token)
      setResult(JSON.stringify(data, null, 2))
    } catch (err) {
      setError(err.message)
      setResult('')
    }
  }

  if (!isAuthenticated) {
    return (
      <div className="panel">
        <p>Sign in to query the microservices.</p>
        <button onClick={() => loginWithRedirect()}>Log in</button>
      </div>
    )
  }

  return (
    <div className="panel">
      <div className="actions">
        {services.map((svc) => (
          <button key={svc.path} onClick={() => loadItems(svc.path)}>
            {svc.label}
          </button>
        ))}
      </div>
      <pre>{result}</pre>
      {error && <div className="error">{error}</div>}
      <button className="secondary" onClick={() => logout({ returnTo: window.location.origin })}>
        Log out
      </button>
    </div>
  )
}

function App() {
  return (
    <Auth0Provider
      domain={import.meta.env.VITE_AUTH0_DOMAIN}
      clientId={import.meta.env.VITE_AUTH0_CLIENT_ID}
      authorizationParams={{
        audience: import.meta.env.VITE_AUTH0_AUDIENCE,
        redirect_uri: window.location.origin
      }}
    >
      <main>
        <h1>Mermaid Architecture React App</h1>
        <p>
          This UI calls the API Gateway which fans out to three protected microservices, all secured with
          Auth0-issued access tokens.
        </p>
        <Items />
      </main>
    </Auth0Provider>
  )
}

export default App
