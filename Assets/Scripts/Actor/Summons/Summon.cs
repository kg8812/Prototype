public abstract class Summon : Actor
{
    private Actor master;
    public virtual Actor Master => master;

    public virtual void SetMaster(Actor master)
    {
        this.master = master;
    }

    public virtual void OnUnSummon()
    {
    }
}