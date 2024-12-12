namespace Work_01.ViewModels
{
    public class GroupedData<T>
    {
        public string Key { get; set; } = default!;
        public IEnumerable<T> Items { get; set; } = [];

    }
}
