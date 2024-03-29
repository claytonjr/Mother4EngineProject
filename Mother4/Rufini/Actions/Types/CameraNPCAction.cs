﻿using System;
using System.Collections.Generic;
using Carbine.Actors;
using Carbine.Graphics;
using Mother4.Actors.NPCs;
using Mother4.Scripts;
using Mother4.Scripts.Actions;

namespace Rufini.Actions.Types
{
	internal class CameraNPCAction : RufiniAction
	{
		public CameraNPCAction()
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
					Name = "spd",
					Type = typeof(float)
				},
				new ActionParam
				{
					Name = "blk",
					Type = typeof(bool)
				}
			};
		}

		public override ActionReturnContext Execute(ExecutionContext context)
		{
			ActionReturnContext result = default(ActionReturnContext);
			string name = base.GetValue<string>("name");
			float value = base.GetValue<float>("spd");
			bool value2 = base.GetValue<bool>("blk");
			if (!string.IsNullOrWhiteSpace(name))
			{
				NPC npc = (NPC)context.ActorManager.Find((Actor x) => x is NPC && ((NPC)x).Name == name);
				if (npc != null)
				{
					ViewManager.Instance.MoveTo(npc, value);
					if (value2)
					{
						this.context = context;
						ViewManager.Instance.OnMoveToComplete += this.OnMoveToComplete;
						result.Wait = ScriptExecutor.WaitType.Event;
					}
				}
			}
			else
			{
				ViewManager.Instance.FollowActor = null;
			}
			return result;
		}

		private void OnMoveToComplete(ViewManager sender)
		{
			ViewManager.Instance.OnMoveToComplete -= this.OnMoveToComplete;
			this.context.Executor.Continue();
		}

		private ExecutionContext context;
	}
}
