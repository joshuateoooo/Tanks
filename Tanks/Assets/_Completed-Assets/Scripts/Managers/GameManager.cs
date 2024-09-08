using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Complete
{
    public class GameManager : MonoBehaviour
    {
        public int m_NumRoundsToWin = 5;
        public float m_StartDelay = 3f;
        public float m_EndDelay = 3f;
        public CameraControl m_CameraControl;
        public Text m_MessageText;
        public GameObject m_PlayerTankPrefab;
        public GameObject m_AITankPrefab;
        public Transform m_PlayerSpawnPoint;
        public Transform m_AISpawnPoint;
        public Color m_PlayerColor = Color.blue;
        public Color m_AIColor = Color.red;

        private int m_RoundNumber;
        private WaitForSeconds m_StartWait;
        private WaitForSeconds m_EndWait;
        private TankManager m_PlayerTank;
        private TankManager m_AITank;
        private TankManager m_RoundWinner;
        private TankManager m_GameWinner;

        private void Start()
        {
            m_StartWait = new WaitForSeconds(m_StartDelay);
            m_EndWait = new WaitForSeconds(m_EndDelay);

            SpawnTanks();
            SetCameraTargets();

            StartCoroutine(GameLoop());
        }

        private void SpawnTanks()
        {
            m_PlayerTank = new TankManager();
            m_AITank = new TankManager();

            m_PlayerTank.m_Instance = Instantiate(m_PlayerTankPrefab, m_PlayerSpawnPoint.position, m_PlayerSpawnPoint.rotation);
            m_PlayerTank.m_PlayerNumber = 1;
            m_PlayerTank.m_SpawnPoint = m_PlayerSpawnPoint;
            m_PlayerTank.Setup();
            SetTankColor(m_PlayerTank.m_Instance, m_PlayerColor);

            m_AITank.m_Instance = Instantiate(m_AITankPrefab, m_AISpawnPoint.position, m_AISpawnPoint.rotation);
            m_AITank.m_PlayerNumber = 2;
            m_AITank.m_SpawnPoint = m_AISpawnPoint;
            m_AITank.Setup();
            SetTankColor(m_AITank.m_Instance, m_AIColor);

            // Set both tanks to inactive initially
            m_PlayerTank.m_Instance.SetActive(false);
            m_AITank.m_Instance.SetActive(false);
        }

        private void SetTankColor(GameObject tank, Color color)
        {
            if (tank != null)
            {
                MeshRenderer[] renderers = tank.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].material.color = color;
                }
            }
        }

        private void SetCameraTargets()
        {
            Transform[] targets = new Transform[2];
            targets[0] = m_PlayerTank.m_Instance.transform;
            targets[1] = m_AITank.m_Instance.transform;

            m_CameraControl.m_Targets = targets;
        }

        private IEnumerator GameLoop()
        {
            yield return StartCoroutine(RoundStarting());
            yield return StartCoroutine(RoundPlaying());
            yield return StartCoroutine(RoundEnding());

            if (m_GameWinner != null)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                StartCoroutine(GameLoop());
            }
        }

        private IEnumerator RoundStarting()
        {
            ResetTanks();
            DisableTankControl();

            m_CameraControl.SetStartPositionAndSize();

            m_RoundNumber++;
            m_MessageText.text = "ROUND " + m_RoundNumber;

            yield return m_StartWait;

            // Activate tanks at the start of each round
            m_PlayerTank.m_Instance.SetActive(true);
            m_AITank.m_Instance.SetActive(true);
        }

        private IEnumerator RoundPlaying()
        {
            EnableTankControl();

            m_MessageText.text = string.Empty;

            while (!OneTankLeft())
            {
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            DisableTankControl();

            m_RoundWinner = null;
            m_RoundWinner = GetRoundWinner();

            if (m_RoundWinner != null)
                m_RoundWinner.m_Wins++;

            m_GameWinner = GetGameWinner();

            string message = EndMessage();
            m_MessageText.text = message;

            yield return m_EndWait;
        }

        private bool OneTankLeft()
        {
            int numTanksLeft = 0;

            if (m_PlayerTank.m_Instance.activeSelf)
                numTanksLeft++;
            if (m_AITank.m_Instance.activeSelf)
                numTanksLeft++;

            return numTanksLeft <= 1;
        }

        private TankManager GetRoundWinner()
        {
            if (m_PlayerTank.m_Instance.activeSelf)
                return m_PlayerTank;
            else if (m_AITank.m_Instance.activeSelf)
                return m_AITank;
            
            return null;
        }

        private TankManager GetGameWinner()
        {
            if (m_PlayerTank.m_Wins == m_NumRoundsToWin)
                return m_PlayerTank;
            else if (m_AITank.m_Wins == m_NumRoundsToWin)
                return m_AITank;
            
            return null;
        }

        private string EndMessage()
        {
            string message = "DRAW!";

            if (m_RoundWinner != null)
                message = m_RoundWinner == m_PlayerTank ? "PLAYER WINS THE ROUND!" : "AI WINS THE ROUND!";

            message += "\n\n\n\n";

            message += ColoredPlayerText(true, m_PlayerColor) + ": " + m_PlayerTank.m_Wins + " WINS\n";
            message += ColoredPlayerText(false, m_AIColor) + ": " + m_AITank.m_Wins + " WINS\n";

            if (m_GameWinner != null)
                message = m_GameWinner == m_PlayerTank ? "PLAYER WINS THE GAME!" : "AI WINS THE GAME!";

            return message;
        }

        private string ColoredPlayerText(bool isPlayer, Color color)
        {
            string colorHex = ColorUtility.ToHtmlStringRGB(color);
            return "<color=#" + colorHex + ">" + (isPlayer ? "PLAYER" : "AI") + "</color>";
        }

        private void ResetTanks()
        {
            if (m_PlayerTank != null && m_PlayerTank.m_Instance != null)
            {
                m_PlayerTank.Reset();
                m_PlayerTank.m_Instance.SetActive(false);
            }

            if (m_AITank != null && m_AITank.m_Instance != null)
            {
                m_AITank.Reset();
                m_AITank.m_Instance.SetActive(false);
            }
        }

        private void EnableTankControl()
        {
            if (m_PlayerTank != null)
            {
                m_PlayerTank.EnableControl();
            }
            // Enable AI control here
        }

        private void DisableTankControl()
        {
            if (m_PlayerTank != null)
            {
                m_PlayerTank.DisableControl();
            }
            // Disable AI control here
        }
    }
}