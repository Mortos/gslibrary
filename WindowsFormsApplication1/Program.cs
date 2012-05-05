using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using CrystalMpq;
using Gibbed.IO;

namespace WindowsFormsApplication1
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            d3mpqs.MPQStorage.MpqRoot = "S:/D3/egris/src/Mooege/bin/Debug/Assets/MPQ/MPQs";
            d3mpqs.MPQStorage.Initialize(9359);
            var scene = d3mpqs.MPQStorage.Data.Assets[d3mpqs.SNOGroup.Scene][0x00008245];
            var file = (scene as d3mpqs.MPQAsset).MpqFile.Open();
            var _scene = new gslibrary.packets.Scene();
            _scene.FileRead(file, 16);

            foreach (var lworld in d3mpqs.MPQStorage.Data.Assets[d3mpqs.SNOGroup.Worlds])
            {
                Console.WriteLine("{0} {1}", lworld.Value.SNOId, lworld.Value.Name);
            }
            var quest = d3mpqs.MPQStorage.Data.Assets[d3mpqs.SNOGroup.Quest];
            foreach (var lquest in quest)
            {
                Console.WriteLine("{0} {1} ", lquest.Value.SNOId, lquest.Value.Name);
            }
            var yquest = d3mpqs.MPQStorage.Data.Assets[d3mpqs.SNOGroup.Quest][87700];
            var qfile = (yquest as d3mpqs.MPQAsset).MpqFile.Open();
            var _quest = new gslibrary.packets.Quest();
            _quest.FileRead(qfile, 16);
            var afile = new System.IO.FileStream("file.txt", System.IO.FileMode.Create);
            afile.WriteString(_quest.AsText());
            afile.Close();
            Console.WriteLine("-------------");
            foreach (var lquest in d3mpqs.MPQStorage.Data.Assets[d3mpqs.SNOGroup.GameBalance])
            {
                Console.WriteLine("{0} {1} ", lquest.Value.SNOId, lquest.Value.Name);
                try
                {
                    var gbfile = (lquest.Value as d3mpqs.MPQAsset).MpqFile.Open();
                    var _gb = new gslibrary.packets.GameBalance();
                    _gb.FileRead(gbfile, 16);
                    var bfile = new System.IO.FileStream(lquest.Value.Name + ".gam.txt", System.IO.FileMode.Create);
                    bfile.WriteString(_gb.AsText());
                    bfile.Close();
                }
                catch
                {
                    Console.WriteLine("failed");
                }
            }


            /*var file = (scene as d3mpqs.MPQAsset).MpqFile.Open();
            var _scene = new gslibrary.packets.Scene();
            _scene.FileRead(file, 16);
            */
            Form1.map = new System.Drawing.Bitmap(96, 96);
            for (int y = 0; y < 96; y++)
            {
                for (int x = 0; x < 96; x++)
                {
                    var color = System.Drawing.Color.Black;
                    if ((_scene.Field5.Squares[x + y * 96].Flags & 1) == 1)
                        color = System.Drawing.Color.Red;
                    Form1.map.SetPixel(x, y, color);
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
