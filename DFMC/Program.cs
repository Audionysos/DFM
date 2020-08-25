using com.audionysos.data;
using System;
using System.IO;

namespace DFMC
{
    class Program
    {
        static void Main(string[] args)
        {
			var dfm = new DFM();
			dfm.root = "test"; Directory.GetCurrentDirectory();
			dfm.Scan();
        }
    }
}
