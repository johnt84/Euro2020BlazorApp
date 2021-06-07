﻿using Euro2020BlazorApp.Models;
using Euro2020BlazorApp.Models.FootballData;
using System.Collections.Generic;
using System.Linq;

namespace Euro2020BlazorApp.Data
{
    public class GroupService
    {
        private readonly FootballDataModel _groupsFootballDataModel;

        const string GROUP_ = "GROUP_";

        public GroupService(FootballDataModel groupsFootballDataModel)
        {
            _groupsFootballDataModel = groupsFootballDataModel;
        }

        public List<Group> GetGroups()
        {
            return _groupsFootballDataModel
                            .standings
                            .ToList()
                            .Select(x => GetGroupFromStanding(x))
                            .ToList();
        }

        private Group GetGroupFromStanding(Standing standing)
        {
            return new Group()
            {
                Name = standing.group.Replace(GROUP_, string.Empty),
                GroupStandings = standing.table.ToList().Select(y => new GroupStanding()
                {
                    Team = new Models.Team()
                    {
                        TeamID = y.team.id,
                        Name = y.team.name,
                        TeamCrestUrl = y.team.crestUrl,
                    },
                    GamesPlayed = y.playedGames,
                    GamesWon = y.won,
                    GamesDrawn = y.draw,
                    GamesLost = y.lost,
                    GoalsScored = y.goalsFor,
                    GoalsAgainst = y.goalsAgainst,
                    GoalDifference = y.goalDifference,
                    PointsTotal = y.points,
                })
                .ToList(),
            };
        }
    }
}
