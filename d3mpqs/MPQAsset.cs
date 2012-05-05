using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;
using System.Threading;
using CrystalMpq;

namespace d3mpqs
{
    public class MPQAsset : Asset
    {
        public MpqFile MpqFile { get; set; }

        protected override bool SourceAvailable
        {
            get { return MpqFile != null && MpqFile.Size != 0; }
        }

        public MPQAsset(SNOGroup group, Int32 snoId, string name)
            : base(group, snoId, name)
        {
        }

        public override void RunParser()
        {
            // Use invariant culture so that we don't hit 
            // pitfalls in non en/US systems with different number formats.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            /*
            _data = (FileFormat)Activator.CreateInstance(Parser, new object[] { MpqFile });
            PersistenceManager.LoadPartial(_data, SNOId.ToString());
             */
        }
    }
}
