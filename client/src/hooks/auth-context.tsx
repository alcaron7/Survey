import React, { createContext, useState, useContext, useEffect, ReactNode } from 'react';
import axios from 'axios';

interface AuthContextType {
  isAuthenticated: boolean;
  token: string | null;
  user: any | null;
  login: (token: string) => void;
  logout: () => void;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<any | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  // Vérifier l'authentification au chargement
  useEffect(() => {
    const checkAuth = async () => {
      const storedToken = localStorage.getItem('token');
      const storedAuth = localStorage.getItem('isAuthenticated');
      
      if (storedToken && storedAuth === 'true') {
        setToken(storedToken);
        setIsAuthenticated(true);
        
        // Configurer l'en-tête Authorization pour toutes les requêtes
        axios.defaults.headers.common['Authorization'] = `Bearer ${storedToken}`;
        
        try {
          // Optionnel: récupérer les infos utilisateur
          const response = await axios.get('/api/user/profile');
          setUser(response.data);
        } catch (error) {
          console.error('Erreur lors de la récupération du profil:', error);
          // Si le token est invalide, déconnecter l'utilisateur
          logout();
        }
      }
      
      setLoading(false);
    };
    
    checkAuth();
  }, []);

  // Fonction de connexion
  const login = (newToken: string) => {
    localStorage.setItem('token', newToken);
    localStorage.setItem('isAuthenticated', 'true');
    
    // Configurer l'en-tête Authorization pour toutes les requêtes
    axios.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
    
    setToken(newToken);
    setIsAuthenticated(true);
  };

  // Fonction de déconnexion
  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('isAuthenticated');
    
    // Supprimer l'en-tête Authorization
    delete axios.defaults.headers.common['Authorization'];
    
    setToken(null);
    setUser(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider
      value={{
        isAuthenticated,
        token,
        user,
        login,
        logout,
        loading
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

// Hook personnalisé pour utiliser le contexte
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth doit être utilisé dans un AuthProvider');
  }
  return context;
};