﻿using System;
using System.Collections.Generic;
using Carbine;
using Mother4.Battle.Actions;
using Mother4.Battle.Combatants;
using Mother4.Data;

namespace Mother4.Battle.EnemyAI
{
	internal class TravisMustDieAI : IEnemyAI
	{
		public TravisMustDieAI(BattleController controller, Combatant sender)
		{
			this.controller = controller;
			this.sender = sender;
			this.battleActionParams = EnemyBattleActions.GetBattleActionParams((sender as EnemyCombatant).Enemy);
		}

		public BattleAction GetAction(int priority, Combatant[] potentialTargets)
		{

			ActionParams aparams = this.battleActionParams[Engine.Random.Next(this.battleActionParams.Count)];
			aparams.controller = this.controller;
			aparams.sender = this.sender;
			aparams.priority = this.sender.Stats.Speed;
			
			Combatant combatant = null;
			List<Combatant> possibleTargets = null;


			if (!(aparams.actionType == typeof(DisablePSI)))
			{
				Console.WriteLine("Choosing to fuck up TRAVIS.");
				foreach (Combatant combatant2 in potentialTargets)
				{
					if (combatant2.Faction == BattleFaction.PlayerTeam)
					{
						PlayerCombatant playerCombatant = combatant2 as PlayerCombatant;
						if (playerCombatant.Character == CharacterType.Travis)
						{
							combatant = playerCombatant;
							break;
						}
					}
				}
			}
			else {
				Console.WriteLine("Choosing to fuck up PSI.");
				possibleTargets = new List<Combatant>();
				foreach (Combatant combatant2 in potentialTargets)
				{
					if (combatant2.Faction == BattleFaction.PlayerTeam)
					{
						PlayerCombatant playerCombatant = combatant2 as PlayerCombatant;
						if (playerCombatant.Character != CharacterType.Travis)
						{
							possibleTargets.Add(playerCombatant);
						}
					}
				}
			}
			
			Combatant[] targets = new Combatant[]
			{
				(combatant != null) ? combatant : possibleTargets[Engine.Random.Next(possibleTargets.Count)]
			};
			aparams.targets = targets;

			return BattleAction.GetInstance(aparams);
		}

		private List<ActionParams> battleActionParams;

		private BattleController controller;

		private Combatant sender;
	}
}
