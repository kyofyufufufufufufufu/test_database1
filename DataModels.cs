// Class for structuring Questions for json file 

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WinFormsApp1
{
    public class QuestionSet
    {
        public List<Question> questions = new List<Question>();
    }

    // Structure for question
    public class Question
    {
        public string question { get; set; } = string.Empty; // Fixed warning
        public string imageLink { get; set; } = string.Empty; // Fixed warning
        public List<Option> options { get; set; } = new List<Option>();
        public int answerIndex { get; set; }

        public int difficulty { get; set; }

        // This is the packed integer that holds both the Module (top 5 bits) 
        // and the Body Location flags (bottom 8 bits)
        public int locations { get; set; }

        // Enum used in the C# code to map CheckBox selections
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
        public string text { get; set; } = string.Empty; // Fixed warning
        public string imageLink { get; set; } = string.Empty; // Fixed warning

        public bool useImage { get; set; }
    }
}