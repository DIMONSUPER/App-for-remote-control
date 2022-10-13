namespace SmartMirror.Models.Aqara
{
    public class DataAqaraResponce<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int TotalCount { get; set; }
    }
}
