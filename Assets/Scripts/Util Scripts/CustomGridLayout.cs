using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomGridLayout : MonoBehaviour
{
    //Determines the order and start position that cells are placed in
    private enum StartCorner
    {
        topLeft = 0,
        topRight = 1,
        bottomLeft = 2,
        bottomRight = 3
    }

    //Determines if cells go left -> right or top -> bottom before reaching the end of the row/column
    private enum StartAxis
    {
        horizontal = 0,
        vertical = 1
    }

    private RectTransform rectTransform;

    [SerializeField]
    private StartCorner startingCorner = StartCorner.topLeft;
    [SerializeField]
    private StartAxis startingAxis = StartAxis.horizontal;

    [SerializeField]
    private float paddingLeft = 0;
    [SerializeField]
    private float paddingRight = 0;
    [SerializeField]
    private float paddingTop = 0;
    [SerializeField]
    private float paddingBottom = 0;

    [SerializeField]
    private float spacing = 0;

    //[SerializeField]
    //private float cellSizeX = 100;
    //[SerializeField]
    //private float cellSizeY = 100;

    //Public so that they can be changed through code, incase someone wants a more dynamic list
    public int columns = 2;
    public int rows = 5;

    Vector2 lastContainerSize;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        if(rectTransform == null)
        {
            Debug.LogWarning("No RectTransform found!\nDestroying...");

            Destroy(this);
        }

        lastContainerSize = Vector2.zero;

        ShiftObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if(lastContainerSize == rectTransform.rect.size)
        {
            return;
        }

        ShiftObjects();
    }

    private void ShiftObjects()
    {
        //Step 1: Determine width and heights of children

        //Part a: width

        //Max width of one cell
        //Subtract padding
        //Subtract spacing (1 column = no spacing, 3 columns = 2 spacing, etc.)
        float _width = (rectTransform.rect.width - (paddingLeft + paddingRight) - (spacing * (columns - 1))) / columns;


        //Part b: height

        //Max width of one cell
        //Subtract padding
        //Subtract spacing (1 column = no spacing, 3 columns = 2 spacing, etc.)
        float _height = (rectTransform.rect.height - (paddingTop + paddingBottom) - (spacing * (rows - 1))) / rows;



        //Step 2:
        // --- DETERMINE DIRECTIONS ---

        // +1 = L -> R, -1 = R -> L
        int horizontalDirection;
        // +1 = T -> B, -1 = B -> T
        int verticalDirection;

        // 0 = left, cols = right
        int horizontalStart;
        // 0 = top, rows = bottom
        int verticalStart;

        switch (startingCorner)
        {
            case (StartCorner.topLeft):
                horizontalDirection = 1;
                verticalDirection = 1;
                horizontalStart = 0;
                verticalStart = 0;
                break;

            case (StartCorner.topRight):
                horizontalDirection = -1;
                verticalDirection = 1;
                horizontalStart = columns - 1;
                verticalStart = 0;
                break;

            case (StartCorner.bottomLeft):
                horizontalDirection = 1;
                verticalDirection = -1;
                horizontalStart = 0;
                verticalStart = rows - 1;
                break;

            case (StartCorner.bottomRight):
                horizontalDirection = -1;
                verticalDirection = -1;
                horizontalStart = columns - 1;
                verticalStart = rows - 1;
                break;

            default:
                Debug.LogWarning("This is never supposed to run");
                horizontalDirection = 1;
                verticalDirection = 1;
                horizontalStart = 0;
                verticalStart = 0;
                break;
        }

        List<Transform> children = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }

        if(children.Count > rows * columns)
        {
            Debug.LogWarning("On: " + gameObject.name + "\nMore children than available cells!");
        }

        int curChild = 0;

        //Step 3: Place Objects
        switch (startingAxis)
        {
            case (StartAxis.horizontal):

                //     Should keep the loop contained no matter which direction it goes
                for(int i = verticalStart; (i >= 0) && (i < rows); i += verticalDirection)
                {
                    for(int j = horizontalStart; (j >= 0) && (j < columns); j += horizontalDirection)
                    {
                        if(curChild >= children.Count)
                        {
                            return;
                        }

                        RectTransform childRect = children[curChild].GetComponent<RectTransform>();

                        //Makes it so that all objects are placed by their top left corners
                        childRect.anchorMin = new Vector2(0, 1);
                        childRect.anchorMax = new Vector2(0, 1);
                        childRect.pivot = new Vector2(0, 1);

                        //Moves and re-sizes the object
                        childRect.sizeDelta = new Vector2(_width, _height);
                        childRect.anchoredPosition = new Vector2(
                            paddingLeft + (spacing * j) + (_width * j),    // x
                            -paddingTop - (spacing * i) - (_height * i));  // y

                        curChild++;
                    }
                }
                break;


            case (StartAxis.vertical):

                //     Should keep the loop contained no matter which direction it goes
                for (int i = horizontalStart; (i >= 0) && (i < columns); i += horizontalDirection)
                {
                    for (int j = verticalStart; (j >= 0) && (j < rows); j += verticalDirection)
                    {
                        if (curChild >= children.Count)
                        {
                            return;
                        }

                        RectTransform childRect = children[curChild].GetComponent<RectTransform>();

                        //Makes it so that all objects are placed by their top left corners
                        childRect.anchorMin = new Vector2(0, 1);
                        childRect.anchorMax = new Vector2(0, 1);
                        childRect.pivot = new Vector2(0, 1);

                        //Moves and re-sizes the object
                        childRect.sizeDelta = new Vector2(_width, _height);
                        childRect.anchoredPosition = new Vector2(
                            paddingLeft + (spacing * i) + (_width * i),    // x
                            -paddingTop - (spacing * j) - (_height * j));  // y

                        curChild++;
                    }
                }
                break;
        }
    }
}
