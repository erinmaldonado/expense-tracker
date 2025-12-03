import { useState, useEffect, useContext, createContext } from 'react';

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(localStorage.getItem('jwt_token') || null);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        if (token) {
            localStorage.setItem('jwt_token', token);
            // In a real app, you would decode the token to get the user data
            // For now, we assume a user is logged in if a token exists
            setUser({ name: 'Authenticated User' }); 
        } else {
            localStorage.removeItem('jwt_token');
            setUser(null);
        }
    }, [token]);

    const login = async (email, password) => {
        setIsLoading(true);
        try {
            // HITS YOUR ASP.NET CORE IDENTITY ENDPOINT
            const response = await fetch('http://localhost:5155/api/Auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });

            if (!response.ok) {
                // Handle API error (e.g., wrong password)
                const errorData = await response.json();
                throw new Error(errorData.message || 'Login failed');
            }

            const data = await response.json();
            setToken(data.token); // Store the returned JWT
            return true;
        } catch (error) {
            console.error("Login Error:", error);
            return false;
        } finally {
            setIsLoading(false);
        }
    };

    const logout = () => {
        setToken(null);
        setUser(null);
    };

    const value = { token, user, isLoading, isAuthenticated: !!user, login, logout };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
    return useContext(AuthContext);
};