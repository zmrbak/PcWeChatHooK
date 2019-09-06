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
            WxRoomList wxRoomList = new WxRoomList();
            wxRoomList.GetData();

            Console.ReadLine();
        }
    }
}
