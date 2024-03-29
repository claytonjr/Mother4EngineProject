﻿using System;
using System.Collections.Generic;
using System.Linq;
using Carbine.Maps;
using Carbine.Utility;
using Mother4.Actors.NPCs;
using Mother4.Scripts;
using Mother4.Scripts.Actions;
using Mother4.Scripts.Actions.ParamTypes;

namespace Rufini.Actions.Types
{
	internal class EntityAddAction : RufiniAction
	{
		public EntityAddAction()
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
					Name = "spr",
					Type = typeof(string)
				},
				new ActionParam
				{
					Name = "sub",
					Type = typeof(string)
				},
				new ActionParam
				{
					Name = "dir",
					Type = typeof(RufiniOption)
				},
				new ActionParam
				{
					Name = "x",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "y",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "mov",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "spd",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "dst",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "dly",
					Type = typeof(int)
				},
				new ActionParam
				{
					Name = "cnstr",
					Type = typeof(string)
				},
				new ActionParam
				{
					Name = "cls",
					Type = typeof(bool)
				},
				new ActionParam
				{
					Name = "shdw",
					Type = typeof(bool)
				},
				new ActionParam
				{
					Name = "txt",
					Type = typeof(string)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			string value = base.GetValue<string>("txt");
			List<Map.NPCtext> text = null;
			if (value != null && value.Length > 0)
			{
				Map.NPCtext item = default(Map.NPCtext);
				item.ID = value;
				item.Flag = 0;
				text = new List<Map.NPCtext>
				{
					item
				};
			}
			Map.NPC data = new Map.NPC
			{
				Name = base.GetValue<string>("name"),
				Sprite = base.GetValue<string>("spr"),
				Direction = (short)base.GetValue<RufiniOption>("dir").Option,
				X = base.GetValue<int>("x"),
				Y = base.GetValue<int>("y"),
				Mode = (short)base.GetValue<int>("mov"),
				Speed = (float)base.GetValue<int>("spd"),
				Distance = (short)base.GetValue<int>("dst"),
				Delay = (short)base.GetValue<int>("dly"),
				Constraint = base.GetValue<string>("cnstr"),
				Solid = base.GetValue<bool>("cls"),
				Shadow = base.GetValue<bool>("shdw"),
				DepthOverride = int.MinValue,
				Text = text
			};
			object moverData = null;
			switch (data.Mode)
			{
			case 4:
				moverData = context.Paths.FirstOrDefault((Map.Path n) => n.Name == data.Constraint);
				break;
			case 5:
				moverData = context.Areas.FirstOrDefault((Map.Area n) => n.Name == data.Constraint);
				break;
			}
			NPC npc = new NPC(context.Pipeline, context.CollisionManager, data, moverData);
			string value2 = base.GetValue<string>("sub");
			if (!string.IsNullOrWhiteSpace(value2))
			{
				npc.OverrideSubsprite(value2);
			}
			context.ActorManager.Add(npc);
			this.context = context;
			this.timerId = TimerManager.Instance.StartTimer(0);
			TimerManager.Instance.OnTimerEnd += this.OnTimerEnd;
			return new ActionReturnContext
			{
				Wait = ScriptExecutor.WaitType.Event
			};
		}

		private void OnTimerEnd(int timerIndex)
		{
			if (this.timerId == timerIndex)
			{
				this.context.Executor.Continue();
				TimerManager.Instance.OnTimerEnd -= this.OnTimerEnd;
			}
		}

		private ExecutionContext context;

		private int timerId;
	}
}
