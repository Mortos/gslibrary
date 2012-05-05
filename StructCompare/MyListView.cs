using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace StructCompare
{
    public class MyListView : System.Windows.Forms.ListView
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLBARINFO
        {
            public int cbSize;
            public Rectangle rcScrollBar;
            public int dxyLineButton;
            public int xyThumbTop;
            public int xyThumbBottom;
            public int reserved;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public int[] rgstate;
        }

        [DllImport("user32.dll", SetLastError = true,
    EntryPoint = "GetScrollBarInfo")]
        private static extern int GetScrollBarInfo(IntPtr hWnd, uint idObject, ref
SCROLLBARINFO psbi);

        const uint OBJID_VSCROLL = 0xFFFFFFFB;

        public delegate void ScrollHandler(object sender, MyScrollEventArgs e);

        public event ScrollHandler Scroll;

        public void OnScroll(MyScrollEventArgs e)
        {

            if (Scroll != null)
            {

                Scroll(this, e);

            }

        }

        protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged (e);
        SCROLLBARINFO psbi = new SCROLLBARINFO();
        psbi.cbSize = Marshal.SizeOf(psbi);

        int nResult = GetScrollBarInfo(this.Handle, OBJID_VSCROLL, ref psbi); //"this" is a scrollbar

        if (nResult == 0)
        {
            int nLatError = Marshal.GetLastWin32Error(); // in kernel32.dll
        }

        fCheck=true;
        oldval=psbi.xyThumbTop;
        Console.WriteLine(psbi.xyThumbTop.ToString());
    }

        bool fCheck = false;
        int oldval = -1;
        IntPtr SB_ENDSCROLL = (IntPtr)8;
        const int WM_VSCROLL = 0x0115;
        const int WM_PAINT = 0xF;
        protected override void WndProc(ref Message m)
    {
            /*
        if(m.Msg==WM_PAINT)
        {
            SCROLLBARINFO psbi = new SCROLLBARINFO();
            psbi.cbSize = Marshal.SizeOf(psbi);

            int nResult = GetScrollBarInfo(this.Handle, OBJID_VSCROLL, ref psbi); //"this" is a scrollbar

            if (nResult == 0)
            {
                int nLatError = Marshal.GetLastWin32Error(); // in kernel32.dll
            }

            Console.WriteLine(psbi.xyThumbTop.ToString());
            if(fCheck&&psbi.xyThumbTop!=oldval)
            {
                fCheck=false;
                MessageBox.Show("WM_VSCROLL for select item");
            }
            else if(psbi.xyThumbTop==oldval)
            {
                fCheck=false;
            }

        }*/
        if(m.Msg==WM_VSCROLL)
        {
            Console.WriteLine("{0} {1} {2}", ((int)m.WParam >> 16), (Int16)m.WParam, m.LParam);
            MyScrollEventArgs ma = new MyScrollEventArgs();
            ma.Orientation = ScrollOrientation.VerticalScroll;
            ma.WParam = m.WParam;
            OnScroll(ma);
        }
        base.WndProc (ref m);
    }
    }

    public class MyScrollEventArgs : EventArgs
    {

        private ScrollOrientation orientation;

        private IntPtr wParam;



        public IntPtr WParam
        {

            get { return wParam; }

            set { wParam = value; }

        }



        public ScrollOrientation Orientation
        {

            get { return orientation; }

            set { orientation = value; }

        }

    }
}
