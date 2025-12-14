import { HttpMethod } from "constants/enums";

export interface ApiRequestOptions<TBody = unknown> {
  method: HttpMethod;
  body?: TBody | null;
  token?: string | null;
}

export interface ApiError extends Error {
  message: string;
  status: number;
  traceId: string;
}
