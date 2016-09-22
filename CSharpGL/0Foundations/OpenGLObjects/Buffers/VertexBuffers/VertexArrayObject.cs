﻿using System;
using System.ComponentModel;

namespace CSharpGL
{
    /// <summary>
    /// VAO是用来管理VBO的。可以进一步减少DrawCall。
    /// <para>VAO is used to reduce draw-call.</para>
    /// </summary>
    public sealed class VertexArrayObject : IDisposable
    {
        private const string strVertexArrayObject = "Vertex Array Object";

        /// <summary>
        /// vertex attribute buffers('in vec3 position;' in shader etc.)
        /// </summary>
        [Category(strVertexArrayObject)]
        [Description("vertex attribute buffers('in vec3 position;' in shader etc.)")]
        public VertexAttributeBufferPtr[] VertexAttributeBufferPtrs { get; private set; }

        /// <summary>
        /// The one and only one index buffer used to indexing vertex attribute buffers.
        /// </summary>
        [Category(strVertexArrayObject)]
        [Description("The one and only one index buffer used to indexing vertex attribute buffers.)")]
        public IndexBufferPtr IndexBufferPtr { get; private set; }

        private uint[] ids = new uint[1];

        /// <summary>
        /// 此VAO的ID，由OpenGL给出。
        /// <para>Id generated by glGenVertexArrays().</para>
        /// </summary>
        [Category(strVertexArrayObject)]
        [Description("Id generated by glGenVertexArrays().")]
        public uint Id { get { return ids[0]; } }

        private static OpenGL.glGenVertexArrays glGenVertexArrays;
        private static OpenGL.glBindVertexArray glBindVertexArray;
        private static OpenGL.glDeleteVertexArrays glDeleteVertexArrays;

        /// <summary>
        /// VAO是用来管理VBO的。可以进一步减少DrawCall。
        /// <para>VAO is used to reduce draw-call.</para>
        /// </summary>
        /// <param name="indexBufferPtr">index buffer pointer that used to invoke draw command.</param>
        /// <param name="vertexAttributeBufferPtrs">给出此VAO要管理的所有VBO。<para>All VBOs that are managed by this VAO.</para></param>
        public VertexArrayObject(IndexBufferPtr indexBufferPtr, params VertexAttributeBufferPtr[] vertexAttributeBufferPtrs)
        {
            if (indexBufferPtr == null)
            {
                throw new ArgumentNullException("indexBufferPtr");
            }
            // Zero vertex attribute is allowed in GLSL.
            //if (vertexAttributeBufferPtrs == null || vertexAttributeBufferPtrs.Length == 0)
            //{
            //    throw new ArgumentNullException("vertexAttributeBufferPtrs");
            //}

            this.IndexBufferPtr = indexBufferPtr;
            this.VertexAttributeBufferPtrs = vertexAttributeBufferPtrs;
        }

        /// <summary>
        /// 在OpenGL中创建VAO。
        /// 创建的过程就是执行一次渲染的过程。
        /// <para>Creates VAO and bind it to specified VBOs.</para>
        /// <para>The whole process of binding is also the process of rendering.</para>
        /// </summary>
        /// <param name="shaderProgram"></param>
        public void Create(ShaderProgram shaderProgram)
        {
            if (this.Id != 0)
            { throw new Exception(string.Format("Id[{0}] is already generated!", this.Id)); }

            if (glGenVertexArrays == null)
            {
                glGenVertexArrays = OpenGL.GetDelegateFor<OpenGL.glGenVertexArrays>();
                glBindVertexArray = OpenGL.GetDelegateFor<OpenGL.glBindVertexArray>();
                glDeleteVertexArrays = OpenGL.GetDelegateFor<OpenGL.glDeleteVertexArrays>();
            }

            glGenVertexArrays(1, ids);

            this.Bind();// this vertex array object will record all stand-by actions.
            VertexAttributeBufferPtr[] vertexAttributeBufferPtrs = this.VertexAttributeBufferPtrs;
            if (vertexAttributeBufferPtrs != null)
            {
                foreach (var item in vertexAttributeBufferPtrs)
                {
                    item.Standby(shaderProgram);
                }
            }
            this.Unbind();// this vertex array object has recorded all stand-by actions.
        }

        private void Bind()
        {
            glBindVertexArray(this.Id);
        }

        private void Unbind()
        {
            glBindVertexArray(0);
        }

        /// <summary>
        /// 执行一次渲染的过程。
        /// <para>Execute rendering command.</para>
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="shaderProgram"></param>
        /// <param name="temporaryIndexBufferPtr">render by a temporary index buffer</param>
        public void Render(RenderEventArgs arg, ShaderProgram shaderProgram, IndexBufferPtr temporaryIndexBufferPtr = null)
        {
            if (temporaryIndexBufferPtr != null)
            {
                this.Bind();
                temporaryIndexBufferPtr.Render(arg);
                this.Unbind();
            }
            else
            {
                this.Bind();
                this.IndexBufferPtr.Render(arg);
                this.Unbind();
            }
        }

        /// <summary>
        ///
        /// </summary>
        public override string ToString()
        {
            return string.Format("VAO Id: {0}", this.Id);
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///
        /// </summary>
        ~VertexArrayObject()
        {
            this.Dispose(false);
        }

        private bool disposedValue;

        private void Dispose(bool disposing)
        {
            if (this.disposedValue == false)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Dispose unmanaged resources.
                IntPtr ptr = Win32.wglGetCurrentContext();
                if (ptr != IntPtr.Zero)
                {
                    {
                        glDeleteVertexArrays(1, this.ids);
                        this.ids[0] = 0;
                    }
                    {
                        VertexAttributeBufferPtr[] vertexAttributeBufferPtrs = this.VertexAttributeBufferPtrs;
                        if (vertexAttributeBufferPtrs != null)
                        {
                            foreach (var item in vertexAttributeBufferPtrs)
                            {
                                item.Dispose();
                            }
                        }
                    }
                    {
                        IndexBufferPtr indexBufferPtr = this.IndexBufferPtr;
                        indexBufferPtr.Dispose();
                    }
                }
            }

            this.disposedValue = true;
        }
    }
}