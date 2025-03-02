import axios from 'axios';

const API_URL = 'https://localhost:7087/api/auth';

// Create a separate axios instance for auth endpoints
const authApi = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json'
  }
});

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiration: string;
  tokenType: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface LogoutRequest {
  refreshToken: string;
}

const authService = {
  /**
   * Login with email and password
   */
  login: async (email: string, password: string): Promise<AuthResponse> => {
    const response = await authApi.post<AuthResponse>('/login', { email, password });
    return response.data;
  },
  
  /**
   * Refresh the access token using a refresh token
   */
  refreshToken: async (refreshToken: string): Promise<AuthResponse> => {
    const response = await authApi.post<AuthResponse>('/refresh', { refreshToken });
    return response.data;
  },
  
  /**
   * Logout (revoke the refresh token)
   */
  logout: async (refreshToken: string): Promise<void> => {
    await authApi.post('/logout', { refreshToken });
  },
  
  /**
   * Logout from all devices
   */
  logoutAll: async (): Promise<void> => {
    await authApi.post('/logout-all');
  }
};

export default authService;