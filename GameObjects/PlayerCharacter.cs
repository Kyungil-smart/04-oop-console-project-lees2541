

using System.Runtime.InteropServices.Marshalling;
using static System.Formats.Asn1.AsnWriter;

public class PlayerCharacter : GameObject
{

    
    public Tile[,] Field { get; set; }
    private Inventory _inventory;
    public bool IsActiveControl { get; private set; }


    public int Length => _segments.Count;

    private readonly List<Vector> _segments = new List<Vector>();
    private readonly List<WormSegment> _bodySegments = new List<WormSegment>();
    private int _pendingGrow;

    public event Action OnPotionEaten;
    public event Action OnDied;

    public PlayerCharacter()
    {
        Symbol = '@';
    }
    public void Spawn(Tile[,] field, Vector startPos, int initialLength = 3)
    {
        Field = field;
        IsActiveControl = true;

        _pendingGrow = 0;

        ClearFromField();
        _segments.Clear();
        _bodySegments.Clear();

        _segments.Add(startPos);
        for (int i = 1; i < initialLength; i++)
        {
            _segments.Add(new Vector(startPos.X - i, startPos.Y));
        }

        
        for (int i = 0; i < _segments.Count; i++)
        {
            Vector p = _segments[i];
            _segments[i] = ClampToField(p);
        }

        EnsureBodySegmentCount();
        ApplyToField();
    }
    public void Init()
    {
        Symbol = 'P';
        IsActiveControl = true;
        _inventory = new Inventory(this);
    }


    public void Update()
    {
        if(!IsActiveControl || Field == null) return;
        
        if (InputManager.GetKey(ConsoleKey.UpArrow))
        {
            Move(Vector.Up);

        }

        if (InputManager.GetKey(ConsoleKey.DownArrow))
        {
            Move(Vector.Down);
  
        }

        if (InputManager.GetKey(ConsoleKey.LeftArrow))
        {
            Move(Vector.Left);
        }

        if (InputManager.GetKey(ConsoleKey.RightArrow))
        {
            Move(Vector.Right);
        }



    }


    private void Move(Vector direction)
    {
        Vector head = _segments[0];
        Vector next = head + direction;

        
        if (!IsInBounds(next))
        {
            Die();
            return;
        }

        GameObject nextObj = Field[next.Y, next.X].OnTileObject;

        
        if (nextObj is WormSegment || nextObj == this)
        {
            Die();
            return;
        }

        
        bool atePotion = nextObj is Potion;
        if (nextObj is IInteractable interactable)
        {
            interactable.Interact(this);
        }

        
        ClearFromField();

        
        _segments.Insert(0, next);

        
        if (_pendingGrow > 0)
        {
            _pendingGrow--;
        }
        else
        {
            _segments.RemoveAt(_segments.Count - 1);
        }

        EnsureBodySegmentCount();
        ApplyToField();

        
        Position = _segments[0];

        if (atePotion)
        {

            OnPotionEaten?.Invoke();
        }
    }

    public void Render()
    {

        
    }

    public void Grow(int amount = 1)
    {
        if (amount <= 0) return;
        _pendingGrow += amount;
    }




    private void EnsureBodySegmentCount()
    {
        int need = Math.Max(0, _segments.Count - 1);
        while (_bodySegments.Count < need) _bodySegments.Add(new WormSegment());
        while (_bodySegments.Count > need) _bodySegments.RemoveAt(_bodySegments.Count - 1);
    }

    private void ApplyToField()
    {
        if (Field == null) return;

        // head
        Position = _segments[0];
        Field[Position.Y, Position.X].OnTileObject = this;

        // body
        for (int i = 1; i < _segments.Count; i++)
        {
            Vector p = _segments[i];
            WormSegment seg = _bodySegments[i - 1];
            seg.Position = p;
            Field[p.Y, p.X].OnTileObject = seg;
        }
    }

    private void ClearFromField()
    {
        if (Field == null) return;

        for (int i = 0; i < _segments.Count; i++)
        {
            Vector p = _segments[i];
            if (!IsInBounds(p)) continue;

            GameObject obj = Field[p.Y, p.X].OnTileObject;
            if (obj == this || obj is WormSegment)
            {
                Field[p.Y, p.X].OnTileObject = null;
            }
        }
    }
    private void Die()
    {
        if (!IsActiveControl) return;
        IsActiveControl = false;
        OnDied?.Invoke();
    }

    private bool IsInBounds(Vector p)
    {
        if (Field == null) return false;
        int h = Field.GetLength(0);
        int w = Field.GetLength(1);
        return p.Y >= 0 && p.Y < h && p.X >= 0 && p.X < w;
    }



    private Vector ClampToField(Vector p)
    {
        if (Field == null) return p;
        int h = Field.GetLength(0);
        int w = Field.GetLength(1);

        int x = p.X;
        int y = p.Y;
        if (x < 0) x = 0;
        if (x >= w) x = w - 1;
        if (y < 0) y = 0;
        if (y >= h) y = h - 1;

        return new Vector(x, y);
    }



}