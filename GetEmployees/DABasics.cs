using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetEmployees
{
    class DABasics
    {
    }
    public class DAGetList
    {
        public static List<T> GetListFromCommand<T>(SqlCommand sqlcmd)
        where T : ICreatable, new()
        {
            List<T> results = new List<T>();
            using (sqlcmd.Connection)
            {
                sqlcmd.Connection.Open();
                SqlDataReader sdr = sqlcmd.ExecuteReader();
                while (sdr.Read())
                {
                    T newthing = new T();
                    newthing.Create(sdr);
                    results.Add(newthing);
                }
            }
            return results;
        }
    }
    public interface ICreatable
    {
        void Create(SqlDataReader reader);
    }
}
