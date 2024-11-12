using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneEnemy : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Tableau des ennemis possibles
    public GameObject[] walls;
    public GameObject coin;
    [SerializeField] private int minCoin, maxCoin;
    public GameObject vfxPrefab; // Prefab de l'effet visuel (VFX)
    public int maxEnemies = 10; // Nombre maximum d'ennemis � instancier
    public Vector3 minSpawnPosition = new Vector3(-10f, 0f, -10f); // Position minimale de spawn
    public Vector3 maxSpawnPosition = new Vector3(10f, 0f, 10f); // Position maximale de spawn
    public float spawnDelay = 1f; // D�lai entre chaque spawn
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // Liste des ennemis instanci�s
    private bool isSpawningFinished = false;
    private bool startedSpawn = false;//eri 
    [SerializeField] private AudioSource sfxSpawn;

    [SerializeField] private bool lastSpawnZone;


    private void Start()
    {
        for (int i = 0; i < Random.Range(minCoin, maxCoin); i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
      Random.Range(minSpawnPosition.x, maxSpawnPosition.x),
     1f,
      Random.Range(minSpawnPosition.z, maxSpawnPosition.z));

            
            Instantiate(coin, spawnPosition, Quaternion.identity);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!startedSpawn && other.CompareTag("Player") && spawnedEnemies.Count == 0)
        {
            startedSpawn = true;
            StartCoroutine(SpawnEnemiesWithDelay());
            foreach (var wall in walls)
            {
                wall.SetActive(true);
            }
        }
    }

    IEnumerator SpawnEnemiesWithDelay()
    {
        for (int i = 0; i < maxEnemies; i++)
        {
            sfxSpawn.Play();// pour sound 
                            // Calculer une position al�atoire � l'int�rieur des limites d�finies
            Vector3 spawnPosition = transform.position + new Vector3(
       Random.Range(minSpawnPosition.x, maxSpawnPosition.x),
       Random.Range(minSpawnPosition.y, maxSpawnPosition.y),
       Random.Range(minSpawnPosition.z, maxSpawnPosition.z)
   );

            // S�lection al�atoire d'un ennemi dans le tableau des pr�fabs
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Instanciation de l'ennemi
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            spawnedEnemies.Add(enemyInstance);

            // Instanciation de l'effet visuel (VFX)
            spawnPosition.y = 0;
            Instantiate(vfxPrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawnDelay);
        }
        isSpawningFinished = true;
    }

    void Update()
    {
        // V�rifier si tous les ennemis ont �t� tu�s
        spawnedEnemies.RemoveAll(item => item == null); // Retirer les ennemis d�truits de la liste
        if (isSpawningFinished && spawnedEnemies.Count == 0)
        {
            // D�truire le script lorsque tous les ennemis sont morts
            UIManager.instance.setMoveRight(true);
            walls[0].SetActive(false);
            if (lastSpawnZone)
            {
                GameManager.instance.winLevel();
            }
             
            Destroy(GetComponent<ZoneEnemy>());
        }
    }

    // Dessiner la zone de spawn dans l'�diteur Unity pour faciliter la visualisation
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position + (maxSpawnPosition + minSpawnPosition) / 2;
        Vector3 size = maxSpawnPosition - minSpawnPosition;
        Gizmos.DrawWireCube(center, size);
    }

}
