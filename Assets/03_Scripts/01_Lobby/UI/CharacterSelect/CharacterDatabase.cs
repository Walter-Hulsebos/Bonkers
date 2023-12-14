using System;
using System.Linq;

using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


#if ODIN_INSPECTOR
[InlineEditor]
#endif
[CreateAssetMenu(fileName = "New Character Database", menuName = "Characters/Database")]
public class CharacterDatabase : ScriptableObject
{
    #if ODIN_INSPECTOR
    [InlineEditor]
    #endif
    [SerializeField] private Character[] characters = new Character[0];

    public Character[] GetAllCharacters() => characters;

    public Character GetCharacterById(Int32 id)
    {
        foreach (Character character in characters)
        {
            if (character.Id == id) { return character; }
        }

        return null;
    }

    public Boolean IsValidCharacterId(Int32 id) { return characters.Any(x => x.Id == id); }
}