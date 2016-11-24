public interface IPersistenceService<K, T>
{
    void Persist(T t);
    T Get(K k);
    T Get();
}