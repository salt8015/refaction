using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace refactor_me.Tests.Controllers
{
    static class TestHelpers
    {
        public static DbSet<T> SetupDbSet<T>(Mock<DbSet<T>> mockSet, IEnumerable<T> list) where T : class
        {
            var queryable = list.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet.Object;
        }
    }
}
