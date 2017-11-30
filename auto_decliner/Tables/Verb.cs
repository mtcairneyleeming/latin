using System.Security;

namespace LatinAutoDecline.Tables
{
    public struct Verb
    {
        public readonly Mood Indicative;
        public readonly Mood Subjunctive;

        /// <summary>
        /// NB: has a whole variety of tenses left out as they just don't exist
        /// </summary>
        public readonly Mood Imperative;

        public readonly Infinitives Infinitives;
        public readonly Participles Participles;
        public readonly Gerund Gerund;
        public readonly Supine Supine;
    }

    public struct Supine
    {
        public readonly string Accusative;
        public readonly string Ablative;
    }

    public struct Gerund
    {
        public readonly string Nominative;
        public readonly string Accusative;
        public readonly string Genitive;
        public readonly string DativeAndAblative;
    }

    public struct Participles
    {
        public readonly Voice<NounPluralities> Active;
        public readonly Voice<NounPluralities> Passive;
    }

    public struct Infinitives
    {
        public readonly ActiveInfinitives Active;
        public readonly PassiveInfinitives Passive;
    }

    public struct ActiveInfinitives
    {
        public readonly string Present;
        public readonly string Perfect;
        public readonly NounPluralities Future;
    }

    public struct PassiveInfinitives
    {
        public readonly string Present;
        public readonly NounPluralities Perfect;
        public readonly NounPluralities Future;
    }
}