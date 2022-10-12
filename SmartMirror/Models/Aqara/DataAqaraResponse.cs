namespace SmartMirror.Models.Aqara
{
    public class DataAqaraResponse<T>
    {
        public IEnumerable<T> Data { get; set; }

        public long TotalCount { get; set; }
    }
}
