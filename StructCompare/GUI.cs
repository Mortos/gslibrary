using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Xml.Linq;

using D3TypeDescriptor;

namespace StructCompare
{
    class ListToLView
    {
        public StructureTypeDescriptor old_item;
        public StructureTypeDescriptor new_item;
        public int old_litem;
        public int new_litem;
        public float compactivility = 0;

        public ListToLView(StructureTypeDescriptor _old_item, StructureTypeDescriptor _new_item, int _old_litem, int _new_litem)
        {
            this.old_item = _old_item;
            this.new_item = _new_item;
            this.old_litem = _old_litem;
            this.new_litem = _new_litem;
        }
    }
    static class GUI
    {
        public static List<ListToLView> listToLView = new List<ListToLView>();
        public static List<TypeDescriptor> basictypes = new List<TypeDescriptor>();

        public static float comparefields(FieldDescriptor[] fields1, FieldDescriptor[] fields2)
        {
            int total = fields1.Length;
            if (total < fields2.Length)
                total = fields2.Length;
            if (total == 0)
                return 1;

            int match = 0;
            for (int i = 0; i < fields1.Length; i++)
            {
                if (i >= fields2.Length) return match/total;
                if (fields1[i].GetFieldSignature() == fields2[i].GetFieldSignature())
                {
                    match += 1;
                }
            }
            return match / total;
        }

        public static void tofieldlist(StructureTypeDescriptor sd1, DataGridView dgv, bool sortbyoffset)
        {
            dgv.Rows.Clear();
            if (sd1 == null)
                return;
            var fields_ = ((StructureTypeDescriptor)sd1).Fields;
            if (sortbyoffset)
            {
                fields_ = ((StructureTypeDescriptor)sd1).Fields.OrderBy(w => { return w.Offset; }).ToArray();

            }
            foreach (var fielditem in fields_)
            {
                if (fielditem == null)
                    continue;
                string[] values = new string[2] {
                    fielditem.Name,
                    fielditem.GetFieldSignature(),
                };

                dgv.Rows.Add(values);
            }
        }

        private static void addtoduallist(Form1 form1, StructureTypeDescriptor sd1, StructureTypeDescriptor sd2)
        {
            int rid1 = form1.dataGridView1.Rows.Add();
            int rid2 = form1.dataGridView2.Rows.Add();
            if (sd1 == null)
            {
                form1.dataGridView1.Rows[rid1].Cells[0].Value = "-";
                form1.dataGridView1.Rows[rid1].Cells[1].Value = "";
            }
            else
            {
                form1.dataGridView1.Rows[rid1].Cells[0].Value = sd1._Name;
                form1.dataGridView1.Rows[rid1].Cells[1].Value = sd1.Fields.Length.ToString();
                form1.dataGridView1.Rows[rid1].Cells[2].Value = sd1.CustomName;
            }

            if (sd2 == null)
            {
                form1.dataGridView2.Rows[rid2].Cells[0].Value = "-";
                form1.dataGridView2.Rows[rid2].Cells[1].Value = "";
            }
            else
            {
                form1.dataGridView2.Rows[rid2].Cells[0].Value = sd2._Name;
                form1.dataGridView2.Rows[rid2].Cells[1].Value = sd2.Fields.Length.ToString();
                form1.dataGridView1.Rows[rid2].Cells[2].Value = sd2.CustomName;
            }

            var ltlv = new ListToLView(sd2, sd1, rid1, rid2);
            if ((sd1 != null) & (sd2 != null))
                ltlv.compactivility = comparefields(sd1.Fields, sd2.Fields);
            listToLView.Add(ltlv);
        }

        public static void transfer_names()
        {
            foreach (var x in listToLView)
            {
                if (x.compactivility == 1)
                {
                    x.new_item.CustomName = x.old_item.CustomName;
                    for (int i = 0; i < x.old_item.Fields.Length; i++)
                    {
                        x.new_item.Fields[i].CustomName = x.old_item.Fields[i].CustomName;
                    }
                }
            }
        }

        public static void Save(bool overrideoriginal)
        {

            XDocument doc1 = new XDocument();
            XElement a1 = new XElement("TypeDescriptors");

            foreach (var x in basictypes)
                a1.Add(x.ToXml());
            foreach (var x in listToLView)
            {
                if (x.new_item != null)
                    a1.Add(x.new_item.ToXml());
            }
            doc1.Add(a1);
            if (overrideoriginal)
            {
                doc1.Save("typedescriptors_new.xml");
            }
            else
            {
                doc1.Save("typedescriptors_updated.xml");
            }
        }

        public static void button1_click(Form1 form1)
        {
            XDocument doc1 = XDocument.Load("typedescriptors_new.xml");
            int protocolHash1;
            var descriptors1 = TypeDescriptor.LoadXml(doc1.Root, out protocolHash1, false);

            basictypes = descriptors1.Where((w1, w2) => { return (w1.IsBasicType); }).ToList();

            descriptors1 = descriptors1.Where((w1, w2) => { return !(w1.IsBasicType); }).ToArray();

            XDocument doc2 = XDocument.Load("typedescriptors_old.xml");
            int protocolHash2;
            var descriptors2 = TypeDescriptor.LoadXml(doc2.Root, out protocolHash2, false).Where((w1, w2) => { return !(w1.IsBasicType); }).ToArray();
            
            Dictionary<int, StructureTypeDescriptor> matches = new Dictionary<int,StructureTypeDescriptor>();
            for (int i = 0; i < descriptors1.Length; i++)
            {
                var samenamed2a = descriptors2
                                .Where((w1,w2)=>{return (w1._Name == descriptors1[i]._Name);} )
                                .ToArray()
                                ;
                var samenamed2 = TypeDescriptor.ToStructures(samenamed2a);
                var sel2 = from al in samenamed2 select comparefields(al.Fields, ((StructureTypeDescriptor)descriptors1[i]).Fields);
                
                var sel3 = sel2.ToArray();
                //samenamed2.OrderBy()
                int bestval = 0;
                for (int j = 0; j < samenamed2.Length; j++)
                {
                    if (sel3[bestval] < sel3[j])
                    {
                        bestval = j;
                    }
                }
                if (sel3.Length == 0)
                    continue;
                
                if (matches.ContainsKey(i))
                        {
                            Console.WriteLine();
                            continue;
                        }
                matches[i] = samenamed2[bestval];
            }

            var nonmatched1 = descriptors1.Where(
                (w1, w2) =>
                {
                    return !matches.ContainsKey(w2);
                }
                    ).ToArray();
            foreach (var sd2a in nonmatched1)
            {
                var sd2 = (StructureTypeDescriptor)sd2a;
                addtoduallist(form1, sd2, null);
            }
            var nonmatched2 = descriptors2.Where(
                (w1, w2) => { 
                    return matches.Count(
                    (w3) => { 
                        return w3.Value.Index == w1.Index; 
                    }
                    ) == 0; 
            }
                    ).ToArray();
            foreach (var sd2a in nonmatched2)
            {
                var sd2 = (StructureTypeDescriptor)sd2a;
                addtoduallist(form1, null, sd2);
            }

            var matches2 = matches.OrderBy((w1) => {
                return comparefields(
                    (descriptors1[w1.Key] as StructureTypeDescriptor).Fields,
                    (w1.Value as StructureTypeDescriptor).Fields
                    );
                });
            foreach (var matchpair in matches2)
            {
                int index1 = matchpair.Key;
                var sd1 = ((StructureTypeDescriptor)descriptors1[index1]);
                var sd2 = matchpair.Value;

                addtoduallist(form1, sd1, sd2);
            }
        }
    }
}
