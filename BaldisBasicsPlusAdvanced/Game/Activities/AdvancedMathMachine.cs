using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Extensions;
using BaldisBasicsPlusAdvanced.Helpers;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
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

        [SerializeField]
        private WeightedSelection<SoundObject>[] praiseSounds;

        private bool isCorrectlySolved;

        public void InitializePrefab(int variant)
        {
            ReflectionHelper.SetValue<MonoBehaviour>(GetComponentInChildren<ClickableLink>(), "link", this);

            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

            Material[] materials = meshRenderer.materials;

            materials[0] = new Material(AssetStorage.materials["math_front_normal"]);
            materials[1] = new Material(AssetStorage.materials["math_side"]);
            materials[2] = materials[1];

            materials[0].SetMainTexture(
                AssetHelper.TextureFromFile("Textures/Activities/AdvancedMathMachine/Adv_AdvancedMathMachine_Front.png"));
            materials[1].SetMainTexture(
                AssetHelper.TextureFromFile("Textures/Activities/AdvancedMathMachine/Adv_AdvancedMathMachine_Side.png"));

            meshRenderer.materials = materials;

            ReflectionHelper.SetValue(this, "correctMat",
                new Material(ReflectionHelper.GetValue<Material>(this, "correctMat")));
            ReflectionHelper.SetValue(this, "incorrectMat",
                new Material(ReflectionHelper.GetValue<Material>(this, "incorrectMat")));
            ReflectionHelper.SetValue(this, "defaultMat",
                new Material(ReflectionHelper.GetValue<Material>(this, "defaultMat")));

            ReflectionHelper.GetValue<Material>(this, "correctMat")
                .SetMainTexture(AssetHelper.TextureFromFile(
                    "Textures/Activities/AdvancedMathMachine/Adv_AdvancedMathMachine_Front_Correct.png"));
            ReflectionHelper.GetValue<Material>(this, "incorrectMat")
                .SetMainTexture(AssetHelper.TextureFromFile(
                    "Textures/Activities/AdvancedMathMachine/Adv_AdvancedMathMachine_Front_Wrong.png"));
            ReflectionHelper.GetValue<Material>(this, "defaultMat")
                .SetMainTexture(materials[0].mainTexture);

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

            baldiPause = 7f; //Thanks Mystman that you made it as field, not something more hardcoded

            praiseSounds = new WeightedSelection<SoundObject>[]
            {
                new WeightedSelection<SoundObject>()
                {
                    selection = AssetStorage.sounds["adv_bal_great_job"],
                    weight = 100
                },
                new WeightedSelection<SoundObject>()
                {
                    selection = AssetStorage.sounds["adv_bal_fantastic"],
                    weight = 100
                },
                new WeightedSelection<SoundObject>()
                {
                    selection = AssetStorage.sounds["adv_bal_incredible"],
                    weight = 100
                }
            };
        }

        void IClickable<int>.Clicked(int player) {
            Clicked(player);
            if (isCorrectlySolved)
            {
                bool baldiFound = false;
                foreach (NPC npc in room.ec.Npcs)
                {
                    if (npc.Character == Character.Baldi)
                    {
                        Baldi baldi = npc.GetComponent<Baldi>();
                        if (baldi.behaviorStateMachine.currentState is Baldi_Praise)
                        {
                            ReflectionHelper.GetValue<AudioManager>(baldi, "audMan")
                                .FlushQueue(true); //Baldi shouldn't talk as propagated audio man
                            baldiFound = true;
                        }
                    }
                }
                if (baldiFound) room.ec.GetAudMan()
                    .PlaySingle(WeightedSelection<SoundObject>.RandomSelection(praiseSounds));
                isCorrectlySolved = false;
            }
        }


        public override void Completed(int player, bool correct)
        {
            base.Completed(player, correct);
            isCorrectlySolved = correct;
        }

        internal void GenerateProblem()
        {
            TMP_Text signText = ReflectionHelper.GetValue<TMP_Text>(this, "signText");
            TMP_Text val1Text = ReflectionHelper.GetValue<TMP_Text>(this, "val1Text");
            TMP_Text val2Text = ReflectionHelper.GetValue<TMP_Text>(this, "val2Text");
            TMP_Text answerText = ReflectionHelper.GetValue<TMP_Text>(this, "answerText");

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

            ReflectionHelper.SetValue(this, "answer", answer);
            ReflectionHelper.SetValue(this, "addition", false);
        }

    }
}
