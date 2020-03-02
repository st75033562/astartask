using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoPlayer : MonoBehaviour
{
    private GameObject mCubeParent;

    //存储路径点
    private List<AStarPoint> mPathPosList;

    //网格大小
    private const int mGridWidth = 80;
    private const int mGridHeight = 70;

    private AStarPoint[,] mPointGrid;

    private AStarPoint mStartPos;
    private bool isStartPoint;
    private AStarPoint mEndPos { get; set; }

    //每一秒发生位移
    private float mTime = 0.7f;
    private float mTimer = 0.0f;

    //目标点
    private int mTargetX { get; set; }
    private int mTargetY { get; set; }

    private void Start()
    {
        mCubeParent = GameObject.Find("Plane");

        mPointGrid = AStarAlgorithm.GetInsatnce.mPointGrid;
       
        InitBG();

        mStartPos = mPointGrid[0, 0];
        mEndPos = mPointGrid[10, 10];
    }

    private void Update()
    {
        mTimer += Time.deltaTime;
        if (mTimer >= mTime)
        {
            mTimer = 0;
         //   Walk();
        }
    }

    private void Walk()
    {
        if (mPathPosList != null && mPathPosList.Count > 1)
        {
            mStartPos = mPathPosList[mPathPosList.Count - 1];
            Color color = mStartPos.mGameObject.GetComponent<Renderer>().material.color;
            mPathPosList.Remove(mStartPos);
            Destroy(mStartPos.mGameObject);
            mStartPos.mGameObject = null;

            mStartPos = mPathPosList[mPathPosList.Count - 1];
            mStartPos.mGameObject.GetComponent<Renderer>().material.color = color;

        }
    }

    private void InitBG()
    {
        for (int i = 0; i < mGridWidth; i++)
        {
            for (int j = 0; j < mGridHeight; j++)
            {
                CreateCube(i, j, Color.gray);
            }
        }
    }


    private void CreateCube(int x, int y, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(mCubeParent.transform);
        go.transform.localPosition = new Vector3(x, y, 0);
        AStarAlgorithm.GetInsatnce.InitPoint1(x, y, x, y);
        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        Destroy(go.GetComponent<Renderer>());
       // go.GetComponent<Renderer>().material.color = color;

        var cube = go.AddComponent<Cube>();
        cube.X = x;
        cube.Y = y;
        cube.wall = (x1, y1) =>
        {
            AStarAlgorithm.GetInsatnce.SetWall1(x1, y1);
        };
        cube.FindPath = FindPath;
    }

    public void OnClickSetPoint(bool isStartPoint)
    {
        this.isStartPoint = isStartPoint;
    }

    public void FindPath(int mTargetX, int mTargetY)
    {
        if (mPathPosList != null)
        {
            mPathPosList.Clear();
        }
        AStarAlgorithm.GetInsatnce.ClearGrid();

        //网格点对象重新刷新了  需要使用网格来索引到点 mPathPosList存储的点是之前的AStarPoint
        if (isStartPoint)
        {
            this.mStartPos = mPointGrid[mTargetX, mTargetY];
            this.mEndPos = mPointGrid[mEndPos.mPositionX, mEndPos.mPositionY];
        }
        else
        {
            this.mEndPos = mPointGrid[mTargetX, mTargetY];
            this.mStartPos = mPointGrid[mStartPos.mPositionX, mStartPos.mPositionY];
        }
        mPathPosList = AStarAlgorithm.GetInsatnce.FindPath(this.mStartPos, mEndPos);
    }
}