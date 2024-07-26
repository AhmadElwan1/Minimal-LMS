namespace LiMS.Mocks
{
    public class MockRepository<T>
    {
        private List<T> _data = new();
        private Func<T, int> _getIdFunc; 

        public MockRepository(Func<T, int> getIdFunc)
        {
            _getIdFunc = getIdFunc ?? throw new ArgumentNullException(nameof(getIdFunc));
        }

        public void Add(T entity)
        {
            _data.Add(entity);
        }

        public void Update(T entity)
        {
            var existingEntity = _data.FirstOrDefault(e => _getIdFunc(e).Equals(_getIdFunc(entity)));
            if (existingEntity != null)
            {
                _data.Remove(existingEntity);
                _data.Add(entity);
            }
        }

        public void Delete(int id)
        {
            var entityToRemove = _data.FirstOrDefault(e => _getIdFunc(e).Equals(id));
            if (entityToRemove != null)
            {
                _data.Remove(entityToRemove);
            }
        }

        public List<T> GetAll()
        {
            return _data;
        }

        public T GetById(int id)
        {
            return _data.FirstOrDefault(e => _getIdFunc(e).Equals(id));
        }
    }
}