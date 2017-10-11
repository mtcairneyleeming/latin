namespace LatinAutoDecline
{
    public enum Declension
    {
        One = 1,
        Two = 2,
        TwoREnd = -2,
        Three = 3,
        ThreeIStem = -3,
        Four = 4,
        Five = 5,
        Irregular = -1
    }
    public enum Conjugation { }

    // Nouns/adjs/pronouns/participles
    public enum Gender
    {
        Masculine,
        Feminine,
        Neuter
    }

    public enum Number
    {
        Singular = 1,
        Plural
    }

    public enum Case
    {
        Nominative,
        Accusative,
        Genitive,
        Dative,
        Ablative,
        Vocative,
    }

    public enum Person
    {
        First =1,
        Second =2,
        Third =3
    }

    public enum Tense
    {
        Present,
        Future,
        Imperfect,
        Perfect,
        FuturePerfect,
        Pluperfect
    }

    public enum Voice
    {
        Active,
        Passive,
    }

    public enum Mood
    {
        Indicative,
        Subjunctive
    }
}
