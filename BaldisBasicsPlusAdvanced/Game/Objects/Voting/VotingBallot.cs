using BaldisBasicsPlusAdvanced.Cache;
using BaldisBasicsPlusAdvanced.Game.NPCs.Behavior.States.Navigation;
using BaldisBasicsPlusAdvanced.Game.Objects.Texts;
using BaldisBasicsPlusAdvanced.Helpers;
using BaldisBasicsPlusAdvanced.Patches;
using MTM101BaldAPI;
using UnityEngine;

namespace BaldisBasicsPlusAdvanced.Game.Objects.Voting
{
    public class VotingBallot : MonoBehaviour, IPrefab
    {
        [SerializeField]
        private MeshRenderer[] renderers;

        [SerializeField]
        private AudioManager audMan;

        [SerializeField]
        private TimerText timer;

        [SerializeField]
        private VotingPickup votingPickup;

        [SerializeField]
        private Material sideMat;

        [SerializeField]
        private Material signSideMat;

        [SerializeField]
        private Material topMat;

        [SerializeField]
        private Material topFulledMat;

        private EnvironmentController ec;

        public AudioManager AudMan => audMan;

        public void InitializePrefab()
        {
            audMan = gameObject.AddComponent<PropagatedAudioManager>();

            renderers = new MeshRenderer[6];

            sideMat = new Material(AssetsStorage.materials["belt"]);
            signSideMat = new Material(AssetsStorage.materials["belt"]);
            topMat = new Material(AssetsStorage.materials["belt"]);
            topFulledMat = new Material(AssetsStorage.materials["belt"]);

            sideMat.mainTexture = AssetsStorage.textures["adv_ballot_front"];
            signSideMat.mainTexture = AssetsStorage.textures["adv_ballot_front_voting"];
            topMat.mainTexture = AssetsStorage.textures["adv_ballot_empty_top"];
            topFulledMat.mainTexture = AssetsStorage.textures["adv_ballot_top"];

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(15f, 10f, 15f);
            collider.center = new Vector3(0f, 5f, 0f);

            InitializeRenderers();

            transform.localScale = Vector3.one * 0.4f;

            timer = ObjectsCreator.CreateBobTimerText(null);
            timer.transform.parent.transform.position = Vector3.up * 7f;
            timer.transform.parent.transform.parent = transform;

            votingPickup = ObjectsCreator.CreateCustomPickup<VotingPickup>(null, 0, Vector3.up * 5f);
            votingPickup.transform.parent = transform;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent(out NPC npc))
            {
                NavigationState navState = ReflectionHelper.GetValue<NavigationState>(npc.navigationStateMachine, "currentState");
                if (navState is NavigationState_VotingEvent) {
                    ((NavigationState_VotingEvent)navState).End();

                    if (votingPickup.VotingIsGoing)
                        votingPickup.InsertVote(new VoteData()
                        {
                            isNPC = true,
                            npc = npc,
                            value = Random.Range(0, 2) == 1 ? true : false
                        });
                }
                
            }
        }

        public void Initialize(EnvironmentController ec)
        {
            this.ec = ec;
            timer.Initialize(ec);
            votingPickup.Initialize(this);
        }

        public void OnVotingStarts(float time)
        {
            timer.SetTime(time);
            //votingPickup.gameObject.SetActive(true); //not hidden
            votingPickup.OnVotingStarts();
        }

        public void OnVotingEnds()
        {
            timer.Stop();
            //votingPickup.gameObject.SetActive(false); //don't hide
            votingPickup.OnVotingEnds();
        }

        private void InitializeRenderers()
        {
            //CreateRenderer(Vector3.zero, Vector3.right * 90f, sideMat, 0);
            CreateRenderer(new Vector3(-5f, 5f, 0f), new Vector3(0f, 90f, 0f), sideMat, 1);
            CreateRenderer(new Vector3(5f, 5f, 0f), new Vector3(0f, 270f, 0f), sideMat, 2);
            CreateRenderer(new Vector3(0f, 5f, 5f), new Vector3(0f, 180f, 0f), signSideMat, 3);
            CreateRenderer(new Vector3(0f, 5f, -5f), new Vector3(0f, 0f, 0f), sideMat, 4);
            CreateRenderer(new Vector3(0f, 10f, 0f), new Vector3(90f, 0f, 0f), topMat, 5);
        }

        private void CreateRenderer(Vector3 pos, Vector3 angles, Material mat, int index)
        {
            GameObject childObj = new GameObject("Renderer" + index);
            childObj.transform.parent = transform;
            childObj.transform.localPosition = pos;
            childObj.transform.localScale = new Vector3(10f, 10f, 1f);

            Quaternion rotation = childObj.transform.rotation;
            rotation.eulerAngles = angles;
            childObj.transform.rotation = rotation;

            renderers[index] = childObj.AddComponent<MeshRenderer>();
            renderers[index].material = mat;

            MeshFilter meshFilter = childObj.AddComponent<MeshFilter>();
            meshFilter.mesh = AssetsStorage.meshes["quad"];
        }
    }
}
