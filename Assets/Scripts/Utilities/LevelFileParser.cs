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
            // Detect file version
            var version = DetectVersion(fileContent);

            var parts = fileContent.Split(new[] { "===" }, StringSplitOptions.None);

            if (parts.Length < 4)
            {
                throw new FormatException($"Expected 4 parts separated by '===', but found {parts.Length}");
            }

            // Parse Part 3 (index 2): Shelf Data
            var shelfData = ParseShelfData(parts[2], version);

            // Parse Part 4 (index 3): Solution Steps
            var moves = ParseSolutionSteps(parts[3]);

            return (shelfData, moves);
        }

        /// <summary>
        /// Detects the version of the level file format.
        /// </summary>
        /// <param name="fileContent">The full file content</param>
        /// <returns>Version number (1 or 2), defaults to 1 if not specified</returns>
        private static int DetectVersion(string fileContent)
        {
            // Check the first 5 lines for a version indicator
            var lines = fileContent.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var linesToCheck = Math.Min(5, lines.Length);

            for (int i = 0; i < linesToCheck; i++)
            {
                var line = lines[i].Trim();
                if (line.StartsWith("Version:", StringComparison.OrdinalIgnoreCase))
                {
                    var versionPart = line.Substring("Version:".Length).Trim();
                    if (int.TryParse(versionPart, out var version))
                    {
                        return version;
                    }
                }
            }

            // Default to version 1 for backward compatibility
            return 1;
        }

        /// <summary>
        /// Parses the shelf data table from Part 3.
        /// </summary>
        /// <param name="tableText">The table text to parse</param>
        /// <param name="version">The file format version (1 or 2)</param>
        private static ShelfPuzzleInputData[] ParseShelfData(string tableText, int version)
        {
            var lines = tableText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            var shelfDataList = new List<ShelfPuzzleInputData>();

            foreach (var line in lines)
            {
                // Skip decorator lines, headers, and separators
                // V1: "Shelf" and "Layer" columns
                // V2: "Single" and "Lock" and "Layer" columns
                if (line.Contains("---") ||
                    (line.Contains("Shelf") && line.Contains("Layer")) ||
                    (line.Contains("Single") && line.Contains("Lock") && line.Contains("Layer")) ||
                    line.StartsWith(".") || line.StartsWith("'"))
                {
                    continue;
                }

                // Process data rows (contain shelf data)
                if (line.Contains("|"))
                {
                    var shelfData = ParseShelfRow(line, version);
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
        /// Dispatches to version-specific parsers.
        /// </summary>
        /// <param name="row">The row text to parse</param>
        /// <param name="version">The file format version (1 or 2)</param>
        private static ShelfPuzzleInputData ParseShelfRow(string row, int version)
        {
            if (version == 2)
            {
                return ParseShelfRowV2(row);
            }
            else
            {
                return ParseShelfRowV1(row);
            }
        }

        /// <summary>
        /// Parses a single shelf row in V1 format.
        /// V1 format: | Shelf X | Lock Count | [layer0] | [layer1] | ...
        /// All shelves in V1 are Common type.
        /// </summary>
        private static ShelfPuzzleInputData ParseShelfRowV1(string row)
        {
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

            // V1 files always have Common type shelves
            return new ShelfPuzzleInputData
            {
                Type = ShelfType.Common,
                LockCount = lockCount,
                Data = layers.ToArray()
            };
        }

        /// <summary>
        /// Parses a single shelf row in V2 format.
        /// V2 format: | Shelf# | Single | Lock | [layer0] | [layer1] | ...
        /// Single column: 'x' for Single type, empty for Common type.
        /// </summary>
        private static ShelfPuzzleInputData ParseShelfRowV2(string row)
        {
            var parts = row.Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            if (parts.Length < 3)
            {
                return null; // Need at least: Shelf#, Single indicator, Lock count
            }

            // Column 1 (index 1): Single indicator ('x' for Single, empty for Common)
            var singleIndicator = parts[1];
            var shelfType = singleIndicator.Equals("x", StringComparison.OrdinalIgnoreCase)
                ? ShelfType.Single
                : ShelfType.Common;

            // Column 2 (index 2): Lock count
            var lockCount = 0;
            if (int.TryParse(parts[2], out var parsedLockCount))
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
                Type = shelfType,
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
