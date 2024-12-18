using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CerberPass.Services
{
    public sealed class Session
    {
        
        private static Session _instance;
        public static Session Instance => _instance ?? (_instance = new Session());

        private Session() { }

        public string DatabasePath { get; set; }
        public string OriginalDatabasePath { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }
}