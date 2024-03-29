﻿using System;
using System.Collections.Generic;
using System.Linq;
using Carbine.Actors;
using Carbine.Maps;
using Mother4.Actors.NPCs;
using Mother4.Scripts;
using Mother4.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class EntityMoveModeAction : RufiniAction
	{
		public EntityMoveModeAction()
		{
			this.paramList = new List<ActionParam>
			{
				new ActionParam
				{
					Name = "name",
					Type = typeof(string)
				},
				new ActionParam
				{
					Name = "mode",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "cnstr",
					Type = typeof(string)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			ActionReturnContext result = default(ActionReturnContext);
			string name = base.GetValue<string>("name");
			int value = base.GetValue<int>("mode");
			string constraintName = base.GetValue<string>("cnstr");
			NPC.MoveMode moveMode = (NPC.MoveMode)value;
			NPC npc = (NPC)context.ActorManager.Find((Actor n) => n is NPC && ((NPC)n).Name == name);
			if (npc != null)
			{
				object moverData = null;
				switch (moveMode)
				{
				case NPC.MoveMode.Path:
					moverData = context.Paths.FirstOrDefault((Map.Path n) => n.Name == constraintName);
					break;
				case NPC.MoveMode.Area:
					moverData = context.Areas.FirstOrDefault((Map.Area n) => n.Name == constraintName);
					break;
				}
				npc.SetMoveMode(moveMode, moverData);
			}
			return result;
		}
	}
}
