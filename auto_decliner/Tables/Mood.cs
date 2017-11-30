namespace LatinAutoDecline.Tables
{
    public class Mood
    {
        public readonly Voice<Tense> Active;
        public readonly Voice<Tense> Passive;

        public Mood(Voice<Tense> active, Voice<Tense> passive)
        {
            Active = active;
            Passive = passive;
        }

        public override string ToString()
        {
            return $"{nameof(Active)}: {Active}, {nameof(Passive)}: {Passive}";
        }
    }
}