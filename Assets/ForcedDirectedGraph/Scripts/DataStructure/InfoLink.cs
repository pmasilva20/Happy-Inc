using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForceDirectedGraph
{
    public class InfoLink
    {
        /// <summary>
        /// Similarity between 2 nodes
        /// Depends on both nodes attributes
        /// </summary>
        private float similarity = 0f;

        public float similairityValue { get { return similarity; } }

        public void setSimilarity(float sim) { similarity = sim; }

        /// <summary>
        /// Represents amount of information between two persons in the social network
        /// Takes into account similarity, desinformation, and if in bubble, any bubble effects
        /// </summary>
        private float informationFlow;

        public float informationFlowValue { get { return informationFlow; } }

        public void setInformationFlow(float info) { informationFlow = info; }

        public InfoLink(float sim)
        {
            this.similarity = sim;
            //It's assumed that if nodes are in a bubble this value will be recalculated in short notice
            this.informationFlow = sim;
        }
    }

}