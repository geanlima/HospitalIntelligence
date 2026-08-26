export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  expiresAtUtc: string;
  username: string;
  roles: string[];
}

export interface AuthSession {
  accessToken: string;
  expiresAtUtc: string;
  username: string;
  roles: string[];
}
