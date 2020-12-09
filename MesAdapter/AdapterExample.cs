using Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesAdapter
{
    public static class AdapterExample
    {
        //初始化，软件启动时调用
        public static void Init() { }
        //软件关闭时调用
        public static void Close() { }
        public static Result 方法1() { return new Result(); }
        public static Result 方法2() { return new Result(); }
    }
}
