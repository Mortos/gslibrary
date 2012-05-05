using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using gslibrary;
using CrystalMpq;

namespace d3mpqs.Assets
{
    public class Scene : Asset
    {
        public gslibrary.packets.Scene data = null;

        public Scene(SNOGroup snoGroup, int snoId, string name)
            : base(snoGroup, snoId, name)
        {
            
        }

        public override void RunParser()
        {
            /*MpqFile file
            var stream = file.Open();
            data = new gslibrary.packets.Scene();
            data.FileRead(stream,16);
            stream.Close();
             */
        }

        protected override bool SourceAvailable { get {return true;} }
    }
}
