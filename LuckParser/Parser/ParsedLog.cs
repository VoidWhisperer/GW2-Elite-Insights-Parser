﻿using System.Collections.Generic;
using System.Linq;
using LuckParser.Exceptions;
using LuckParser.Models.Logic;
using LuckParser.Models.ParseModels;

namespace LuckParser.Parser
{
    public class ParsedLog
    {
        public readonly LogData LogData;
        public readonly FightData FightData;
        public readonly AgentData AgentData;
        public readonly SkillData SkillData;
        public readonly CombatData CombatData;
        public readonly MechanicData MechanicData;
        public readonly List<Player> PlayerList;
        public readonly HashSet<ushort> PlayerIDs;
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public readonly Dictionary<string, List<Player>> PlayerListBySpec;
        public readonly Target LegacyTarget;

        

        public ParsedLog(LogData logData, FightData fightData, AgentData agentData, SkillData skillData, 
                CombatData combatData, List<Player> playerList, Target target)
        {
            LogData = logData;
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            CombatData = combatData;
            PlayerList = playerList;
            LegacyTarget = target;
            MechanicData = new MechanicData(fightData);
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());
            PlayerIDs = new HashSet<ushort>(playerList.Select(x => x.InstID));

            FightData.SetSuccess(this);
            FightData.SetCM(this);
            CombatData.Update(FightData.FightEnd);
            if (FightData.FightDuration <= 2200)
            {
                throw new TooShortException();
            }
            if (Properties.Settings.Default.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
        }
    }
}
