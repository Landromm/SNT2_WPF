using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel;

public class Message
{
	public Message(string? testText, string? description)
	{
		TestText = testText;
		Description = description;
	}

	public string? TestText { get; set; }
	public string? Description { get; set;}

}