namespace VerletRope
{
    public class Conections
    {
        public Dot DotA { get; }

        public Dot DotB { get; }

        public float Length { get; }

        public Conections(Dot dotA, Dot dotB, float length)
        {
            DotA = dotA;
            DotB = dotB;
            Length = length;
        }

        public Conections(Dot dotA, Dot dotB)
        {
            DotA = dotA;
            DotB = dotB;
            Length = (dotA.CurrentPosition - dotB.CurrentPosition).magnitude;
        }

        public Dot other(Dot dot) => dot == DotA ? DotB : DotA;
    }
}
