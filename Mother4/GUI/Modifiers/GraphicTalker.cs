﻿using System;
using Carbine.Graphics;
using Mother4.Data;
using SFML.System;

namespace Mother4.GUI.Modifiers
{
	internal class GraphicTalker : IGraphicModifier, IDisposable
	{
		public bool Done
		{
			get
			{
				return this.done;
			}
		}

		public Graphic Graphic
		{
			get
			{
				return this.graphic;
			}
		}

		public GraphicTalker(RenderPipeline pipeline, Graphic graphic)
		{
			this.pipeline = pipeline;
			this.graphic = graphic;
			this.done = false;
			this.rightOffset = new Vector2f((float)this.graphic.TextureRect.Width, 0f);
			this.talker = new IndexedColorGraphic(Paths.GRAPHICS + "chat.dat", "left", this.graphic.Position - this.graphic.Origin, this.graphic.Depth + 1);
			this.talker.OnAnimationComplete += this.AnimationComplete;
			this.pipeline.Add(this.talker);
		}

		~GraphicTalker()
		{
			this.Dispose(false);
		}

		private void AnimationComplete(Graphic graphic)
		{
			this.count++;
			if (this.count > 2)
			{
				this.right = !this.right;
				this.talker.SetSprite(this.right ? "right" : "left", true);
				this.count = 0;
				this.Update();
			}
		}

		public void Update()
		{
			if (this.right)
			{
				this.talker.Position = this.graphic.Position - this.graphic.Origin + this.rightOffset;
				return;
			}
			this.talker.Position = this.graphic.Position - this.graphic.Origin;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.pipeline.Remove(this.talker);
					this.talker.Dispose();
				}
				this.disposed = true;
			}
		}

		private const int LOOP_TOTAL = 2;

		private bool disposed;

		private RenderPipeline pipeline;

		private Graphic graphic;

		private IndexedColorGraphic talker;

		private Vector2f rightOffset;

		private int count;

		private bool right;

		private bool done;
	}
}
