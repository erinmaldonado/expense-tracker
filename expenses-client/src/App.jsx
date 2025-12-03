import { useEffect, useState } from 'react';
import { useAuth } from './useAuth'; // Import our new hook

// Component to handle login form input
const LoginForm = () => {
    const { login, isLoading } = useAuth();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        const success = await login(email, password);
        if (!success) {
            setError('Login failed. Check username and password.');
        }
    };

    return (
        <form onSubmit={handleSubmit} className="p-4 border rounded shadow-sm bg-light">
            <h4>Login to your Tracker</h4>
            <div className="mb-3">
                <label className="form-label">Email</label>
                <input
                    type="email"
                    className="form-control"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    required
                />
            </div>
            <div className="mb-3">
                <label className="form-label">Password</label>
                <input
                    type="password"
                    className="form-control"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                />
            </div>
            {error && <div className="alert alert-danger">{error}</div>}
            <button type="submit" className="btn btn-primary" disabled={isLoading}>
                {isLoading ? 'Logging In...' : 'Log In'}
            </button>
            <p className="mt-3">
                <small>Need an account? Run the `/api/auth/register` endpoint in Swagger first!</small>
            </p>
        </form>
    );
};


// Main App Component
function App() {
  const { token, isAuthenticated, logout, user } = useAuth();
  const [expenses, setExpenses] = useState([])

  useEffect(() => {
    if (isAuthenticated && token) {
      fetch(`${import.meta.env.VITE_API_BASE_URL}/api/expenses`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      })
      .then(response => {
          if (response.status === 401) { throw new Error('Unauthorized'); }
          return response.json();
      })
      .then(data => setExpenses(data))
      .catch(error => console.error('Error fetching data:', error));
    }
  }, [isAuthenticated, token]) 


  return (
    <div className="container mt-5">
      <div className="card shadow-sm">
        <div className="card-header bg-primary text-white d-flex justify-content-between align-items-center">
          <h2 className="mb-0">Expense Tracker</h2>
          
          {isAuthenticated && (
            <div className="d-flex align-items-center gap-3">
              <span>Welcome, {user.name}</span>
              <button className="btn btn-danger btn-sm" onClick={logout}>
                Log Out
              </button>
            </div>
          )}
        </div>

        <div className="card-body">
          {isAuthenticated ? (
             // Table Display Code
             <table className="table table-striped table-hover">
                <thead className="table-dark">
                  {/* ... headers ... */}
                  <tr><th>Date</th><th>Category</th><th>Note</th><th className="text-end">Amount</th></tr>
                </thead>
                <tbody>
                  {expenses.map((expense) => (
                    <tr key={expense.id}>
                      <td>{new Date(expense.date).toLocaleDateString()}</td>
                      <td><span className="badge bg-secondary">{expense.category ? expense.category.name : 'Uncategorized'}</span></td>
                      <td>{expense.note}</td>
                      <td className="text-end fw-bold">${expense.amount}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
          ) : (
            <div className="text-center p-5">
              <LoginForm /> 
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default App