// 'use client';

// import React, { createContext, useContext, useState, useEffect } from 'react';
// import { useRouter } from 'next/navigation';
// import { authApi } from '../api/auth';
// import { LoginRequest, RegisterRequest, DecodedToken } from '../types/auth';
// import toast from 'react-hot-toast';

// interface AuthContextType {
//   user: DecodedToken | null;
//   isLoading: boolean;
//   login: (data: LoginRequest) => Promise<void>;
//   register: (data: RegisterRequest) => Promise<void>;
//   logout: () => void;
//   isAuthenticated: boolean;
// }

// const AuthContext = createContext<AuthContextType | undefined>(undefined);

// const decodeToken = (token: string): DecodedToken | null => {
//   try {
//     const payload = token.split('.')[1];
//     const decoded = JSON.parse(atob(payload));
//     return {
//       nameid: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
//       email: decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
//       role: decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
//       exp: decoded.exp,
//     };
//   } catch (error) {
//     return null;
//   }
// };

// export function AuthProvider({ children }: { children: React.ReactNode }) {
//   const [user, setUser] = useState<DecodedToken | null>(null);
//   const [isLoading, setIsLoading] = useState(true);
//   const router = useRouter();

//   useEffect(() => {
//     const token = localStorage.getItem('token');
//     if (token) {
//       const decoded = decodeToken(token);
//       if (decoded && decoded.exp * 1000 > Date.now()) {
//         setUser(decoded);
//       } else {
//         localStorage.removeItem('token');
//       }
//     }
//     setIsLoading(false);
//   }, []);

//   const login = async (data: LoginRequest) => {
//     try {
//       const response = await authApi.login(data);
//       localStorage.setItem('token', response.token);
//       const decoded = decodeToken(response.token);
//       setUser(decoded);
//       toast.success('Login successful!');
//       router.push('/dashboard');
//     } catch (error: any) {
//       toast.error(error.response?.data?.error || 'Login failed');
//       throw error;
//     }
//   };

//   const register = async (data: RegisterRequest) => {
//     try {
//       const response = await authApi.register(data);
//       localStorage.setItem('token', response.token);
//       const decoded = decodeToken(response.token);
//       setUser(decoded);
//       toast.success('Registration successful!');
//       router.push('/dashboard');
//     } catch (error: any) {
//       toast.error(error.response?.data?.error || 'Registration failed');
//       throw error;
//     }
//   };

//   const logout = () => {
//     authApi.logout();
//     setUser(null);
//     toast.success('Logged out successfully');
//     router.push('/login');
//   };

//   return (
//     <AuthContext.Provider
//       value={{
//         user,
//         isLoading,
//         login,
//         register,
//         logout,
//         isAuthenticated: !!user,
//       }}
//     >
//       {children}
//     </AuthContext.Provider>
//   );
// }

// export const useAuth = () => {
//   const context = useContext(AuthContext);
//   if (context === undefined) {
//     throw new Error('useAuth must be used within an AuthProvider');
//   }
//   return context;
// };