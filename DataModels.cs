// Class for structuring Questions for json file 

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WinFormsApp1
{
    public class QuestionSet
    {
        public List<Question> questions = new List<Question>();

        // Bulk Upload Section

        // Turns a string of comma-separated body locations into the packed integer format for storage in the JSON file
        public static int EncodeLocations(int moduleIndex, string locationString)
        {
            int locationValue = 0;

            // Split strings by comma and trim spaces, then match to enum values
            string[] parts = locationString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                // Match string to LocationFlags enum
                // Ignores spaces and NOT case sensitive
                if (Enum.TryParse(part.Trim().Replace(" ", "_"), true, out Question.LocationFlags flag))
                {
                    locationValue |= (int)flag;
                }
            }

            // Shift module index by 8 bits and combine with flags
            return (moduleIndex << 8) | locationValue;
        }
    }

    // Structure for question
    public class Question
    {
        public string question { get; set; } = string.Empty;
        public string imageLink { get; set; } = string.Empty;
        public List<Option> options { get; set; } = new List<Option>();
        public int answerIndex { get; set; }

        public int difficulty { get; set; }

        // Minigame holds three options: Card Match, Whack-a-Mole, and Slapjack
        public string minigameType { get; set; } = "MultipleChoice";

        // This is the packed integer that holds both the Module (top 5 bits) 
        // and the Body Location flags (bottom 8 bits)
        public int locations { get; set; }

        // Enum used to map checkbox selections to bits in the locations integer
        [Flags]
        public enum LocationFlags
        {
            Bladder = 1,
            Brain = 2,
            Eyes = 4,
            GI_Tract = 8,
            Heart = 16,
            Lungs = 32,
            Smooth_Muscle = 64,
            Other = 128,
        }
    }

    public class Option
    {
        public string text { get; set; } = string.Empty;
        public string imageLink { get; set; } = string.Empty;


        // Tells Unity whether to attempt to render an image for this specific option
        public bool useImage { get; set; } = false;
    }
}