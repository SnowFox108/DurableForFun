using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FenicsDispatcher.Infrastructure
{
    public sealed class DataContext
    {
        #region singleton

        public DataContext()
        {
            fenicsEntities = new List<FenicsEntity>();
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

        private readonly List<FenicsEntity> fenicsEntities;
        private int _id = 0;

        public List<FenicsEntity> FenicsEntities
        {
            get
            {
                return fenicsEntities;
            }
        }

        public int NextId()
        {
            _id++;
            return _id;
        }

    }
}
