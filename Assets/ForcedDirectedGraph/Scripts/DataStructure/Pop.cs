using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ForceDirectedGraph
{
    public class Pop
    {


        /// <summary>
        /// Emotional state between -10 and 10
        /// </summary>
        private float emotionalState = 0f;

        public static float maxEmotionValue = 10;

        public static float minimumEmotionValue = -10;

        /// <summary>
        /// Increase emotion state by value of rate that goes from 0 to 1
        /// </summary>
        /// <param name="rate"></param>
        public void emotionChangeHappy(float rate) {
            emotionalState = emotionalState + rate;
            if (emotionalState > maxEmotionValue) emotionalState = maxEmotionValue;
        }

        /// <summary>
        /// Decrease emotion state by value of rate that goes from 0 to 1
        /// </summary>
        /// <param name="rate"></param>
        public void emotionChangeAngry(float rate) {
            emotionalState = emotionalState - rate;
            if (emotionalState < minimumEmotionValue) emotionalState = minimumEmotionValue;
        }

        public float emotionalValue { get { return emotionalState; } }

        public string emotionalName { get
            {
                if (emotionalState > 0) return "Happy";
                return "Angry";
            }
        }

        /// <summary>
        /// Value added when hobbyies differ
        /// </summary>
        public const float maxHobbyDifference = 10;

        public static float maximumSimilarity { get { return maxHobbyDifference + 25; } }


        /// <summary>
        /// Ideology of the pop, used to form bubles and similarity.
        /// </summary>
        private Ideology ideology;

        public float ideologyValue { get { return ideology.value; } }

        public string ideologyName { get { return Ideology.mapIdeology(ideology.denomination); } }

        public Ideology.IdeologyDenomination ideologyDenominaion { get { return ideology.denomination; } }


        /// <summary>
        /// Hobby of the pop, used only for similarity.
        /// </summary>
        private Hobby hobbby;


        public string hobbyName { get { return Hobby.mapHobby(hobbby.denomination); } }

        public Hobby.HobbyDenomination hobbyDenomination { get { return hobbby.denomination; } }


        public Pop(float emotionalState, Ideology ideology, Hobby hobbby)
        {
            this.emotionalState = emotionalState;
            this.ideology = ideology;
            this.hobbby = hobbby;
        }



        /// <summary>
        /// Calculates similarity between two persons:
        /// is = abs(id1 - id2), Comm - Pop == maxValueDifference
        /// hs = if hb1 == hb2: 0, else +maxValueDifference
        /// </summary>
        /// <returns>hs + is, 0 means best similarity</returns>
        public static float calculateSimilarity(Pop person1, Pop person2)
        {
            var ideologyScore = Mathf.Abs(person1.ideologyValue - person2.ideologyValue);
            var hobbyScore = person1.hobbby.denomination == person2.hobbby.denomination ? 0 : maxHobbyDifference;
            return ideologyScore + hobbyScore;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1} , {2}", emotionalName, ideologyName, hobbyName);
        }
    }



    #region Class Definitions
    public class Ideology
    {

        public enum IdeologyDenomination
        {
            Communist,
            Socialist,
            Centrist,
            Liberal,
            Populist
        }
        public IdeologyDenomination denomination;

        public float value;

        public Ideology(float value)
        {
            setIdeologyValue(value);
        }


        private void setIdeologyValue(float value)
        {
            if (value > 25 || value < 0) return;
            this.value = value;
            if (value >= 0) this.denomination = IdeologyDenomination.Populist;
            if (value >= 5) this.denomination = IdeologyDenomination.Liberal;
            if (value >= 10) this.denomination = IdeologyDenomination.Centrist;
            if (value >= 15) this.denomination = IdeologyDenomination.Socialist;
            if (value >= 20) this.denomination = IdeologyDenomination.Communist;
        }

        public static string mapIdeology(IdeologyDenomination denomination)
        {
            switch (denomination)
            {
                case IdeologyDenomination.Communist:
                    return "Communist";
                case IdeologyDenomination.Socialist:
                    return "Socialist";
                case IdeologyDenomination.Centrist:
                    return "Centrist";
                case IdeologyDenomination.Liberal:
                    return "Liberal";
                case IdeologyDenomination.Populist:
                    return "Populist";

            }
            return "";
        }
    }

    public class Hobby
    {

        public enum HobbyDenomination
        {
            Film,
            Games,
            Books,
            Football,
            Fashion
        }
        public HobbyDenomination denomination;

        public Hobby(HobbyDenomination denomination)
        {
            this.denomination = denomination;
        }

        public static string mapHobby(HobbyDenomination denomination)
        {
            switch (denomination)
            {
                case HobbyDenomination.Film:
                    return "Movies";
                case HobbyDenomination.Books:
                    return "Books";
                case HobbyDenomination.Games:
                    return "Games";
                case HobbyDenomination.Football:
                    return "Football";
                case HobbyDenomination.Fashion:
                    return "Fashion";
            }
            return "";
        }
    }

    #endregion

}