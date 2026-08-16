import { Link, NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const PageLayout = ({ title, children }) => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  return (
    <div className="app-shell">
      <header className="app-header">
        <div>
          <Link className="brand" to="/">
            TaskMaster
          </Link>
        </div>
        <nav>
          <NavLink to="/">Home</NavLink>
          {isAuthenticated && <NavLink to="/tasks">Tasks</NavLink>}
          {isAuthenticated && <NavLink to="/tasks/mine">My Tasks</NavLink>}
          {isAuthenticated && <NavLink to="/tasks/assigned">Assigned</NavLink>}
          {isAuthenticated && <NavLink to="/profile">Profile</NavLink>}
          {isAuthenticated && <NavLink to="/sessions">Sessions</NavLink>}
          {user?.roles?.includes('Admin') && <NavLink to="/users">Users</NavLink>}
          {user?.roles?.includes('Admin') && <NavLink to="/users/roles">Roles</NavLink>}
          {user?.roles?.includes('Admin') && <NavLink to="/categories">Categories</NavLink>}
          {!isAuthenticated && <NavLink to="/login">Login</NavLink>}
          {!isAuthenticated && <NavLink to="/register">Register</NavLink>}
          {isAuthenticated && (
            <button className="nav-button" onClick={handleLogout}>
              Logout
            </button>
          )}
        </nav>
      </header>
      <main className="app-content">
        <div className="page-header">
          <h1>{title}</h1>
        </div>
        {children}
      </main>
      <footer className="app-footer">
        <p>Secure task management powered by JWT auth and role-aware APIs.</p>
      </footer>
    </div>
  );
};

export default PageLayout;
