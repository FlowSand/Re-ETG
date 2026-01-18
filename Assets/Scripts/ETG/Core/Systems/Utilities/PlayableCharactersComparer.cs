using System.Collections.Generic;

#nullable disable

public class PlayableCharactersComparer : IEqualityComparer<PlayableCharacters>
    {
        public bool Equals(PlayableCharacters x, PlayableCharacters y) => x == y;

        public int GetHashCode(PlayableCharacters obj) => (int) obj;
    }

