using System;
using System.Collections.Generic;
using System.Linq;

namespace Municiple_Project_st10259527.Services
{
    public static class RequestPriorityInferenceService
    {
        //====================================================================
        // Main Inference Logic
        //====================================================================
        public static (int Priority, string Category, string Reason) Infer(
            string title,
            string description,
            string preferredCategory = null)
        {
            string text = (title + " " + description).ToLowerInvariant();
            var reasons = new List<string>();
            int score = 0;

            // Rule groups with weighted scoring
            score += ApplyCriticalHazards(text, reasons);
            score += ApplyUrgency(text, reasons);
            score += ApplyLocation(text, reasons);
            score += ApplyInfrastructure(text, reasons);
            score += ApplyEnvironmental(text, reasons);
            score += ApplySecurity(text, reasons);
            score -= ApplyDemotions(text, reasons);

            // Priority calculation (Balanced thresholds)
            int priority = score >= 9 ? 1 : score >= 4 ? 2 : 3;

            string category = string.IsNullOrWhiteSpace(preferredCategory)
                ? InferCategory(text)
                : preferredCategory;

            string reasonSummary = reasons.Count > 0
                ? string.Join("; ", reasons)
                : "No specific triggers found.";

            return (priority, category, $"Priority {priority} due to: {reasonSummary}");
        }

        //====================================================================
        // CRITICAL HAZARDS — Priority 1 (weight = 5 each)
        //====================================================================
        private static int ApplyCriticalHazards(string text, List<string> reasons)
        {
            int score = 0;
            void Add(int val, string reason) { score += val; reasons.Add(reason); }

            if (text.Contains("gas leak")) Add(5, "Gas leak detected");
            if (text.Contains("burst pipe") || text.Contains("pipe burst")) Add(5, "Burst pipe issue");
            if (text.Contains("fire") || text.Contains("smoke") || text.Contains("burning")) Add(5, "Fire hazard");
            if (text.Contains("toxic") || text.Contains("chemical spill") || text.Contains("hazardous")) Add(5, "Toxic or chemical hazard");
            if (text.Contains("collapse") || text.Contains("structural damage") || text.Contains("unsafe building")) Add(5, "Structural hazard");
            if (text.Contains("flood") || text.Contains("sewage overflow") || text.Contains("sewer backup")) Add(5, "Flooding or sewage issue");
            if (text.Contains("live wire") || text.Contains("downed power line")) Add(5, "Exposed electrical wire");
            if (text.Contains("explosion") || text.Contains("blast")) Add(5, "Explosion risk");
            if (text.Contains("blocked emergency exit")) Add(5, "Blocked emergency access");
            if (text.Contains("contaminated water")) Add(5, "Unsafe drinking water");

            return score;
        }

        //====================================================================
        // URGENCY — Priority 1–2 (weights = 1–3)
        //====================================================================
        private static int ApplyUrgency(string text, List<string> reasons)
        {
            int score = 0;
            void Add(int val, string reason) { score += val; reasons.Add(reason); }

            if (text.Contains("emergency") || text.Contains("urgent")) Add(3, "Marked as urgent");
            if (text.Contains("immediately") || text.Contains("asap") || text.Contains("right away")) Add(2, "Immediate attention requested");
            if (text.Contains("today") || text.Contains("now") || text.Contains("tonight")) Add(1, "Time-sensitive");
            if (text.Contains("critical") || text.Contains("severe")) Add(2, "Critical descriptor used");

            return score;
        }

        //====================================================================
        // LOCATION — Priority 2–3 (weights = 1–3)
        //====================================================================
        private static int ApplyLocation(string text, List<string> reasons)
        {
            int score = 0;
            void Add(int val, string reason) { score += val; reasons.Add(reason); }

            if (text.Contains("school") || text.Contains("hospital") || text.Contains("clinic")) Add(3, "Sensitive public location");
            if (text.Contains("elderly") || text.Contains("nursing home") || text.Contains("care centre")) Add(2, "Vulnerable population area");
            if (text.Contains("park") || text.Contains("playground") || text.Contains("sports field")) Add(2, "Public recreation area");
            if (text.Contains("shopping centre") || text.Contains("mall")) Add(1, "Public commercial area");

            return score;
        }

        //====================================================================
        // INFRASTRUCTURE — Priority 2 (weights = 2–4)
        //====================================================================
        private static int ApplyInfrastructure(string text, List<string> reasons)
        {
            int score = 0;
            void Add(int val, string reason) { score += val; reasons.Add(reason); }

            if (text.Contains("power outage") || text.Contains("blackout")) Add(4, "Power disruption");
            if (text.Contains("water outage") || text.Contains("no water")) Add(4, "Water service disruption");
            if (text.Contains("pothole") || text.Contains("road damage")) Add(3, "Road surface issue");
            if (text.Contains("streetlight out") || text.Contains("traffic light")) Add(3, "Traffic signal/light issue");
            if (text.Contains("blocked drain") || text.Contains("drain blockage")) Add(3, "Drainage issue");
            if (text.Contains("lift broken") || text.Contains("elevator broken")) Add(3, "Building access problem");
            if (text.Contains("roof leak") || text.Contains("leaking ceiling")) Add(3, "Building water leak");
            if (text.Contains("cracked pavement") || text.Contains("sidewalk damage")) Add(2, "Sidewalk/pavement hazard");

            return score;
        }

        //====================================================================
        // ENVIRONMENTAL — Priority 2 (weights = 2–3)
        //====================================================================
        private static int ApplyEnvironmental(string text, List<string> reasons)
        {
            int score = 0;
            void Add(int val, string reason) { score += val; reasons.Add(reason); }

            if (text.Contains("illegal dumping") || text.Contains("waste overflow")) Add(3, "Illegal waste or overflow");
            if (text.Contains("dead animal") || text.Contains("carcass")) Add(3, "Health/environmental hazard");
            if (text.Contains("pest infestation") || text.Contains("rats") || text.Contains("insects")) Add(2, "Pest control issue");
            if (text.Contains("pollution") || text.Contains("smoke plume")) Add(3, "Environmental pollution");
            if (text.Contains("tree fallen") || text.Contains("branch blocking")) Add(3, "Obstruction from fallen tree");

            return score;
        }

        //====================================================================
        // SECURITY — Priority 2 (weights = 2–3)
        //====================================================================
        private static int ApplySecurity(string text, List<string> reasons)
        {
            int score = 0;
            void Add(int val, string reason) { score += val; reasons.Add(reason); }

            if (text.Contains("security breach") || text.Contains("break-in") || text.Contains("forced entry")) Add(3, "Security breach reported");
            if (text.Contains("cctv not working") || text.Contains("alarm faulty")) Add(2, "Security system malfunction");
            if (text.Contains("vandalism") || text.Contains("graffiti")) Add(1, "Vandalism incident");

            return score;
        }

        //====================================================================
        // DEMOTIONS — Reduce score for low-urgency contexts
        //====================================================================
        private static int ApplyDemotions(string text, List<string> reasons)
        {
            int penalty = 0;
            void Add(int val, string reason) { penalty += val; reasons.Add(reason); }

            if (text.Contains("test") || text.Contains("training") || text.Contains("simulation")) Add(2, "Test or training scenario");
            if (text.Contains("routine") || text.Contains("scheduled") || text.Contains("maintenance")) Add(3, "Routine maintenance task");
            if (text.Contains("not urgent") || text.Contains("no danger")) Add(3, "Explicitly marked non-urgent");

            return penalty;
        }

        //====================================================================
        // CATEGORY INFERENCE
        //====================================================================
        private static string InferCategory(string text)
        {
            if (text.Contains("water") || text.Contains("pipe") || text.Contains("drain") || text.Contains("sewer"))
                return "Utilities";
            if (text.Contains("electric") || text.Contains("power") || text.Contains("light") || text.Contains("wiring"))
                return "Electrical";
            if (text.Contains("road") || text.Contains("street") || text.Contains("pothole") || text.Contains("bridge"))
                return "Infrastructure";
            if (text.Contains("waste") || text.Contains("trash") || text.Contains("dumping") || text.Contains("recycling"))
                return "Waste";
            if (text.Contains("security") || text.Contains("alarm") || text.Contains("breach"))
                return "Safety";
            if (text.Contains("fire") || text.Contains("smoke"))
                return "Fire";
            if (text.Contains("tree") || text.Contains("garden") || text.Contains("park"))
                return "Parks";
            if (text.Contains("pollution") || text.Contains("rats") || text.Contains("pest"))
                return "Environmental";

            return "General";
        }
    }
}
