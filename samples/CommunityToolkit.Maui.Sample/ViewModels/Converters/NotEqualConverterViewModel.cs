﻿namespace CommunityToolkit.Maui.Sample.ViewModels.Converters;

public class NotEqualConverterViewModel : BaseViewModel
{
	string inputValue = string.Empty;

	public string InputValue
	{
		get => inputValue;
		set => SetProperty(ref inputValue, value);
	}
}
