import { useState } from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Loader2 } from "lucide-react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { useNavigate } from "react-router-dom";
import authService from "@/services/auth.service";

const formSchema = z.object({
  email: z.string().email({ message: "Adresse courriel invalide" }).min(1, { message: "Adresse courriel requise" }),
  password: z.string().min(1, { message: "Mot de passe requis" }),
});

type FormValues = z.infer<typeof formSchema>;

interface LoginFormProps extends React.ComponentPropsWithoutRef<"div"> {
  onLoginSuccess?: (accessToken: string, refreshToken: string) => void;
}

export function LoginForm({
  className,
  onLoginSuccess,
  ...props
}: LoginFormProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const onSubmit = async (data: FormValues) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const response = await authService.login(data.email, data.password);
      
      if (response && response.accessToken && response.refreshToken) {
        if (onLoginSuccess) {
          onLoginSuccess(response.accessToken, response.refreshToken);
        }
        
        navigate('/dashboard');
      } else {
        setError("Format de réponse invalide");
      }
    } catch (error: any) {
      console.error('Erreur de connexion:', error);
      
      if (error.response) {
        if (error.response.status === 401) {
          setError("Identifiants incorrects");
        } else if (error.response.status === 400) {
          setError("Données invalides");
        } else {
          setError(`Erreur du serveur: ${error.response.status}`);
        }
      } else if (error.request) {
        setError("Impossible de joindre le serveur");
      } else {
        setError(`Erreur: ${error.message}`);
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl">Administration</CardTitle>
          <CardDescription>
            Entrez votre adresse courriel ci-dessous pour vous connecter à votre compte
          </CardDescription>
        </CardHeader>
        <CardContent>
          {error && (
            <div className="mb-4 rounded-md bg-red-50 p-3 text-sm text-red-500">
              {error}
            </div>
          )}
          
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Adresse courriel</FormLabel>
                    <FormControl>
                      <Input 
                        type="email" 
                        placeholder="exemple@domaine.com" 
                        {...field} 
                        disabled={isLoading}
                      />
                    </FormControl>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />
              
              <FormField
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Mot de passe</FormLabel>
                    <FormControl>
                      <Input 
                        type="password" 
                        {...field} 
                        disabled={isLoading}
                      />
                    </FormControl>
                    <FormMessage className="text-red-500" />
                  </FormItem>
                )}
              />
              
              <Button 
                type="submit" 
                className="w-full bg-lime-400 hover:bg-lime-500"
                disabled={isLoading}
              >
                {isLoading ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Chargement...
                  </>
                ) : (
                  "Se connecter"
                )}
              </Button>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  );
}