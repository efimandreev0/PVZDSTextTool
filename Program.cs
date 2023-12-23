using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace PVZNDS
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Contains(".txt"))
            {
                Rebuild(args[0]);
            }
            else
            {
                Extract(args[0], args[1]);
            }
        }
        public static void Extract(string toc, string txt)
        {
            var tocReader = new BinaryReader(File.OpenRead(toc));
            var arcReader = new BinaryReader(File.OpenRead(txt));
            List<int> pointers = new List<int>();
            while (tocReader.BaseStream.Position <= tocReader.BaseStream.Length - 1)
            {
                pointers.Add(tocReader.ReadInt32());
            }
            string[] allText = new string[pointers.Count];
            
            for (int i = 0; i < pointers.Count; i++)
            {
                arcReader.BaseStream.Position = pointers[i];
                allText[i] = Utils.ReadString(arcReader, Encoding.UTF8);
                allText[i] = allText[i].Replace("\n", "<lf>").Replace("\r", "<br>");
            }
            File.WriteAllLines(txt + ".txt", allText);
        }
        public static void Rebuild(string textFile)
        {
            string[] strs = File.ReadAllLines(textFile);
            int[] pointers = new int[strs.Length];
            using (BinaryWriter tocWriter = new BinaryWriter(File.Create(textFile.Replace(".txt", "") + "_pointers.dat")))
            using (BinaryWriter arcWriter = new BinaryWriter(File.Create(textFile.Replace(".txt", "") + ".dat")))
            {
                for (int i = 0; i < strs.Length; i++)
                {
                    pointers[i] = (int)arcWriter.BaseStream.Position;
                    strs[i] = strs[i].Replace("<lf>", "\n").Replace("<br>","\r");
                    arcWriter.Write(Encoding.UTF8.GetBytes(strs[i]));
                    arcWriter.Write(new byte());
                    tocWriter.Write(pointers[i]);
                }
            }
        }
    }
}
