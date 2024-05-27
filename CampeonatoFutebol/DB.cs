using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampeonatoFutebol
{
    internal class DB
    {
        string conection;

        public DB()
        {
            conection += "Data Source=127.0.0.1;"; // Server
            conection += "Initial Catalog=CampeonatoFutebol;"; //DataBase
            conection += " User Id=sa; Password=SqlServer2019!;";//User and Password
            conection += "TrustServerCertificate=Yes;";//certificate

        }

        public string Path()
        {
            return conection;   
        }
    }
}
