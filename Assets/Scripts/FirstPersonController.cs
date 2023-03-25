using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
// import TextMeshProUGUI
using TMPro;


namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player Health")]
        [SerializeField] public float maxHealth = 120.0f;
        [SerializeField] public float currentHealth;
        [SerializeField] public float iFrames;
        public GameObject playerCam;
        public GameObject playerDamage;
        public GameObject healthBar;
        public GameObject healthBarFill;
        public Slider healthBarSlider;
        public GameObject iFrameIcon;
        public AIExample enemyManager;

        [Header("Weapon(Guns)")]
        [SerializeField] public float gunBaseDamage = 20;
        [SerializeField] public float gunBaseFireRate = 0.5f;
        [SerializeField] public float gunBaseRange = 200f;
        [SerializeField] public GameObject pauseMenu;

        [Header("Scores")]
        [SerializeField] public int score = 0;
        [SerializeField] public GameObject ScoreUI;
        [SerializeField] public int kills = 0;
        [SerializeField] public int deaths = 0;
        [SerializeField] public int headshots = 0;
        [SerializeField] public int shotsFired = 0;
        [SerializeField] public int shotsHit = 0;
        [SerializeField] public float timeSurvived = 0;
        [SerializeField] public GameObject TimeSurvivedUI;

        [Header("Currency")]
        [SerializeField] public int currency = 0;
        [SerializeField] public int currencyMultiplier = 1;
        [SerializeField] public GameObject CurrencyUI;

        [Header("Movement")]
        public GameObject sprintIcon;
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        public Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        public GameObject damageEffect;

        // Use this for initialization
        private void Start()
        {
            Application.targetFrameRate = 144;
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            currentHealth = maxHealth;
            iFrames = 0f;
            playerDamage.SetActive(false); // disable the damage taking effect
            resetHealthBar();
        }


        // Update is called once per frame
        private void Update()
        {
            if (currentHealth > 0){
                timeSurvived += Time.deltaTime;
            }
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;


            // update player currnecy every frame
            CurrencyUI.GetComponent<TextMeshProUGUI>().text = "$: " + currency + " HKD";

            //update player score every frame
            if(score > 0){
                ScoreUI.GetComponent<TextMeshProUGUI>().text = "Score: " + score;
            } else {
                ScoreUI.GetComponent<TextMeshProUGUI>().text = "Scores: " + score;
            }

            // update player time survived every frame
            if(timeSurvived < 60){
                TimeSurvivedUI.GetComponent<TextMeshProUGUI>().text = "Survived: " + timeSurvived.ToString("F0") + "s";
            } else {
                TimeSurvivedUI.GetComponent<TextMeshProUGUI>().text = "Survived: " + (timeSurvived/60).ToString("F0") + "m" + (timeSurvived%60).ToString("F0") + "s";
            }


            // on each frame, if player has iFrames, reduce them by 1*Time.deltaTime
            if (iFrames > 0){
                iFrames -= 1.0f*Time.deltaTime;
            }

            if(m_IsWalking == false){
                // set opacity of sprint icon to 1
                // else set opacity to 0.1
                sprintIcon.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            } else {
                sprintIcon.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
            }

            if(iFrames > 0f){
                // set opacity of iFrame icon to 1
                // else set opacity to 0.1
                iFrameIcon.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            } else {
                iFrameIcon.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
            }

            if (Input.GetMouseButtonDown(0)) {
                 RaycastHit  hit;
                 Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                  
                  if (Physics.Raycast(ray, out hit)) {
                      if (hit.transform.name == "Zombie" ){
                        Instantiate (damageEffect, hit.point, Quaternion.identity);

                        hit.transform.GetComponent<AIExample>().onHit(25);
                        
                    }
                  }
              }
        

            setHealthBar(currentHealth);
            // less than 30% red, less than 50% orange, 70% yellow and 100% green
            if (currentHealth/maxHealth <= 0.3f){
                healthBarFill.GetComponent<Image>().color = Color.red;
            } else if (currentHealth/maxHealth <= 0.5f){
                healthBarFill.GetComponent<Image>().color = new Color(1.0f, 0.64f, 0.0f);
            } else if (currentHealth/maxHealth <= 0.7f){
                healthBarFill.GetComponent<Image>().color = Color.yellow;
            } else {
                healthBarFill.GetComponent<Image>().color = Color.green;
            }
        }

        public IEnumerator Shake(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                m_Camera.transform.localPosition = m_OriginalCameraPosition + (Random.insideUnitSphere * magnitude);
                elapsed += Time.deltaTime*2;
                yield return 0;
            }
            m_Camera.transform.localPosition = m_OriginalCameraPosition;
        }

        public float takeDamage(float damage, Vector3 source){

            Debug.Log("distance to player: " + Vector3.Distance(source, m_Camera.transform.position ));
            if (iFrames <= 0f && Vector3.Distance(source, m_Camera.transform.position ) < 3.4f){
                Debug.Log("Player took " + damage + " damage");
                currentHealth -= damage;
                setHealthBar(currentHealth);
                StartCoroutine(Shake(0.4f, 0.2f));
                StartCoroutine(playerDamageFlash());
                iFrames = 1.0f;  
            }

            // check if player health is below 0
            if (currentHealth <= 0f){
                // kill player
                currentHealth = 0;
                playerDie();
            }
            return currentHealth;
        }

        void onShoot() {
            RaycastHit hit; 
            if(Physics.Raycast(playerCam.transform.position, transform.forward, out hit, 100f)){
                Debug.Log("hit");
                enemyManager = hit.transform.GetComponent<AIExample>();
                if(enemyManager != null) {
                    enemyManager.onHit(20);
                }
            }

        }

        public void playerDie(){
            // kill player
            Debug.Log("Player is dead");
            // Destroy(gameObject, 1.0f);
        }

        public IEnumerator playerDamageFlash(){
            playerDamage.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            playerDamage.SetActive(false);
        }

        public void resetHealthBar(){
            currentHealth = maxHealth;
            healthBarSlider.value = 1;
        }

        public void setHealthBar(float health){
            healthBarSlider.value = health/maxHealth;
        }

        public void addCurrency(int amount){
            currency += amount;
        }

        public void addScore(int amount){
            score += amount;
        }

        public void removeCurrency(int amount){
            currency -= amount;
        }

        public void removeScore(int amount){
            score -= amount;
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                // add collision checking for hitting something above
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
            // check if pauseMenu is active
            if (pauseMenu.activeSelf == false){
                m_MouseLook.UpdateCursorLock();
            }
            // m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude * 0.3f +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }

        public int GetPlayerStealthProfile()
        {
            if (m_IsWalking)
            {
                return 0;
            } else
            {
                return 1;
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
