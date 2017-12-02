namespace decliner.Tables
{
    public class Verb
    {
        public Verb()
        {
            Indicative = new Mood();
            Subjunctive = new Mood();
            Imperative = new Mood();
            Infinitives = new Infinitives();
            Participles = new Participles();
            Gerund = new Gerund();
            Supine = new Supine();
        }

        public Verb(Mood indicative, Mood subjunctive, Mood imperative, Infinitives infinitives,
            Participles participles, Gerund gerund, Supine supine)
        {
            Indicative = indicative;
            Subjunctive = subjunctive;
            Imperative = imperative;
            Infinitives = infinitives;
            Participles = participles;
            Gerund = gerund;
            Supine = supine;
        }

        public Mood Indicative { get; set; }
        public Mood Subjunctive { get; set; }

        /// <summary>
        ///     NB: has a whole variety of tenses left out as they just don't exist
        /// </summary>
        public Mood Imperative { get; set; }

        public Infinitives Infinitives { get; set; }
        public Participles Participles { get; set; }
        public Gerund Gerund { get; set; }
        public Supine Supine { get; set; }
    }

    public class Supine
    {
        public Supine()
        {
        }

        public Supine(string accusative, string ablative)
        {
            Accusative = accusative;
            Ablative = ablative;
        }

        public string Accusative { get; set; }
        public string Ablative { get; set; }
    }

    public class Gerund
    {
        public Gerund()
        {
        }

        public Gerund(string nominative, string accusative, string genitive, string dativeAndAblative)
        {
            Nominative = nominative;
            Accusative = accusative;
            Genitive = genitive;
            DativeAndAblative = dativeAndAblative;
        }

        public string Nominative { get; set; }
        public string Accusative { get; set; }
        public string Genitive { get; set; }
        public string DativeAndAblative { get; set; }
    }

    public class Participles
    {
        public Participles(Voice<NounPluralities> active, Voice<NounPluralities> passive)
        {
            Active = active;
            Passive = passive;
        }

        public Participles()
        {
            Active = new Voice<NounPluralities>();
            Passive = new Voice<NounPluralities>();
        }

        public Voice<NounPluralities> Active { get; set; }
        public Voice<NounPluralities> Passive { get; set; }
    }

    public class Infinitives
    {
        public Infinitives(ActiveInfinitives active, PassiveInfinitives passive)
        {
            Active = active;
            Passive = passive;
        }

        public Infinitives()
        {
            Active = new ActiveInfinitives();
            Passive = new PassiveInfinitives();
        }

        public ActiveInfinitives Active { get; set; }
        public PassiveInfinitives Passive { get; set; }
    }

    public class ActiveInfinitives
    {
        public ActiveInfinitives(string present, string perfect, NounPluralities future)
        {
            Present = present;
            Perfect = perfect;
            Future = future;
        }

        public ActiveInfinitives()
        {
            Present = string.Empty;
            Perfect = string.Empty;
            Future = new NounPluralities();
        }

        public string Present { get; set; }
        public string Perfect { get; set; }
        public NounPluralities Future { get; set; }
    }

    public class PassiveInfinitives
    {
        public NounPluralities Future;
        public NounPluralities Perfect;
        public string Present;

        public PassiveInfinitives(string present, NounPluralities perfect, NounPluralities future)
        {
            Present = present;
            Perfect = perfect;
            Future = future;
        }

        public PassiveInfinitives()
        {
            Present = string.Empty;
            Perfect = new NounPluralities();
            Future = new NounPluralities();
        }
    }
}