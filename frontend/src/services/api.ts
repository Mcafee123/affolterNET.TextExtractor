/**
 * Centralized API service with authentication and CSRF handling
 * for affolterNET.Web.Bff integration
 */

/**
 * Get CSRF token from cookie
 */
function getCsrfToken(): string | null {
  const match = document.cookie.match(/XSRF-TOKEN=([^;]+)/)
  return match && match[1] ? decodeURIComponent(match[1]) : null
}

/**
 * Handle 401 responses by redirecting to BFF login
 */
function handleUnauthorized(): void {
  const returnUrl = encodeURIComponent(window.location.pathname + window.location.search)
  window.location.href = `/bff/account/login?returnUrl=${returnUrl}`
}

/**
 * API client configuration
 */
interface ApiOptions extends RequestInit {
  skipAuth?: boolean
}

/**
 * Make an API request with automatic CSRF and 401 handling
 */
async function request<T>(url: string, options: ApiOptions = {}): Promise<T> {
  const { skipAuth = false, ...fetchOptions } = options

  // Add CSRF token for state-changing requests
  const method = (fetchOptions.method || 'GET').toUpperCase()
  if (['POST', 'PUT', 'DELETE', 'PATCH'].includes(method)) {
    const csrfToken = getCsrfToken()
    if (csrfToken) {
      fetchOptions.headers = {
        ...fetchOptions.headers,
        'X-XSRF-TOKEN': csrfToken
      }
    }
  }

  const response = await fetch(url, fetchOptions)

  // Handle 401 Unauthorized
  if (response.status === 401 && !skipAuth) {
    handleUnauthorized()
    throw new Error('Unauthorized - redirecting to login')
  }

  // Handle other error responses
  if (!response.ok) {
    throw new Error(`API error: ${response.status} ${response.statusText}`)
  }

  // Return JSON response (or empty object for 204)
  if (response.status === 204) {
    return {} as T
  }

  return response.json()
}

/**
 * API client with convenience methods
 */
export const api = {
  /**
   * GET request
   */
  get<T>(url: string, options?: ApiOptions): Promise<T> {
    return request<T>(url, { ...options, method: 'GET' })
  },

  /**
   * POST request with JSON body
   */
  post<T>(url: string, body?: unknown, options?: ApiOptions): Promise<T> {
    return request<T>(url, {
      ...options,
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers
      },
      body: body ? JSON.stringify(body) : undefined
    })
  },

  /**
   * POST request with FormData (for file uploads)
   */
  postForm<T>(url: string, formData: FormData, options?: ApiOptions): Promise<T> {
    return request<T>(url, {
      ...options,
      method: 'POST',
      body: formData
    })
  },

  /**
   * PUT request with JSON body
   */
  put<T>(url: string, body?: unknown, options?: ApiOptions): Promise<T> {
    return request<T>(url, {
      ...options,
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers
      },
      body: body ? JSON.stringify(body) : undefined
    })
  },

  /**
   * DELETE request
   */
  delete<T>(url: string, options?: ApiOptions): Promise<T> {
    return request<T>(url, { ...options, method: 'DELETE' })
  },

  /**
   * Get current user info from BFF
   */
  async getUser(): Promise<{ isAuthenticated: boolean; claims?: Record<string, string> }> {
    try {
      return await request('/bff/account/user', { skipAuth: true })
    } catch {
      return { isAuthenticated: false }
    }
  },

  /**
   * Redirect to login
   */
  login(returnUrl?: string): void {
    const url = returnUrl
      ? `/bff/account/login?returnUrl=${encodeURIComponent(returnUrl)}`
      : '/bff/account/login'
    window.location.href = url
  },

  /**
   * Logout via BFF
   */
  logout(): void {
    window.location.href = '/bff/account/logout'
  }
}

export default api
