using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AppNiZiAPI.Models.Repositories
{
    public abstract class Repository
    {
        public SqlConnection conn = new SqlConnection(Environment.GetEnvironmentVariable("sqldb_connection"));

    }
}
