﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpGL
{
    public class ScissorTestSwitch : EnableSwitch
    {

        public ScissorTestSwitch()
            : base(OpenGL.GL_SCISSOR_TEST, true)
        {
            int x, y, width, height;
            OpenGL.GetViewport(out x, out y, out width, out height);
            this.Init(x, y, width, height);
        }

        public ScissorTestSwitch(bool enableCap)
            : base(OpenGL.GL_SCISSOR_TEST, enableCap)
        {
            int x, y, width, height;
            OpenGL.GetViewport(out x, out y, out width, out height);
            this.Init(x, y, width, height);
        }

        public ScissorTestSwitch(int x, int y, int width, int height)
            : base(OpenGL.GL_SCISSOR_TEST, true)
        {
            this.Init(x, y, width, height);
        }

        public ScissorTestSwitch(int x, int y, int width, int height, bool enableCap)
            : base(OpenGL.GL_SCISSOR_TEST, enableCap)
        {
            this.Init(x, y, width, height);
        }

        private void Init(int x, int y, int width, int height)
        {
            this.X = x; this.Y = y;
            this.Width = width; this.Height = height;
        }

        public override string ToString()
        {
            if (this.EnableCapacity)
            {
                return string.Format("Enable glScissor({0}, {1}, {2}, {3});",
                    X, Y, Width, Height);
            }
            else
            {
                return string.Format("Disable glScissor({0}, {1}, {2}, {3});",
                    X, Y, Width, Height);
            }
        }

        protected override void SwitchOn()
        {
            base.SwitchOn();

            if (this.enableCapacityWhenSwitchOn)
            {
                OpenGL.Scissor(this.X, this.Y, this.Width, this.Height);
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

}
