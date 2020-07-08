using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EArrowDir { Left, Right, Up, Down }

public class NewBehaviourScript : MonoBehaviour
{
    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _pause = !_pause;
        }

        if (_pause)
            return;

        UpdateBorn();
        UpdateInput();
        UpdateMove();
        UpdateResetColor();
    }

    private void UpdateResetColor()
    {
        if (Time.time > _resetColorTime)
        {
            CenterColorMat.material.SetColor("_Color", Color.white);
            _resetColorTime = float.MaxValue;
        }
    }
    private void UpdateMove()
    {
        foreach (var go in _arrows)
        {
            go.transform.position += Vector3.left * Speed * Time.deltaTime;
        }
    }

    private void UpdateBorn()
    {
        _bornCounter += Time.deltaTime;
        if (_bornCounter > BornTime)
        {
            CreateArrow(Random.Range(0, 4));
            _bornCounter = 0f;
            BornTime = Random.Range(0.8f / Speed, 5 * 0.8f / Speed);
        }
    }
    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            UpLevel();
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            DownLevel();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckLevel(0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckLevel(1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CheckLevel(2);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CheckLevel(3);
        }
    }

    private void CreateArrow(int index)
    {
        var arr = Instantiate(lrud[index], StartPosition.position, Quaternion.identity);
        _arrows.Add(arr);

        if (_arrows.Count > 50)
        {
            var go = _arrows[0];
            _arrows.Remove(go);
            DestroyImmediate(go);
        }
    }

    private void UpLevel()
    {
        Speed = Mathf.Clamp(Speed + 0.5f, 2f, 12f);
    }

    private void DownLevel()
    {
        Speed = Mathf.Clamp(Speed - 0.5f, 2f, 12f);
    }

    private void CheckLevel (int dirIndex)
    {
        _resetColorTime = Time.time + 0.2f;
        var go = FindClosest();
        
        int level;
        if (!go || !go.name.Contains(dirIndex.ToString()))
        {
            level = 0;
        }
        else
        {
            var dis = Vector3.Distance(go.transform.position, CenterTransform.position);
            if (dis > 0.5)
                level = 0;
            else if (dis > 0.3)
                level = 1;
            else if (dis > 0.2)
                level = 2;
            else if (dis > 0.1)
                level = 3;
            else
                level = 4;
        }

        DestroyImmediate(go);
        CenterColorMat.material.SetColor("_Color", GetColor(level));
    }

    private GameObject FindClosest()
    {
        var minDis = float.MaxValue;
        GameObject minGo = null;
        for (var i = _arrows.Count - 1; i >= 0; i--)
        {
            var go = _arrows[i];
            var dis = Vector3.Distance(go.transform.position, CenterTransform.position);
            if (dis < minDis)
            {
                minGo = go;
                minDis = dis;
            }
        }

        if (minGo)
        {
            _arrows.Remove(minGo);
        }

        return minGo;
    }


    private Color GetColor(int level)
    {
        switch(level)
        {
            case 1:
                return Color.green;
            case 2:
                return Color.blue;
            case 3:
                return Color.yellow;
            case 4:
                return Color.red;
            default:
                return Color.black;
        }
    }

    public GameObject[] lrud;

    public float BornTime = 0f;

    public Transform StartPosition;

    public Transform CenterTransform;

    public MeshRenderer CenterColorMat;

    public float Speed = 3f;

    private List<GameObject> _arrows = new List<GameObject>();

    private float _bornCounter;

    private float _resetColorTime;

    private bool _pause;
}
