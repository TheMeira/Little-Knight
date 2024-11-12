using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.PlayerLoop;
using System.Transactions;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<Quest> quests = new List<Quest>();
    private int currentQuestIndex = -1;
    public TextMeshProUGUI descriptionText;

    [SerializeField] private GameObject manaGO;

    [SerializeField] private GameObject wallBoundGO;

    [SerializeField] private GameObject enemyGO;

    public AudioSource sucessSFX;

   


    void Start()
    {
       
        descriptionText = UIManager.instance.tutorialDesc;
        InitializeQuests();
        StartNextQuest();
    }

    private void InitializeQuests()
    {
        // Clear existing quests if any
        quests.Clear();

        MoveQuest moveQuest = new MoveQuest();
        moveQuest.titleQuest = "Exploration";
        moveQuest.descriptionQuest = "Master movement using WASD or Arrow Keys.";

        AttackQuest attackQuest = new AttackQuest();
        attackQuest.titleQuest = "Combat";
        attackQuest.descriptionQuest = "Execute swift attacks with the Left Mouse Button (Light Attack).";

        JumpQuest jumpQuest = new JumpQuest();
        jumpQuest.titleQuest = "Leap";
        jumpQuest.descriptionQuest = "Ascend heights effortlessly by pressing the Space key.";

        ManaQuest manaQuest = new ManaQuest();
        manaQuest.titleQuest = "Resource Gathering";
        manaQuest.descriptionQuest = "Seek out and acquire the vital mana resource.";
        manaQuest.SetManaObject(manaGO);
        manaQuest.SetWallBoundObject(wallBoundGO);

        Attack2Quest attack2Quest = new Attack2Quest();
        attack2Quest.titleQuest = "Power Strike";
        attack2Quest.descriptionQuest = "Unleash devastating heavy attacks with precision using the Right Mouse Button.";

        EnemeyQuest enemyQuest = new EnemeyQuest();
        enemyQuest = new EnemeyQuest();
        enemyQuest.titleQuest = "Golem";
        enemyQuest.descriptionQuest = "Overcome the formidable golem adversary. Fear not, it poses no threat.";
        enemyQuest.SetEnmeyObject(enemyGO);

        FinishQuest finishQuest = new FinishQuest();
        finishQuest.titleQuest = "End";
        finishQuest.descriptionQuest = "Celebrate your triumph as you successfully conclude the Tutorial.";




        // Add quests to the list
        quests.Add(moveQuest);
        quests.Add(jumpQuest);
        quests.Add(attackQuest);
        quests.Add(manaQuest);
        quests.Add(attack2Quest);
        quests.Add(enemyQuest);
        quests.Add(finishQuest);

        // Reset current quest index
        currentQuestIndex = -1;
    }

    private void StartNextQuest()
    {
        if (currentQuestIndex >= 0) // Assurez-vous qu'il y a une qu�te pr�c�dente � f�liciter.
        {
            descriptionText.text = "Bravo !"; // Message de f�licitation pour la qu�te pr�c�dente.
            sucessSFX.Play();
            StartCoroutine(WaitAndShowNextQuestDescription(2)); // Attendre 2 secondes avant de montrer la prochaine description.
        }
        else
        {
            StartCoroutine(WaitAndShowNextQuestDescription(2));
        }
    }

    IEnumerator WaitAndShowNextQuestDescription(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowNextQuestDescription();
    }

    private void ShowNextQuestDescription()
    {
        currentQuestIndex++;
        if (currentQuestIndex < quests.Count)
        {
            quests[currentQuestIndex].OnQuestCompleted += QuestCompleted;
            quests[currentQuestIndex].StartQuest();
            descriptionText.text = quests[currentQuestIndex].descriptionQuest;
        }
    }

    private void Update()
    {
        if (currentQuestIndex >= 0) // Assurez-vous qu'il y a une qu�te pr�c�dente � f�liciter.
        {
            quests[currentQuestIndex].Update();
        }
    }

    private void QuestCompleted(Quest quest)
    {
        quest.OnQuestCompleted -= QuestCompleted; // D�sabonnement � l'�v�nement
        Debug.Log($"Quest completed: {quest.titleQuest}");
        StartNextQuest(); // Commence la prochaine qu�te
    }

    [System.Serializable]
    public abstract class Quest
    {
       public string titleQuest; // Visible dans l'inspecteur
       public string descriptionQuest; // Visible dans l'inspecteur
        public event Action<Quest> OnQuestCompleted;
      

        public abstract void StartQuest();

        protected void CompleteQuest()
        {
            OnQuestCompleted?.Invoke(this);
        }

        public abstract void Update();
    }
}

// Exemple de qu�te de mouvement
public class MoveQuest : TutorialManager.Quest
{
    public override void StartQuest()
    {
        UIManager.instance.panelQuest.SetActive(true);

        // S'abonner aux entr�es du joueur ou � tout autre syst�me n�cessaire pour d�tecter le mouvement
        Debug.Log("MoveQuest started. Please move.");
    }

   public override void Update() // Vous aurez besoin d'une mani�re de faire appel � Update, possiblement via le TutorialManager ou un syst�me d'�v�nements global.
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CompleteQuest(); // La qu�te est compl�t�e lorsque le joueur se d�place
        }
    }
}
public class JumpQuest : TutorialManager.Quest
{
    public override void StartQuest()
    {
        // Affiche un message ou un indicateur visuel pour indiquer au joueur de sauter
        Debug.Log("JumpQuest started. Please jump.");
    }

    // Vous pouvez appeler cette m�thode depuis votre script de contr�leur de personnage lorsqu'un saut est effectu�
    public override void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            CompleteQuest(); // La qu�te est compl�t�e lorsque le joueur se d�place
        }
       
    }
}


public class AttackQuest : TutorialManager.Quest
{
    public override void StartQuest()
    {
        // Affiche un message ou un indicateur visuel pour indiquer au joueur de sauter
        Debug.Log("JumpQuest started. Please jump.");
    }

    // Vous pouvez appeler cette m�thode depuis votre script de contr�leur de personnage lorsqu'un saut est effectu�
    public override void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            CompleteQuest(); // La qu�te est compl�t�e lorsque le joueur se d�place
        }

    }
}
[System.Serializable]
public class ManaQuest : TutorialManager.Quest
{
    private GameObject manaObject;// Le GameObject � r�cup�rer
    private GameObject wallBoundObject;

    public override void StartQuest()
    {
        // Afficher un message ou un indicateur visuel pour indiquer au joueur de r�cup�rer le mana
        Debug.Log("ManaQuest started. Find the mana object!");
        wallBoundObject.SetActive(false);
        manaObject.SetActive(true);
        UIManager.instance.setMoveRight(true);
    }

    public override void Update()
    {
        // V�rifier si le joueur a r�cup�r� le GameObject de mana
        if (manaObject == null)
        {
            CompleteQuest(); // La qu�te est compl�t�e lorsque le joueur attaque et r�cup�re le mana
        }
    }

    public void SetManaObject(GameObject go)
    {
        this.manaObject = go;
    }
    public void SetWallBoundObject(GameObject go)
    {
        this.wallBoundObject = go;
    }
}

public class Attack2Quest : TutorialManager.Quest
{
    public override void StartQuest()
    {
        // Affiche un message ou un indicateur visuel pour indiquer au joueur de sauter
        Debug.Log("JumpQuest started. Please jump.");
    }

    // Vous pouvez appeler cette m�thode depuis votre script de contr�leur de personnage lorsqu'un saut est effectu�
    public override void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            CompleteQuest(); // La qu�te est compl�t�e lorsque le joueur se d�place
        }

    }

  

}

public class EnemeyQuest : TutorialManager.Quest
{
    private GameObject enemyObject;// Le GameObject � r�cup�rer
    private GameObject wallBoundObject;

    public override void StartQuest()
    {
        // Afficher un message ou un indicateur visuel pour indiquer au joueur de r�cup�rer le mana
        Debug.Log("ManaQuest started. Find the mana object!");
        enemyObject.SetActive(true);
    }

    public override void Update()
    {
        // V�rifier si le joueur a r�cup�r� le GameObject de mana
        if (enemyObject == null)
        {
            CompleteQuest(); // La qu�te est compl�t�e lorsque le joueur attaque et r�cup�re le mana
        }
    }

    public void SetEnmeyObject(GameObject go)
    {
        this.enemyObject = go;
    }
}

public class FinishQuest : TutorialManager.Quest
{
    
    public override void StartQuest()
    {
        GameManager.instance.winLevel();


    }


    private void changeScene()
    {
        
        SceneManager.LoadScene("Menu");
    }
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            changeScene();
        }
        
    }

}

