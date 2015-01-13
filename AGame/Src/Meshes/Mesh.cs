﻿using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

using AGame.Src.ModelFormats;
using AGame.Utils;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

using AGame.Utils;
using AGame.Src.OGL;
using AGame.Src.Meshes;

namespace AGame.Src.Meshes {
	struct Vertex {
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV;

		public Vertex(Vector3 Position, Vector3 Normal, Vector2 UV) {
			this.Position = Position;
			this.Normal = Normal;
			this.UV = UV;
		}
	}

	class Mesh {
		public Vertex[] Verts;
		public List<uint> Inds;

		public Mesh(int Len, bool Transparent = false) {
			Verts = new Vertex[Len];
			Inds = new List<uint>();
			IsTransparent = Transparent;
		}

		/*public void Unroll() {
			Vertex[] Vrts = new Vertex[Inds.Count];
			for (int i = 0; i < Inds.Count; i++)
				Vrts[i] = Verts[Inds[i]];
			Verts = Vrts.Reverse().ToArray();
		}*/

		public VAO MeshVAO;
		public Texture Tex;
		public ShaderProgram Shader;
		public bool IsTransparent;

		public void GLInit(ShaderProgram Shader) {
			//Unroll();
			Inds.Reverse();

			this.Shader = Shader;
			MeshVAO = new VAO(PrimitiveType.Triangles);
			Bind();

			if (Shader.PosAttrib >= 0) {
				VBO Positions = new VBO(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
				Positions.Bind();
				Positions.Data(this.Positions);
				Positions.VertexAttribPointer(Shader.PosAttrib);
			}

			if (Shader.NormAttrib >= 0) {
				VBO Normals = new VBO(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
				Normals.Bind();
				Normals.Data(this.Normals);
				Normals.VertexAttribPointer(Shader.NormAttrib);
			}

			if (Shader.UVAttrib >= 0) {
				VBO UVs = new VBO(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
				UVs.Bind();
				UVs.Data(this.UVs);
				UVs.VertexAttribPointer(Shader.UVAttrib);
			}

			VBO Elements = new VBO(BufferTarget.ElementArrayBuffer, BufferUsageHint.StaticDraw);
			Elements.Bind();
			Elements.Data(Indices);

			Unbind();
		}

		public void Bind() {
			MeshVAO.Bind();
		}

		public void Unbind() {
			MeshVAO.Unbind();
		}

		public void Render() {
			if (Tex != null)
				Tex.Bind();
			Bind();
			Shader.Bind();
			MeshVAO.DrawElements(Inds.Count);
			Shader.Unbind();
			Unbind();
			if (Tex != null)
				Tex.Unbind();
		}

		public Vector3[] Positions {
			get {
				Vector3[] Pos = new Vector3[Verts.Length];
				for (int i = 0; i < Pos.Length; i++)
					Pos[i] = Verts[i].Position;
				return Pos;
			}
		}

		public Vector3[] Normals {
			get {
				Vector3[] Norms = new Vector3[Verts.Length];
				for (int i = 0; i < Norms.Length; i++)
					Norms[i] = Verts[i].Normal;
				return Norms;
			}
		}

		public Vector2[] UVs {
			get {
				Vector2[] UVs = new Vector2[Verts.Length];
				for (int i = 0; i < UVs.Length; i++)
					UVs[i] = Verts[i].UV;
				return UVs;
			}
		}

		public uint[] Indices {
			get {
				return Inds.ToArray();
			}
		}

		public Vertex this[int Idx] {
			get {
				return Verts[Idx];
			}
			set {
				Verts[Idx] = value;
			}
		}
	}
}