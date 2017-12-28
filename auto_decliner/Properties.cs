namespace decliner
{
    public enum Part
    {
        Noun = 1,
        Verb = 2,
        Participle = -1,
        Adjective = 3,
        Adverb = 8,
        Conjunction = 4,
        Preposition = 6,
        Pronoun = 7,
        Numeral = 5,
        Interjection = -2,
        Exclamation = -3
    }

    public enum Declension
    {
        One = 1,
        Two = 2,
        TwoREnd = -2,
        Three = 3,
        ThreeIStem = -3,
        Four = 4,
        Five = 5,
        Irregular = -1,
        TwoOneTwo = -4 // For adjectives that take 2-1-2 endings
    }

    public enum Conjugation
    {
    }


    // Nouns/adjs/pronouns/participles
    public enum Gender
    {
        Masculine,
        Feminine,
        Neuter,
        Indeterminate
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
        Instrumental,
        Locative
    }

    public enum Person
    {
        First = 1,
        Second = 2,
        Third = 3
    }

    public enum Tense
    {
        Present,
        Future,
        Imperfect,
        Perfect,
        FuturePerfect,
        Pluperfect,
        Aorist
    }

    public enum VoiceEnum
    {
        Active,
        Passive,
        Deponent,
        MedioPassive
    }

    public enum Mood
    {
        Indicative,
        Subjunctive,
        Infinitive,
        Imperative,
        Gerundive,
        Supine,
        Gerund,
        Participle
    }

    public enum Degree
    {
        Positive,
        Comparative,
        Superlative
    }
}