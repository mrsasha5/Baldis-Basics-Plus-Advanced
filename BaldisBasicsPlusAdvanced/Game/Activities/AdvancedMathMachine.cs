using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using HarmonyLib;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Activities
{
    public class AdvancedMathMachine : MathMachine, IPrefab, IClickable<int>
    {
        [SerializeField]
        private int minDivVal1;

        [SerializeField]
        private int maxDivVal1;

        [SerializeField]
        private int minDivVal2;

        [SerializeField]
        private int maxDivVal2;

        [SerializeField]
        private int minMulVal1;

        [SerializeField]
        private int maxMulVal1;

        [SerializeField]
        private int minMulVal2;

        [SerializeField]
        private int maxMulVal2;

        [SerializeField]
        private int minExponent;

        [SerializeField]
        private int maxExponent;

        [SerializeField]
        private int minExponentBase;

        [SerializeField]
        private int maxExponentBase;

        [SerializeField]
        private int maxAnswer;

        private static FieldInfo _signText = AccessTools.Field(typeof(MathMachine), "signText");
        private static FieldInfo _val1Text = AccessTools.Field(typeof(MathMachine), "val1Text");
        private static FieldInfo _val2Text = AccessTools.Field(typeof(MathMachine), "val2Text");
        private static FieldInfo _answerText = AccessTools.Field(typeof(MathMachine), "answerText");
        private static FieldInfo _answer = AccessTools.Field(typeof(MathMachine), "answer"); 
        private static FieldInfo _addition = AccessTools.Field(typeof(MathMachine), "addition");

        public void InitializePrefab(int variant)
        {
            GetComponentInChildren<ClickableLink>().ReflectionSetValue("link", this);

            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

            Material[] materials = meshRenderer.materials;

            materials[0] = new Material(AssetStorage.materials["math_front_normal"]);
            materials[1] = new Material(AssetStorage.materials["math_side"]);
            materials[2] = materials[1];

            materials[0].SetMainTexture(AssetStorage.textures["AMM_Front"]);
            materials[1].SetMainTexture(AssetStorage.textures["AMM_Side"]);

            meshRenderer.materials = materials;

            // Initializing new materials (to do not affect original ones) & applying new textures 
            this.ReflectionSetValue("correctMat", new Material(this.ReflectionGetValue<Material>("correctMat")));
            this.ReflectionSetValue("incorrectMat", new Material(this.ReflectionGetValue<Material>("incorrectMat")));
            this.ReflectionSetValue("defaultMat", new Material(this.ReflectionGetValue<Material>("defaultMat")));
            this.ReflectionGetValue<Material>("correctMat").SetMainTexture(AssetStorage.textures["AMM_Front_Correct"]);
            this.ReflectionGetValue<Material>("incorrectMat").SetMainTexture(AssetStorage.textures["AMM_Front_Wrong"]);
            this.ReflectionGetValue<Material>("defaultMat").SetMainTexture(materials[0].mainTexture);

            minDivVal1 = 0;
            maxDivVal1 = 9;

            minDivVal2 = 1;
            maxDivVal2 = 9;

            minMulVal1 = 0;
            maxMulVal1 = 9;

            minMulVal2 = 0;
            maxMulVal2 = 9;

            minExponentBase = 0;
            maxExponentBase = 9;
            minExponent = 0;
            maxExponent = 3;

            maxAnswer = 9;

            baldiPause = 7f;
        }

        internal void GenerateProblem()
        {
            TMP_Text signText = _signText.GetValue<TMP_Text>(this);
            TMP_Text val1Text = _val1Text.GetValue<TMP_Text>(this);
            TMP_Text val2Text = _val2Text.GetValue<TMP_Text>(this);
            TMP_Text answerText = _answerText.GetValue<TMP_Text>(this);

            answerText.text = "?";

            int val1 = 0;
            int val2 = 0;

            int answer = -1;

            int operation = UnityEngine.Random.Range(0, 3);

            switch (operation)
            {
                case 0:
                    signText.text = "/";

                    val1 = UnityEngine.Random.Range(minDivVal1, maxDivVal1 + 1);
                    val2 = UnityEngine.Random.Range(minDivVal2, maxDivVal2 + 1);

                    if (val1 % val2 != 0)
                    {
                        List<int> potentialNumbers = new List<int>();
                        for (int n = minDivVal2; n < val1 + 1; n++)
                        {
                            potentialNumbers.Add(n);
                        }

                        while (val1 % val2 != 0 && potentialNumbers.Count > 0)
                        {
                            int index = UnityEngine.Random.Range(0, potentialNumbers.Count);
                            if (val1 % potentialNumbers[index] == 0)
                            {
                                val2 = potentialNumbers[index];
                                break;
                            }
                            else
                            {
                                potentialNumbers.RemoveAt(index);
                            }
                        }
                    }
                    answer = val1 / val2;
                    break;

                case 1:
                    signText.text = "×";

                    val1 = UnityEngine.Random.Range(minMulVal1, maxMulVal1 + 1);
                    val2 = UnityEngine.Random.Range(minMulVal2, maxMulVal2 + 1);

                    if (val1 * val2 > maxAnswer)
                    {
                        List<int> potentialNumbers = new List<int>();

                        for (int n = minMulVal2; n < maxMulVal2 + 1; n++)
                        {
                            potentialNumbers.Add(n);
                        }

                        while (val1 * val2 > maxAnswer && potentialNumbers.Count > 0)
                        {
                            int index = UnityEngine.Random.Range(0, potentialNumbers.Count);
                            if (val1 * potentialNumbers[index] <= maxAnswer)
                            {
                                val2 = potentialNumbers[index];
                                break;
                            }
                            else
                            {
                                potentialNumbers.RemoveAt(index);
                            }
                        }
                    }

                    answer = val1 * val2;
                    break;

                case 2:
                    signText.text = "^";

                    val1 = UnityEngine.Random.Range(minExponentBase, maxExponentBase + 1);
                    val2 = UnityEngine.Random.Range(minExponent, maxExponent + 1);

                    List<int> potentialExponents = new List<int>();

                    for (int n = minExponent; n < maxExponent + 1; n++)
                    {
                        potentialExponents.Add(n);
                    }

                    while (Math.Pow(val1, val2) > maxAnswer || val1 == 0 && val2 == 0)
                        val1 = potentialExponents[UnityEngine.Random.Range(0, potentialExponents.Count)];

                    answer = (int)Math.Pow(val1, val2);
                    break;
            }

            val1Text.text = val1.ToString();
            val2Text.text = val2.ToString();

            _answer.SetValue(this, answer);
            _addition.SetValue(this, false);
        }
    }
}
