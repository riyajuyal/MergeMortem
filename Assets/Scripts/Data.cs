using UnityEngine;

namespace MergeMortem
{
    public enum ChainType
    {
        Fabric = 0,
        Prints = 1,
        Phone = 2
    }

    [System.Serializable]
    public class ChainDef
    {
        public ChainType type;
        public string chainName;
        public Color color;
        public string[] tierLabels;   // length 3: tier1, tier2, tier3
        public string revealTitle;
        public string revealBody;
        public string suspect;
        public string suspectVerdict; // "lied" | "cleared" | "unexplained"
    }

    public static class Data
    {
        public const int BoardSize = 5;
        public const int TierCount = 3;
        public const int StartEnergy = 20;
        public const int XpPerMerge = 5;
        public const int XpPerFile = 20;
        public const int CoinsPerFile = 20;
        public const int XpPerLevel = 50;
        public const int AdEnergyReward = 5;

        public static readonly Color PanelWhite = new Color(1f, 1f, 1f, 1f);
        public static readonly Color SlateBackground = new Color(0.29f, 0.33f, 0.41f, 1f);
        public static readonly Color EvidenceYellow = HexToColor("#f2c230");

        public static Color HexToColor(string hex)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString(hex, out c)) return c;
            return Color.yellow;
        }

        public static ChainDef[] Chains = new ChainDef[]
        {
            new ChainDef
            {
                type = ChainType.Fabric,
                chainName = "Fabric",
                color = new Color(0.90f, 0.47f, 0.13f), // orange
                tierLabels = new string[] { "Thread", "Cloth scrap", "Jacket sleeve" },
                revealTitle = "Fabric evidence filed",
                revealBody = "The sleeve matches Karan’s jacket. He swore he never entered the study, so he lied.",
                suspect = "Karan",
                suspectVerdict = "lied"
            },
            new ChainDef
            {
                type = ChainType.Prints,
                chainName = "Prints",
                color = new Color(0.20f, 0.47f, 0.84f), // blue
                tierLabels = new string[] { "Smudge", "Partial print", "Full print" },
                revealTitle = "Print evidence filed",
                revealBody = "The print on the whisky glass is Dev’s, but dust shows it was placed at 6 pm, hours before the murder. Dev is cleared.",
                suspect = "Dev",
                suspectVerdict = "cleared"
            },
            new ChainDef
            {
                type = ChainType.Phone,
                chainName = "Phone",
                color = new Color(0.22f, 0.63f, 0.36f), // green
                tierLabels = new string[] { "Signal", "Call record", "Phone log" },
                revealTitle = "Phone evidence filed",
                revealBody = "The victim’s phone sent a message at 10:15 pm, after the reported time of death. Meena is still unexplained.",
                suspect = "Meena",
                suspectVerdict = "unexplained"
            }
        };

        public const string IntroStory =
            "Arvind Rao, a tea-estate owner in Coorg, is found dead in his study. The reported time of death is 9:30 pm. " +
            "Three people are suspects: Karan, his nephew, who says he left at 9:00 pm and never went into the study; " +
            "Dev, the estate manager; and Meena, the housekeeper.";

        public const string IntroStep1 = "1. Tap a generator to drop a clue.";
        public const string IntroStep2 = "2. Merge two identical items.";
        public const string IntroStep3 = "3. File finished evidence at the top tier.";

        public const string HintEmptyBoard = "Tap a generator to drop evidence.";
        public const string HintMatchExists = "A matching pair! Tap one, then the other to merge.";
        public const string HintFileReady = "Select the glowing item, then tap File as evidence.";
        public const string HintKeepDropping = "Drop more evidence until you have two identical items.";
        public const string HintBoardFull = "Board’s full — file or merge something first.";

        public const string ChapterCompleteTitle = "Chapter 1 complete";
        public const string ChapterCompleteBody =
            "You’ve filed all three pieces of evidence. A coffee-jar souvenir has been added to your Detective HQ shelf.";

        public const string Day1HookTitle = "One thing still doesn’t add up";
        public const string Day1HookBody =
            "The victim’s phone sent a message at 10:15 pm, after the time of death. Who sent it? " +
            "Chapter 2 unlocks tomorrow, with 20 free energy waiting.";

        public const string SeeYouTomorrowTitle = "See you tomorrow, detective";
        public const string SeeYouTomorrowBody =
            "The case file is sealed for the night. Come back tomorrow to keep digging.";

        public const string Chapter2TeaserTitle = "Chapter 2 teaser";
        public const string Chapter2TeaserBody =
            "The message came from inside the estate Wi-Fi. Only three phones were connected, and one was Meena’s. " +
            "The prototype ends here.";

        public const string AdModalTitle = "Out of energy";
        public const string AdModalBody = "Watch ad: +5 energy (simulated)";
    }
}
