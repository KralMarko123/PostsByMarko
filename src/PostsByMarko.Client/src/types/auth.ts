export interface AuthUser {
  id: string;
  token?: string | null;
  email?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  roles?: string[] | null;
}

export interface AuthContextValue {
  user: AuthUser | null;
  isAdmin: boolean;
  login: (user: AuthUser) => void;
  logout: () => void;
  checkToken: () => void;
}

export interface AuthProviderProps {
  children: React.ReactNode;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}
