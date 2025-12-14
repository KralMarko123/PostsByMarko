import { HttpMethod } from "constants/enums";
import { ApiError, ApiRequestOptions } from "types/api";

export const ApiClient = {
  async apiRequest<TResponse>(
    path: string,
    { method = HttpMethod.GET, body, token }: ApiRequestOptions
  ) {
    const headers: Record<string, string> = {
      "Content-Type": "application/json",
    };

    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch(path, {
      method,
      headers,
      body: body ? JSON.stringify(body) : undefined,
    });

    if (!response.ok) {
      const error: ApiError = await response.json().catch((error: ApiError) => error);
      throw new Error(error?.message || response.statusText);
    }

    // Handle No content responses
    if (response.status === 204) {
      return null as TResponse;
    }

    return (await response.json().catch(() => null)) as TResponse;
  },
};
