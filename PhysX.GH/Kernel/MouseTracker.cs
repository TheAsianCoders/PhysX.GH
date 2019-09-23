using System.Windows.Forms;
using Rhino.Geometry;
using Rhino.UI;


namespace PhysX.GH.Kernel
{
    internal class MouseTracker : MouseCallback
    {
        internal static bool LeftMousePressed;
        internal static Line MouseLine;
        internal static bool JustReleased;

        internal static MouseTracker Instance = new MouseTracker();


        public MouseTracker()
        {
            Enabled = true;
        }


        protected override void OnMouseDown(MouseCallbackEventArgs e)
        {
            if (Control.ModifierKeys != Keys.Alt || e.Button != MouseButtons.Left) return;

            LeftMousePressed = true;

            MouseLine = GetMouseLine(e);
            MouseLine = new Line(MouseLine.To, MouseLine.From);
            e.Cancel = true; // This prevents Rhino from unwantedly going into drag-and-select mode when the left mouse is being pressed while inside the viewport
        }


        protected override void OnMouseUp(MouseCallbackEventArgs e)
        {
            LeftMousePressed = false;
            JustReleased = true;
            e.Cancel = true; // This prevents Rhino from unwantedly going into drag-and-select mode when the left mouse is being pressed while inside the viewport
        }


        protected override void OnMouseMove(MouseCallbackEventArgs e)
        {
            if (Control.ModifierKeys != Keys.Alt) return;

            if (LeftMousePressed)
            {
                MouseLine = GetMouseLine(e);
                MouseLine = new Line(MouseLine.To, MouseLine.From);
            }
            e.Cancel = true; // This prevents Rhino from unwantedly going into drag-and-select mode when the left mouse is being pressed while inside the viewport
        }


        protected override void OnMouseLeave(MouseCallbackEventArgs e)
        {
            LeftMousePressed = false;
        }


        private Line GetMouseLine(MouseCallbackEventArgs e)
        {
            if (!e.View.ActiveViewport.IsParallelProjection)
                return e.View.ActiveViewport.ClientToWorld(e.ViewportPoint);

            Point3d P = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint).From;
            Vector3d v = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint).From - e.View.ActiveViewport.CameraLocation;

            Vector3d camDir = e.View.ActiveViewport.CameraDirection;
            camDir.Unitize();

            Point3d start = P - camDir * (camDir * v);
            Point3d end = start + camDir * 100000.0;

            return new Line(start, end);
        }
    }
}