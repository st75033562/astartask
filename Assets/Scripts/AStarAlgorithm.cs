using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarAlgorithm
{
    private const int mGridWidth = 80;
    private const int mGridHeight = 70;

    //使用二维数组存储点网格    
    public AStarPoint[,] mPointGrid = new AStarPoint[mGridWidth, mGridHeight];
    //存储路径方格子
    public List<AStarPoint> mPathPosList = new List<AStarPoint>();

    private static AStarAlgorithm _instance;
    public static AStarAlgorithm GetInsatnce
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AStarAlgorithm();
            }

            return _instance;
        }
    }

    public AStarAlgorithm()
    {
  //      InitPoint();
    }

    //在网格上设置点的信息
    public void InitPoint()
    {
        for (int i = 0; i < mGridWidth; i++)
        {
            for (int j = 0; j < mGridHeight; j++)
            {
                mPointGrid[i, j] = new AStarPoint(i, j);
            }
        }

        //设置障碍物
        mPointGrid[4, 2].mIsObstacle = true;
        mPointGrid[4, 3].mIsObstacle = true;
        mPointGrid[4, 4].mIsObstacle = true;
        mPointGrid[4, 5].mIsObstacle = true;
        mPointGrid[4, 6].mIsObstacle = true;

        //显示障碍物
        for (int x = 0; x < mGridWidth; x++)
        {
            for (int y = 0; y < mGridHeight; y++)
            {
                if (mPointGrid[x, y].mIsObstacle)
                {
                    CreatePath(x, y, Color.blue);
                }
            }
        }
    }

    public void InitPoint1(int i, int j, int pointX, int pointY)
    {
        //for (int i = 0; i < mGridWidth; i++)
        //{
        //    for (int j = 0; j < mGridHeight; j++)
        //    {
        //        mPointGrid[i, j] = new AStarPoint(i, j);
        //    }
        //}

        mPointGrid[i, j] = new AStarPoint(pointX, pointY);

       
    }

    public void SetWall1(int i, int j)
    {
        mPointGrid[i, j].mIsObstacle = true;
       // CreatePath(i, j, Color.blue);
    }

    public void ClearGrid()
    {
        for (int x = 0; x < mGridWidth; x++)
        {
            for (int y = 0; y < mGridHeight; y++)
            {
                if (!mPointGrid[x, y].mIsObstacle)
                {
                    if (mPointGrid[x, y].mGameObject != null)
                    {
                        GameObject.Destroy(mPointGrid[x, y].mGameObject);
                        mPointGrid[x, y].mGameObject = null;

                        //重新设置父节点
                        mPointGrid[x, y].mParentPoint = null;
                    }
                }
            }
        }
    }

    //寻路
    public List<AStarPoint> FindPath(AStarPoint mStartPoint, AStarPoint mEndPoint)
    {
        if (mEndPoint.mIsObstacle || mStartPoint.mPosition == mEndPoint.mPosition)
        {
            return null;
        }

        //开启列表
        List<AStarPoint> openPointList = new List<AStarPoint>();
        //关闭列表
        List<AStarPoint> closePointList = new List<AStarPoint>();

        openPointList.Add(mStartPoint);

        while (openPointList.Count > 0)
        {
            //寻找开启列表中最小预算值的表格
            AStarPoint minFPoint = FindPointWithMinF(openPointList);
            //将当前表格从开启列表移除 在关闭列表添加
            openPointList.Remove(minFPoint);
            closePointList.Add(minFPoint);
            //找到当前点周围的全部点
            List<AStarPoint> surroundPoints = FindSurroundPoint(minFPoint);
            //在周围的点中，将关闭列表里的点移除掉
            SurroundPointsFilter(surroundPoints, closePointList);
            //寻路逻辑
            foreach (var surroundPoint in surroundPoints)
            {
                if (openPointList.Contains(surroundPoint))
                {
                    //计算下新路径下的G值（H值不变的，比较G相当于比较F值）
                    float newPathG = CalcG(surroundPoint, minFPoint);
                    if (newPathG < surroundPoint.mG)
                    {
                        surroundPoint.mG = newPathG;
                        surroundPoint.mF = surroundPoint.mG + surroundPoint.mH;
                        surroundPoint.mParentPoint = minFPoint;
                    }
                }
                else
                {
                    //将点之间的
                    surroundPoint.mParentPoint = minFPoint;
                    CalcF(surroundPoint, mEndPoint);
                    openPointList.Add(surroundPoint);
                }
            }

            //如果开始列表中包含了终点，说明找到路径
            if (openPointList.IndexOf(mEndPoint) > -1)
            {
                break;
            }
        }

        return ShowPath(mStartPoint, mEndPoint);
    }

    private List<AStarPoint> ShowPath(AStarPoint start, AStarPoint end)
    {
        mPathPosList.Clear();

        AStarPoint temp = end;
        while (true)
        {
            mPathPosList.Add(temp);

            Color c = Color.white;
            if (temp == start)
            {
                c = Color.green;
            }
            else if (temp == end)
            {
                c = Color.red;
            }
            CreatePath(temp.mPositionX, temp.mPositionY, c);

            if (temp.mParentPoint == null)
                break;
            temp = temp.mParentPoint;
        }

        return mPathPosList;
    }

    private void CreatePath(int x, int y, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.SetParent(GameObject.Find("Plane").transform);
        go.name = "aasdas";
        go.transform.localPosition = new Vector3(x, y, 0);
        go.GetComponent<Renderer>().material.color = color;
        

        if (mPointGrid[x, y].mGameObject != null)
        {
            GameObject.Destroy(mPointGrid[x, y].mGameObject);
        }
        mPointGrid[x, y].mGameObject = go;
    }

    //寻找预计值最小的格子
    private AStarPoint FindPointWithMinF(List<AStarPoint> openPointList)
    {
        float f = float.MaxValue;
        AStarPoint temp = null;
        foreach (AStarPoint p in openPointList)
        {
            if (p.mF < f)
            {
                temp = p;
                f = p.mF;
            }
        }
        return temp;
    }

    //寻找周围的全部点
    private List<AStarPoint> FindSurroundPoint(AStarPoint point)
    {
        List<AStarPoint> list = new List<AStarPoint>();

        ////////////判断周围的八个点是否在网格内/////////////
        AStarPoint up = null, down = null, left = null, right = null;
        AStarPoint lu = null, ru = null, ld = null, rd = null;
        if (point.mPositionY < mGridHeight - 1)
        {
            up = mPointGrid[point.mPositionX, point.mPositionY + 1];
        }
        if (point.mPositionY > 0)
        {
            down = mPointGrid[point.mPositionX, point.mPositionY - 1];
        }
        if (point.mPositionX > 0)
        {
            left = mPointGrid[point.mPositionX - 1, point.mPositionY];
        }
        if (point.mPositionX < mGridWidth - 1)
        {
            right = mPointGrid[point.mPositionX + 1, point.mPositionY];
        }
        if (up != null && left != null)
        {
            lu = mPointGrid[point.mPositionX - 1, point.mPositionY + 1];
        }
        if (up != null && right != null)
        {
            ru = mPointGrid[point.mPositionX + 1, point.mPositionY + 1];
        }
        if (down != null && left != null)
        {
            ld = mPointGrid[point.mPositionX - 1, point.mPositionY - 1];
        }
        if (down != null && right != null)
        {
            rd = mPointGrid[point.mPositionX + 1, point.mPositionY - 1];
        }


        /////////////将可以经过的表格添加到开启列表中/////////////
        if (down != null && down.mIsObstacle == false)
        {
            list.Add(down);
        }
        if (up != null && up.mIsObstacle == false)
        {
            list.Add(up);
        }
        if (left != null && left.mIsObstacle == false)
        {
            list.Add(left);
        }
        if (right != null && right.mIsObstacle == false)
        {
            list.Add(right);
        }
        if (lu != null && lu.mIsObstacle == false && left.mIsObstacle == false && up.mIsObstacle == false)
        {
            list.Add(lu);
        }
        if (ld != null && ld.mIsObstacle == false && left.mIsObstacle == false && down.mIsObstacle == false)
        {
            list.Add(ld);
        }
        if (ru != null && ru.mIsObstacle == false && right.mIsObstacle == false && up.mIsObstacle == false)
        {
            list.Add(ru);
        }
        if (rd != null && rd.mIsObstacle == false && right.mIsObstacle == false && down.mIsObstacle == false)
        {
            list.Add(rd);
        }

        return list;
    }

    //将关闭带你从周围点列表中关闭
    private void SurroundPointsFilter(List<AStarPoint> surroundPoints, List<AStarPoint> closePoints)
    {
        foreach (var closePoint in closePoints)
        {
            if (surroundPoints.Contains(closePoint))
            {
                Debug.Log("将关闭列表的点移除");
                surroundPoints.Remove(closePoint);
            }
        }
    }

    //计算最小预算值点G值
    private float CalcG(AStarPoint surround, AStarPoint minFPoint)
    {
        return Vector3.Distance(surround.mPosition, minFPoint.mPosition) + minFPoint.mG;
    }

    //计算该点到终点的F值
    private void CalcF(AStarPoint now, AStarPoint end)
    {
        //F = G + H
        float h = Mathf.Abs(end.mPositionX - now.mPositionX) + Mathf.Abs(end.mPositionY - now.mPositionY);
        float g = 0;
        if (now.mParentPoint == null)
        {
            g = 0;
        }
        else
        {
            g = Vector2.Distance(new Vector2(now.mPositionX, now.mPositionY), new Vector2(now.mParentPoint.mPositionX, now.mParentPoint.mPositionY)) + now.mParentPoint.mG;
        }
        float f = g + h;
        now.mF = f;
        now.mG = g;
        now.mH = h;
    }
}
