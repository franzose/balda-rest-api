using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Balda.WebApi
{
    /// <summary>
    /// The game's dictionary to search the given words in. It is set up once during the startup and is not intended to be used directly but via <see cref="IOptions{WordDictionary}"/>.
    /// </summary>
    public sealed class WordDictionary
    {
        public const string SectionName = "Words";
        public IEnumerable<string> Words { get; set; } = new List<string>();

        /// <summary>
        /// Checks whether the dictionary contains the given word. 
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>True if the dictionary contains the word, false otherwise.</returns>
        public bool Contains(string word) => Words.Contains(word);

        /// <summary>
        /// Picks a random word of the given length from the dictionary.
        /// </summary>
        /// <param name="length">The word length.</param>
        /// <returns>A random word of an empty string if no word was found.</returns>
        public string PickRandomWord(int length)
        {
            var words = Words.Where(w => w.Length == length).ToArray();

            return words.Length == 0 ? "" : words.ElementAt(new Random().Next(0, words.Length));
        }
    }

    public static class WordDictionaryOptionsExtension
    {
        /// <summary>
        /// Check whether the dictionary contains the given word. 
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="word">The word.</param>
        /// <returns>True if the dictionary contains the word, false otherwise.</returns>
        public static bool Contains(this IOptions<WordDictionary> dictionary, string word)
            => dictionary.Value.Contains(word);

        /// <summary>
        /// Picks a random word of the given length from the dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="length">The word length.</param>
        /// <returns>A random word of an empty string if no word was found.</returns>
        public static string PickRandomWord(this IOptions<WordDictionary> dictionary, int length)
            => dictionary.Value.PickRandomWord(length);
    }
}