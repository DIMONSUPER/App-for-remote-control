using System;
using System.Windows.Input;

namespace SmartMirror.Interfaces
{
	public interface IChipModel : IBaseChipModel
	{
		public string Text { get; set; }

		public string FontFamily { get; set; }

		public float FontSize { get; set; }
    }
}