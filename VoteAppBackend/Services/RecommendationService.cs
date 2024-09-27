using System;
using System.Collections.Generic;
using System.Linq;
using VoteAppBackend.Models;

namespace VoteAppBackend.Services
{
    public class RecommendationService
    {
        private readonly Dictionary<string, Dictionary<string, int>> _userPreferences;

        public RecommendationService(List<UserPreference> userPreferences)
        {
            _userPreferences = userPreferences.ToDictionary(
                up => up.UserId,
                up => up.Preferences);
        }

        // Función de distancia de Manhattan
        public double ManhattanDistance(Dictionary<string, int> prefs1, Dictionary<string, int> prefs2)
        {
            double distance = 0;
            bool commonItems = false;

            foreach (var item in prefs1.Keys)
            {
                if (prefs2.ContainsKey(item))
                {
                    distance += Math.Abs(prefs1[item] - prefs2[item]);
                    commonItems = true;
                }
            }

            return commonItems ? distance : double.MaxValue;
        }

        // Función computeNearestNeighbor
        public List<(double distance, string userId)> ComputeNearestNeighbor(string userId)
        {
            var distances = new List<(double, string)>();
            var prefs1 = _userPreferences[userId];

            foreach (var otherUserId in _userPreferences.Keys)
            {
                if (otherUserId != userId)
                {
                    var prefs2 = _userPreferences[otherUserId];
                    double distance = ManhattanDistance(prefs1, prefs2);
                    distances.Add((distance, otherUserId));
                }
            }

            // Ordenar por distancia
            distances.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            return distances;
        }
    }
}
