/**
 * The one way this application talks to the backend.
 *
 * Requests are same-origin in both environments — the dev server proxies `/api`
 * and `/mcp`, and in production the backend serves these files — so the session
 * cookie travels without any CORS configuration.
 */

/** A request the backend answered with a status outside 2xx. */
export class HttpError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.name = 'HttpError'
    this.status = status
  }
}

type RequestBody = Record<string, unknown>

async function send(method: string, path: string, body?: RequestBody): Promise<Response> {
  const response = await fetch(path, {
    method,
    headers: body === undefined ? undefined : { 'Content-Type': 'application/json' },
    body: body === undefined ? undefined : JSON.stringify(body),
  })

  if (!response.ok) {
    throw new HttpError(response.status, `${method} ${path} answered ${String(response.status)}.`)
  }

  return response
}

export async function get<TResponse>(path: string): Promise<TResponse> {
  const response = await send('GET', path)
  return (await response.json()) as TResponse
}

export async function post(path: string, body: RequestBody): Promise<void> {
  await send('POST', path, body)
}

export async function remove(path: string): Promise<void> {
  await send('DELETE', path)
}
