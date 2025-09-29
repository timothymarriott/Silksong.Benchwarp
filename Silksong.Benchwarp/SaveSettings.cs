﻿using System.Collections.Generic;

namespace Benchwarp
{
    public class SaveSettings
    {
        public bool benchDeployed;
        public bool atDeployedBench;
        public float benchX;
        public float benchY;
        public string benchScene;
        public HashSet<BenchKey> visitedBenchScenes = new();
        public HashSet<BenchKey> lockedBenches = new();
    }
}
