public interface IStateItem
{
    void OnInDistance();
    void OnEnter();
    void OnStay();
    void OnExit();
    void OnDestroyItem();

}
