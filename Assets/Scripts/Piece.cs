using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public float stepDelay = 0.8f;
    public float moveDelay = 0.3f;
    public float lockDelay = 0.5f;
    private float stepTime;
    private float moveTime;
    private float lockTime;

    private UserProfile _userProfile;
#if UNITY_ANDROID
    Vector2 firstPressPos;
    Vector2 secondPressPos;
    Vector2 currentSwipe;
#endif

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.position = position;
        this.data = data;
        this.board = board;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    void Update()
    {
        this.board.Clear(this);
        this.lockTime += Time.deltaTime;
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
#endif

#if UNITY_ANDROID
        if (Time.time > moveTime)
            AndroidInput();
#endif

        if (Time.time >= this.stepTime)
        {
            Step();
        }


        this.board.Set(this);
    }

    private void AndroidInput() 
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }

            if (t.phase == TouchPhase.Ended)
            {
                secondPressPos = new Vector2(t.position.x, t.position.y);
                if (firstPressPos == secondPressPos)
                {
                    Rotate(1);
                }

                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                currentSwipe.Normalize();

                if (currentSwipe.y < 0 && currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    HardDrop();
                }

                if (currentSwipe.x < 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    if (IsCanMove(Vector2Int.left) && IsCanMove(Vector2Int.down))
                        Move(Vector2Int.left);
                }

                if (currentSwipe.x > 0 && currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    if (IsCanMove(Vector2Int.right) && IsCanMove(Vector2Int.down))
                        Move(Vector2Int.right);
                }
            }
        }
    }

    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S) && IsCanMove(Vector2Int.down))
        {
            Move(Vector2Int.down);
            stepTime = Time.time + stepDelay;
        }

        if (Input.GetKey(KeyCode.A) && IsCanMove(Vector2Int.left) && IsCanMove(Vector2Int.down))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D) && IsCanMove(Vector2Int.right) && IsCanMove(Vector2Int.down))
        {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;
        Move(Vector2Int.down);
        if (this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    private void HardDrop()
    {
        while (IsCanMove(Vector2Int.down))
        {
            Move(Vector2Int.down);
        }

        Lock();
    }

    private void Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        if (IsCanMove(translation))
        {
            this.position = newPosition;
            this.moveTime = Time.time + moveDelay;
            this.lockTime = 0f;
        }
    }

    private bool IsCanMove(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        return board.IsValidPosition(this, newPosition);
    }

    private void Rotate(int direction)
    {
        int originalRotation = this.rotationIndex;
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);
        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.data.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            int x, y;
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) +
                                        (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) +
                                        (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) +
                                         (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) +
                                         (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);
        for (int i = 0; i < this.data.wallkicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallkicks[wallKickIndex, i];
            if (IsCanMove(translation))
            {
                Move(translation);
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;
        if (rotationIndex < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallkicks.GetLength(0));
    }
}