namespace FenicsDispatcher.Infrastructure
{
    public sealed class DataContext
    {
        #region singleton

        public DataContext()
        {
        }

        public static DataContext Instance
        {
            get
            {
                return Nested.NestedInstance;
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            internal static readonly DataContext NestedInstance = new DataContext();
        }

        #endregion

        private int _id = 0;

        public int NextId()
        {
            _id++;
            return _id;
        }

    }
}
