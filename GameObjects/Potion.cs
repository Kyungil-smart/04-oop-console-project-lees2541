

public class Potion : Item, IInteractable
{

    public Potion() => Init();
    
    private void Init()
    {
        Symbol = '*';
    }

    public override void Use()
    {
        Owner?.Grow(1);

        Inventory?.Remove(this);
        Inventory = null;
        Owner = null;
    }

    public void Interact(PlayerCharacter player)
    {
        player.Grow(1);
    }
}