import { BrowserRouter, Routes, Route, Navigate, Outlet } from 'react-router-dom';
import LoginPage from './pages/login/page';
import { SidebarInset, SidebarProvider, SidebarTrigger } from './components/ui/sidebar';
import { AppSidebar } from './components/ui/app-sidebar';
import { Separator } from './components/ui/separator';
import { AuthProvider, useAuth } from './hooks/auth-context';
import DashboardPage from './pages/dashboard/page';

// Layout pour utilisateurs authentifiés
const AuthenticatedLayout = () => {
  const { logout } = useAuth();
  
  return (
    <SidebarProvider>
      <AppSidebar/>
      <SidebarInset>
        <header className="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-[[data-collapsible=icon]]/sidebar-wrapper:h-12">
          <div className="flex items-center gap-2 px-4">
            <SidebarTrigger className="-ml-1" />
            <Separator orientation="vertical" className="mr-2 h-4" />
          </div>
        </header>
        <Outlet />
      </SidebarInset>
    </SidebarProvider>
  );
};

// Layout pour utilisateurs non authentifiés
const UnauthenticatedLayout = () => {
  return (
    <div className="min-h-screen bg-gray-50 flex flex-col justify-center items-center p-4">
      <Outlet />
    </div>
  );
};

// Component pour les routes protégées
const PrivateRoute = () => {
  const { isAuthenticated, loading } = useAuth();
  
  if (loading) {
    return <div className="flex items-center justify-center h-screen">Chargement...</div>;
  }
  
  return isAuthenticated ? <Outlet /> : <Navigate to="/login" replace />;
};

// Component pour les routes publiques
const PublicRoute = () => {
  const { isAuthenticated, loading } = useAuth();
  
  if (loading) {
    return <div className="flex items-center justify-center h-screen">Chargement...</div>;
  }
  
  return !isAuthenticated ? <Outlet /> : <Navigate to="/dashboard" replace />;
};

// Composant pour toutes les routes
const AppRoutes = () => {
  const { isAuthenticated } = useAuth();
  
  return (
    <Routes>
      <Route element={<PublicRoute />}>
        <Route element={<UnauthenticatedLayout />}>
          <Route path="/login" element={<LoginPage />} />
        </Route>
      </Route>

      <Route element={<PrivateRoute />}>
        <Route element={<AuthenticatedLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
        </Route>
      </Route>

      {/* Redirection par défaut */}
      <Route path="*" element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />} />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
};

export default App;