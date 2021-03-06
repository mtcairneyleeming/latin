﻿namespace database
{
    public enum Part
    {
        Noun = 1,
        Verb = 2,

        Adjective = 3,
        Conjunction = 4,
        Numeral = 5,
        Preposition = 6,
        Pronoun = 7,
        Adverb = 8,
        Interrogative = 9,
        Interjection = 10,
        Participle = -1,
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
        First = 1,
        Second,
        Third,
        Fourth,
        Irregular = 0
    }

    public enum Category
    {
        OneD = 1,
        TwoD,
        ThreeD,
        FourD,
        FiveD,
        TwoOneTwo,
        IrrD,
        OneC,
        TwoC,
        ThreeC,
        FourC,
        IrrC
    }


    // Nouns/adjs/pronouns/participles
    public enum Gender
    {
        Masculine = 1,
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

    public enum Voice
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