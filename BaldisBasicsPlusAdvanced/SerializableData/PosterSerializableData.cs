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

            public IntVector2? position;

            public IntVector2? size;

            public string font;

            public int fontSize;

            public Color? color;

            public string style;

            public string alignment;

            public PosterTextData Convert()
            {
                PosterTextData data = new PosterTextData();

                data.textKey = textKey;
                if (position != null) data.position = (IntVector2)position;
                if (size != null) data.size = (IntVector2)size;

                if (string.IsNullOrEmpty(font)) throw new Exception("Font name of the poster is empty or missing!");

                data.font = ((BaldiFonts)Enum.Parse(typeof(BaldiFonts), font)).FontAsset();

                if (fontSize <= 0)
                {
                    data.fontSize = (int)((BaldiFonts)Enum.Parse(typeof(BaldiFonts), font)).FontSize();
                }
                else data.fontSize = fontSize;

                data.color = (Color)color;
                if (!string.IsNullOrEmpty(style)) data.style = 
                        (FontStyles)Enum.Parse(typeof(FontStyles), style);
                if (!string.IsNullOrEmpty(alignment)) data.alignment = 
                        (TextAlignmentOptions)Enum.Parse(typeof(TextAlignmentOptions), alignment);

                return data;
            }

        }

        public int weight;

        private PosterText[] texts;

        [JsonIgnore]
        private PosterTextData[] posterTextDatas;

        public PosterTextData[] Texts => posterTextDatas;

        public void ConvertTextsToGameStandard()
        {
            if (texts == null) return;
            posterTextDatas = new PosterTextData[texts.Length];
            for (int i = 0; i < texts.Length; i++)
            {
                posterTextDatas[i] = texts[i].Convert();
            }
            texts = null;
        }

    }
}
