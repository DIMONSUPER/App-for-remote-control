using System;
namespace SmartMirror.Interfaces
{
    public interface IGroupableCollection
    {
        public string GroupName { get; set; }

        public int ItemsCount { get; set; }
    }
}