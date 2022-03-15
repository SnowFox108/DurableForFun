using System.Collections.Generic;

namespace PdfGenerator.Infrastructure
{
    public sealed class DataContext
    {
        #region singleton

        public DataContext()
        {
            pdfTaskEntities = new List<PdfTaskEntity>();
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

        private readonly List<PdfTaskEntity> pdfTaskEntities;

        public List<PdfTaskEntity> PdfTaskEntities
        {
            get
            {
                return pdfTaskEntities;
            }
        }

    }
}
