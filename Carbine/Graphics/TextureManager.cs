﻿using System;
using System.Collections.Generic;
using fNbt;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Graphics
{
	public class TextureManager
	{
		public static TextureManager Instance
		{
			get
			{
				return TextureManager.instance;
			}
		}

		private TextureManager()
		{
			this.instances = new Dictionary<int, int>();
			this.textures = new Dictionary<int, ICarbineTexture>();
		}

		private IndexedTexture LoadFromNbtTag(NbtCompound root)
		{
			NbtTag nbtTag = root.Get("pal");
			IEnumerable<NbtTag> enumerable = (nbtTag is NbtList) ? ((NbtList)nbtTag) : ((NbtCompound)nbtTag).Tags;
			uint intValue = (uint)root.Get<NbtInt>("w").IntValue;
			byte[] byteArrayValue = root.Get<NbtByteArray>("img").ByteArrayValue;
			List<int[]> list = new List<int[]>();
			foreach (NbtTag nbtTag2 in enumerable)
			{
				if (nbtTag2.TagType == NbtTagType.IntArray)
				{
					list.Add(((NbtIntArray)nbtTag2).IntArrayValue);
				}
			}
			SpriteDefinition spriteDefinition = null;
			Dictionary<int, SpriteDefinition> dictionary = new Dictionary<int, SpriteDefinition>();
			NbtCompound nbtCompound = root.Get<NbtCompound>("spr");
			if (nbtCompound != null)
			{
				foreach (NbtTag nbtTag3 in nbtCompound.Tags)
				{
					if (nbtTag3 is NbtCompound)
					{
						NbtCompound nbtCompound2 = (NbtCompound)nbtTag3;
						string text = nbtCompound2.Name.ToLowerInvariant();
						NbtIntArray nbtIntArray = null;
						NbtByteArray nbtByteArray = null;
						NbtInt nbtInt = null;
						int[] array = nbtCompound2.TryGet<NbtIntArray>("crd", out nbtIntArray) ? nbtIntArray.IntArrayValue : new int[2];
						int[] array2 = nbtCompound2.TryGet<NbtIntArray>("bnd", out nbtIntArray) ? nbtIntArray.IntArrayValue : new int[2];
						int[] array3 = nbtCompound2.TryGet<NbtIntArray>("org", out nbtIntArray) ? nbtIntArray.IntArrayValue : new int[2];
						byte[] array4 = nbtCompound2.TryGet<NbtByteArray>("opt", out nbtByteArray) ? nbtByteArray.ByteArrayValue : new byte[3];
						IList<NbtTag> list2 = nbtCompound2.Get<NbtList>("spd");
						int frames = nbtCompound2.TryGet<NbtInt>("frm", out nbtInt) ? nbtInt.IntValue : 1;
						NbtIntArray nbtIntArray2 = nbtCompound2.Get<NbtIntArray>("d");
						int[] data = (nbtIntArray2 == null) ? null : nbtIntArray2.IntArrayValue;
						Vector2i coords = new Vector2i(array[0], array[1]);
						Vector2i bounds = new Vector2i(array2[0], array2[1]);
						Vector2f origin = new Vector2f((float)array3[0], (float)array3[1]);
						bool flipX = array4[0] == 1;
						bool flipY = array4[1] == 1;
						int mode = (int)array4[2];
						float[] array5 = (list2 != null) ? new float[list2.Count] : new float[0];
						for (int i = 0; i < array5.Length; i++)
						{
							NbtTag nbtTag4 = list2[i];
							array5[i] = nbtTag4.FloatValue;
						}
						SpriteDefinition spriteDefinition2 = new SpriteDefinition(text, coords, bounds, origin, frames, array5, flipX, flipY, mode, data);
						if (spriteDefinition == null)
						{
							spriteDefinition = spriteDefinition2;
						}
						int hashCode = text.GetHashCode();
						dictionary.Add(hashCode, spriteDefinition2);
					}
				}
			}
			return new IndexedTexture(intValue, list.ToArray(), byteArrayValue, dictionary, spriteDefinition);
		}

		public IndexedTexture Use(string spriteFile)
		{
			int hashCode = spriteFile.GetHashCode();
			IndexedTexture indexedTexture;
			if (!this.textures.ContainsKey(hashCode))
			{
				NbtFile nbtFile = new NbtFile(spriteFile);
				indexedTexture = this.LoadFromNbtTag(nbtFile.RootTag);
				this.instances.Add(hashCode, 1);
				this.textures.Add(hashCode, indexedTexture);
			}
			else
			{
				indexedTexture = (IndexedTexture)this.textures[hashCode];
				Dictionary<int, int> dictionary;
				int key;
				(dictionary = this.instances)[key = hashCode] = dictionary[key] + 1;
			}
			return indexedTexture;
		}

		public IndexedTexture[] UseMultipart(string file)
		{
			NbtFile nbtFile = new NbtFile(file);
			NbtCompound rootTag = nbtFile.RootTag;
			int value = rootTag.Get<NbtInt>("f").Value;
			IndexedTexture[] array = new IndexedTexture[value];
			for (int i = 0; i < value; i++)
			{
				string text = string.Format("{0}-{1}", file, i);
				int hashCode = text.GetHashCode();
				IndexedTexture indexedTexture;
				if (!this.textures.ContainsKey(hashCode))
				{
					string tagName = string.Format("img{0}", i);
					NbtCompound root = rootTag.Get<NbtCompound>(tagName);
					indexedTexture = this.LoadFromNbtTag(root);
					this.instances.Add(hashCode, 1);
					this.textures.Add(hashCode, indexedTexture);
				}
				else
				{
					indexedTexture = (IndexedTexture)this.textures[hashCode];
					Dictionary<int, int> dictionary;
					int key;
					(dictionary = this.instances)[key = hashCode] = dictionary[key] + 1;
				}
				array[i] = indexedTexture;
			}
			return array;
		}

		public FullColorTexture UseUnprocessed(string file)
		{
			int hashCode = file.GetHashCode();
			FullColorTexture fullColorTexture;
			if (!this.textures.ContainsKey(hashCode))
			{
				Image image = new Image(file);
				fullColorTexture = new FullColorTexture(image);
				this.instances.Add(hashCode, 1);
				this.textures.Add(hashCode, fullColorTexture);
			}
			else
			{
				fullColorTexture = (FullColorTexture)this.textures[hashCode];
				Dictionary<int, int> dictionary;
				int key;
				(dictionary = this.instances)[key = hashCode] = dictionary[key] + 1;
			}
			return fullColorTexture;
		}

		public FullColorTexture UseFramebuffer()
		{
			int hashCode = Engine.Frame.GetHashCode();
			RenderStates states = new RenderStates(BlendMode.Alpha, Transform.Identity, Engine.FrameBuffer.Texture, null);
			VertexArray vertexArray = new VertexArray(PrimitiveType.Quads, 4U);
			vertexArray[0U] = new Vertex(new Vector2f(0f, 0f), new Vector2f(0f, 180f));
			vertexArray[1U] = new Vertex(new Vector2f(320f, 0f), new Vector2f(320f, 180f));
			vertexArray[2U] = new Vertex(new Vector2f(320f, 180f), new Vector2f(320f, 0f));
			vertexArray[3U] = new Vertex(new Vector2f(0f, 180f), new Vector2f(0f, 0f));
			RenderTexture renderTexture = new RenderTexture(320U, 180U);
			renderTexture.Clear(Color.Black);
			renderTexture.Draw(vertexArray, states);
			Texture tex = new Texture(renderTexture.Texture);
			renderTexture.Dispose();
			vertexArray.Dispose();
			FullColorTexture fullColorTexture = new FullColorTexture(tex);
			this.instances.Add(hashCode, 1);
			this.textures.Add(hashCode, fullColorTexture);
			return fullColorTexture;
		}

		public void Unuse(ICollection<ICarbineTexture> textures)
		{
			if (textures != null)
			{
				foreach (ICarbineTexture texture in textures)
				{
					this.Unuse(texture);
				}
			}
		}

		public void Unuse(ICarbineTexture texture)
		{
			foreach (KeyValuePair<int, ICarbineTexture> keyValuePair in this.textures)
			{
				int key = keyValuePair.Key;
				ICarbineTexture value = keyValuePair.Value;
				if (value == texture)
				{
					Dictionary<int, int> dictionary;
					int key2;
					(dictionary = this.instances)[key2 = key] = dictionary[key2] - 1;
					break;
				}
			}
		}

		public void Purge()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, ICarbineTexture> keyValuePair in this.textures)
			{
				int key = keyValuePair.Key;
				ICarbineTexture value = keyValuePair.Value;
				if (value != null && this.instances[key] <= 0)
				{
					list.Add(key);
				}
			}
			foreach (int key2 in list)
			{
				this.textures[key2].Dispose();
				this.instances.Remove(key2);
				this.textures.Remove(key2);
			}
		}

		private Dictionary<int, int> instances;

		private Dictionary<int, ICarbineTexture> textures;

		private static TextureManager instance = new TextureManager();
	}
}
