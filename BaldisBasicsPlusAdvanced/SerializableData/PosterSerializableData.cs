using System;
using MTM101BaldAPI.UI;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.SerializableData
{
    [JsonObject(MemberSerialization.Fields)]
    public class PosterSerializableData
    {

        private class PosterText
        {

            public string textKey;

            public IntVector2 position;

            public IntVector2 size;

            public string font;

            public int fontSize;

            public Color color;

            public string style;

            public string alignment;

            public PosterTextData Convert()
            {
                PosterTextData data = new PosterTextData();

                data.textKey = textKey;
                data.position = position;
                data.size = size;
                data.font = ((BaldiFonts)Enum.Parse(typeof(BaldiFonts), font)).FontAsset();

                if (fontSize <= 0)
                {
                    data.fontSize = (int)((BaldiFonts)Enum.Parse(typeof(BaldiFonts), font)).FontSize();
                }
                
                data.color = color;
                data.style = (FontStyles)Enum.Parse(typeof(FontStyles), style);
                data.alignment = (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), alignment);

                return data;
            }

        }

        public int weight;

        public string[] fonts;

        public string[] alignments;

        private PosterText[] texts;

        [JsonIgnore]
        private PosterTextData[] posterTextDatas;

        public PosterTextData[] Texts => posterTextDatas;

        public void ConvertTextsToGameStandard()
        {
            posterTextDatas = new PosterTextData[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                posterTextDatas[i] = texts[i].Convert();
            }
            texts = null;
        }

    }
}
