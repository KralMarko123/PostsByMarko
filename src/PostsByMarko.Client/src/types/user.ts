export interface User {
  id?: string | null;
  firstName: string;
  lastName: string;
  email: string;
}

export interface Registration {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface Login {
  email: string;
  password: string;
}
