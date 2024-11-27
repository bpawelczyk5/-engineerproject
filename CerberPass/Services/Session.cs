using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CerberPass.Services
{
    public sealed class Session
    {
        private static readonly Lazy<Session> lazy = new Lazy<Session>(() => new Session());
        public static Session Instance { get { return lazy.Value; } }

        private Session() { }

        public string DatabasePath { get; set; }
        public string UserName { get; set; }
    }
}