using System;

namespace Municiple_Project_st10259527.Services
{
    public static class RequestPriorityInferenceService
    {
        //====================================================================
        // Public API
        //====================================================================
        public static (int Priority, string Category) Infer(string title, string description, string preferredCategory = null)
        {
            var t = (title ?? string.Empty).ToLowerInvariant();
            var d = (description ?? string.Empty).ToLowerInvariant();
            var s = t + " " + d;

            int critical = 0;
            int high = 0;
            int demote = 0;

            // Critical hazards (Priority 1)
            CriticalHazards(s, ref critical);
            // Time urgency and commands
            TimeUrgency(s, ref high);
            // Sensitive locations
            SensitiveLocations(s, ref high);
            // Infrastructure/equipment issues
            InfrastructureIssues(s, ref high);
            // Punctuation signal
            PunctuationSignal(s, ref high);
            // Demotion clues
            DemotionClues(s, ref demote);

            int priority;
            if (critical >= 2) priority = 1;
            else if (high - demote >= 2) priority = 2;
            else priority = 3;

            var cat = string.IsNullOrWhiteSpace(preferredCategory) ? InferCategory(s) : preferredCategory;
            return (priority, cat);
        }

        //====================================================================
        // Scoring helpers (no arrays/lists)
        //====================================================================
        private static void CriticalHazards(string s, ref int critical)
        {
            if (s.Contains("gas leak")) critical += 3;
            if (s.Contains("burst pipe") || s.Contains("pipe burst")) critical += 3;
            if (s.Contains("power outage") || s.Contains("blackout")) critical += 2;
            if (s.Contains("structural damage") || s.Contains("collapse")) critical += 3;
            if (s.Contains("fire hazard") || s.Contains("spark") || s.Contains("smoke")) critical += 2;
            if (s.Contains("contaminated water") || s.Contains("toxic") || s.Contains("hazardous") || s.Contains("chemical") || s.Contains("spill")) critical += 2;
            if (s.Contains("blocked emergency exit")) critical += 3;
            // Additional critical patterns
            if (s.Contains("explosion") || s.Contains("explosive")) critical += 3;
            if (s.Contains("sewage overflow") || s.Contains("sewer backup")) critical += 3;
            if (s.Contains("fallen tree") || s.Contains("tree down")) critical += 2;
            if (s.Contains("bridge collapse") || s.Contains("road collapse")) critical += 3;
            if (s.Contains("flooding") || s.Contains("flood")) critical += 2;
            if (s.Contains("electrical fire") || s.Contains("burning smell")) critical += 3;
            if (s.Contains("gas smell") || s.Contains("gas odor")) critical += 3;
            if (s.Contains("live wire") || s.Contains("downed power line")) critical += 3;
            if (s.Contains("building collapse") || s.Contains("roof collapse")) critical += 3;
        }

        private static void TimeUrgency(string s, ref int high)
        {
            if (s.Contains("urgent") || s.Contains("asap") || s.Contains("immediately") || s.Contains("right away") || s.Contains("emergency")) high += 2;
            if (s.Contains("today") || s.Contains("tonight") || s.Contains("now")) high += 1;
            if (s.Contains("as soon as possible") || s.Contains("need it now")) high += 2;
            if (s.Contains("critical") || s.Contains("severe")) high += 1;
        }

        private static void SensitiveLocations(string s, ref int high)
        {
            if (s.Contains("school") || s.Contains("hospital") || s.Contains("clinic") || s.Contains("elderly") || s.Contains("nursing")) high += 2;
            if (s.Contains("daycare") || s.Contains("childcare") || s.Contains("kindergarten")) high += 2;
            if (s.Contains("public space") || s.Contains("park") || s.Contains("playground")) high += 1;
            if (s.Contains("shopping centre") || s.Contains("mall")) high += 1;
        }

        private static void InfrastructureIssues(string s, ref int high)
        {
            if (s.Contains("broken window") || s.Contains("leaking tap") || s.Contains("hvac") || s.Contains("aircon") || s.Contains("heating")) high += 1;
            if (s.Contains("alarm") || s.Contains("cctv")) high += 1;
            if (s.Contains("waste overflow") || s.Contains("overflowing")) high += 2;
            if (s.Contains("poor ventilation") || s.Contains("mold") || s.Contains("mould")) high += 1;
            if (s.Contains("faulty") || s.Contains("malfunction") || s.Contains("not working") || s.Contains("out of order")) high += 1;
            if (s.Contains("door broken") || s.Contains("lock broken") || s.Contains("exposed wiring") || s.Contains("electrical wiring")) high += 2;
            // Additional high patterns
            if (s.Contains("water outage") || s.Contains("no water")) high += 2;
            if (s.Contains("streetlight out") || s.Contains("traffic light out") || s.Contains("traffic lights out")) high += 2;
            if (s.Contains("pothole") || s.Contains("road damage")) high += 1;
            if (s.Contains("drain blockage") || s.Contains("blocked drain")) high += 1;
            if (s.Contains("lift broken") || s.Contains("elevator broken")) high += 2;
            if (s.Contains("fire alarm") || s.Contains("smoke detector")) high += 2;
            if (s.Contains("security breach") || s.Contains("forced entry")) high += 2;
        }

        private static void PunctuationSignal(string s, ref int high)
        {
            int bang = 0; for (int i = 0; i < s.Length; i++) { if (s[i] == '!') bang++; }
            if (bang >= 3) high += 1; if (bang >= 6) high += 1;
        }

        private static void DemotionClues(string s, ref int demote)
        {
            if (s.Contains("test") || s.Contains("drill") || s.Contains("training")) demote += 1;
            if (s.Contains("no immediate danger") || s.Contains("not urgent")) demote += 2;
            if (s.Contains("routine") || s.Contains("scheduled")) demote += 1;
        }

        //====================================================================
        // Category inference (no arrays/lists)
        //====================================================================
        private static string InferCategory(string text)
        {
            if (text.Contains("road") || text.Contains("pothole") || text.Contains("street") || text.Contains("sidewalk") || text.Contains("traffic") || text.Contains("bridge"))
                return "Infrastructure";
            if (text.Contains("water") || text.Contains("pipe") || text.Contains("sewer") || text.Contains("drain") || text.Contains("sewage"))
                return "Utilities";
            if (text.Contains("electric") || text.Contains("power") || text.Contains("light") || text.Contains("wiring") || text.Contains("streetlight") || text.Contains("lighting") || text.Contains("cable"))
                return "Electrical";
            if (text.Contains("waste") || text.Contains("bin") || text.Contains("recycling") || text.Contains("trash") || text.Contains("overflow"))
                return "Waste";
            if (text.Contains("health") || text.Contains("mold") || text.Contains("pest") || text.Contains("sanitary") || text.Contains("contaminated"))
                return "Health";
            if (text.Contains("security") || text.Contains("cctv") || text.Contains("lock") || text.Contains("alarm") || text.Contains("breach") || text.Contains("forced"))
                return "Safety";
            if (text.Contains("fire") || text.Contains("smoke") || text.Contains("spark") || text.Contains("burning"))
                return "Fire";
            if (text.Contains("tree") || text.Contains("park") || text.Contains("garden") || text.Contains("landscape"))
                return "Parks";
            return "General";
        }
    }
    //====================================End=Of=File=============================================
}
