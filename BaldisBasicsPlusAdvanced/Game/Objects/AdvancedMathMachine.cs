using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects
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
        private int maxMulAnswer;

        [SerializeField]
        private WeightedSelection<SoundObject>[] praiseSounds;

        private bool isCorrectSolved;

        public void InitializePrefab()
        {
            ReflectionHelper.SetValue<MonoBehaviour>(GetComponentInChildren<ClickableLink>(), "link", this);

            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();

            Material[] materials = meshRenderer.materials;

            materials[0].SetMainTexture(AssetsStorage.textures["adv_advanced_math_machine_face"]);

            ReflectionHelper.GetValue<Material>(this, "correctMat")
                .SetMainTexture(AssetsStorage.textures["adv_advanced_math_machine_face_right"]);
            ReflectionHelper.GetValue<Material>(this, "incorrectMat")
                .SetMainTexture(AssetsStorage.textures["adv_advanced_math_machine_face_wrong"]);

            minDivVal1 = 0;
            maxDivVal1 = 9;

            minDivVal2 = 1;
            maxDivVal2 = 9;

            minMulVal1 = 0;
            maxMulVal1 = 9;

            minMulVal2 = 0;
            maxMulVal2 = 9;

            maxMulAnswer = 9;

            baldiPause = 10f; //thanks Mystman

            praiseSounds = new WeightedSelection<SoundObject>[]
            {
                new WeightedSelection<SoundObject>()
                {
                    selection = AssetsStorage.sounds["adv_bal_great_job"],
                    weight = 100
                },
                new WeightedSelection<SoundObject>()
                {
                    selection = AssetsStorage.sounds["adv_bal_fantastic"],
                    weight = 100
                },
                new WeightedSelection<SoundObject>()
                {
                    selection = AssetsStorage.sounds["adv_bal_incredible"],
                    weight = 100
                }
            };
        }

        void IClickable<int>.Clicked(int player) {
            base.Clicked(player);
            if (isCorrectSolved)
            {
                foreach (NPC npc in room.ec.Npcs)
                {
                    if (npc.Character == Character.Baldi)
                    {
                        Baldi baldi = npc.GetComponent<Baldi>();
                        if (baldi.behaviorStateMachine.currentState is Baldi_Praise)
                        {
                            ReflectionHelper.GetValue<AudioManager>(baldi, "audMan")
                                .FlushQueue(true); //baldi shouldn't talk as propagated object
                        }
                    }
                }
                room.ec.GetAudMan()
                    .PlaySingle(WeightedSelection<SoundObject>.RandomSelection(praiseSounds));
            }
        }


        public override void Completed(int player, bool correct, Activity activity)
        {
            base.Completed(player, correct, activity);
            isCorrectSolved = correct;
        }

        public override void ReInit()
        {
            base.ReInit();
            TMP_Text signText = ReflectionHelper.GetValue<TMP_Text>(this, "signText");
            TMP_Text val1Text = ReflectionHelper.GetValue<TMP_Text>(this, "val1Text");
            TMP_Text val2Text = ReflectionHelper.GetValue<TMP_Text>(this, "val2Text");

            int val1 = 0;
            int val2 = 0;

            int answer = -1;

            bool isDividing = Random.Range(0, 2) == 1;

            if (isDividing)
            {
                signText.text = "/";

                val1 = Random.Range(minDivVal1, maxDivVal1 + 1);

                val2 = Random.Range(minDivVal2, maxDivVal2 + 1);

                if (val1 % val2 != 0)
                {
                    List<int> potentialNumbers = new List<int>();
                    for (int n = minDivVal2; n < val1 + 1; n++)
                    {
                        potentialNumbers.Add(n);
                    }

                    while (val1 % val2 != 0 && potentialNumbers.Count > 0)
                    {
                        int index = Random.Range(0, potentialNumbers.Count);
                        if (val1 % potentialNumbers[index] == 0)
                        {
                            val2 = potentialNumbers[index];
                            break;
                        } else
                        {
                            potentialNumbers.RemoveAt(index);
                        }
                    }
                }
                answer = val1 / val2;

            } else
            {
                signText.text = "*";

                val1 = Random.Range(minMulVal1, maxMulVal1 + 1);

                val2 = Random.Range(minMulVal2, maxMulVal2 + 1);

                if (val1 * val2 > maxMulAnswer)
                {
                    List<int> potentialNumbers = new List<int>();
                    for (int n = minMulVal2; n < maxMulVal2 + 1; n++)
                    {
                        potentialNumbers.Add(n);
                    }

                    while (val1 * val2 > maxMulAnswer && potentialNumbers.Count > 0)
                    {
                        int index = Random.Range(0, potentialNumbers.Count);
                        if (val1 * potentialNumbers[index] <= maxMulAnswer)
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

            }

            val1Text.text = val1.ToString();
            val2Text.text = val2.ToString();

            ReflectionHelper.SetValue<int>(this, "answer", answer);
            ReflectionHelper.SetValue<bool>(this, "addition", false);
        }

    }
}
