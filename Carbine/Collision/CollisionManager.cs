﻿using System;
using System.Collections.Generic;
using Carbine.Utility;
using SFML.Graphics;
using SFML.System;

namespace Carbine.Collision
{
	public class CollisionManager
	{
		public CollisionManager(int width, int height)
		{
			this.spatialHash = new SpatialHash(width, height);
			this.resultStack = new Stack<ICollidable>(512);
			this.resultList = new List<ICollidable>(4);
		}

		public void Add(ICollidable collidable)
		{
			this.spatialHash.Insert(collidable);
		}

		public void AddAll<T>(ICollection<T> collidables) where T : ICollidable
		{
			foreach (T t in collidables)
			{
				ICollidable collidable = t;
				this.Add(collidable);
			}
		}

		public void Remove(ICollidable collidable)
		{
			this.spatialHash.Remove(collidable);
		}

		public void Update(ICollidable collidable, Vector2f oldPosition, Vector2f newPosition)
		{
			this.spatialHash.Update(collidable, oldPosition, newPosition);
		}

		public PlaceFreeContext PlaceFree(ICollidable obj, Vector2f position)
		{
			PlaceFreeContext result = default(PlaceFreeContext);
			result.PlaceFree = true;
			Vector2f position2 = obj.Position;
			obj.Position = position;
			this.resultList.Clear();
			this.spatialHash.Query(obj, this.resultStack);
			obj.Position = position2;
			while (this.resultStack.Count > 0)
			{
				ICollidable collidable = this.resultStack.Pop();
				if (this.PlaceFreeBroadPhase(obj, position, collidable))
				{
					bool flag = this.CheckPositionCollision(obj, position, collidable);
					if (flag)
					{
						result.PlaceFree = false;
						result.CollidingObject = collidable;
						break;
					}
				}
			}
			this.resultStack.Clear();
			return result;
		}

		public IEnumerable<ICollidable> ObjectsAtPosition(Vector2f position)
		{
			this.resultList.Clear();
			this.spatialHash.Query(position, this.resultStack);
			while (this.resultStack.Count > 0)
			{
				ICollidable collidable = this.resultStack.Pop();
				if (position.X >= collidable.Position.X + collidable.AABB.Position.X && position.X < collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X && position.Y >= collidable.Position.Y + collidable.AABB.Position.Y && position.Y < collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y)
				{
					this.resultList.Add(collidable);
				}
			}
			return this.resultList;
		}

		private bool PlaceFreeBroadPhase(ICollidable objA, Vector2f position, ICollidable objB)
		{
			if (objA == objB)
			{
				return false;
			}
			if (objA.AABB.OnlyPlayer && !objB.AABB.IsPlayer)
			{
				return false;
			}
			if (!objA.Solid || !objB.Solid)
			{
				return false;
			}
			FloatRect floatRect = objA.AABB.GetFloatRect();
			floatRect.Left += position.X;
			floatRect.Top += position.Y;
			FloatRect floatRect2 = objB.AABB.GetFloatRect();
			floatRect2.Left += objB.Position.X;
			floatRect2.Top += objB.Position.Y;
			return floatRect.Intersects(floatRect2);
		}

		private bool CheckPositionCollision(ICollidable objA, Vector2f position, ICollidable objB)
		{
			int count = objA.Mesh.Edges.Count;
			int count2 = objB.Mesh.Edges.Count;
			for (int i = 0; i < count + count2; i++)
			{
				Vector2f vector2f;
				if (i < count)
				{
					vector2f = objA.Mesh.Normals[i];
				}
				else
				{
					vector2f = objB.Mesh.Normals[i - count];
				}
				vector2f = VectorMath.Normalize(vector2f);
				float minA = 0f;
				float minB = 0f;
				float maxA = 0f;
				float maxB = 0f;
				this.ProjectPolygon(vector2f, objA.Mesh, position, ref minA, ref maxA);
				this.ProjectPolygon(vector2f, objB.Mesh, objB.Position, ref minB, ref maxB);
				if (this.IntervalDistance(minA, maxA, minB, maxB) > 0f)
				{
					return false;
				}
			}
			return true;
		}

		private float IntervalDistance(float minA, float maxA, float minB, float maxB)
		{
			if (minA < minB)
			{
				return minB - maxA;
			}
			return minA - maxB;
		}

		private void ProjectPolygon(Vector2f normal, Mesh mesh, Vector2f offset, ref float min, ref float max)
		{
			float num = VectorMath.DotProduct(normal, mesh.Vertices[0] + offset);
			min = num;
			max = num;
			for (int i = 0; i < mesh.Vertices.Count; i++)
			{
				num = VectorMath.DotProduct(mesh.Vertices[i] + offset, normal);
				if (num < min)
				{
					min = num;
				}
				else if (num > max)
				{
					max = num;
				}
			}
		}

		public void Draw(RenderTarget target)
		{
			this.spatialHash.DebugDraw(target);
		}

		private SpatialHash spatialHash;

		private Stack<ICollidable> resultStack;

		private List<ICollidable> resultList;
	}
}