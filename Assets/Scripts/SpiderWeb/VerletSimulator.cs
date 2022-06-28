using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VerletRope
{
    public class VerletSimulator : MonoBehaviour
    {
        public List<Dot> Dots { get; } = new List<Dot>();

        private Vector3 curretForce = Vector3.zero;

        private readonly float mass;
        private int interations;

        public VerletSimulator(float mass, int interations)
        {
            this.mass = mass;
            this.interations = interations;
        }

        public void AddForce(Vector3 force) {
            curretForce += force;
        }

        public void Simulate(float deltaTime)
        {
            ApplyPhisicsToDots(deltaTime);
            ConstraintLenght();
        
        }

        private void ConstraintLenght()
        {
            for (int i = 0; i < interations; i++) 
            { 
                foreach (Dot dotA in Dots) 
                {
                    foreach (Conections conection in dotA.conections)
                    {
                        Dot dotB = conection.other(dotA);
                        Vector3 center = (dotA.CurrentPosition + dotB.CurrentPosition) / 2f;
                        Vector3 direction = (dotA.CurrentPosition - dotB.CurrentPosition).normalized;
                        Vector3 connectionSize = direction * conection.Length / 2f;

                        if (!dotA.isStatic) dotA.CurrentPosition = center + connectionSize;
                        if (!dotB.isStatic) dotB.CurrentPosition = center - connectionSize;

                    }

                }
            }
        }

        private void ApplyPhisicsToDots(float deltaTime)
        {
            float squareDeltaTime = deltaTime * deltaTime;
            Vector3 acceleration = curretForce / mass;
            Vector3 positionVariation = acceleration * squareDeltaTime;

            foreach (Dot dot in Dots) {
                if (dot.isStatic) continue;
                Vector3 oldPosition = dot.CurrentPosition;

                dot.CurrentPosition += dot.CurrentPosition - dot.LastPosition;
                dot.CurrentPosition +=  positionVariation;
                dot.LastPosition = oldPosition;
                
            }
            curretForce = Vector3.zero;
        }
    }

}
