﻿using FootballEngine.Logic.Interfaces;
using FootballShared.Models;
using FootballShared.Models.FootballData;
using static FootballShared.Models.Enums.Enums;

namespace FootballEngine.Services
{
    public class TeamLogic : ITeamLogic
    {
        private readonly Teams _teamsFootballDataModel;
        private readonly FootballShared.Models.FootballData.Team _teamFootballDataModel;

        public TeamLogic(Teams teamsFootballDataModel)
        {
            _teamsFootballDataModel = teamsFootballDataModel;
        }

        public TeamLogic(FootballShared.Models.FootballData.Team teamFootballDataModel)
        {
            _teamFootballDataModel = teamFootballDataModel;
        }

        public List<FootballShared.Models.Team> GetTeams()
        {
            var teams =  _teamsFootballDataModel
                        .teams
                        .ToList()
                        .Select(x => new FootballShared.Models.Team()
                        {
                            TeamID = x.id,
                            Name = x.shortName,
                            TeamCrestUrl = x.crest,
                            YearFounded = x.founded,
                            Website = x.website,
                            TeamColours = x.clubColors,
                            HomeStadium = x.venue,
                            Squad = x.squad
                                    .ToList()
                                    .Select(y => GetPlayerFromSquad(y, x.id))
                                    .ToList(),
                            Coach = new FootballShared.Models.Coach() { 
                                CoachID = x.coach.id,
                                Name = x.coach.name,
                                DateOfBirth = x.coach.dateOfBirth,
                                Nationality = x.coach.nationality,
                                Age = CalculateAge(x.coach.dateOfBirth),
                                TeamID = x.id,
                            },
                        })
                        .OrderBy(x => x.Name)
                        .ToList();

            teams.ForEach(x => x.SquadByPosition = GetPlayersByPosition(x.Squad));

            return teams;
        }

        private List<PlayerByPosition> GetPlayersByPosition(List<Player> squad)
        {
            return squad
                    .Where(x => x.SquadRole == SquadRole.Player && !string.IsNullOrEmpty(x.Position))
                    .GroupBy(x => x.Position)
                    .Select(x => new PlayerByPosition()
                    {
                        Position = TidySquadPosition(x.Key),
                        SortOrder = GetSquadSortOrder(x.Key),
                        Players = x.ToList(),
                    })
                    .OrderBy(x => x.SortOrder)
                    .ToList();
        }

        private Player GetPlayerFromSquad(Squad squad, int TeamID)
        {
            return new Player()
            {
                PlayerID = squad.id,
                Name = squad.name,
                Position = squad.position,
                DateOfBirth = squad.dateOfBirth,
                Nationality = squad.nationality,
                Age = CalculateAge(squad.dateOfBirth),
                TeamID = TeamID,
            };
        }


        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            return dateOfBirth > today.AddYears(-age) 
                                ? age - 1 
                                : age;
        }

        private string TidySquadPosition(string position)
        {
            string tidySquadPosition = string.Empty;

            string positionForDisplay = position.Replace("_", " ");

            switch (positionForDisplay)
            {
                case "Goalkeeper":
                    tidySquadPosition = PlayerPosition.Goalkeeper.ToString();
                    break;
                case "Defence":
                    tidySquadPosition = PlayerPosition.Defender.ToString();
                    break;
                case "Midfield":
                    tidySquadPosition = PlayerPosition.Midfielder.ToString();
                    break;
                case "Offence":
                    tidySquadPosition = PlayerPosition.Attacker.ToString();
                    break;
                default:
                    break;
            }

            return tidySquadPosition;
        }

        private int GetSquadSortOrder(string position)
        {
            int squadSortOrder = (int)PlayerPosition.Goalkeeper;

            string positionForDisplay = position.Replace("_", " ");

            switch (positionForDisplay)
            {
                case "Goalkeeper":
                    squadSortOrder = (int)PlayerPosition.Goalkeeper;
                    break;
                case "Defence":
                    squadSortOrder = (int)PlayerPosition.Defender;
                    break;
                case "Midfield":
                    squadSortOrder = (int)PlayerPosition.Midfielder;
                    break;
                case "Offence":
                    squadSortOrder = (int)PlayerPosition.Attacker;
                    break;
                default:
                    break;
            }

            return squadSortOrder;
        }
    }
}
