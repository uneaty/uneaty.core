public interface IPersistenceService<K, T>
{
    public abstract void Persist(T t);
    public abstract T Get(K k);
    public abstract T Get();
}