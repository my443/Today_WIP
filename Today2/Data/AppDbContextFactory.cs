using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Today2.Data
{
    public class AppDbContextFactory : IAppDbContextFactory
    {
        public AppDbContext Create(string databasePath)
        {
            return new AppDbContext(databasePath);
        }
    }
}
