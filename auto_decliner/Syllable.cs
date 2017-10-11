using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LatinAutoDecline
{
  
    //
    // Original JS code:
    // Author(s):
    // Fr. Matthew Spencer, OSJ <mspencer@osjusa.org>
    //
    // Copyright (c) 2008-2016 Fr. Matthew Spencer, OSJ
    //
    // Permission is hereby granted, free of charge, to any person obtaining a copy
    // of this software and associated documentation files (the "Software"), to deal
    // in the Software without restriction, including without limitation the rights
    // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    // copies of the Software, and to permit persons to whom the Software is
    // furnished to do so, subject to the following conditions:
    //
    // The above copyright notice and this permission notice shall be included in
    // all copies or substantial portions of the Software.
    //
    // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    // THE SOFTWARE.
    //


    /**
     * @class
     */

 

    /**
     * @class
     */
    public class LatinSyllablifier
    {
        // fixme: ui is only diphthong in the exceptional cases below (according to Wheelock's Latin)
        private List<string> _diphthongs = new List<string> { "ae", "au", "oe", "aé", "áu", "oé" };
        // for centering over the vowel, we will need to know any combinations that might be diphthongs:
        private List<string> _possibleDiphthongs = new List<string> { "ei", "eu", "ui", "éi", "éu", "úi" };
        private Dictionary<string, String[]> _wordExceptions = new Dictionary<string, string[]>(){
            // ui combos pronounced as diphthongs
            { "huius", new[]{"hui", "us"}},
            {"cuius", new []{"cui", "us"} },
            {"huic", new []{"huic"} },
            {"hui", new []{"hui"} },
            // eu combos pronounced as diphthongs
            {"euge", new []{"eu", "ge"} },
            {"seu", new []{"seu"} },
        };
        private List<char> _vowels = new List<char>() {'a', 'e', 'i', 'o', 'u',
            'á', 'é', 'í', 'ó', 'ú',
            'æ', 'œ',
            'ǽ',  // no accented œ in unicode?
            'y'}; // y is treated as a vowel; not native to Latin but useful for words borrowed from Greek;
        private List<char> _vowelsThatMightBeConsonants = new List<char> { 'i', 'u' };
        private List<char> _muteConsonantsAndF = new List<char> { 'b', 'c', 'd', 'g', 'p', 't', 'f' };

        private List<char> _liquidConsonants = new List<char> { 'l', 'r' };

        /**
         * @constructs
         */
        public LatinSyllablifier()
        {

            _possibleDiphthongs.AddRange(_diphthongs);

        }


        // c must be lowercase!
        bool IsVowel(char c)
        {
            for (int i = 0, end = _vowels.Count; i < end; i++)
                if (_vowels[i] == c)
                    return true;

            return false;
        }

        bool IsVowelThatMightBeConsonant(char c)
        {
            for (int i = 0, end = _vowelsThatMightBeConsonants.Count; i < end; i++)
                if (_vowelsThatMightBeConsonants[i] == c)
                    return true;

            return false;
        }

        // Substring should be a vowel and the character following
        bool IsVowelActingAsConsonant(string substring)
        {
            return IsVowelThatMightBeConsonant(substring[0]) && IsVowel(substring[1]);
        }

        /**
         * f is not a mute consonant, but we lump it together for syllabification
         * since it is syntactically treated the same way
         *
         * @param {String} c The character to test; must be lowercase
         * @return {boolean} true if c is an f or a mute consonant
         */
        bool IsMuteConsonantOrF(char c)
        {
            for (int i = 0, end = _muteConsonantsAndF.Count; i < end; i++)
                if (_muteConsonantsAndF[i] == c)
                    return true;

            return false;
        }

        /**
         *
         * @param {String} c The character to test; must be lowercase
         * @return {boolean} true if c bool is a liquid consonant
         */
        bool IsLiquidConsonant(char c)
        {
            for (int i = 0, end = _liquidConsonants.Count; i < end; i++)
                if (_liquidConsonants[i] == c)
                    return true;

            return false;
        }

        /**
         *
         * @param {String} s The string to test; must be lowercase
         * @return {boolean} true if s is a diphthong
         */
        bool IsDiphthong(string s)
        {
            for (int i = 0, end = _diphthongs.Count; i < end; i++)
                if (_diphthongs[i] == s)
                    return true;

            return false;
        }

        /**
         *
         * @param {String} s The string to test; must be lowercase
         * @return {boolean} true if s is a diphthong
         */
        bool IsPossibleDiphthong(string s)
        {
            for (int i = 0, end = _possibleDiphthongs.Count; i < end; i++)
            {
                if (_possibleDiphthongs[i] == s)
                    return true;
            }

            return false;
        }

        /**
         * Rules for Latin syllabification (from Collins, "A Primer on Ecclesiastical Latin")
         *
         * Divisions occur when:
         *   1. After open vowels (those not followed by a consonant) (e.g., "pi-us" and "De-us")
         *   2. After vowels followed by a single consonant (e.g., "vi-ta" and "ho-ra")
         *   3. After the first consonant when two or more consonants follow a vowel
         *      (e.g., "mis-sa", "minis-ter", and "san-ctus").
         *
         * Exceptions:
         *   1. In compound words the consonants stay together (e.g., "de-scribo").
         *   2. A mute consonant (b, c, d, g, p, t) or f followed by a liquid consonant (l, r)
         *      go with the succeeding vowel: "la-crima", "pa-tris"
         *
         * In addition to these rules, Wheelock's Latin provides this sound exception:
         *   -  Also counted as single consonants are qu and the aspirates ch, ph,
         *      th, which should never be separated in syllabification:
         *      architectus, ar-chi-tec-tus; loquacem, lo-qua-cem.
         *
         */
        public List<string> SyllabifyWord(string word)
        {
            var syllables = new List<string>();
            var haveCompleteSyllable = false;
            var previousWasVowel = false;
            var workingString = word.ToLower();
            var startSyllable = 0;

            char c, lookahead;
            bool haveLookahead = false;

            // a helper function to create syllables
            Action<int> makeSyllable = (int length) =>
            {
                if (haveCompleteSyllable)
                {
                    syllables.Add(word.Substring(startSyllable, length));
                    startSyllable += length;
                }

                haveCompleteSyllable = false;
            };

            for (int i = 0, wordCount = workingString.Length; i < wordCount; i++)
            {

                c = workingString[i];

                // get our lookahead in case we need them...
                lookahead = '*';
                haveLookahead = (i + 1) < wordCount;

                if (haveLookahead)
                    lookahead = workingString[i + 1];

                var cIsVowel = IsVowel(c);

                // i is a special case for a vowel. when i is at the beginning
                // of the word (Iesu) or i is between vowels (alleluia),
                // then the i is treated as a consonant (y)
                if (c == 'i')
                {
                    if (i == 0 && haveLookahead && IsVowel(lookahead))
                        cIsVowel = false;
                    else if (previousWasVowel && haveLookahead && IsVowel(lookahead))
                    {
                        cIsVowel = false;
                    }
                }

                if (c == '-')
                {

                    // a hyphen forces a syllable break, which effectively resets
                    // the logic...

                    haveCompleteSyllable = true;
                    previousWasVowel = false;
                    makeSyllable(i - startSyllable);
                    startSyllable++;

                }
                else if (cIsVowel)
                {

                    // once we get a vowel, we have a complete syllable
                    haveCompleteSyllable = true;

                    if (previousWasVowel && !IsDiphthong(workingString[i - 1] + "" + c))
                    {
                        makeSyllable(i - startSyllable);
                        haveCompleteSyllable = true;
                    }

                    previousWasVowel = true;

                }
                else if (haveLookahead)
                {

                    if ((c == 'q' && lookahead == 'u') ||
                        (lookahead == 'h' && (c == 'c' || c == 'p' || c == 't')))
                    {
                        // handle wheelock's exceptions for qu, ch, ph and th
                        makeSyllable(i - startSyllable);
                        i++; // skip over the 'h' or 'u'
                    }
                    else if (previousWasVowel && IsVowel(lookahead))
                    {
                        // handle division rule 2
                        makeSyllable(i - startSyllable);
                    }
                    else if (IsMuteConsonantOrF(c) && IsLiquidConsonant(lookahead))
                    {
                        // handle exception 2
                        makeSyllable(i - startSyllable);
                    }
                    else if (haveCompleteSyllable)
                    {
                        // handle division rule 3
                        makeSyllable(i + 1 - startSyllable);
                    }

                    previousWasVowel = false;
                }
            }

            // if we have a complete syllable, we can add it as a new one. Otherwise
            // we tack the remaining characters onto the last syllable.
            if (haveCompleteSyllable)
                syllables.Add(word.Substring(startSyllable));
            else if (startSyllable > 0)
                syllables[syllables.Count - 1] += word.Substring(startSyllable);

            return syllables;
        }

        struct VowelSegment
        {
            public bool Found;
            public int StartIndex;
            public int Count;

            public VowelSegment(bool found, int startIndex, int count)
            {
                this.Found = found;
                this.StartIndex = startIndex;
                Count = count;
            }
        }
        /**
         * @param {String} s the string to search
         * @param {PluralOnly} startIndex The index at which to start searching for a vowel in the string
         * @retuns a custom class with three properties: {found: (true/false) startIndex: (start index in s of vowel segment) Count ()}
         */
        
            VowelSegment FindVowelSegment(string s, int startIndex)
        {

            int i, end, index;
            var workingString = s.ToLower();

            // do we have a diphthong?
            for (i = 0, end = _possibleDiphthongs.Count; i < end; i++)
            {
                var d = _possibleDiphthongs[i];
                index = workingString.IndexOf(d, startIndex);

                if (index >= 0)
                    return new VowelSegment(true, index, d.Length );
            }

            // no diphthongs. Let's look for single vowels then...
            for (i = 0, end = _vowels.Count; i < end; i++)
            {
                index = workingString.IndexOf(_vowels[i], startIndex);

                if (index >= 0)
                {
                    // if the first vowel found might also be a consonant (U or I), and it is immediately followed by another vowel, (e.g., sanguis, quis), the first u counts as a consonant:
                    // (in practice, this only affects words such as equus that contain a uu, since the alphabetically earlier vowel would be found before the U)
                    if (IsVowelActingAsConsonant(workingString.Substring(index, 2)))
                    {
                        ++index;
                    }
                    return new VowelSegment(  true,  index,  1 );
                }
            }

            // no vowels sets found after startIndex!
            return new VowelSegment( false, -1, -1 );
        }
    }


   
}

