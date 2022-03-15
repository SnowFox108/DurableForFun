namespace FunctionDurable.Infrastructure
{
    public sealed class IdGenerator
    {
        #region singleton

        public IdGenerator()
        {
        }

        public static IdGenerator Instance
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

            internal static readonly IdGenerator NestedInstance = new IdGenerator();
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
