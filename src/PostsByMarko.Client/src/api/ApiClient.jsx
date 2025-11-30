const ApiClient = {
  async apiRequest(path, { method = "GET", body, token } = {}) {
    const headers = { "Content-Type": "application/json" };

    if (token) {
      headers.Authorization = `Bearer ${token}`;
    }

    const response = await fetch(path, {
      method,
      headers,
      body: body ? JSON.stringify(body) : undefined,
    });

    if (!response.ok) {
      const errorBody = await response.json().catch(() => null);
      throw new Error(errorBody?.message || response.statusText);
    }

    return await response.json().catch(() => null);
  },
};

export default ApiClient;
