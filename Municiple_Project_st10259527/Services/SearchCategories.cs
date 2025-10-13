using System.Collections.Generic;

namespace Municiple_Project_st10259527.Services
{
    public static class SearchCategories
    {
        /// <summary>
        /// Dictionary mapping search terms to their corresponding categories.
        /// The dictionary uses case-insensitive string comparison.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> TermToCategoryMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Sports
            ["sport"] = "Sports",
            ["soccer"] = "Sports",
            ["basketball"] = "Sports",
            ["football"] = "Sports",
            ["tennis"] = "Sports",
            ["cricket"] = "Sports",
            ["swimming"] = "Sports",
            ["running"] = "Sports",
            ["marathon"] = "Sports",
            
            // Music
            ["music"] = "Music",
            ["concert"] = "Music",
            ["band"] = "Music",
            ["gig"] = "Music",
            ["festival"] = "Music",
            ["jazz"] = "Music",
            ["rock"] = "Music",
            ["classical"] = "Music",
            
            // Art
            ["art"] = "Art",
            ["exhibition"] = "Art",
            ["gallery"] = "Art",
            ["painting"] = "Art",
            ["sculpture"] = "Art",
            ["photography"] = "Art",
            ["craft"] = "Art",
            ["pottery"] = "Art",
            
            // Community (Expanded)
            ["community"] = "Community",
            ["meeting"] = "Community",
            ["gathering"] = "Community",
            ["town hall"] = "Community",
            ["neighborhood"] = "Community",
            ["volunteer"] = "Community",
            ["charity"] = "Community",
            ["fundraiser"] = "Community",
            ["donation"] = "Community",
            ["cleanup"] = "Community",
            ["recycling"] = "Community",
            ["sustainability"] = "Community",
            ["neighborhood watch"] = "Community",
            ["block party"] = "Community",
            ["parade"] = "Community",
            ["celebration"] = "Community",
            ["heritage"] = "Community",
            ["cultural"] = "Community",
            ["diversity"] = "Community",
            ["inclusion"] = "Community",
            
            // Food & Dining
            ["food"] = "Food & Dining",
            ["restaurant"] = "Food & Dining",
            ["dining"] = "Food & Dining",
            ["cooking"] = "Food & Dining",
            ["culinary"] = "Food & Dining",
            ["wine"] = "Food & Dining",
            ["beer"] = "Food & Dining",
            ["coffee"] = "Food & Dining",
            
            // Education
            ["education"] = "Education",
            ["workshop"] = "Education",
            ["seminar"] = "Education",
            ["lecture"] = "Education",
            ["class"] = "Education",
            ["tutorial"] = "Education",
            ["learning"] = "Education",
            ["course"] = "Education",
            
            // Health & Wellness
            ["health"] = "Health & Wellness",
            ["fitness"] = "Health & Wellness",
            ["yoga"] = "Health & Wellness",
            ["gym"] = "Health & Wellness",
            ["wellness"] = "Health & Wellness",
            ["meditation"] = "Health & Wellness",
            ["nutrition"] = "Health & Wellness",
            ["mental health"] = "Health & Wellness",
            
            // Business
            ["business"] = "Business",
            ["networking"] = "Business",
            ["conference"] = "Business",
            ["entrepreneur"] = "Business",
            ["startup"] = "Business",
            ["career"] = "Business",
            ["job fair"] = "Business",
            
            // Technology
            ["technology"] = "Technology",
            ["coding"] = "Technology",
            ["programming"] = "Technology",
            ["ai"] = "Technology",
            ["robotics"] = "Technology",
            ["blockchain"] = "Technology",
            ["cybersecurity"] = "Technology",
            ["webinar"] = "Technology"
        };

        /// <summary>
        /// Gets the category for a given search term.
        /// </summary>
        /// <param name="searchTerm">The term to find a category for.</param>
        /// <returns>The category name, or "General" if no match is found.</returns>
        public static string GetCategoryForTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return "General";

            string searchLower = searchTerm.ToLower();

            // First try exact matches
            if (TermToCategoryMap.TryGetValue(searchLower, out var category))
            {
                return category;
            }

            // Then try partial matches
            foreach (var (term, cat) in TermToCategoryMap)
            {
                if (searchLower.Contains(term))
                {
                    return cat;
                }
            }

            return "General";
        }
    }
}
