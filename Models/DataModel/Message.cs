using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNT2_WPF.Models.DataModel;

public class Message
{	
	public Message(string? hashId, string? description, string? nameParametr)
	{
		HashId = hashId;
		Description = description;
		NameParametr = nameParametr;
	}
	
	public string? HashId { get; set; }
	public string? Description { get; set;}
	public string? NameParametr { get; set; }

}