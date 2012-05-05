using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Mooege.Common.Helpers.IO;

namespace d3mpqs
{
    public static class MPQStorage
    {
        public static string MpqRoot = "";

        public static List<string> MPQList { get; private set; }
        public static SNOPack Data { get; private set; }
        public static bool Initialized { get; private set; }

        public static void Initialize(int requiredVersion)
        {
            Initialized = false;

            if (!Directory.Exists(MpqRoot))
                throw new Exception(string.Format("MPQ root folder does not exist: {0}.", MpqRoot));


            MPQList = FileHelpers.GetFilesByExtensionRecursive(MpqRoot, ".mpq");

            Data = new SNOPack(requiredVersion);
            if (Data.Loaded)
            {
                Data.Init();
                Initialized = true;
            }
        }

        public static string GetMPQFile(string name)
        {
            return MPQList.FirstOrDefault(file => file.Contains(name));
        }
    }

}
