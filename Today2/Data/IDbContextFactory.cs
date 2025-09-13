using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Today2.Data
{
    public interface IAppDbContextFactory
    {
        AppDbContext Create(string databasePath);
    }
}
