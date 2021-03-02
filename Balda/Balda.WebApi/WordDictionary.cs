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
        public IEnumerable<string> Words { get; set; }
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
            => dictionary.Value?.Words.Contains(word) ?? false;
    }
}