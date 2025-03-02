import React, { createContext, useState, useContext, useEffect, ReactNode, useCallback } from 'react';
import axios from 'axios';
import { jwtDecode } from 'jwt-decode';
import authService from '@/services/auth.service';

interface AuthContextType {
  isAuthenticated: boolean;
  token: string | null;
  user: any | null;
  login: (accessToken: string, refreshToken: string) => void;
  logout: () => Promise<void>;
  loading: boolean;
  isRefreshing: boolean;
}

interface TokenPayload {
  exp: number;
  nameid: string;
  email: string;
  unique_name: string;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [token, setToken] = useState<string | null>(null);
  const [refreshToken, setRefreshToken] = useState<string | null>(null);
  const [user, setUser] = useState<any | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [refreshInProgress, setRefreshInProgress] = useState<boolean>(false);
  const [refreshPromise, setRefreshPromise] = useState<Promise<any> | null>(null);
  const [isRefreshing, setIsRefreshing] = useState<boolean>(false);

  const logoutInternal = useCallback(async () => {
    if (isRefreshing) return;
    
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    
    delete axios.defaults.headers.common['Authorization'];
    
    setToken(null);
    setRefreshToken(null);
    setUser(null);
    setIsAuthenticated(false);
  }, [isRefreshing]);

  const refreshAccessToken = useCallback(async () => {
    if (refreshInProgress && refreshPromise) {
      return refreshPromise;
    }

    setRefreshInProgress(true);
    setIsRefreshing(true);

    const newRefreshPromise = (async () => {
      try {
        if (!refreshToken) {
          return null;
        }
        
        const response = await authService.refreshToken(refreshToken);
        
        if (!response || !response.accessToken || !response.refreshToken) {
          return null;
        }
        
        const { accessToken, refreshToken: newRefreshToken } = response;
        
        localStorage.setItem('accessToken', accessToken);
        localStorage.setItem('refreshToken', newRefreshToken);
        
        setToken(accessToken);
        setRefreshToken(newRefreshToken);
        
        axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
        
        const decoded = jwtDecode<TokenPayload>(accessToken);
        setUser({
          id: decoded.nameid,
          email: decoded.email,
          name: decoded.unique_name
        });
        
        setIsAuthenticated(true);
        
        return {
          accessToken,
          refreshToken: newRefreshToken
        };
      } catch (error) {
        await logoutInternal();
        return null;
      } finally {
        setRefreshInProgress(false);
        setIsRefreshing(false);
      }
    })();

    setRefreshPromise(newRefreshPromise);
    return newRefreshPromise;
  }, [refreshToken, logoutInternal, refreshInProgress, refreshPromise]);
  
  useEffect(() => {
    const requestInterceptor = axios.interceptors.request.use(
      async config => {
        if (config.url?.includes('/api/auth/login') || config.url?.includes('/api/auth/refresh')) {
          return config;
        }
        
        if (!token) {
          return config;
        }
        
        try {
          const decoded = jwtDecode<TokenPayload>(token);
          const currentTime = Date.now() / 1000;
          const timeRemaining = decoded.exp - currentTime;
          
          if (timeRemaining < 30 && refreshToken) {
            const newTokens = await refreshAccessToken();
            if (newTokens) {
              config.headers.Authorization = `Bearer ${newTokens.accessToken}`;
            }
          } else {
            config.headers.Authorization = `Bearer ${token}`;
          }
        } catch (error) {
        }
        
        return config;
      },
      error => Promise.reject(error)
    );
    
    const responseInterceptor = axios.interceptors.response.use(
      response => response,
      async error => {
        const originalRequest = error.config;
        
        if (!originalRequest || originalRequest._retry) {
          return Promise.reject(error);
        }
        
        if (error.response?.status === 401 && refreshToken) {
          originalRequest._retry = true;
          
          try {
            const newTokens = await refreshAccessToken();
            if (newTokens) {
              originalRequest.headers.Authorization = `Bearer ${newTokens.accessToken}`;
              return axios(originalRequest);
            }
          } catch (refreshError) {
          }
        }
        
        return Promise.reject(error);
      }
    );
    
    return () => {
      axios.interceptors.request.eject(requestInterceptor);
      axios.interceptors.response.eject(responseInterceptor);
    };
  }, [token, refreshToken, refreshAccessToken]);
  
  useEffect(() => {
    const checkAuth = async () => {
      const storedToken = localStorage.getItem('accessToken');
      const storedRefreshToken = localStorage.getItem('refreshToken');
      
      if (storedToken && storedRefreshToken) {
        setToken(storedToken);
        setRefreshToken(storedRefreshToken);
        
        try {
          const decoded = jwtDecode<TokenPayload>(storedToken);
          const currentTime = Date.now() / 1000;
          const timeRemaining = decoded.exp - currentTime;
          
          setIsAuthenticated(true);
          
          setUser({
            id: decoded.nameid,
            email: decoded.email,
            name: decoded.unique_name
          });
          
          axios.defaults.headers.common['Authorization'] = `Bearer ${storedToken}`;
          
          if (timeRemaining <= 0) {
            const newTokens = await refreshAccessToken();
            if (!newTokens) {
              await logoutInternal();
            }
          }
        } catch (error) {
          await logoutInternal();
        }
      }
      
      setLoading(false);
    };
    
    checkAuth();
  }, [refreshAccessToken, logoutInternal]);
  
  const login = useCallback((accessToken: string, refreshToken: string) => {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    
    axios.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`;
    
    try {
      const decoded = jwtDecode<TokenPayload>(accessToken);
      setUser({
        id: decoded.nameid,
        email: decoded.email,
        name: decoded.unique_name
      });
    } catch (error) {
    }
    
    setToken(accessToken);
    setRefreshToken(refreshToken);
    setIsAuthenticated(true);
  }, []);
  
  const logout = useCallback(async () => {
    try {
      if (refreshToken) {
        await authService.logout(refreshToken);
      }
    } catch (error) {
    } finally {
      await logoutInternal();
    }
  }, [refreshToken, logoutInternal]);
  
  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        token,
        user,
        login,
        logout,
        loading,
        isRefreshing
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth doit être utilisé dans un AuthProvider');
  }
  return context;
};