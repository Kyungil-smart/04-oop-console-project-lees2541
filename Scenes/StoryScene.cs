

public class StoryScene : Scene
{
    private string[] _script =
    {
        "깜깜한 콘솔 한가운데, 작은 지렁이가 꿈틀거렸다.",
        "배가 고프다… 아주 많이.",
        "먹이를 먹을수록 몸은 길어지지만,",
        "벽에 부딪히면 모든 게 끝난다.",
        "",
        "[ENTER] 를 눌러 모험을 시작하자."
    };

    private int _visibleLineCount;     
    private Ractangle _box;

    public override void Enter()
    {
        _visibleLineCount = 0;
        _box = new Ractangle(width: 58, height: 12);

        Debug.Log("스토리 씬 진입");
    }

    public override void Update()
    {
        if (!InputManager.GetKey(ConsoleKey.Enter))
            return;

        
        if (_visibleLineCount < _script.Length)
        {
            _visibleLineCount++;
            return;
        }

        
        SceneManager.Change("Town");
    }

    public override void Render()
    {
        
        Console.SetCursorPosition(8, 2);
        "STORY".Print(ConsoleColor.Yellow);

        
        _box.X = 2;
        _box.Y = 4;
        _box.Draw();

        
        int x = 4;
        int y = 6;

        if (_visibleLineCount == 0)
        {
            Console.SetCursorPosition(x, y);
            "Enter를 누르면 스토리가 출력됩니다.".Print(ConsoleColor.DarkGray);
            return;
        }

        for (int i = 0; i < _visibleLineCount; i++)
        {
            Console.SetCursorPosition(x, y + i);
            _script[i].Print();
        }

        
        Console.SetCursorPosition(x, _box.Y + _box.Height - 2);
        (_visibleLineCount < _script.Length ? "[ENTER] 다음" : "[ENTER] 시작").Print(ConsoleColor.DarkGray);
    }

    public override void Exit()
    {
    }
}