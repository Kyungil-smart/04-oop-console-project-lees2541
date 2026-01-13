

public class TownScene : Scene
{
    private Tile[,] _field = new Tile[10, 20];
    private PlayerCharacter _player;

    private readonly Random _random = new Random();

    private bool _gameOver;

    private const int InitialPotionCount = 3;

    public TownScene(PlayerCharacter player) => Init(player);

    public void Init(PlayerCharacter player)
    {
        _player = player;
        
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                Vector pos = new Vector(x, y);
                _field[y, x] = new Tile(pos);
            }
        }
    }

    public override void Enter()
    {


        ClearAllObjects();
        _gameOver = false;


        _player.OnPotionEaten += HandlePotionEaten;
        _player.OnDied += HandlePlayerDied;
        _player.Spawn(_field, startPos: new Vector(4, 2), initialLength: 3);


        for (int i = 0; i < InitialPotionCount; i++)
            SpawnPotion();

        Debug.Log("타운(지렁이) 씬 진입");
    }

    public override void Update()
    {
        if (_gameOver)
        {
            
            if (InputManager.GetKey(ConsoleKey.Enter))
            {
                SceneManager.Change("Title");
            }
            return;
        }
        _player.Update();
    }

    public override void Render()
    {
        PrintField(offsetX: 2, offsetY: 3);
        _player.Render();
        if (_gameOver)
        {
            Console.SetCursorPosition(2, 3 + _field.GetLength(0) + 2);
            "GAME OVER! [ENTER] 타이틀로".Print(ConsoleColor.Red);
        }
    }

    public override void Exit()
    {
        _field[_player.Position.Y, _player.Position.X].OnTileObject = null;
        _player.Field = null;
    }

    private void PrintField(int offsetX, int offsetY)
    {
        int h = _field.GetLength(0);
        int w = _field.GetLength(1);

        
        Console.SetCursorPosition(offsetX, offsetY);
        ("+" + new string('-', w) + "+").Print();

        for (int y = 0; y < h; y++)
        {
            Console.SetCursorPosition(offsetX, offsetY + 1 + y);
            '|'.Print();

            for (int x = 0; x < w; x++)
                _field[y, x].Print();

            '|'.Print();
        }

        
        Console.SetCursorPosition(offsetX, offsetY + 1 + h);
        ("+" + new string('-', w) + "+").Print();
    }

    private void HandlePlayerDied()
    {
        _gameOver = true;
    }
    private void HandlePotionEaten()
    {

        SpawnPotion();
    }

    private void SpawnPotion()
    {
        int h = _field.GetLength(0);
        int w = _field.GetLength(1);


        for (int i = 0; i < 200; i++)
        {
            int y = _random.Next(0, h);
            int x = _random.Next(0, w);

            if (_field[y, x].HasGameObject) continue;

            Potion potion = new Potion { Name = "Potion" };
            potion.Position = new Vector(x, y);
            _field[y, x].OnTileObject = potion;
            return;
        }
    }
    private void ClearAllObjects()
    {
        for (int y = 0; y < _field.GetLength(0); y++)
        {
            for (int x = 0; x < _field.GetLength(1); x++)
            {
                _field[y, x].OnTileObject = null;
            }
        }
    }


}