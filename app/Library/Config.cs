using System.Configuration;

namespace Library
{
    public class Config
    {
        public static int RunSqlTimeout
        {
            get
            {
                var configTimeout = ConfigurationManager.AppSettings["RunSqlTimeout"];
                return configTimeout != null ? int.Parse(configTimeout) : 30;
            }
        }
    }
}
