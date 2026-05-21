namespace LSH.Utils.Pooling
{
    public interface IPoolable
    {
        void OnCreated();
        void OnGet();
        void OnRelease();
    }
}
