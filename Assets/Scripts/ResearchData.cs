using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Profile;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public static class ResearchData
    {
        public static string data = "";

        public static int FringeMem;
        public static int AStarMem;

        public static double FringeTime;
        public static double AStarTime;

        static ResearchData()
        {
            ClearData();
            Chrono.TimeRecorded += AddTimeToData;
        }

        private static void AddTimeToData(string msg, TimeSpan time)
        {
            if (msg == "Fringe")
                ResearchData.FringeTime = time.TotalMilliseconds;
            else
                ResearchData.AStarTime = time.TotalMilliseconds;
        }

        public static void ClearData()
        {
            data = "firnge mem;astar mem;fringe time;astar time\n";
        }

        public static void WriteLine()
        {
            data += string.Format("{0};{1};{2};{3}\n", FringeMem, AStarMem, FringeTime, AStarTime);
        }

        [MenuItem("Research/Save Data")]
        public static void SaveData()
        {
            File.AppendAllText(string.Format("Data_{0}.csv", 0), data);
            ClearData();
        }
    }
}
