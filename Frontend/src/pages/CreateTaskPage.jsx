import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/httpClient';
import PageLayout from '../components/PageLayout';

const CreateTaskPage = () => {
  const navigate = useNavigate();
  const [categories, setCategories] = useState([]);
  const [form, setForm] = useState({
    title: '',
    description: '',
    priority: 'Medium',
    categoryId: '',
    assignedToUserId: '',
    dueDate: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const response = await api.get('/api/categories');
        setCategories(response.data || []);
      } catch {
        setCategories([]);
      }
    };

    loadCategories();
  }, []);

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((previous) => ({ ...previous, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);

    try {
      const payload = {
        title: form.title,
        description: form.description,
        priority: form.priority,
        categoryId: form.categoryId || null,
        assignedToUserId: form.assignedToUserId || null,
        dueDate: form.dueDate || null
      };

      const response = await api.post('/api/tasks', payload);
      navigate(`/tasks/${response.data.id}`);
    } catch (submitError) {
      setError(submitError.response?.data?.message || submitError.message || 'Unable to create task.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <PageLayout title="Create Task">
      <form className="card form-card" onSubmit={handleSubmit}>
        {error && <div className="form-error">{error}</div>}

        <label>
          Title
          <input
            type="text"
            name="title"
            value={form.title}
            onChange={handleChange}
            required
          />
        </label>

        <label>
          Description
          <textarea
            name="description"
            rows="4"
            value={form.description}
            onChange={handleChange}
          />
        </label>

        <div className="two-column-grid">
          <label>
            Priority
            <select name="priority" value={form.priority} onChange={handleChange}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </label>

          <label>
            Category
            <select name="categoryId" value={form.categoryId} onChange={handleChange}>
              <option value="">Uncategorized</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div className="two-column-grid">
          <label>
            Assign to user ID
            <input
              type="text"
              name="assignedToUserId"
              value={form.assignedToUserId}
              onChange={handleChange}
              placeholder="Optional"
            />
          </label>

          <label>
            Due date
            <input type="date" name="dueDate" value={form.dueDate} onChange={handleChange} />
          </label>
        </div>

        <button type="submit" className="primary-button" disabled={loading}>
          {loading ? 'Creating...' : 'Create task'}
        </button>
      </form>
    </PageLayout>
  );
};

export default CreateTaskPage;
