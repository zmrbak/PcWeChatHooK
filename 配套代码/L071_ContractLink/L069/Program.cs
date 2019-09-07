using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L069
{
    class Program
    {
        static void Main(string[] args)
        {
            WxContractList wxContractList = new WxContractList();
            wxContractList.GetData();

            Console.ReadLine();
        }
    }
}
