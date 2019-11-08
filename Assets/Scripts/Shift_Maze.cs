using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shift_Maze : MonoBehaviour
{
    public int numberOfWalls;
    public GameObject wallPrefab;
    public Vector3 bounds;
    public float x;
    public float z;
    public float wallLength;
    public float movesPerSecond;
    List<GameObject> walls = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetupMaze();
    }
    void SetupMaze()
    {
        int a = 0;
        for (x = wallLength/2f-bounds.x; x <bounds.x; x+=wallLength)
        {
            a++;
            if (a > 1000)
            {
                break;
            }
            Instantiate(wallPrefab, this.transform).transform.localPosition = new Vector3(x,0f,bounds.z);
            Instantiate(wallPrefab, this.transform).transform.localPosition = new Vector3(x, 0f, -bounds.z);

        }
        a = 0;
        
        for (z = wallLength/ 2f - bounds.z; z < bounds.z; z += wallLength)
        {
            a++;
            if (a > 1000)
            {
                break;
            }
            if (a != 8)
            {
                GameObject _wallA = Instantiate(wallPrefab, this.transform);
                _wallA.transform.localPosition = new Vector3(bounds.x, 0f, z);
                _wallA.transform.Rotate(0, 90, 0);
            }

            if (a != 2)
            {
                GameObject _wallB = Instantiate(wallPrefab, this.transform);
                _wallB.transform.localPosition = new Vector3(-bounds.x, 0f, z);

                _wallB.transform.Rotate(0, 90, 0);
            }
           

        }
        for (int i =0; i<numberOfWalls; i++)
        {
            Vector3 _wallPosition = GetRandomPosition();
            GameObject _wall = Instantiate(wallPrefab, this.transform);
            _wall.transform.localPosition = _wallPosition;
            _wall.transform.Rotate(0, 90*Mathf.RoundToInt(Random.value), 0);
            walls.Add(_wall);
        }
    }
    Vector3 GetRandomPosition()
    {
        float wallx = Mathf.RoundToInt(Random.Range(-bounds.x + wallLength/2, bounds.x - wallLength/2f) / (wallLength/2f))*wallLength/2f;
        float wallz = Mathf.RoundToInt(Random.Range(-bounds.z + wallLength/2f, bounds.z - wallLength/2f) / (wallLength/2f)) * wallLength/2f;

        return new Vector3(wallx,0f,wallz);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 520 == 0)
        {
            int i = 0;
            while (i < movesPerSecond)
            {
                //pick a random wall
                GameObject wallToMove = walls[Random.Range(0, walls.Count)];
                float movePosition = Mathf.RoundToInt(Random.Range(-bounds.x-wallLength, bounds.x+wallLength) / (wallLength / 2f)) * wallLength / 2f;
                movePosition = Mathf.Clamp(movePosition, -bounds.x + wallLength / 2f, bounds.x - wallLength / 2f);
                StartCoroutine(MoveWall(wallToMove, movePosition));
                i++;
            }
        }
    }
    IEnumerator MoveWall(GameObject _wall, float _position)
    {
        Vector3 destination = _wall.transform.right * _position+Vector3.Scale(_wall.transform.localPosition, _wall.transform.forward);
        while (_wall.transform.localPosition != destination)
        {
            _wall.transform.localPosition = Vector3.MoveTowards(_wall.transform.localPosition, destination, 0.4f);
            yield return new WaitForEndOfFrame();
        }

    }

}
