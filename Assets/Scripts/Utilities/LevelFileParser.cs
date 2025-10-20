using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Engine.ShelfPuzzle;
using manager;
using Newtonsoft.Json;
using UnityEngine;

namespace Utilities
{
    public class LevelFileParser
    {
        /// <summary>
        /// Parses a level file and returns shelf data and solution moves.
        /// </summary>
        /// <returns>Tuple of (ShelfPuzzleInputData array, Move array)</returns>
        public static (ShelfPuzzleInputData[], Move[]) ParseLevelFile(string fileContent)
        {
            var parts = fileContent.Split(new[] { "===" }, StringSplitOptions.None);

            if (parts.Length < 4)
            {
                throw new FormatException($"Expected 4 parts separated by '===', but found {parts.Length}");
            }

            // Parse Part 3 (index 2): Shelf Data
            var shelfData = ParseShelfData(parts[2]);

            // Parse Part 4 (index 3): Solution Steps
            var moves = ParseSolutionSteps(parts[3]);

            return (shelfData, moves);
        }

        /// <summary>
        /// Parses the shelf data table from Part 3.
        /// </summary>
        private static ShelfPuzzleInputData[] ParseShelfData(string tableText)
        {
            var lines = tableText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var shelfDataList = new List<ShelfPuzzleInputData>();

            foreach (var line in lines)
            {
                // Skip decorator lines, headers, and separators
                if (line.Contains("---") || line.Contains("Shelf") && line.Contains("Layer") ||
                    line.StartsWith(".") || line.StartsWith("'"))
                {
                    continue;
                }

                // Process data rows (contain shelf data)
                if (line.Contains("|"))
                {
                    var shelfData = ParseShelfRow(line);
                    if (shelfData != null)
                    {
                        shelfDataList.Add(shelfData);
                    }
                }
            }

            return shelfDataList.ToArray();
        }

        /// <summary>
        /// Parses a single shelf row and extracts layer data.
        /// </summary>
        private static ShelfPuzzleInputData ParseShelfRow(string row)
        {
            // Extract Lock Count value from the row (new format has Lock Count column)
            var lockCount = 0;
            var parts = row.Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            // Check if second part is Lock Count (numeric value without brackets)
            if (parts.Length >= 2 && int.TryParse(parts[1], out var parsedLockCount))
            {
                lockCount = parsedLockCount;
            }

            // Extract all array patterns [x,y,z] from the row
            var arrayPattern = @"\[([^\]]+)\]";
            var matches = Regex.Matches(row, arrayPattern);

            if (matches.Count == 0)
            {
                return null; // No data arrays found
            }

            var layers = new List<int[]>();

            foreach (Match match in matches)
            {
                // Parse the array content "x,y,z"
                var arrayContent = match.Groups[1].Value;
                var values = arrayContent.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(int.Parse)
                    .ToArray();

                if (values.Length > 0)
                {
                    layers.Add(values);
                }
            }

            if (layers.Count == 0)
            {
                return null;
            }

            return new ShelfPuzzleInputData
            {
                Type = ShelfType.Common,
                LockCount = lockCount,
                Data = layers.ToArray()
            };
        }

        /// <summary>
        /// Parses the solution steps table from Part 4.
        /// </summary>
        private static Move[] ParseSolutionSteps(string tableText)
        {
            var lines = tableText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var moves = new List<Move>();

            foreach (var line in lines)
            {
                // Skip decorator lines, headers, and separators
                if (line.Contains("---") || line.Contains("Step") ||
                    line.StartsWith(".") || line.StartsWith("'"))
                {
                    continue;
                }

                // Process data rows (contain move data)
                if (line.Contains("|"))
                {
                    var move = ParseMoveRow(line);
                    if (move != null)
                    {
                        moves.Add(move);
                    }
                }
            }

            return moves.ToArray();
        }

        /// <summary>
        /// Parses a single move row and extracts step data.
        /// </summary>
        private static Move ParseMoveRow(string row)
        {
            // Split by | and extract values
            var parts = row.Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (parts.Length < 4)
            {
                return null; // Invalid row format
            }

            try
            {
                // Parse: Step | Item ID | From Shelf | To Shelf
                var itemId = int.Parse(parts[1]);
                var fromShelf = int.Parse(parts[2]);
                var toShelf = int.Parse(parts[3]);

                return new Move(fromShelf, toShelf, itemId);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse move row: {row}. Error: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Converts ShelfPuzzleInputData array to JSON format and logs it.
        /// </summary>
        /// <param name="shelfData">Array of shelf data to convert</param>
        public static void ConvertToJsonAndLog(ShelfPuzzleInputData[] shelfData)
        {
            var levelConfig = new LevelConfigJson
            {
                LevelStrategies = new[]
                {
                    new LevelStrategyJson
                    {
                        Data = shelfData.Select(shelf => new BoxData
                        {
                            Box = shelf.Data
                        }).ToArray()
                    }
                }
            };

            var json = JsonConvert.SerializeObject(levelConfig, Formatting.Indented);
            CleanLogger.Log(json);
        }

        public static void JustLog(ShelfPuzzleInputData[] shelfData)
        {
            var json = JsonConvert.SerializeObject(shelfData, Formatting.Indented);
            CleanLogger.Log(json);
        }

        #region JSON Helper Classes

        [Serializable]
        private class LevelConfigJson
        {
            public LevelStrategyJson[] LevelStrategies { get; set; }
        }

        [Serializable]
        private class LevelStrategyJson
        {
            public BoxData[] Data { get; set; }
        }

        [Serializable]
        private class BoxData
        {
            public int[][] Box { get; set; }
        }

        #endregion
    }
}
