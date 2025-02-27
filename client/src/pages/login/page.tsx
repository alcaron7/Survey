import { LoginForm } from "./components/login-form";
import { useAuth } from "@/hooks/auth-context";

export default function LoginPage() {
  const { login } = useAuth();
  
  return (
    <div className="flex min-h-svh w-full items-center justify-center p-6 md:p-10">
      <div className="w-full max-w-sm">
        <LoginForm onLoginSuccess={login} />
      </div>
    </div>
  )
}