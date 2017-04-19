using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDG.Framework.DAL;

namespace FrameworkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new BizData())
            {
                var items=db.Roles.Find();
            } 
        }
    }
}
