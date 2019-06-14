using System;
//using PinvokeGen.Managed;
using PinvokeGen.Native;

namespace PinvokeGen.Cli
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var parser = new CParser();
            CModel model = parser.Parse();

//            var csEmitter = new CSharpEmitter(model);
//            string cs = csEmitter.Emit();
//            Console.WriteLine(cs);
//            Console.WriteLine("/**************/\n");

            var cEmitter = new CEmitter(model);
            string c = cEmitter.Emit();
            Console.WriteLine(c);
        }
    }
}
