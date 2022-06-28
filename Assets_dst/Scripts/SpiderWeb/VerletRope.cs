using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VerletRope
{
    public class Dot {
        public Vector3 CurrentPosition { get; set; }
        public Vector3 LastPosition { get; set; }
        public bool isStatic { get; set; }
        public List<Conections> conections { get; } = new List<Conections>();

        public Dot(Vector3 atualPosition, bool isStatic)
        {
            this.CurrentPosition = atualPosition;
            this.LastPosition = atualPosition;
            this.isStatic = isStatic; 
        }

        public static Conections Connect(Dot dotA, Dot dotB, float length = -1f) {
            Conections conections = length < 0f ?
                new Conections(dotA, dotB) :
                new Conections(dotA, dotB, length);
            dotA.conections.Add(conections);
            dotB.conections.Add(conections);
            return conections;
        }
        public static void Disconnect(Conections conections) {

            List<Conections> dotAConections = conections.DotA.conections;
            List<Conections> dotBConections = conections.DotB.conections;

            if (dotAConections.Contains(conections)) dotAConections.Remove(conections);
            if (dotBConections.Contains(conections)) dotBConections.Remove(conections);

        
        }
    }
}
