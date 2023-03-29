using iffnsStuff.iffnsBaseSystemForUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace iffnsStuff.iffnsCastleBuilder
{
    

    public class DefaultCastles : MonoBehaviour
    {
        [SerializeField] List<TextAsset> CastleFiles; 

        public void Setup()
        {
            foreach (TextAsset file in CastleFiles)
            {
                List<string> content = new(Regex.Split(file.text, MyStringComponents.newLine));

                StaticSaveAndLoadSystem.internalSaveData.Add(file.name, content);
            }
        }
    }
}