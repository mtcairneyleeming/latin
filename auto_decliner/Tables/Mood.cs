namespace decliner.Tables
{
    public class Mood
    {
        public Mood()
        {
            Active = new Voice<Tense>();
            Passive = new Voice<Tense>();
        }

        public Mood(Voice<Tense> active, Voice<Tense> passive)
        {
            Active = active;
            Passive = passive;
        }

        public Voice<Tense> Active { get; set; }
        public Voice<Tense> Passive { get; set; }

        public override string ToString()
        {
            return $"{nameof(Active)}: {Active}, {nameof(Passive)}: {Passive}";
        }
    }
}